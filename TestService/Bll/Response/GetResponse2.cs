using System;
using System.Runtime.Serialization;

namespace TestService.Bll
{
	/// <summary>Extender for sample demo class</summary>
	[DataContract(Namespace = "")]
	public class GetResponse2 : GetResponse
	{
		/// <summary>This is a Decimal property</summary>
		[DataMember]
		public Decimal DecimalProperty4 { get; set; }

		private GetResponse2() : base() { }//WebService parameterless constructor

		internal GetResponse2(String field1, Int32 field2, Guid field3,Decimal field4)
			: base(field1,field2,field3)
		{
			this.DecimalProperty4 = field4;
		}
	}
}