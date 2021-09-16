# PowerPointConversion
Convert Power Point Documents into PDF or Images Without using Microsoft Powerpoint


# Project Stack 
.Net Core 5
MVC WebApi - Swagger
Postman
Ubuntu Linux
LibreOffice
Ghostscript


* The aim of the project is to provide a REST based service to Convert Power Point Documents to PDF or Images without Using Microsoft Powerpoint.

* It starts the conversion process of the PowerPoint presentation file by converting it to PDF with LibreOffice.

* After the PDF conversion is complete, Ghostscript creates an image with a .png extension for each page in the PDF.

* All operations are done by triggering cli interfaces and linux-shell commands by .Net Core 5.0, no wrapper is used.


Libraries Used:

[LibreOffice](https://www.libreoffice.org/)

[Ghostscript](https://www.ghostscript.com/)

[Ubuntu](https://ubuntu.com/)

[.Net Core 5.0](https://dotnet.microsoft.com/)


## Task List
- [x] Converting PowerPoint Documents to PDF
- [x] Exporting PowerPoint Documents as Image
- [x] Transformation from outside with service call
- [ ] The image format will be parametric
