using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Reportes
{
    public partial class Galeria : System.Web.UI.Page
    {
        // Cantidad de tickets por página
        private int pageSize = 8;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGallery();
            }
        }

        private void BindGallery()
        {
            DataTable dt = ObtenerTickets();
            int totalRecords = dt.Rows.Count;

            // Ajuste de la URL de la imagen
            foreach (DataRow row in dt.Rows)
            {
                string imagenUrl = row["Imagen"].ToString();
                if (string.IsNullOrEmpty(imagenUrl))
                {
                    imagenUrl = ResolveUrl("/Content/img/Logo.jpg");
                    row["Imagen"] = imagenUrl;
                }
                else if (imagenUrl.StartsWith("~/"))
                {
                    imagenUrl = ResolveUrl(imagenUrl);
                    row["Imagen"] = imagenUrl;
                }
            }

            int currentPage = 1;
            if (Request.QueryString["page"] != null)
            {
                int.TryParse(Request.QueryString["page"], out currentPage);
                if (currentPage < 1)
                    currentPage = 1;
            }

            var pagedRows = dt.AsEnumerable().Skip((currentPage - 1) * pageSize).Take(pageSize);
            DataTable dtPaged = dt.Clone();
            foreach (var row in pagedRows)
            {
                dtPaged.ImportRow(row);
            }

            RepeaterGallery.DataSource = dtPaged;
            RepeaterGallery.DataBind();

            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            litPaging.Text = BuildPagingLinks(currentPage, totalPages);
        }

        private string BuildPagingLinks(int currentPage, int totalPages)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<nav><ul class='pagination justify-content-center'>");

            if (currentPage > 1)
                sb.Append($"<li class='page-item'><a class='page-link' href='?page={currentPage - 1}'><strong>Anterior</strong></a></li>");
            else
                sb.Append("<li class='page-item disabled'><span class='page-link'><strong>Anterior</strong></span></li>");

            for (int i = 1; i <= totalPages; i++)
            {
                if (i == currentPage)
                    sb.Append($"<li class='page-item active'><span class='page-link'><strong>{i}</strong></span></li>");
                else
                    sb.Append($"<li class='page-item'><a class='page-link' href='?page={i}'><strong>{i}</strong></a></li>");
            }

            if (currentPage < totalPages)
                sb.Append($"<li class='page-item'><a class='page-link' href='?page={currentPage + 1}'><strong>Siguiente</strong></a></li>");
            else
                sb.Append("<li class='page-item disabled'><span class='page-link'><strong>Siguiente</strong></span></li>");

            sb.Append("</ul></nav>");
            return sb.ToString();
        }

        private DataTable ObtenerTickets()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                string query = "SELECT * FROM vwGaleriaTickets";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // Evento que se dispara al hacer clic en "Ver Detalles"
        protected void RepeaterGallery_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalles")
            {
                int idTicket = Convert.ToInt32(e.CommandArgument);
                // Usamos la vista dbo.vwDetallesTicket que ya une Tickets con Usuarios y concatena Comentarios
                litDetalles.Text = GetDetallesTicket(idTicket);
                litMensajes.Text = GetMensajesTicket(idTicket);
                litArchivos.Text = GetArchivosTicket(idTicket);

                // Mostrar el modal en el cliente
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModal", "$('#ModalDetalles').modal('show');", true);
            }
        }

        private string GetDetallesTicket(int idTicket)
        {
            string detalles = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                conn.Open();
                // Se utiliza la vista vwDetallesTicket para obtener los datos y comentarios del ticket
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vwDetallesTicket WHERE idTicket = @idTicket", conn))
                {
                    cmd.Parameters.AddWithValue("@idTicket", idTicket);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            detalles = $"<strong>ID del Ticket:</strong> {reader["idTicket"]}<br />" +
                                       $"<strong>Título:</strong> {reader["titulo"]}<br />" +
                                       $"<strong>Descripción:</strong> {reader["descripcion"]}<br />" +
                                       $"<strong>Clasificación:</strong> {reader["clasificacion"]}<br />" +
                                       $"<strong>Prioridad:</strong> {reader["prioridad"]}<br />" +
                                       $"<strong>Estado:</strong> {reader["estado"]}<br />" +
                                       $"<strong>Reportador:</strong> {reader["Reportador"]}<br />" +
                                       $"<strong>Técnico asignado:</strong> {reader["Tecnico"]}<br />" +
                                       $"<strong>Fecha de creación:</strong> {reader["fechaCreacion"]}<br />" +
                                       $"<strong>Fecha de cierre:</strong> {reader["fechaCierra"]}<br />" +
                                       $"<hr /><strong>Comentarios:</strong><br />{reader["Comentarios"]}";
                        }
                    }
                }
            }
            return detalles;
        }

        private string GetMensajesTicket(int idTicket)
        {
            string mensajes = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                conn.Open();
                // Se utiliza la vista dbo.vwMensajesTicket para obtener los mensajes
                string query = "SELECT * FROM dbo.vwMensajesTicket WHERE TicketId = @TicketId ORDER BY FechaEnvio";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TicketId", idTicket);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mensajes += $"<li class='list-group-item'><strong>{reader["RemitenteNombre"]} ({reader["TipoRemitente"]}):</strong> {reader["Mensaje"]} " +
                                        $"<span class='text-muted'>({reader["FechaEnvio"]})</span></li>";
                        }
                    }
                }
            }
            return mensajes;
        }

        private string GetArchivosTicket(int idTicket)
        {
            string archivos = string.Empty;
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vwArchivosPorTicket WHERE idTicket = @idTicket", conn))
                {
                    cmd.Parameters.AddWithValue("@idTicket", idTicket);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rutaArchivo = reader["rutaArchivo"].ToString();
                            string urlArchivo = ResolveUrl(rutaArchivo);
                            archivos += $"<li class='list-group-item'><a href='{urlArchivo}' target='_blank'><strong>{reader["nombreArchivo"]}</strong></a> " +
                                        $"<span class='text-muted'>({reader["fechaSubida"]})</span></li>";
                        }
                    }
                }
            }
            return archivos;
        }
    }
}


