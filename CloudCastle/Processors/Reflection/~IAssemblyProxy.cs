using System;

namespace DocBuilder.Processors.Reflection
{
	public interface IAssemblyProxy
	{
		void LoadAssembly(String assemblyPath);
	}
}