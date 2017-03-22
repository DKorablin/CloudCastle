using System;
using System.Runtime.Serialization;

namespace TestService.Bll
{
	/// <summary>Sample demo class</summary>
	[DataContract(Namespace = "")]
	public class GetResponse
	{
		/// <summary>This is a String property</summary>
		[DataMember]
		public String StringProperty1 { get; set; }

		/// <summary>This is a integer property</summary>
		[DataMember]
		public Int32 IntegerProperty2 { get; set; }

		/// <summary>This is a GUID property</summary>
		[DataMember]
		public Guid UidProperty3 { get; set; }

		/// <summary>Create instance on GetResponse class</summary>
		protected GetResponse() { }//WebService parameterless constructor

		internal GetResponse(String field1, Int32 field2, Guid field3)
		{
			this.StringProperty1 = field1;
			this.IntegerProperty2 = field2;
			this.UidProperty3 = field3;
		}
	}
}