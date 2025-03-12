using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;


namespace HelpPine.Vistas.Gestion.Definiciones.Tickets
{
    public partial class GenerarTickets : Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Inicializar DropDownList de Prioridad
                ddlPrioridad.Items.Clear();
                ddlPrioridad.Items.Add(new ListItem("Baja", "Baja"));
                ddlPrioridad.Items.Add(new ListItem("Media", "Media"));
                ddlPrioridad.Items.Add(new ListItem("Alta", "Alta"));

                // Inicializar DropDownList de Estado
                DropDownList1.Items.Clear();
                DropDownList1.Items.Add(new ListItem("Abierto", "Abierto"));

                // Inicializar DropDownList de Clasificación
                ddlClasificacion.Items.Clear();
                ddlClasificacion.Items.Add(new ListItem("Hardware", "Hardware"));
                ddlClasificacion.Items.Add(new ListItem("Software", "Software"));


            }
        }

        protected void btnGenerarTicket_Click(object sender, EventArgs e)
        {
            // Recoger datos del formulario
            string titulo = txtTitulo.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();
            string prioridad = ddlPrioridad.SelectedValue;
            string estado = DropDownList1.SelectedValue;
            string clasificacion = ddlClasificacion.SelectedValue;
            string comentario = txtComentario.Text.Trim();
            string creadoPor = Session["IdUser"].ToString();
            int ticketID = 0;

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                conn.Open();
                // Insertar el ticket y comentario
                using (SqlCommand cmd = new SqlCommand("spInsertarTicketConComentario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Titulo", titulo);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@Clasificacion", clasificacion);
                    cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                    cmd.Parameters.AddWithValue("@Estado", estado);
                    cmd.Parameters.AddWithValue("@CreadoPor", creadoPor);
                    cmd.Parameters.AddWithValue("@Comentario", comentario);
                    cmd.Parameters.AddWithValue("@IdUsuario", creadoPor);

                    try
                    {
                        object result = cmd.ExecuteScalar();
                        ticketID = result != null ? Convert.ToInt32(result) : 0;
                        if (ticketID == 0)
                        {
                            throw new Exception("El procedimiento almacenado no devolvió un ID de ticket válido.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error al insertar el ticket: " + ex.Message);
                        ScriptManager.RegisterStartupScript(this, GetType(), "mostrarAlertaError", $"mostrarAlertaError('Error al insertar el ticket: {ex.Message}');", true);
                        return;
                    }
                }

                if (ticketID > 0 && FileUpload.HasFiles)
                {
                    foreach (HttpPostedFile uploadedFile in FileUpload.PostedFiles)
                    {
                        try
                        {
                            string originalFileName = Path.GetFileName(uploadedFile.FileName);
                            string extension = Path.GetExtension(originalFileName).ToLower();

                            if (extension == ".exe" || extension == ".js")
                            {
                                Debug.WriteLine("Archivo no permitido: " + originalFileName);
                                continue;
                            }

                            if (uploadedFile.ContentLength > 104857600)
                            {
                                Debug.WriteLine("Archivo demasiado grande: " + originalFileName);
                                continue;
                            }
                            string subFolder = "Otros";
                            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif")
                                subFolder = "Images";
                            else if (extension == ".mp4" || extension == ".avi" || extension == ".mov" || extension == ".wmv")
                                subFolder = "Videos";
                            else if (extension == ".pdf" || extension == ".doc" || extension == ".docx" || extension == ".xls" || extension == ".xlsx")
                                subFolder = "Documents";

                            // Construir la ruta física
                            string saveDir = Server.MapPath("~/Uploads/" + subFolder);
                            if (!Directory.Exists(saveDir))
                                Directory.CreateDirectory(saveDir);
                            string savePath = Path.Combine(saveDir, originalFileName);
                            uploadedFile.SaveAs(savePath);

                            string rutaArchivo = "~/Uploads/" + subFolder + "/" + originalFileName;

                            ViewState["rutaArchivoSeleccionado"] = savePath;

                            string nombreFinalDocumento = EstablecerNombreDocumento(originalFileName);
                            byte[] fileBinary = ViewState["pdfBinario"] as byte[];

                            // Registrar en la BD la ruta y nombre del archivo (usando spInsertarArchivo)
                            using (SqlCommand cmdArchivo = new SqlCommand("spInsertarArchivo", conn))
                            {
                                cmdArchivo.CommandType = CommandType.StoredProcedure;
                                cmdArchivo.Parameters.AddWithValue("@TicketID", ticketID);
                                cmdArchivo.Parameters.AddWithValue("@NombreArchivo", nombreFinalDocumento);
                                cmdArchivo.Parameters.AddWithValue("@RutaArchivo", rutaArchivo);
                                cmdArchivo.ExecuteNonQuery();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error al subir o procesar archivo: " + ex.Message);
                            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarAlertaError", $"mostrarAlertaError('Error al subir o procesar archivo: {ex.Message}');", true);
                            return;
                        }
                    }
                }
            }

            string rol = Session["Perfil"] != null ? Session["Perfil"].ToString() : "Usuario";


            // Enviar notificación al administrador
            string adminCorreo = ConfigurationManager.AppSettings["ADMIN_CORREO"];
            string asunto = "Nuevo ticket creado";
            Ticket mensaje = new Ticket
            {
                TicketID = ticketID,
                Titulo = titulo,
                Descripcion = descripcion,
                Prioridad = prioridad,
                Clasificacion = clasificacion,
                Estado = "creado",
                Comentario = comentario
            };

            try
            {
                EmailHelper.EnviarNotificacion(adminCorreo, asunto, mensaje, rol);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al enviar la notificación: " + ex.Message);
            }

            // Enviar notificación al usuario
            string usuarioCorreo = Session["email"].ToString();
            string asuntoUsuario = "Su ticket ha sido creado";
            try
            {
                EmailHelper.EnviarNotificacion(usuarioCorreo, asuntoUsuario, mensaje, rol);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al enviar la notificación al usuario: " + ex.Message);
            }

            // Mostrar alerta de éxito después de crear el ticket
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarAlertaExito", "mostrarAlertaExito('El ticket ha sido creado correctamente.');", true);

            // Limpiar el formulario después de guardar
            LimpiarFormulario();
        }
        public string EstablecerNombreDocumento(string originalFileName)
        {
            return originalFileName;
        }

        private void LimpiarFormulario()
        {
            Debug.WriteLine("Limpiando formulario...");
            txtTitulo.Text = "";
            txtDescripcion.Text = "";
            txtComentario.Text = "";
        }

        protected void Control_Load(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            int idUsuario = Convert.ToInt32(Session["IdUser"].ToString());
            int idFormulario = int.Parse(control.Attributes["FormId"]);
            control.Visible = false;

            if (hide == 0)
            {
                control.Visible = util.FormulariosAccion(idFormulario, idUsuario);
                var formularios = (List<FormulariosUsuario>)Session["Formularios"];
                var form_req = formularios.Where(f => f.FormId == idFormulario).FirstOrDefault();
            }
        }
    }
}