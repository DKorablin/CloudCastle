using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace CloudCastle.Processors.Reflection
{
	[Serializable]
	public class ApiTypeInfo : IApiMember
	{
		private readonly ApiTypeInfo _base;
		private readonly ApiTypeInfo _parent;
		private readonly String _name;
		private readonly String _typeName;
		private readonly String _displayName;
		private readonly String _displayTypeName;
		private readonly ApiTypeInfo[] _parameters;

		private readonly Boolean _isArray;
		private readonly Boolean _isNullable;

		public ApiTypeInfo Base { get { return this._base; } }

		/// <summary>Тип параметра</summary>
		[XmlIgnore]
		public ApiTypeInfo Parent { get { return this._parent; } }

		/// <summary>Видимые вовне параметры типа</summary>
		public ApiTypeInfo[] Parameters { get { return this._parameters; } }

		public String Name { get { return this._name; } }
		public String TypeName { get { return this._typeName; } }
		public String DisplayName { get { return this._displayName; } }
		public String DisplayTypeName { get { return this._displayTypeName; } }
		public Boolean IsArray { get { return this._isArray; } }
		public Boolean IsNullable { get { return this._isNullable; } }

		public String XmlName
		{
			get
			{
				String result;
				if(this.Parent == null)
				{
					String name = String.Concat(
						this.TypeName/*,
						this.IsNullable ? "?" : null,
						this.IsArray ? "[]" : null*/);

					result = String.Concat("T:", name);
				} else
					result = String.Concat("P:", this.Parent.TypeName + "." + this.Name);

				return result;
			}
		}

		public String FileName { get { return this.XmlName.Replace(':', '_').Replace('.', '_'); } }

		public String Documentation { get; set; }

		public ApiTypeInfo(Type type, String name, String displayName, ApiTypeInfo parent)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			this._isArray = type.IsArray;
			if(this.IsArray)
				type = type.GetElementType();

			//Для генериков типа IEnumerable<T> и Nullable<T>
			if(type.IsGenericType)
				if(type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					this._isArray = true;
					type = type.GetGenericArguments()[0];
				}

			if(type.IsGenericType)//TODO: Тут может быть рекурсия генерик параметров
				if(type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					this._isNullable = true;
					type = type.GetGenericArguments()[0];
				}

			this._typeName = type.FullName.Replace('+', '.');//HACK: Так мы поступаем классами в классах

			String displayTypeName = (type.IsEnum
				? type.GetEnumUnderlyingType().FullName
				: type.FullName).Replace('+', '.');
			this._displayTypeName = displayTypeName.Substring(displayTypeName.LastIndexOf('.') + 1);
			this._displayName = displayName ?? name;

			this._name = name;//У return нет названия. TODO: Но название нужно для данных в XML формате

			if(parent == null || parent.TypeName != this.TypeName)
			{//Спасаемся от переполнения стека
				List<ApiTypeInfo> parameters = new List<ApiTypeInfo>();
				parameters.AddRange(this.GetParameters(type));
				this._parameters = parameters.Count == 0 ? null : parameters.ToArray();
			}

			this._parent = parent;
			this._base = this.GetBaseType(type);
		}

		private ApiTypeInfo GetBaseType(Type type)
		{
			Type baseType = type.BaseType;
			return baseType != null && ApiTypeInfo.IsValidType(baseType)
				? new ApiTypeInfo(baseType, null, null, null)
				: null;
		}

		private IEnumerable<ApiTypeInfo> GetParameters(Type type)
		{
			if(type.Assembly.GetName().Name == "mscorlib")
				yield break;

			Boolean isDataContract = type.IsDefined(Constant.Reflection.DataContract);
			foreach(PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
			{
				if(isDataContract)
				{
					Object attribute = property.GetCustomAttribute(Constant.Reflection.DataMember);
					if(attribute != null)
					{
						String displayName = TypeExtender.GetAttributeValue<String>(attribute, "Name");

						yield return new ApiTypeInfo(property.PropertyType, property.Name, displayName, this);
					}
				} else
					yield return new ApiTypeInfo(property.PropertyType, property.Name, property.Name, this);
			}
		}

		public XmlElement ToXml(XmlDocument document)
		{
			XmlElement result = document.CreateElement("parameter");
			if(this.Name != null)
				result.SetAttribute("name", this.Name);
			//if(this.Parameters == null)
			//	result.SetAttribute("type", this.TypeName);//TODO: Заменить на displayType

			result.SetAttribute("type", this.DisplayTypeName);

			result.SetAttribute("isArray", this.IsArray.ToString());
			result.SetAttribute("isNullable", this.IsNullable.ToString());

			if(this.Documentation != null)
			{
				XmlElement doc = document.CreateElement("documentation");
				doc.InnerXml = this.Documentation;
				result.AppendChild(doc);
			}

			if(this.Parameters != null || this.Base != null)
			{
				XmlElement items = document.CreateElement("items");
				if(this.Parameters != null)
					foreach(ApiTypeInfo parameter in this.Parameters)
						items.AppendChild(parameter.ToXml(document));
				if(this.Base != null)
					foreach(ApiTypeInfo parameter in GetBaseParameters(this.Base))
						items.AppendChild(parameter.ToXml(document));

				result.AppendChild(items);
			}

			return result;
		}

		/// <summary>Преобразование древовидного объекта к линейной структуре через рекурсию</summary>
		/// <param name="type">Базовый тип объекта</param>
		/// <returns>Линейная стрктура параметров</returns>
		private static IEnumerable<ApiTypeInfo> GetBaseParameters(ApiTypeInfo type)
		{
			foreach(ApiTypeInfo parameter in type.Parameters)
				yield return parameter;
			if(type.Base != null)
				foreach(ApiTypeInfo parameter in GetBaseParameters(type.Base))
					yield return parameter;
		}

		public static Boolean IsValidType(Type type)
		{
			foreach(PropertyInfo property in type.GetRealType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
				if(property.IsDefined(Constant.Reflection.DataMember))
					return true;
			return false;
		}

		public override String ToString()
		{
			return this.XmlName; //return base.ToString();
		}
	}
}