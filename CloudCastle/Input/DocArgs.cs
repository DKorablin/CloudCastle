using AlphaOmega.Console;
using System;
using System.Collections.Generic;
using System.IO;

namespace CloudCastle.Input
{
	internal class DocArgs
	{
		#region Fields
		private String[] _xmlFiles;
		private String _root;
		private String _dllFile;
		private String _outPath;
		private String _xsltFile;
		private ProcessorType _type;
		#endregion Fields

		#region Properties
		[CmdLine("InXml", "IX", "Входящие XML файлы сгенерённые студией", IsRequired = true)]
		public String[] XmlFiles
		{
			get { return this._xmlFiles; }
			private set { this._xmlFiles = value; }
		}

		[CmdLine("InDll", "ID", "Входящие DLL файлы сгенерённые студией", IsRequired = false)]
		public String DllFile
		{
			get { return this._dllFile; }
			private set { this._dllFile = value; }
		}

		[CmdLine("Out", "O", "Исходящая папка", IsRequired = false)]
		public String OutputPath
		{
			get { return this._outPath; }
			private set { this._outPath = value; }
		}

		[CmdLine("InXslt", "IT", "XSLT файл преобразования", IsRequired = false)]
		public String XsltFile
		{
			get { return this._xsltFile; }
			private set { this._xsltFile = value; }
		}

		[CmdLine("Root", "R", "Абсолютная ссылка для сервиса", IsRequired = false)]
		public String Root
		{
			get { return this._root; }
			private set { this._root = value; }
		}

		[CmdLine("Type", "T", "Тип запускаемого процессора", IsRequired = false, DefaultValue = ProcessorType.Xslt)]
		public ProcessorType Type
		{
			get { return this._type; }
			private set { this._type = value; }
		}

		public Result Validate
		{
			get
			{
				this.XmlFiles = DocArgs.CheckExistence(this.XmlFiles);
				this.DllFile = ((this.DllFile == null) ? null : Path.GetFullPath(this.DllFile));
				this.OutputPath = ((this.OutputPath == null) ? Environment.CurrentDirectory : Path.GetFullPath(this.OutputPath));
				if(!Directory.Exists(this.OutputPath))
				{
					Directory.CreateDirectory(this.OutputPath);
				}
				Result result;
				if(this.XmlFiles == null || String.IsNullOrEmpty(this.OutputPath))
					result = Result.InsufficientParameters;
				else if(!Directory.Exists(this.OutputPath))
					result = Result.DirectoryNotFound;
				else if((this.Type.HasFlag(ProcessorType.RestWcf) || this.Type.HasFlag(ProcessorType.RestWebApi)) && !File.Exists(this.DllFile))
					result = Result.InFileNotFound;
				else if(this.Type.HasFlag(ProcessorType.Xslt) && !File.Exists(this.XsltFile))
					result = Result.InFileNotFound;
				else
					result = Result.Success;
				return result;
			}
		}
		#endregion Properties

		private static String[] CheckExistence(params String[] files)
		{
			if(files == null || files.Length == 0)
				return null;

			List<String> result = new List<String>(files.Length);
			foreach(String file in files)
				if(file.IndexOfAny(new Char[] { '*', '?' }) > -1)
				{
					Int32 index = file.LastIndexOf('\\');
					String path = index > -1 ? file.Substring(0, index) : Environment.CurrentDirectory;
					if(Directory.Exists(path))
					{
						String searchPattern = file.Substring(index + 1, file.Length - (index + 1));
						foreach(String searchFile in Directory.EnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly))
							result.Add(Path.GetFullPath(searchFile));
					}
				} else if(File.Exists(file))
					result.Add(Path.GetFullPath(file));

			return result.Count > 0
				? result.ToArray()
				: null;
		}
	}
}