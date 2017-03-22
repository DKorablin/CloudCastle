using System;
using System.Runtime.Serialization;
using System.Web.Services;

namespace TestService.Bll
{
	/// <summary>Sample response with basic and complex array</summary>
	[DataContract(Namespace = "")]
	public class GetArrayResponse
	{
		/// <summary>Basic String array</summary>
		[DataMember]
		public String[] StringArray { get; set; }

		/// <summary>Complex array</summary>
		[DataMember]
		public GetArrayResponse2[] ComplexArray { get; set; }

		private GetArrayResponse() { }//WebService parameterless constructor

		internal GetArrayResponse(String[] stringArray)
		{
			this.StringArray = stringArray;
			this.ComplexArray = Array.ConvertAll(stringArray, delegate(String item) { return new GetArrayResponse2(item); });
		}
	}
}