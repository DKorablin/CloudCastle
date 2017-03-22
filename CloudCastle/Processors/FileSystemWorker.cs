using System;
using System.IO;
using System.Text;
using System.Xml;
using CloudCastle.Processors.Reflection;

namespace CloudCastle.Processors
{
	public class FileSystemWorker
	{
		private readonly String _workingFolder;

		public String WorkingFolder { get { return this._workingFolder; } }

		public FileSystemWorker(String workingFolder)
		{
			if(String.IsNullOrWhiteSpace(workingFolder))
				throw new ArgumentNullException("workingFolder");

			this._workingFolder = workingFolder;

			String dataFolder = Path.Combine(this.WorkingFolder, Constant.File.DataFolder);
			if(!Directory.Exists(dataFolder))
				Directory.CreateDirectory(dataFolder);
		}

		public void SaveToc(ApiServiceInfo[] services)
		{
			XmlDocument document = FileSystemWorker.CreateDocument();
			XmlElement root = document.CreateElement("topics");
			foreach(ApiServiceInfo service in services)
			{
				XmlElement xService = document.CreateElement("topic");
				xService.SetAttribute("id", service.XmlName);
				xService.SetAttribute("project", service.Name);
				xService.SetAttribute("file", service.FileName);

				if(service.Methods != null)
					foreach(ApiMethodInfo method in service.Methods)
					{
						XmlElement xMethod = document.CreateElement("topic");
						xMethod.SetAttribute("id", method.XmlName);
						xMethod.SetAttribute("project", method.Name);
						xMethod.SetAttribute("file", method.FileName);
						xService.AppendChild(xMethod);
					}
				root.AppendChild(xService);
			}
			document.AppendChild(root);

			String filePath = Path.Combine(this.WorkingFolder, Constant.File.DataFolder, Constant.File.Toc);
			FileSystemWorker.SaveDocument(document, filePath);
		}

		public void SaveService(params ApiServiceInfo[] services)
		{
			foreach(ApiServiceInfo service in services)
			{
				String filePath = Path.Combine(this.WorkingFolder, Constant.File.DataFolder, service.FileName + ".xml");

				XmlDocument document = FileSystemWorker.CreateDocument();
				document.AppendChild(service.ToXml(document));
				FileSystemWorker.SaveDocument(document, filePath);

				if(service.Methods != null)
					foreach(ApiMethodInfo method in service.Methods)
						this.SaveMethod(method);
			}
		}

		private void SaveMethod(ApiMethodInfo method)
		{
			String filePath = Path.Combine(this.WorkingFolder, Constant.File.DataFolder, method.FileName + ".xml");

			XmlDocument document = FileSystemWorker.CreateDocument();
			document.AppendChild(method.ToXml(document));
			FileSystemWorker.SaveDocument(document, filePath);
		}

		private static XmlDocument CreateDocument()
		{
			XmlDocument document = new XmlDocument();
			document.CreateXmlDeclaration("1.0", "utf-8", null);

			return document;
		}

		private static void SaveDocument(XmlDocument document, String filePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "\t",
				NewLineChars = Environment.NewLine,
				Encoding = Encoding.UTF8,
				NewLineHandling = NewLineHandling.Replace,
			};

			using(XmlWriter writer = XmlWriter.Create(filePath, settings))
				document.Save(writer);
		}
	}
}