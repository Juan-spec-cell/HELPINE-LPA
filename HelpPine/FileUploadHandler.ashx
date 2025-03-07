<%@ WebHandler Language="C#" Class="FileUploadHandler" %>

using System;
using System.IO;
using System.Web;

public class FileUploadHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        try
        {
            context.Response.ContentType = "text/plain";

            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];
                string fileName = Path.GetFileName(file.FileName);
                string uploadPath = context.Server.MapPath("~/Uploads/");

                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string filePath = Path.Combine(uploadPath, fileName);
                file.SaveAs(filePath);

                context.Response.Write("Archivo subido correctamente: " + fileName);
            }
            else
            {
                context.Response.Write("No se recibió ningún archivo.");
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("Error al subir el archivo: " + ex.Message);
        }
    }

    public bool IsReusable
    {
        get { return false; }
    }
}
