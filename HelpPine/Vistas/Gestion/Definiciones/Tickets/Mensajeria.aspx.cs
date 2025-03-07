using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Tickets
{
    public partial class Mensajeria : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CargarMensajes();
                CargarDetallesTicket();
                CargarArchivos();
            }
        }

        private void CargarArchivos()
        {
            int ticketId = ObtenerTicketId();
            if (ticketId == 0)
            {
                rptArchivos.DataSource = null;
                rptArchivos.DataBind();
                lblMensajeArchivos.Text = "El ticket no tiene archivos subidos.";
                lblMensajeArchivos.Visible = true;
                return;
            }

            string query = "SELECT NombreArchivo, RutaArchivo FROM Archivos WHERE idTicket = @idTicket";
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idTicket", ticketId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    lblMensajeArchivos.Text = "El ticket no tiene archivos subidos.";
                    lblMensajeArchivos.Visible = true;
                }
                else
                {
                    lblMensajeArchivos.Visible = false;
                }

                foreach (DataRow row in dt.Rows)
                {
                    string rutaArchivo = row["RutaArchivo"].ToString();
                    row["RutaArchivo"] = ResolveUrl(rutaArchivo);
                }

                rptArchivos.DataSource = dt;
                rptArchivos.DataBind();
            }
        }

        private void CargarDetallesTicket()
        {
            int ticketId = ObtenerTicketId();
            if (ticketId == 0)
            {
                return;
            }

            string query = "SELECT titulo, estado, descripcion, prioridad FROM V_TicketDetalles WHERE idTicket = @TicketId";

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTicketId.Text = ticketId.ToString();
                    lblTitulo.Text = reader["titulo"].ToString();
                    lblDescripcion.Text = reader["descripcion"].ToString();
                    lblEstado.Text = reader["estado"].ToString();
                    lblPrioridad.Text = reader["prioridad"].ToString();
                }
            }
        }

        private void CargarMensajes()
        {
            int ticketId = ObtenerTicketId();
            if (ticketId == 0)
            {
                rptMensajes.DataSource = null;
                rptMensajes.DataBind();
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerMensajesPorTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptMensajes.DataSource = dt;
                rptMensajes.DataBind();
            }
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMensaje.Text))
            {
                return;
            }

            int ticketId = ObtenerTicketId();
            int remitenteId = ObtenerUsuarioId();
            int destinatarioId = ObtenerDestinatarioId();

            string query = "INSERT INTO Mensajes (TicketId, RemitenteId, DestinatarioId, Mensaje, FechaEnvio) VALUES (@TicketId, @RemitenteId, @DestinatarioId, @Mensaje, @FechaEnvio)";

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.Parameters.AddWithValue("@RemitenteId", remitenteId);
                cmd.Parameters.AddWithValue("@DestinatarioId", destinatarioId);
                cmd.Parameters.AddWithValue("@Mensaje", txtMensaje.Text);
                cmd.Parameters.AddWithValue("@FechaEnvio", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Obtener detalles del ticket
            Ticket ticket = ObtenerDetallesTicket(ticketId);

            // Enviar notificación por correo electrónico al destinatario
            string correoDestinatario = ObtenerCorreoDestinatario(destinatarioId);
            string asunto = "Nuevo mensaje en el ticket #" + ticketId;
            string cuerpo = "Se ha enviado un nuevo mensaje en el ticket #" + ticketId + ":\n\n" + txtMensaje.Text;
            string nombreRemitente = ObtenerNombreRemitente(remitenteId);

            EmailHelper.EnviarNotificacion(correoDestinatario, asunto, ticket, nombreRemitente);

            // Enviar notificación por correo electrónico al remitente
            string correoRemitente = ObtenerCorreoDestinatario(remitenteId);
            EmailHelper.EnviarNotificacion(correoRemitente, asunto, ticket, nombreRemitente);

            txtMensaje.Text = string.Empty;
            CargarMensajes();
        }

        private string ObtenerNombreRemitente(int remitenteId)
        {
            string query = "SELECT nombre FROM Usuarios WHERE idUsuario = @RemitenteId";
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RemitenteId", remitenteId);

                conn.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        protected void btnVolverLista_Click(object sender, EventArgs e)
        {
            Response.Redirect("TicketsAbiertos.aspx");
        }

        private int ObtenerTicketId()
        {
            if (Request.QueryString["TicketId"] != null)
            {
                return int.Parse(Request.QueryString["TicketId"]);
            }
            return 0;
        }

        private int ObtenerUsuarioId()
        {
            if (Session["IdUser"] != null)
            {
                return int.Parse(Session["IdUser"].ToString());
            }
            return 0;
        }

        private Ticket ObtenerDetallesTicket(int ticketId)
        {
            Ticket ticket = new Ticket();

            string query = "SELECT titulo, descripcion, prioridad, estado FROM V_TicketDetalles WHERE idTicket = @TicketId";

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ticket.TicketID = ticketId;
                    ticket.Titulo = reader["titulo"].ToString();
                    ticket.Descripcion = reader["descripcion"].ToString();
                    ticket.Prioridad = reader["prioridad"].ToString();
                    ticket.Estado = reader["estado"].ToString();
                }
            }

            return ticket;
        }



        private int ObtenerDestinatarioId()
        {
            int remitenteId = ObtenerUsuarioId();
            int ticketId = ObtenerTicketId();
            string query = "SELECT TecnicoAsignado, CreadoPor FROM Tickets WHERE idTicket = @TicketId";

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int tecnicoAsignado = reader.GetInt32(0);
                    int creadoPor = reader.GetInt32(1);
                    return remitenteId == tecnicoAsignado ? creadoPor : tecnicoAsignado;
                }
            }
            return 0;
        }

        private string ObtenerCorreoDestinatario(int destinatarioId)
        {
            string query = "SELECT email FROM Usuarios WHERE idUsuario = @DestinatarioId";
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DestinatarioId", destinatarioId);

                conn.Open();
                return cmd.ExecuteScalar().ToString();
            }
        }

        protected void TimerMensajes_Tick(object sender, EventArgs e)
        {
            CargarMensajes();
        }
    }
}







