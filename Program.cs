using System.IO.Compression;
using System.Net;

class Program
{
	static int progress;
	static void Main()
	{
		try
		{
			Download();
			Extract();
			Move();
			Clear();
			Pause();
		}
		catch (Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Ошибка!");
			Console.WriteLine(ex.ToString());
			Pause();
		}
	}

	static void Pause()
	{
		Console.ForegroundColor = ConsoleColor.White;
		Console.Write("Нажмите любую клавишу, чтобы закрыть это окно:");
		Console.ReadKey();
		Environment.Exit(0);
	}

	static void Download()
	{
		//ссылка
		Uri uri = new("https://getfile.dokpub.com/yandex/get/" + "https://disk.yandex.ru/d/PBneYozqMm8FZg");

		//веб клиент
		WebClient webClient = new();

		//отслеживание прогресса
		webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Progress);
		Timer timer = new(Update, null, 1000, 1000);

		//загрузка
		Console.ForegroundColor = ConsoleColor.Yellow;
		try
		{
			webClient.DownloadFileTaskAsync(uri, "tmp_mods.zip").Wait();
		}
		catch
		{
			File.Delete("tmp_mods.zip");
			webClient.DownloadFileTaskAsync(uri, "tmp_mods.zip").Wait();
		}

		//окончание загрузки
		Update(null);
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"Загрузка завершена");
		timer.Change(Timeout.Infinite, Timeout.Infinite);
	}
	static void Progress(object sender, DownloadProgressChangedEventArgs e)
	{
		progress = e.ProgressPercentage;
	}
	static void Update(object state)
	{
		Console.Clear();
		Console.WriteLine($"Загрузка... {progress}% [{String.Concat(Enumerable.Repeat("#", progress / 5))}" +
												  $"{String.Concat(Enumerable.Repeat("-", 20 - progress / 5))}]");
	}

	static void Extract()
	{
		//распаковка
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"Распаковка...");
		try
		{
			ZipFile.ExtractToDirectory("tmp_mods.zip", "tmp_mods/");
		}
		catch
		{
			Directory.Delete("tmp_mods", true);
			ZipFile.ExtractToDirectory("tmp_mods.zip", "tmp_mods/");
		}

		//окончание распаковки
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"Распаковка завершена");
	}

	static void Move()
	{
		//перемещение
		string dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\.minecraft\\mods";
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine($"Перемещение файлов в {dir}");
		try
		{
			Directory.Delete(dir, true);
		}
		catch { }
		try
		{
			Directory.Move("tmp_mods/mods", dir);
		}
		catch (DirectoryNotFoundException)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"Папка {dir} не найдена, перенесите файлы из tmp_mods/ в .minecraft/ вручную");
			Pause();
		}

		//окончание перемещения
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"Файлы перемещены в {dir}");
	}

	static void Clear()
	{
		//очистка
		File.Delete("tmp_mods.zip");
		Directory.Delete("tmp_mods");
		Console.ForegroundColor = ConsoleColor.Green;
		Console.WriteLine($"Очистка завершена!");
	}
}