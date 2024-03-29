﻿namespace XACTAS.WEB.Controllers;

[Route("[controller]/[action]")]
public class AppController : ControllerBase
{
    public async Task<IActionResult> DownloadWinApp()
    {
        return new FileContentResult(await System.IO.File.ReadAllBytesAsync("Files/XACTAS Setup.exe"), "application/octeat-stream")
        {
            FileDownloadName = "XACTAS Setup.exe"
        };
    }
}