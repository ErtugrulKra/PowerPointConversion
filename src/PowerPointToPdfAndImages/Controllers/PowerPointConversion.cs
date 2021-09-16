using Microsoft.AspNetCore.Mvc;
using PowerPointToPdfAndImages.Services.Contracts;
using System;
using System.IO;
using System.Net;


/// 
///     Copyright (c)  All Rights Reserved
///
///     Author 		: Ertuğrul KARA 
///     Create Date : 16.09.2021
///     Company 	: 
/// 
///
///     Descriptions: Controller Layer
///		


namespace PowerPointToPdfAndImages.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [DisableRequestSizeLimit]
    public class PowerPointConversion : ControllerBase
    {
        private readonly IPptConvertUtil convertService;
        public PowerPointConversion(IPptConvertUtil convertService)
        {
            this.convertService = convertService;
        }

        /// <summary>
        /// Converts PPT or PPTX file into PDF file.
        /// </summary>
        /// <returns>Compressed PDF File as Filename.ppt.zip</returns>
        [HttpPost("ConvertToPDF")]
        public IActionResult ConvertToPDF()
        {
            try
            {
                string requestId = BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "");

                Console.WriteLine($"{requestId} Basladi");

                MemoryStream ms = new MemoryStream();

                Request.Form.Files[0].CopyTo(ms);

                Console.WriteLine($"File Okundu {ms.Length} byte");

                var result = convertService.ConvertPptToPdf(requestId, Request.Form.Files[0].FileName, ms);

                Console.WriteLine($"Donusum Tamamlandi {result.Length} byte");

                var memory = new MemoryStream(result);

                memory.Position = 0;
                return File(memory, "application/zip", $"{Request.Form.Files[0].FileName}.zip");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Converts PPT or PPTX file into Images, each slaty convert an image.
        /// </summary>
        /// <returns>Compressed images with .png extension</returns>
        [HttpPost("ConvertToImage")]
        public IActionResult ConvertToImage()
        {
            try
            {
                string requestId = BitConverter.ToString(Guid.NewGuid().ToByteArray()).Replace("-", "");

                Console.WriteLine($"{requestId} Basladi");

                MemoryStream ms = new MemoryStream();

                Request.Form.Files[0].CopyTo(ms);

                var result = convertService.ConvertPptToImages(requestId, Request.Form.Files[0].FileName, ms);

                var memory = new MemoryStream(result);

                memory.Position = 0;
                return File(memory, "application/zip", $"{Request.Form.Files[0].FileName}.zip");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
               
            }
        }
    }
}
