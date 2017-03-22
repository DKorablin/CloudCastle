namespace DocBuilder
{
	/// <summary>Результаты выполнения приложения</summary>
	internal enum ResultType : int
	{
		/// <summary>Проверка пройдена успешно</summary>
		Success = 0,
		/// <summary>Исключение в коде</summary>
		Exception = -1,
		/// <summary>Недостаточно параметров</summary>
		InsufficientParameters = -2,
		/// <summary>Входной файл не найден</summary>
		InFileNotFound = -3,
		/// <summary>Выходной файл существует и перезаписывать его не надо</summary>
		DoNotOverwriteOutFile = -4
	}
}