using System;
using System.Xml;

namespace CloudCastle.Processors.Reflection
{
	public interface IApiMember
	{
		/// <summary>Наименование объекта</summary>
		String Name { get; }

		/// <summary>Наименование типа объекта</summary>
		String TypeName { get; }

		/// <summary>Наименование объекта в XML для получения документации от студии</summary>
		String XmlName { get; }

		/// <summary>Наименование файла для объекта</summary>
		String FileName { get; }

		/// <summary>Документация для метода из XML документации от студии</summary>
		/// <remarks>Добавляется отдельным процессором</remarks>
		String Documentation { get; set; }

		/// <summary>Трансформировать объект в XML</summary>
		/// <returns>XML содержимое объекта, но без корневого элемента</returns>
		XmlElement ToXml(XmlDocument document);
	}
}