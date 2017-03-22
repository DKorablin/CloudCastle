using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace CloudCastle.Processors.Reflection
{
	[Serializable]
	public class ApiServiceInfo : IApiMember
	{
		private readonly String _name;
		private readonly String _absoluteUri;
		private readonly String _typeName;
		private readonly ApiMethodInfo[] _methods;

		public String Name { get { return this._name; } }

		public String AbsoluteUri { get { return this._absoluteUri; } }

		public String TypeName { get { return this._typeName; } }

		public String XmlName { get { return String.Concat("T:", this.TypeName); } }

		public String FileName { get { return this.XmlName.Replace(':', '_').Replace('.', '_'); } }

		public String Documentation { get; set; }

		public ApiMethodInfo[] Methods { get { return this._methods; } }

		public ApiServiceInfo(Type type, String name, String absoluteUri)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			this._typeName = type.FullName;
			this._name = name;
			this._absoluteUri = absoluteUri;

			List<ApiMethodInfo> methods = new List<ApiMethodInfo>();
			methods.AddRange(this.GetMethods(type));
			this._methods = methods.Count == 0 ? null : methods.ToArray();
			//this._type = type;
		}

		public XmlElement ToXml(XmlDocument document)
		{
			XmlElement result = document.CreateElement("service");
			result.SetAttribute("name", this.Name);
			if(this.AbsoluteUri != null)
				result.SetAttribute("path", this.AbsoluteUri);

			if(this.Documentation != null)
			{
				XmlElement doc = document.CreateElement("documentation");
				doc.InnerXml = this.Documentation;
				result.AppendChild(doc);
			}

			if(this.Methods != null)
			{
				XmlElement methods = document.CreateElement("methods");
				foreach(ApiMethodInfo method in this.Methods)
				{
					XmlElement xMethod = document.CreateElement("method");
					xMethod.SetAttribute("name", method.Name);
					xMethod.SetAttribute("file", method.FileName);
					XmlElement xMethodDoc = document.CreateElement("documentation");

					xMethodDoc.InnerXml = method.Documentation;
					xMethod.AppendChild(xMethodDoc);
					methods.AppendChild(xMethod);
				}
				result.AppendChild(methods);
			}

			return result;
		}

		private IEnumerable<ApiMethodInfo> GetMethods(Type type)
		{
			foreach(MethodInfo method in type.GetMethods())
			{
				String requestType = null;
				String methodName = null;

				if(method.IsDefined(Constant.Reflection.WS.WebMethod))
				{//WebService
					requestType = "SOAP";
					methodName = method.Name;
				}

				if(method.IsDefined(Constant.Reflection.WebApi.Route) ||
					method.IsDefined(Constant.Reflection.WebApi.ActionName) ||
					method.IsDefined(Constant.Reflection.WebApi.HttpMethods.Keys))
				{//WebApi
					foreach(var item in Constant.Reflection.WebApi.HttpMethods)
						if(method.IsDefined(item.Key))
						{
							requestType = item.Value;
							break;
						}
					if(requestType == null)
						throw new NotSupportedException();

					methodName = method.GetAttributeValue<String>(Constant.Reflection.WebApi.Route, "Name");
					if(String.IsNullOrEmpty(methodName))//Поиск в шаблоне
						methodName = method.GetAttributeValue<String>(Constant.Reflection.WebApi.Route, "Template");
					if(String.IsNullOrEmpty(methodName))
						method.GetAttributeValue<String>(Constant.Reflection.WebApi.ActionName, "Name");

					//У метода может не быть названия, если он вызывается по заголовку
					methodName = "/" + methodName;
				}
				else if(method.IsDefined(Constant.Reflection.Wcf.WebInvoke))
				{//WCF
					//Object attribute = Array.Find(method.GetCustomAttributes(false), delegate(Object item) { return item.ToString() == Constant.Reflection.WebApi.Route; });
					Object attribute = method.GetCustomAttribute(Constant.Reflection.Wcf.WebInvoke);
					if(attribute != null)
					{
						requestType = TypeExtender.GetAttributeValue<String>(attribute, "Method");
						methodName = TypeExtender.GetAttributeValue<String>(attribute, "UriTemplate");
					}
				}

				if(methodName != null)
					yield return new ApiMethodInfo(this, method, methodName, requestType);
			}
		}
	}
}