using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web;

public class EmailHelper
{
    public static void EnviarNotificacion(string destinatario, string asunto, Ticket mensaje, string nombreRemitente)
    {
        // Recuperar las credenciales y servicio desde el web.config
        string usuarioCorreo = ConfigurationManager.AppSettings["USUARIO_CORREO"];
        string contrasenaCorreo = ConfigurationManager.AppSettings["CONTRASENA_CORREO"];
        string servicioCorreo = ConfigurationManager.AppSettings["SERVICIO_CORREO"];

        // Determinar el mensaje basado en el estado del ticket
        string rol = HttpContext.Current.Session["Perfil"] != null ? HttpContext.Current.Session["Perfil"].ToString() : "Usuario";
        string mensajeEstado;
        string tituloCorreo;
        switch (mensaje.Estado.ToLower())
        {
            case "creado":
                mensajeEstado = $"El {rol} ha creado el ticket #{mensaje.TicketID}:";
                tituloCorreo = "¡Nuevo Ticket Creado!";
                break;
            case "asignado":
                mensajeEstado = $"El {rol} ha asignado el ticket #{mensaje.TicketID}:";
                tituloCorreo = "¡Ticket Asignado!";
                break;
            case "cancelado":
                mensajeEstado = $"El {rol} ha cancelado el ticket #{mensaje.TicketID}:";
                tituloCorreo = "¡Ticket Cancelado!";
                break;
            default:
                mensajeEstado = $"El {rol} ha respondido en el ticket #{mensaje.TicketID}:";
                tituloCorreo = "¡Actualización de Ticket!";
                break;
        }

        // Crear el cuerpo del correo con HTML y CSS
        string cuerpoCorreo = $@"
<html>
<head>
    <style>
        body {{
            margin: 0;
            padding: 0;
            background-color: #f0f0f0;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }}
        .email-wrapper {{
            width: 100%;
            padding: 20px;
            background-color: #f0f0f0;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }}
        .email-header {{
            background-color: #007bff;
            padding: 20px;
            text-align: center;
            color: #ffffff;
        }}
        .email-body {{
            padding: 30px;
            color: #333333;
            line-height: 1.6;
        }}
        .email-body h1 {{
            font-size: 24px;
            margin-bottom: 20px;
        }}
        .email-body table {{
            width: 100%;
            border-collapse: collapse;
        }}
        .email-body th, .email-body td {{
            border: 1px solid #dddddd;
            text-align: left;
            padding: 8px;
        }}
        .email-body th {{
            background-color: #f2f2f2;
        }}
        .email-footer {{
            background-color: #e9ecef;
            padding: 15px;
            text-align: center;
            font-size: 12px;
            color: #666666;
        }}
        .btn {{
            display: inline-block;
            background-color: #28a745;
            color: #ffffff;
            padding: 10px 15px;
            margin-top: 20px;
            text-decoration: none;
            border-radius: 4px;
        }}
    </style>
</head>
<body>
    <div class='email-wrapper'>
        <div class='email-container'>
            <div class='email-header'>
                <h2>Notificación de Ticket</h2>
            </div>
            <div class='email-body'>
                <h1>{tituloCorreo}</h1>
                <p>{mensajeEstado}</p>
                <table>
                    <tr>
                        <th>Ticket ID</th>
                        <td>{mensaje.TicketID}</td>
                    </tr>
                    <tr>
                        <th>Título</th>
                        <td>{mensaje.Titulo}</td>
                    </tr>
                    <tr>
                        <th>Descripción</th>
                        <td>{mensaje.Descripcion}</td>
                    </tr>
                    <tr>
                        <th>Prioridad</th>
                        <td>{mensaje.Prioridad}</td>
                    </tr>
                    <tr>
                        <th>Estado</th>
                        <td>{mensaje.Estado}</td>
                    </tr>
                    <tr>
                        <th>Comentario</th>
                        <td>{mensaje.Comentario}</td>
                    </tr>
                </table>
            </div>
            <div class='email-footer'>
                <p>Este correo ha sido generado automáticamente. No responda a este mensaje.</p>
            </div>
        </div>
    </div>
</body>
</html>";

        // Configurar el mensaje de correo
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(usuarioCorreo);
        mail.To.Add(destinatario);
        mail.Subject = asunto;
        mail.Body = cuerpoCorreo;
        mail.IsBodyHtml = true;  // Cambia a false si envías texto plano

        // Configurar el cliente SMTP
        SmtpClient smtp = new SmtpClient(servicioCorreo, 587); // Gmail utiliza el puerto 587 para TLS/SSL
        smtp.EnableSsl = true;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = new NetworkCredential(usuarioCorreo, contrasenaCorreo);

        try
        {
            smtp.Send(mail);
        }
        catch (Exception ex)
        {
            // Manejo básico de errores; en producción, registra el error o notifica de forma adecuada
            throw new Exception("Error al enviar el correo: " + ex.Message);
        }
    }
}


public class Ticket
{
    public int TicketID { get; set; } // Changed from string to int
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Prioridad { get; set; }
    public string Clasificacion { get; set; }
    public string Estado { get; set; }
    public string Comentario { get; set; }
}

