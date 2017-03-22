using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace DocBuilder.Processors.Reflection
{
	public class AssemblyLoader<O, P> : IDisposable
	{
		private AppDomain _domain;
		private P _proxy;

		/// <summary>Домен, в который загружена сборка</summary>
		private AppDomain Domain { get { return this._domain; } }

		public P Proxy { get { return this._proxy; } }

		public AssemblyLoader()
		{
			this._domain = this.BuildChildDomain(AppDomain.CurrentDomain);
			this._proxy = this.CreateProxy();
		}

		private P CreateProxy()
		{
			Type loaderType = typeof(P);
			if(!loaderType.IsDefined(typeof(SerializableAttribute), false))
				throw new ArgumentException("Loader must be serializable");

			return loaderType.Assembly == null
				? default(P)
				: (P)this.Domain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
		}

		/// <summary>Creates a new AppDomain based on the parent AppDomains Evidence and AppDomainSetup</summary>
		/// <param name="parentDomain">The parent AppDomain</param>
		/// <returns>A newly created AppDomain</returns>
		private AppDomain BuildChildDomain(AppDomain parentDomain)
		{
			Evidence evidence = new Evidence(parentDomain.Evidence);
			AppDomainSetup setup = parentDomain.SetupInformation;
			return AppDomain.CreateDomain(this.GetType().FullName, evidence, setup);
		}

		public void Dispose()
		{
			this._proxy = default(P);
			if(this._domain != null)
			{
				AppDomain.Unload(this._domain);
				this._domain = null;
			}
		}
	}
}