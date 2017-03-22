using System;
using System.Web.Services;
using TestService.Bll;

namespace TestService.WS
{
	/// <summary>Sample Web Service</summary>
	[WebService(Namespace = "https://AlphaOmega.somee.com/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class Sample : System.Web.Services.WebService
	{

		/// <summary>Sample GET method</summary>
		/// <returns>Returns complex type</returns>
		[WebMethod]
		public GetResponse2 Get()
		{
			return new GetResponse2("value1", 1, Guid.NewGuid(), 2);
		}

		/// <summary>Sample GET method with array result</summary>
		/// <returns>Complex type</returns>
		[WebMethod]
		public GetArrayResponse GetArray2()
		{
			return new GetArrayResponse(new String[] { "value1", "value2" });
		}

		/// <summary>Sample POST method</summary>
		/// <param name="value"><c>value</c> value to POST</param>
		[WebMethod]
		public void Post(String value)
		{

		}
	}
}
