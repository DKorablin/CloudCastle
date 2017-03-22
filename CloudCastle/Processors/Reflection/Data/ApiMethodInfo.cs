using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace CloudCastle.Processors.Reflection
{
	[Serializable]
	public class ApiMethodInfo : IApiMember
	{
		private readonly ApiServiceInfo _parent;
		private readonly String _typeName;
		private readonly String _name;
		private readonly String _requestType;

		private readonly ApiTypeInfo[] _in;
		private readonly ApiTypeInfo[] _out;

		[XmlIgnore]
		public ApiServiceInfo Parent { get { return this._parent; } }

		public String Name { get { return this._name; } }
		public String TypeName { get { return this._typeName; } }

		public ApiTypeInfo[] In { get { return this._in; } }
		public ApiTypeInfo[] Out { get { return this._out; } }

		public String RequestType { get { return this._requestType; } }

		public String XmlName
		{
			get
			{
				String inParams = null;
				if(this.In != null)
				{
					inParams = "(" + String.Join(",",
						Array.ConvertAll(this.In,
							delegate(ApiTypeInfo parameter)
							{
								String typeName = parameter.IsNullable
									? String.Format("System.Nullable{{{0}}}", parameter.TypeName)
									: parameter.TypeName;
								return String.Concat(typeName, parameter.IsArray ? "[]" : null);
							})) + ")";
				}

				String result = String.Concat("M:", this.TypeName, inParams);
				return result;
			}
		}

		public String FileName
		{
			get
			{
				String inParams = null;
				if(this.In != null)
				{
					inParams = "(" + String.Join(",",
						Array.ConvertAll(this.In,
							delegate(ApiTypeInfo parameter)
							{
								String typeName = parameter.IsNullable
									? String.Format("System.Nullable{{{0}}}", parameter.TypeName)
									: parameter.TypeName;
								return String.Concat(typeName, parameter.IsArray ? "[]" : null);
							})) + ")";
					inParams = "(" + inParams.GetHashCode().ToString() + ")";//Слишком длинные наименования файлов
				}

				String result = String.Concat("M_", this.TypeName, inParams);

				return result.Replace('.', '_').Replace('?', '_');
			}
		}

		public String Documentation { get; set; }

		public ApiMethodInfo(ApiServiceInfo parent, MethodInfo method, String name, String requestType)
		{
			if(parent == null)
				throw new ArgumentNullException("parent");
			if(method == null)
				throw new ArgumentNullException("method");
			if(String.IsNullOrWhiteSpace(requestType))
				throw new ArgumentNullException("requestType");

			this._parent = parent;
			this._typeName = method.DeclaringType.FullName + "." + method.Name;
			this._name = name;
			this._requestType = requestType;

			List<ApiTypeInfo> inParams = new List<ApiTypeInfo>();
			inParams.AddRange(this.GetInParameters(method));
			List<ApiTypeInfo> outParams = new List<ApiTypeInfo>();
			outParams.AddRange(this.GetOutParameters(method));

			this._in = inParams.Count == 0 ? null : inParams.ToArray();
			this._out = outParams.Count == 0 ? null : outParams.ToArray();
		}

		public XmlElement ToXml(XmlDocument document)
		{
			XmlElement result = document.CreateElement("method");
			result.SetAttribute("name", this.Name);
			result.SetAttribute("path", this.Parent.AbsoluteUri + this.Parent.Name + this.Name);
			result.SetAttribute("request", this.RequestType);
			result.SetAttribute("file", this.Parent.FileName);

			if(this.Documentation != null)
			{
				XmlElement xDocument = document.CreateElement("documentation");
				xDocument.InnerXml = this.Documentation;
				result.AppendChild(xDocument);
			}

			if(this.In != null)
			{
				XmlElement xIn = document.CreateElement("in");
				foreach(ApiTypeInfo parameter in this.In)
					xIn.AppendChild(parameter.ToXml(document));
				result.AppendChild(xIn);
			}
			if(this.Out != null)
			{
				XmlElement xOut = document.CreateElement("out");
				foreach(ApiTypeInfo parameter in this.Out)
					xOut.AppendChild(parameter.ToXml(document));
				result.AppendChild(xOut);
			}

			return result;
		}

		private IEnumerable<ApiTypeInfo> GetInParameters(MethodInfo method)
		{
			foreach(ParameterInfo parameter in method.GetParameters())
			{
				yield return new ApiTypeInfo(parameter.ParameterType, parameter.Name, null, null);
			}
		}

		private IEnumerable<ApiTypeInfo> GetOutParameters(MethodInfo method)
		{
			Type returnType = null;
			if(method.IsDefined(Constant.Reflection.Wcf.WebInvoke))//WCF Service
				returnType = ApiTypeInfo.IsValidType(method.ReturnType) ? method.ReturnType : null;//TODO: WebMessageBodyStyle.Wrapped. В таком случае у параметра будет название
			else if(method.IsDefined(Constant.Reflection.WS.WebMethod))//WebService
				returnType = method.ReturnType == typeof(void) ? null : method.ReturnType;
			else
			{//WebApi Service
				Object[] attributes = method.GetCustomAttributes(Constant.Reflection.WebApi.ResponseType);
				if(attributes.Length == 0)
					returnType = ApiTypeInfo.IsValidType(method.ReturnType) ? method.ReturnType : null;
				else
				{
					foreach(Object attribute in attributes)
					{
						Type responseType = TypeExtender.GetAttributeValue<Type>(attribute, "ResponseType");
						yield return new ApiTypeInfo(responseType, null, null, null);
					}
				}
			}

			if(returnType != null)
				yield return new ApiTypeInfo(returnType, null, null, null);
		}
	}
}