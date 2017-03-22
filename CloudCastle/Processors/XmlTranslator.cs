using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using CloudCastle.Processors.Reflection;

namespace CloudCastle.Processors
{
	public class XmlTranslator
	{
		private readonly XmlDocument[] _documents;

		public XmlTranslator(String[] xmlPath)
		{
			this._documents = this.LoadXmlFiles(xmlPath);
		}

		public void AnalyzeServices(ApiServiceInfo[] services)
		{
			foreach(var service in services)
			{
				service.Documentation = this.FindDocumentation(service.XmlName);
				if(service.Methods != null)
					this.AnalyzeMethods(service.Methods);
			}
		}

		/// <summary>Получение документации ко всем методам сервиса</summary>
		/// <param name="methods">Массив методов</param>
		private void AnalyzeMethods(ApiMethodInfo[] methods)
		{
			foreach(var method in methods)
			{
				method.Documentation = this.FindDocumentation(method.XmlName);
				if(method.Out != null)
					this.AnalyzeType(method.Out);
				if(method.In != null)
					this.AnalyzeType(method.In);
			}
		}

		private void AnalyzeType(params ApiTypeInfo[] types)
		{
			foreach(ApiTypeInfo type in types)
			{
				type.Documentation = this.FindDocumentation(type.XmlName);
				if(type.Parameters != null)
					this.AnalyzeType(type.Parameters);
				if(type.Base != null)
					this.AnalyzeType(type.Base);
			}
		}

		private XmlDocument[] LoadXmlFiles(String[] xmlPath)
		{
			XmlDocument[] documets = new XmlDocument[xmlPath.Length];
			for(Int32 loop = 0; loop < documets.Length; loop++)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(xmlPath[loop]);
				documets[loop] = doc;
			}
			return documets;
		}

		private String FindDocumentation(String memberName)
		{
			XPathNavigator result = null;
			foreach(XmlDocument document in this._documents)
			{
				var navigator = document.CreateNavigator();
				result = navigator.SelectSingleNode(String.Format("/doc/members/member[@name=\"{0}\"]", memberName));
				if(result != null)
					break;
			}

			if(result == null && !memberName.Contains(":System."))//HACK: Совсем некрасиво...
				Console.WriteLine(String.Format("Description for member {0} not found in XML file", memberName));

			return result == null ? null : result.InnerXml;
		}

		/// <summary>Делегад для определения необходимости добавлять тег в XML</summary>
		/// <param name="propertyName">Наименование свойства</param>
		/// <returns>Тег необходимо добавить в XML</returns>
		public delegate Boolean RenderTag(String propertyName);

		/// <summary>Отформатировать XML строку из свойств объекта</summary>
		/// <param name="source">Исходный элемент</param>
		/// <param name="method">Элемент, который предполагается включить в форматирование</param>
		/// <returns>Результат создания XML строки</returns>
		public static String FormatXmlString(Object source, RenderTag method)
		{
			StringBuilder result = new StringBuilder();
			foreach(PropertyInfo property in source.GetType().GetProperties())
			{
				String name = property.Name;
				Object value = property.GetValue(source, null);
				Object[] attributes = property.GetCustomAttributes(false);
				if(attributes.Length == 1 && attributes[0].GetType() == typeof(XmlIgnoreAttribute))
					continue;
				if(value != null && (method == null || method(property.Name)))
				{
					if(property.PropertyType.IsArray)
					{
						Array arr = (Array)value;
						if(arr.Length > 0)
						{
							result.AppendFormat("<{0}>", name);
							foreach(Object item in (Array)value)
							{
								result.Append("<Item>");
								result.Append(XmlTranslator.FormatXmlString(item, method));
								result.Append("</Item>");
							}
							result.AppendFormat("</{0}>", name);
						}
					} else
						result.AppendFormat("<{0}>{1}</{0}>", name, value);
				}
			}
			return result.ToString().TrimEnd(' ');
		}
	}
}
