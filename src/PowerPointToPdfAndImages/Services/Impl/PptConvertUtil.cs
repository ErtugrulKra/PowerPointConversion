using PowerPointToPdfAndImages.Services.Contracts;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;


/// 
///     Copyright (c)  All Rights Reserved
///
///     Author 		: Ertuğrul KARA 
///     Create Date : 16.09.2021
///     Company 	: 
/// 
///
///     Descriptions: Converts Powerpoint to Image or PDf file using by linux libreoffice and Ghostscript
///		


namespace PowerPointToPdfAndImages.Services.Impl
{
    public class PptConvertUtil : IPptConvertUtil
    {
        private readonly int timeOutLimit;
        private readonly string exportRoot;
        public PptConvertUtil()
        {
            this.exportRoot = "/exportRoot";
            this.timeOutLimit = 1000 * 60 * 3;
        }
        public byte[] ConvertPptToImages(string requestId, string fileName, MemoryStream ms)
        {
            try
            {
                DirectoryInfo rootDirectory = new DirectoryInfo(exportRoot);
                if (!rootDirectory.Exists)
                    rootDirectory.Create();

                Console.WriteLine($"Root Klasor {exportRoot} Olustu");

                DirectoryInfo requestDirectory = new DirectoryInfo(Path.Combine(rootDirectory.FullName, requestId));
                if (!requestDirectory.Exists)
                    requestDirectory.Create();

                Console.WriteLine($"{requestDirectory.FullName} Olustu");

                File.WriteAllBytes(Path.Combine(requestDirectory.FullName, fileName), ms.ToArray());

                FileInfo expotedPdf = new FileInfo(Path.Combine(requestDirectory.FullName, fileName));

                string targetPdf = expotedPdf.FullName.Replace(expotedPdf.Extension, ".pdf");

                Process sofficeProcess = new Process();
                sofficeProcess.EnableRaisingEvents = true;
                sofficeProcess.StartInfo = new ProcessStartInfo("soffice", $"--headless --convert-to pdf --outdir {requestDirectory.FullName} {Path.Combine(requestDirectory.FullName, fileName)}");


                Process gsProcess = new Process();
                gsProcess.EnableRaisingEvents = true;
                gsProcess.StartInfo = new ProcessStartInfo($"gs", $"-dSAFER -sDEVICE=pngalpha -o {requestDirectory.FullName}/{expotedPdf.Name}-%02d.png -r96 {targetPdf}");

                sofficeProcess.Start();

                Console.WriteLine($"PDF Donusum Basladi...");
                
                sofficeProcess.Exited += delegate (Object sender, EventArgs e)
                {
                    Console.WriteLine($"PDF Donusum Sonuclandi... Image donusumu Basliyor");
                    gsProcess.Start();
                };

                byte[] fileResult = null;
                gsProcess.Exited += delegate (object sender, EventArgs e)
                {
                    var result = CompressFile(requestDirectory.GetFiles("*.png").OrderBy(s => s.Name).Select(s => s.FullName).ToArray());

                    fileResult = result;
                    Console.WriteLine($"Image Donusum Sonuclandi... {fileResult.Length}");
                };

                int timeout = 0;
                do
                {
                    Thread.Sleep(100);
                    timeout += 100;
                    Console.WriteLine($"Donusum Sonucu Bekleniyor... {fileName}");
                }
                while (fileResult == null && timeout < timeOutLimit);

                //requestDirectory.Delete(true);

                Console.WriteLine($"Sonuc Klasoru Silindi, Sonuc Gonderiliyor");
                
                return fileResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public byte[] ConvertPptToPdf(string requestId, string fileName, MemoryStream ms)
        {
            try
            { 
                DirectoryInfo rootDirectory = new DirectoryInfo(exportRoot);
                if (!rootDirectory.Exists)
                    rootDirectory.Create();

                Console.WriteLine($"Root Klasor {exportRoot} Olustu");

                DirectoryInfo requestDirectory = new DirectoryInfo(Path.Combine(rootDirectory.FullName, requestId));
                if (!requestDirectory.Exists)
                    requestDirectory.Create();

                Console.WriteLine($"{requestDirectory.FullName} Olustu");

                File.WriteAllBytes(Path.Combine(requestDirectory.FullName, fileName), ms.ToArray());

                FileInfo expotedPdf = new FileInfo(Path.Combine(requestDirectory.FullName, fileName));

                string targetPdf = expotedPdf.FullName.Replace(expotedPdf.Extension, ".pdf");

                Process sofficeProcess = new Process();
                sofficeProcess.EnableRaisingEvents = true;
                sofficeProcess.StartInfo = new ProcessStartInfo("soffice", $"--headless --convert-to pdf --outdir {requestDirectory.FullName} {Path.Combine(requestDirectory.FullName, fileName)}");

                sofficeProcess.Start();

                Console.WriteLine($"PDF Donusum Basladi...");

                byte[] fileResult = null;
                sofficeProcess.Exited += delegate (Object sender, EventArgs e)
                {

                    var result = CompressFile( requestDirectory.GetFiles("*.pdf").OrderBy(s => s.Name).Select(s => s.FullName).ToArray());
                    fileResult = result;

                    Console.WriteLine($"PDF Donusum Sonuclandi.. {fileResult.Length}");
                };

                int timeout = 0;
                do
                {
                    Thread.Sleep(100);
                    timeout += 100;
                    Console.WriteLine($"Donusum Sonucu Bekleniyor... {fileName}");
                }
                while (fileResult == null && timeout < timeOutLimit);

                requestDirectory.Delete(true);

                Console.WriteLine($"Sonuc Klasoru Silindi, Sonuc Gonderiliyor");

                return fileResult;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        private byte[] CompressFile(string[] files)
        {
            try
            {
                MemoryStream zipStream = new MemoryStream();
                using (ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in files)
                    {
                        var fileInfo = new FileInfo(item);
                        ZipArchiveEntry zipElaman = zip.CreateEntry(fileInfo.Name, CompressionLevel.Fastest);
                        byte[] unzippedData = File.ReadAllBytes(item);
                        Stream entryStream = zipElaman.Open();
                        entryStream.Write(unzippedData, 0, unzippedData.Length);
                        entryStream.Flush();
                        entryStream.Close();
                    }
                }
                zipStream.Position = 0;

                return  zipStream.ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
