using System;

namespace CloudCastle.Input
{
	internal enum Result
	{
		Success,
		Exception = -1,
		InsufficientParameters = -2,
		InFileNotFound = -3,
		DirectoryNotFound = -4
	}
}
