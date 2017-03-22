using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AlphaOmega.Reflection;

namespace CloudCastle.Processors.Reflection
{
	[Serializable]
	internal class AssemblyProxy : MarshalByRefObject, IAssemblyProxy
	{
		private Assembly _loadedAssembly;

		public ApiServiceInfo[] GetDataItems(String path, String absoluteUri, Boolean searchWcf, Boolean searchWebApi, Boolean searchWebService)
		{
			List<ApiServiceInfo> result = new List<ApiServiceInfo>();
			DirectoryInfo directory = new DirectoryInfo(path);
			ResolveEventHandler reflectionResolveEventHandler = delegate(Object s, ResolveEventArgs e) { return OnReflectionOnlyResolve(e, directory); };
			ResolveEventHandler resolveEventHanlder = delegate(Object s, ResolveEventArgs e) { return OnResolve(e, directory); };

			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += reflectionResolveEventHandler;
			AppDomain.CurrentDomain.AssemblyResolve += resolveEventHanlder;//TODO: Попытка загрузить нехватающие сборки

			try
			{
				Assembly reflectionOnlyAssembly = this._loadedAssembly;

				String assemblyName = reflectionOnlyAssembly.GetName().Name;
				foreach(Type assemblyType in reflectionOnlyAssembly.GetTypes())
				{
					String url = null;
					if(searchWebApi)
					{//WebAPI Service
						url = assemblyType.GetAttributeValue<String>(Constant.Reflection.WebApi.RoutePrefix, "Prefix");

						if(url != null)
							url = "/" + url;
					}

					if(url == null && searchWcf)
					{//WCF Service
						url = assemblyType.GetAttributeValue<String>(Constant.Reflection.Wcf.ServiceContract, "Namespace");
						if(url != null)
						{
							//Подпапка
							String folder = assemblyType.Namespace.StartsWith(assemblyName + '.')
								? "/" + assemblyType.Namespace.Remove(0, assemblyName.Length + 1)
								: null;
							url = folder + "/" + assemblyType.Name + ".svc/json";//WCF Service
						}
					}

					if(url == null && searchWebService)
					{//WebService
						if(assemblyType.IsDefined(Constant.Reflection.WS.WebService))
							url = "/" + assemblyType.Name + ".asmx/";
					}

					if(url != null)
						result.Add(new ApiServiceInfo(assemblyType, url, absoluteUri));
				}
			} finally
			{
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= reflectionResolveEventHandler;
				AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHanlder;//TODO: Попытка загрузить нехватающие сборки
			}

			return result.OrderBy(p=>p.Name).ToArray();
		}

		private Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
		{

			Assembly loadedAssembly = Array.Find(AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies(), delegate(Assembly asm) { return String.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase); });

			if(loadedAssembly != null)
				return loadedAssembly;

			AssemblyName assemblyName = new AssemblyName(args.Name);
			String dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");

			if(File.Exists(dependentAssemblyFilename))
				return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename);
			return Assembly.ReflectionOnlyLoad(args.Name);
		}

		private Assembly OnResolve(ResolveEventArgs args, DirectoryInfo directory)
		{
			Assembly loadedAssembly = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), delegate(Assembly asm) { return String.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase); });

			if(loadedAssembly != null)
				return loadedAssembly;

			AssemblyName assemblyName = new AssemblyName(args.Name);
			String dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");

			if(File.Exists(dependentAssemblyFilename))
				return Assembly.LoadFrom(dependentAssemblyFilename);

			throw new FileNotFoundException("Assembly not found", args.Name);
			//return Assembly.Load(args.Name);//StackOverflowException
		}

		public void LoadAssembly(String assemblyPath)
		{
			if(String.IsNullOrWhiteSpace(assemblyPath))
				throw new ArgumentNullException("assemblyPath");
			if(!File.Exists(assemblyPath))
				throw new FileNotFoundException("Assembly not found", assemblyPath);

			this._loadedAssembly = Assembly.LoadFile(assemblyPath);
		}
	}
}