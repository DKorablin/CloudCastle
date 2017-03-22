using System;
using System.Collections.Generic;
using System.Reflection;

namespace CloudCastle
{
	internal static class TypeExtender
	{
		public static Object[] GetCustomAttributes(this MemberInfo info, String attributeName)
		{
			return Array.FindAll(info.GetCustomAttributes(false), delegate(Object item) { return item.ToString() == attributeName; });
		}

		public static Object GetCustomAttribute(this MemberInfo info, String attributeName)
		{
			return Array.Find(info.GetCustomAttributes(false), delegate(Object item) { return item.ToString() == attributeName; });
		}

		public static T GetAttributeValue<T>(this MemberInfo info, String attributeType, String propertyName)
		{
			Object attribute = Array.Find(info.GetCustomAttributes(false), delegate(Object item) { return item.ToString() == attributeType; });
			if(attribute == null)
				return default(T);

			return (T)attribute.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, attribute, null);
		}

		public static T GetAttributeValue<T>(Object attribute, String propertyName)
		{
			return (T)attribute.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, attribute, null);
		}

		public static Boolean IsDefined(this MemberInfo info, String attributeType)
		{
			return Array.Exists(info.GetCustomAttributes(false), delegate(Object item) { return item.ToString() == attributeType; });
		}

		/// <summary>Проверяет наличие атрибута в заголовке метода</summary>
		/// <param name="info">Информация о методе</param>
		/// <param name="attributeTypes">Типы атрибутов</param>
		/// <returns></returns>
		public static Boolean IsDefined(this MemberInfo info,IEnumerable<String> attributeTypes)
		{
			foreach(String attributeType in attributeTypes)
				if(info.IsDefined(attributeType))
					return true;
			return false;
		}

		public static Type GetRealType(this Type type)
		{
			if(type.IsGenericType)
			{
				Type genericType = type.GetGenericTypeDefinition();
				if(genericType == typeof(System.Nullable<>)
					|| genericType == typeof(System.Collections.Generic.IEnumerator<>)
					|| genericType == typeof(System.Collections.Generic.IEnumerable<>)
					/*|| genericType == typeof(System.Collections.Generic.SortedList<,>)*/)
					return type.GetGenericArguments()[0].GetRealType();
			}
			if(type.HasElementType)
				//if(type.BaseType == typeof(Array))//+Для out и ref параметров
				return type.GetElementType().GetRealType();
			return type;
		}
	}
}