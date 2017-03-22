using System;
using System.IO;
using AlphaOmega.Reflection;
using CloudCastle.Input;
using CloudCastle.Processors.Reflection;

namespace CloudCastle.Processors
{
	internal class ReflectionAnalyzer
	{
		public ApiServiceInfo[] Invoke(DocArgs args)
		{
			if(args == null)
				throw new ArgumentNullException("args");
			if(String.IsNullOrWhiteSpace(args.DllFile))
				throw new ArgumentNullException("DllFile");

			String path = Path.GetDirectoryName(args.DllFile);

			ApiServiceInfo[] result;
			using(AssemblyLoader<AssemblyProxy> asm = new AssemblyLoader<AssemblyProxy>())
			{
				asm.Proxy.LoadAssembly(args.DllFile);
				result = asm.Proxy.GetDataItems(path,
					args.Root,
					args.Type.HasFlag(ProcessorType.RestWcf),
					args.Type.HasFlag(ProcessorType.RestWebApi),
					args.Type.HasFlag(ProcessorType.WebService));
			}

			return result;
		}

		/*
		<ItemGroup>
			<COMReference Include="mscoree">
				<Guid>{5477469E-83B1-11D2-8B49-00A0C9B7C9C4}</Guid>
				<VersionMajor>2</VersionMajor>
				<VersionMinor>0</VersionMinor>
				<Lcid>0</Lcid>
				<WrapperTool>tlbimp</WrapperTool>
				<Isolated>False</Isolated>
				<EmbedInteropTypes>True</EmbedInteropTypes>
			</COMReference>
		</ItemGroup>
		private static IEnumerable<AppDomain> EnumAppDomains()
		{
			IntPtr enumHandle = IntPtr.Zero;
			ICorRuntimeHost host = null;

			try
			{
				host = new CorRuntimeHost();
				host.EnumDomains(out enumHandle);
				Object domain = null;

				do
				{
					host.NextDomain(enumHandle, out domain);
					if(domain != null)
						yield return (AppDomain)domain;
				} while(domain != null);
			} finally
			{
				if(host != null)
				{
					if(enumHandle != IntPtr.Zero)
						host.CloseEnum(enumHandle);

					Marshal.ReleaseComObject(host);
				}
			}
		}*/
	}
}
