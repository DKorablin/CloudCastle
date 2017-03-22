using System;
using System.ServiceModel;
using System.ServiceModel.Web;
using TestService.Bll;

namespace TestService.WCF
{
	/// <summary>Sample WCF JSON service</summary>
	[ServiceContract(Namespace = "https://AlphaOmega.somee.com/")]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
	public class Sample
	{
		/// <summary>Sample GET method</summary>
		/// <returns>Returns complex type</returns>
		[OperationContract]
		[WebInvoke(Method = "GET", UriTemplate = "/Get")]
		public GetResponse2 Get()
		{
			return new GetResponse2("value1", 1, Guid.NewGuid(), 2);
		}

		/// <summary>Sample GET method with array result</summary>
		/// <returns>Complex type</returns>
		[OperationContract]
		[WebInvoke(Method="GET",UriTemplate="/GetArray")]
		public GetArrayResponse GetArray()
		{
			return new GetArrayResponse(new String[] { "value1", "value2" });
		}

		/// <summary>Sample POST method</summary>
		/// <param name="value"><c>value</c> value to POST</param>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "/Post", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
		public void Post(String value)
		{

		}
	}
}