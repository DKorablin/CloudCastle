using System;

namespace CloudCastle.Input
{
	[Flags]
	public enum ProcessorType
	{
		None = 0,
		Xslt = 1 << 0,//2
		RestWcf = 1 << 1,//4
		RestWebApi = 1 << 2,//8
		WebService = 1 << 3,//16
	}
}