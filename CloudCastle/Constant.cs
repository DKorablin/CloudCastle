using System;
using System.Collections.Generic;

namespace CloudCastle
{
	internal struct Constant
	{
		public struct Reflection
		{
			public struct Wcf
			{
				public const String ServiceContract = "System.ServiceModel.ServiceContractAttribute";
				public const String WebInvoke = "System.ServiceModel.Web.WebInvokeAttribute";
			}
			public struct WebApi
			{
				public const String RoutePrefix = "System.Web.Http.RoutePrefixAttribute";
				public const String Route = "System.Web.Http.RouteAttribute";
				public const String ActionName = "System.Web.Http.ActionNameAttribute";
				public const String ResponseType = "System.Web.Http.Description.ResponseTypeAttribute";

				public static Dictionary<String, String> HttpMethods = new Dictionary<String, String>()
				{
					{"System.Web.Http.HttpDeleteAttribute","DELETE"},
					{"System.Web.Http.HttpGetAttribute","GET"},
					{"System.Web.Http.HttpHeadAttribute","HEAD"},
					{"System.Web.Http.HttpOptionsAttribute","OPTIONS"},
					{"System.Web.Http.HttpPostAttribute","POST"},
					{"System.Web.Http.HttpPutAttribute","PUT"},
					{"System.Web.Http.HttpTraceAttribute","TRACE"},
				};
			}
			public struct WS
			{
				public const String WebService = "System.Web.Services.WebServiceAttribute";
				public const String WebMethod = "System.Web.Services.WebMethodAttribute";
			}
			public const String DataContract = "System.Runtime.Serialization.DataContractAttribute";
			public const String DataMember = "System.Runtime.Serialization.DataMemberAttribute";
		}

		public struct File
		{
			public const String Toc = "toc.xml";
			public const String DataFolder = "Xml";
		}
	}
}