using HelpPine.Clases;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace HelpPine.Vistas.Gestion.Definiciones.Tickets
{
    public partial class TicketsAbiertosGaleria : System.Web.UI.Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarTicketsAbiertos();
            }
        }

        private void CargarTicketsAbiertos()
        {
            string idUsuario = Session["IdUser"]?.ToString();
            string nombreUsuario = Session["Usuario"]?.ToString();
            string idPerfil = Session["Perfil"]?.ToString(); // Se obtiene el perfil de la sesión

            if (!string.IsNullOrEmpty(idUsuario) && !string.IsNullOrEmpty(nombreUsuario) && !string.IsNullOrEmpty(idPerfil))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
                    {
                        conn.Open();
                        SqlCommand cmd;
                        if (idPerfil == "Administrador") // Administrador
                        {
                            // El administrador ve todos los tickets
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion, Departamento, prioridad, estado, Creador, CreadorId, TipoTecnico, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso", conn);
                        }
                        else if (idPerfil == "Reportador")
                        {
                            // El reportador solo ve sus tickets
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion, Departamento, prioridad, estado, Creador, TipoTecnico, CreadorId, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso WHERE CreadorId = @idUsuario", conn);
                            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        }
                        else if (idPerfil == "Tecnico Programador" || idPerfil == "Técnico Soporte")
                        {
                            // Los técnicos solo ven los tickets asignados a ellos
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion, Departamento, TipoTecnico, prioridad, estado, Creador, CreadorId, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso WHERE TecnicoId = @idUsuario", conn);
                            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        }
                        else
                        {
                            // Otros perfiles no ven ningún ticket
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion, Departamento, TipoTecnico, prioridad, estado, Creador, CreadorId, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso WHERE 1 = 0", conn);
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            lblNoTickets.Text = "No tienes tickets asignados.";
                            lblNoTickets.Visible = true;
                            RepeaterTickets.Visible = false;
                        }
                        else
                        {
                            RepeaterTickets.DataSource = dt;
                            RepeaterTickets.DataBind();
                            lblNoTickets.Visible = false;
                            RepeaterTickets.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblNoTickets.Text = "Error: " + ex.Message;
                    lblNoTickets.Visible = true;
                    RepeaterTickets.Visible = false;
                }
            }
            else
            {
                lblNoTickets.Text = "Error: El ID de usuario, el nombre de usuario o el ID de perfil no es válido.";
                lblNoTickets.Visible = true;
                RepeaterTickets.Visible = false;
            }
        }

        protected void RepeaterTickets_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int idTicket = Convert.ToInt32(row["idTicket"]);
                int creadorId = Convert.ToInt32(row["CreadorId"]);
                int idUsuario = int.Parse(Session["IdUser"].ToString());
                string idPerfil = Session["Perfil"].ToString();

                // Lógica para condicionar la visibilidad de los botones según el perfil del usuario
                if (idPerfil == "Reportador")
                {
                    // Para reportadores, se muestra el botón "Responder" solo si el técnico ya envió mensaje
                    LinkButton btnResponder = (LinkButton)e.Item.FindControl("btnResponder");
                    btnResponder.Visible = TecnicoHaEnviadoMensaje(idTicket);

                    // Ocultar los demás botones
                    LinkButton btnEnviarMensaje = (LinkButton)e.Item.FindControl("btnEnviarMensaje");
                    LinkButton btnCerrarTicket = (LinkButton)e.Item.FindControl("btnCerrarTicket");
                    LinkButton btnCancelarTicket = (LinkButton)e.Item.FindControl("btnCancelarTicket");
                    btnEnviarMensaje.Visible = false;
                    btnCerrarTicket.Visible = false;
                    btnCancelarTicket.Visible = false;
                }
                else
                {
                    // Para otros perfiles se muestran los botones de enviar mensaje, cerrar y cancelar
                    LinkButton btnEnviarMensaje = (LinkButton)e.Item.FindControl("btnEnviarMensaje");
                    LinkButton btnCerrarTicket = (LinkButton)e.Item.FindControl("btnCerrarTicket");
                    LinkButton btnCancelarTicket = (LinkButton)e.Item.FindControl("btnCancelarTicket");
                    btnEnviarMensaje.Visible = true;
                    btnCerrarTicket.Visible = true;
                    btnCancelarTicket.Visible = true;

                    // Mostrar el botón "Responder" únicamente si el usuario es el creador del ticket
                    LinkButton btnResponder = (LinkButton)e.Item.FindControl("btnResponder");
                    btnResponder.Visible = (creadorId == idUsuario);
                }
            }
        }

        protected void RepeaterTickets_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int ticketId = Convert.ToInt32(e.CommandArgument);
            string command = e.CommandName;
            if (command == "Responder")
            {
                Response.Redirect($"Mensajeria.aspx?TicketId={ticketId}");
            }
            else if (command == "EnviarMensaje")
            {
                ActualizarEstadoTicket(ticketId, "En Proceso");
                Response.Redirect($"Mensajeria.aspx?TicketId={ticketId}");
            }
            else if (command == "CerrarTicket")
            {
                CerrarTicket(ticketId);
                CargarTicketsAbiertos();
            }
            else if (command == "CancelarTicket")
            {
                CancelarAsignacionTicket(ticketId);
                EnviarNotificacionCancelacion(ticketId);
                CargarTicketsAbiertos();
            }
        }

        private bool TecnicoHaEnviadoMensaje(int idTicket)
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TecnicoHaEnviadoMensaje", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TicketId", idTicket);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void CerrarTicket(int ticketId)
        {
            string query = "UPDATE Tickets SET estado = 'Cerrado', fechaCierra = @FechaCierra WHERE idTicket = @TicketId";
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.Parameters.AddWithValue("@FechaCierra", DateTime.Now);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizarEstadoTicket(int ticketId, string nuevoEstado)
        {
            string query = "UPDATE Tickets SET estado = @NuevoEstado WHERE idTicket = @TicketId";
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                cmd.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            if (nuevoEstado.ToLower() == "abierto")
            {
                EnviarNotificacionCancelacion(ticketId);
            }
        }

        private void EnviarNotificacionCancelacion(int ticketId)
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                conn.Open();
                // Usamos la vista V_GetTicketDetails que creaste
                string query = "SELECT Email, Titulo, Descripcion, Prioridad, Clasificacion, Estado, Comentario FROM V_GetTicketDetails WHERE idTicket = @TicketId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string email = reader["Email"].ToString();
                    string titulo = reader["Titulo"].ToString();
                    string descripcion = reader["Descripcion"].ToString();
                    string prioridad = reader["Prioridad"].ToString();
                    string clasificacion = reader["Clasificacion"].ToString();
                    // Forzamos el estado a "cancelado" para la notificación, sin importar lo que tenga en la BD
                    string estado = "cancelado";
                    string comentario = reader["Comentario"] != DBNull.Value ? reader["Comentario"].ToString() : string.Empty;

                    Ticket mensaje = new Ticket
                    {
                        TicketID = ticketId,
                        Titulo = titulo,
                        Descripcion = descripcion,
                        Prioridad = prioridad,
                        Clasificacion = clasificacion,
                        Estado = estado,
                        Comentario = comentario
                    };

                    try
                    {
                        string rol = Session["Perfil"] != null ? Session["Perfil"].ToString() : "Usuario"; string asunto = "Ticket Cancelado";
                        EmailHelper.EnviarNotificacion(email, asunto, mensaje, rol);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error al enviar la notificación de cancelación: " + ex.Message);
                    }
                }
            }
        }
        private void CancelarAsignacionTicket(int ticketId)
        {
            // Actualiza ambos campos: tecnicoAsignado y TecnicoId a NULL
            string query = "UPDATE Tickets SET tecnicoAsignado = NULL WHERE idTicket = @TicketId;";
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


    }
}


