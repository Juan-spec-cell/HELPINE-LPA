using HelpPine.Clases;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

namespace HelpPine.Vistas.Gestion.Definiciones.Tickets
{
    public partial class TicketsAbiertos : System.Web.UI.Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

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
            string idPerfil = Session["Perfil"]?.ToString(); // Obtener el idPerfil de la sesión
            if (!string.IsNullOrEmpty(idUsuario) && !string.IsNullOrEmpty(nombreUsuario) && !string.IsNullOrEmpty(idPerfil))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
                    {
                        conn.Open();
                        SqlCommand cmd;
                        if (idUsuario == "2") // Administrador
                        {
                            // Si el usuario es administrador, obtener todos los tickets
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion,Departamento,prioridad, estado, Creador, CreadorId,TipoTecnico, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso", conn);
                        }
                        else if (idPerfil == "Reportador")
                        {
                            // Si el usuario es reportador, obtener solo los tickets creados por él
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion,Departamento,prioridad, estado, Creador,TipoTecnico, CreadorId, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso WHERE CreadorId = @idUsuario", conn);
                            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        }
                        else
                        {
                            // Para otros perfiles, obtener los tickets asignados al usuario
                            cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion,Departamento,TipoTecnico, prioridad, estado, Creador, CreadorId, TecnicoAsignado, TecnicoId, fechaCreacion, fechaCierra FROM V_TicketsAbiertosEnProceso WHERE TecnicoId = @idUsuario", conn);
                            cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            // Mostrar mensaje si no hay tickets
                            lblNoTickets.Text = "No tienes tickets asignados.";
                            lblNoTickets.Visible = true;
                            GridViewTickets.Visible = false;
                        }
                        else
                        {
                            // Enlazar datos al GridView si hay tickets
                            GridViewTickets.DataSource = dt;
                            GridViewTickets.DataBind();

                            // Ocultar la columna "Creador" para reportadores
                            if (idPerfil == "Reportador")
                            {
                                foreach (DataControlField col in GridViewTickets.Columns)
                                {
                                    if (col.HeaderText.Equals("Creador", StringComparison.OrdinalIgnoreCase)
                                        || col.HeaderText.Equals("Técnico Asignado", StringComparison.OrdinalIgnoreCase)
                                        || col.HeaderText.Equals("Clasificacion", StringComparison.OrdinalIgnoreCase)
                                        )

                                    {
                                        col.Visible = false;
                                    }
                                }
                            }


                            if (idPerfil != "2" && idPerfil != "Reportador")
                            {
                                foreach (DataControlField col in GridViewTickets.Columns)
                                {
                                    if (col.HeaderText.Equals("Tipo Técnico", StringComparison.OrdinalIgnoreCase)
                                        || col.HeaderText.Equals("Técnico Asignado", StringComparison.OrdinalIgnoreCase)
                                        )
                                    {
                                        col.Visible = false;
                                    }
                                }
                            }




                            // Forzar que el encabezado se coloque en la sección <thead>
                            if (GridViewTickets.HeaderRow != null)
                            {
                                GridViewTickets.HeaderRow.TableSection = TableRowSection.TableHeader;
                            }

                            lblNoTickets.Visible = false;
                            GridViewTickets.Visible = true;

                            // Llamar a la función applyDataTable desde el servidor
                            string script = "applyDataTable('#GridViewTickets');";
                            ScriptManager.RegisterStartupScript(this, GetType(), "applyDataTable", script, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error que ocurra durante la ejecución de la consulta SQL
                    lblNoTickets.Text = "Error: " + ex.Message;
                    lblNoTickets.Visible = true;
                    GridViewTickets.Visible = false;
                }
            }
            else
            {
                // Manejar el caso donde el idUsuario, nombreUsuario o idPerfil no es válido
                lblNoTickets.Text = "Error: El ID de usuario, el nombre de usuario o el ID de perfil no es válido.";
                lblNoTickets.Visible = true;
                GridViewTickets.Visible = false;
            }
        }

        protected void GridViewTickets_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int idUsuario = int.Parse(Session["IdUser"].ToString());
                int idTicket = int.Parse(DataBinder.Eval(e.Row.DataItem, "idTicket").ToString());
                int creadorId = int.Parse(DataBinder.Eval(e.Row.DataItem, "CreadorId").ToString());
                string idPerfil = Session["Perfil"]?.ToString();

                if (idPerfil == "Reportador")
                {
                    LinkButton btnResponder = (LinkButton)e.Row.FindControl("btnResponder");
                    btnResponder.Visible = TecnicoHaEnviadoMensaje(idTicket);
                }
                else
                {
                    LinkButton btnEnviarMensaje = (LinkButton)e.Row.FindControl("btnEnviarMensaje");
                    LinkButton btnCerrarTicket = (LinkButton)e.Row.FindControl("btnCerrarTicket");
                    LinkButton btnCancelarTicket = (LinkButton)e.Row.FindControl("btnCancelarTicket");
                    btnEnviarMensaje.Visible = true;
                    btnCerrarTicket.Visible = true;
                    btnCancelarTicket.Visible = true;

                    // Mostrar botón de responder si el usuario es el creador del ticket
                    if (creadorId == idUsuario)
                    {
                        LinkButton btnResponder = (LinkButton)e.Row.FindControl("btnResponder");
                        btnResponder.Visible = true;
                    }
                }
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

        protected void GridViewTickets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Responder")
            {
                int ticketId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"Mensajeria.aspx?TicketId={ticketId}");
            }
            else if (e.CommandName == "EnviarMensaje")
            {
                int ticketId = Convert.ToInt32(e.CommandArgument);
                ActualizarEstadoTicket(ticketId, "En Proceso");
                Response.Redirect($"Mensajeria.aspx?TicketId={ticketId}");
            }
            else if (e.CommandName == "CerrarTicket")
            {
                int ticketId = Convert.ToInt32(e.CommandArgument);
                CerrarTicket(ticketId);
                CargarTicketsAbiertos();
            }
            else if (e.CommandName == "CancelarTicket")
            {
                int ticketId = Convert.ToInt32(e.CommandArgument);
                ActualizarEstadoTicket(ticketId, "Abierto");
                CargarTicketsAbiertos();
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

        protected void Control_Load(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            string idUsuario = Session["IdUser"]?.ToString();
            int idFormulario = int.Parse(control.Attributes["FormId"]);
            control.Visible = false;
            string btn = control.ClientID;

            if (hide == 0)
            {
                control.Visible = util.FormulariosAccion(idFormulario, int.Parse(idUsuario));
                var formularios = (List<FormulariosUsuario>)Session["Formularios"];
                var form_req = formularios.Where(f => f.FormId == idFormulario).FirstOrDefault();
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
        }
    }
}

