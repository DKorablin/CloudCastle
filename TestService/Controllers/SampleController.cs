using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using TestService.Bll;

namespace TestService.Controllers
{
	/// <summary>Documentation for sample controller</summary>
	[RoutePrefix("Sample")]
	public class SampleController : ApiController
	{
		// GET api/<controller>
		/// <summary>Basic GET method</summary>
		/// <returns>Returns "value1"</returns>
		[HttpGet]
		[Route]
		public String Get()
		{
			return "value1";
		}

		// GET api/<controller>/5
		/// <summary>Extended GET method</summary>
		/// <param name="asArray">returns data as array or as single class</param>
		/// <returns></returns>
		[HttpGet]
		[Route]
		[ResponseType(typeof(GetResponse))]
		[ResponseType(typeof(GetArrayResponse))]
		public IHttpActionResult Get(Boolean asArray)
		{
			if(asArray)
				return Ok(new GetArrayResponse(new String[] { "value1", "value2" }));
			else
				return Ok(new GetResponse2("value1", 1, Guid.NewGuid(), 2));
		}

		// POST api/<controller>
		/// <summary>Sample POST method</summary>
		/// <param name="value"><c>value</c> to POST</param>
		[HttpPost]
		[Route]
		public void Post([FromBody]String value)
		{
		}

		// PUT api/<controller>/5
		/// <summary>Sample PUT method</summary>
		/// <param name="id">id to PUT</param>
		/// <param name="value"><c>value</c> to PUT</param>
		[HttpPut]
		[Route]
		public void Put(Int32 id, [FromBody]String value)
		{
		}

		// DELETE api/<controller>/5
		/// <summary>Sample DELETE method</summary>
		/// <param name="id"><c>id</c> to delete</param>
		[HttpDelete]
		[Route]
		public void Delete(Int32 id)
		{
		}
	}
}