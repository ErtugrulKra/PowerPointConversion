using System.IO;


/// 
///     Copyright (c)  All Rights Reserved
///
///     Author 		: Ertuğrul KARA 
///     Create Date : 16.09.2021
///     Company 	: 
/// 
///
///     Descriptions: Service Layer of Convert Util
///		

namespace PowerPointToPdfAndImages.Services.Contracts
{
    public interface IPptConvertUtil
    {
        byte[] ConvertPptToImages(string requestId, string fileName, MemoryStream ms);

        byte[] ConvertPptToPdf(string requestId, string fileName, MemoryStream ms);
    }
}
