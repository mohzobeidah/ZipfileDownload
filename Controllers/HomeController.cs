using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using ZipfileDownload.Models;

namespace ZipfileDownload.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // public async Task<IActionResult> DownloadMulipleFileWithZip()
    // {

    //     string[] urls = new[] {@"https://download.quranicaudio.com/quran/yasser_ad-dussary/114.mp3",
    //     @"https://download.quranicaudio.com/quran/yasser_ad-dussary/114.mp3"};
    //     var memoryStream = new MemoryStream();
    //     var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

    //     foreach (var url in urls)
    //     {
    //         using var cleint = _httpClientFactory.CreateClient();
    //         await using var stream = await cleint.GetStreamAsync(url);
    //         if (stream is null)
    //             continue;
    //         // var fileInfo = new FileInfo($"{Guid.NewGuid()}.mp3");
    //         // await using var fs = System.IO.File.Create(fileInfo.FullName);
    //         //stream.Seek(0, SeekOrigin.Begin);
    //         //  stream.CopyTo(fs);
    //         // achive 
    //         var entry = zipArchive.CreateEntry($"{Guid.NewGuid()}.mp3");
    //         await using var entryStream = entry.Open();
    //         await stream.CopyToAsync(entryStream);

    //     }


    //     memoryStream.Seek(0, SeekOrigin.Begin);
    //     return File(memoryStream, "application/octet-stream", "Bots.zip");
    // }
    [HttpPost]
    public async Task<IActionResult> DownloadTe()
    {

        string[] urls = new[] {@"https://download.quranicaudio.com/quran/yasser_ad-dussary/114.mp3",
        @"https://download.quranicaudio.com/quran/yasser_ad-dussary/114.mp3"};


        var list = new List<FileInfo>();
        var guidFolder = $"f_{Guid.NewGuid()}";
        
        var fPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) , guidFolder);
        if(!Directory.Exists(fPath))
        Directory.CreateDirectory(fPath);

        foreach (var url in urls)
        {
            var guid = Guid.NewGuid();

            _logger.LogInformation($"Downloading File with GUID=[{guid}].");
            var fileInfo = new FileInfo($@"{fPath}\{guid}.mp3");
            var response = await _httpClientFactory.CreateClient().GetAsync($"{url}?guid={guid}");
            response.EnsureSuccessStatusCode();
            await using var ms = await response.Content.ReadAsStreamAsync();
            await using var fs = System.IO.File.Create(fileInfo.FullName);
            ms.Seek(0, SeekOrigin.Begin);
            ms.CopyTo(fs);

            _logger.LogInformation($"File saved as [{fileInfo.Name}].");
            list.Add(fileInfo);
        }
        // using var zipFileMemoryStream = new MemoryStream();
        var x= await getZipAsync(fPath);
        Directory.Delete(fPath,true);
         return File(x, "application/zip", "my.zip");

    }

    public async Task<MemoryStream> getZipAsync(string DirectoryPath)
    {
       var botFilePaths = Directory.GetFiles(DirectoryPath);
        var memoryStream = new MemoryStream();
     
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in botFilePaths)
                {
                    zipArchive.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }
            memoryStream.Position = 0;
            return memoryStream;
        
    }
}
