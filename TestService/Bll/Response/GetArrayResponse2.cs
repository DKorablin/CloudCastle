using System;
using System.Runtime.Serialization;

namespace TestService.Bll
{
	/// <summary>Sample of complex item in the array</summary>
	[DataContract(Namespace = "")]
	public class GetArrayResponse2
	{
		/// <summary>Basic string</summary>
		[DataMember]
		public String Field1 { get; set; }

		/// <summary>Basic Byte array</summary>
		[DataMember]
		public Byte[] Field2 { get; set; }

		private GetArrayResponse2() { }//WebService parameterless constructor

		internal GetArrayResponse2(String field1)
		{
			this.Field1 = field1;
			this.Field2 = System.Text.Encoding.UTF8.GetBytes(this.Field1);
		}
	}
}