using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using AlphaOmega.Console;
using CloudCastle.Processors;
using CloudCastle.Processors.Reflection;
using CloudCastle.Properties;
using CloudCastle.Input;

namespace CloudCastle
{
	class Program
	{
		/*
		Генерация XML:
		/IX:Application.Crystal.Web.xml;Application.Crystal.Bll.xml;Application.Dal.Crystal.xml /O:"C:\Visual Studio Projects\C#\CloudCastle\Test" /ID:Application.Crystal.Web.dll /R:"https://api.Application.ru/Crystal/v100" /T:8

		Генерация HTML:
		/IX:"C:\Visual Studio Projects\C#\CloudCastle\Test\Xml\M_*.xml" /IT:"C:\Visual Studio Projects\C#\CloudCastle\Test\Template\Xslt\method.xslt" /O:"C:\Visual Studio Projects\C#\CloudCastle\Test\Html" /T:1
		*/
		static void Main(String[] args)
		{
			Result type = Result.InsufficientParameters;
			try
			{
				CmdLineProcessor processor = CmdLineProcessor.CreateProcessor(args);
				DocArgs argsEx = new DocArgs();
				Boolean result = processor.DataBind(argsEx);
				if(result)
					type = argsEx.Validate;

				switch(type)
				{
					case Result.Success:
						Program.InvokeProcessor(argsEx);
						break;
					case Result.InsufficientParameters:
						Console.Write(Resources.Help);
						break;
					case Result.DirectoryNotFound:
						Console.WriteLine("Output path '{0}' not found", argsEx.OutputPath);
						break;
					case Result.InFileNotFound:
						Console.WriteLine("In files for Type='{0}' not found", argsEx.Type);
						break;
				}
			} catch(Exception exc)
			{
				Console.WriteLine(exc.Message);
				if(exc.InnerException != null)
					Console.WriteLine(exc.InnerException.Message);

				type = Result.Exception;
#if DEBUG
				throw;
#endif
			}
			Environment.Exit((Int32)type);
		}

		private static void InvokeProcessor(DocArgs args)
		{
			if(args.Type.HasFlag(ProcessorType.Xslt))
				Program.ApplyXslt(args);
			else if(args.Type.HasFlag(ProcessorType.RestWcf) || args.Type.HasFlag(ProcessorType.RestWebApi) || args.Type.HasFlag(ProcessorType.WebService))
			{
				Console.WriteLine("Creating domain for assembly {0}...", args.DllFile);
				ReflectionAnalyzer analyzer = new ReflectionAnalyzer();
				ApiServiceInfo[] services = analyzer.Invoke(args);

				Console.WriteLine("Reflection returned {0} services.", services.Length);
				Console.WriteLine("Obtaining XML from {0}...", args.XmlFiles);
				XmlTranslator xml = new XmlTranslator(args.XmlFiles);
				xml.AnalyzeServices(services);

				Console.WriteLine("Saving collected data to XML files...");
				FileSystemWorker fs = new FileSystemWorker(args.OutputPath);
				fs.SaveToc(services);
				fs.SaveService(services);
			} else
				throw new NotImplementedException(String.Format("Type '{0}' not implemented", args.Type));
		}

		/// <summary>Генерация исходящего файла документации</summary>
		/// <param name="dargs">Аргументы приложения</param>
		private static void ApplyXslt(DocArgs args)
		{
			XslCompiledTransform xsl = new XslCompiledTransform();
			xsl.Load(args.XsltFile);

			foreach(String path in args.XmlFiles)
			{
				String outFileName = Path.Combine(args.OutputPath, Path.GetFileName(path));

				//TODO: Если происходит перезпись файла, то необходимо сначала считать файл в память, а только потом перезаписывать.
				XmlWriter writer = XmlWriter.Create(outFileName, xsl.OutputSettings);
				xsl.Transform(path, null, writer);
				writer.Close();
			}
		}
	}
}