using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Tickets
{
    public partial class TicketsCerrados : System.Web.UI.Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarTicketsCerrados();
            }
        }

        private void CargarTicketsCerrados()
        {
            string idUsuario = Session["IdUser"]?.ToString();
            string nombreUsuario = Session["Usuario"]?.ToString();
            string perfilUsuario = Session["Perfil"]?.ToString();

            if (!string.IsNullOrEmpty(idUsuario) && !string.IsNullOrEmpty(nombreUsuario) && !string.IsNullOrEmpty(perfilUsuario))
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
                    {
                        conn.Open();
                        SqlCommand cmd;

                        if (nombreUsuario == "aadmin")
                        {
                            // Si el usuario es administrador, obtener todos los tickets cerrados
                            cmd = new SqlCommand("SELECT idTicket, titulo,  email_creador, tecnico_asignado, email_tecnico, perfil_tecnico, estado, fechaCreacion,fechaCierra, departamento FROM Vista_Tickets_Tecnicos WHERE estado = 'Cerrado'", conn);
                        }
                        else
                        {
                            // Si el usuario no es administrador, filtrar por perfil_tecnico
                            cmd = new SqlCommand("SELECT idTicket, titulo,  email_creador, tecnico_asignado, email_tecnico, perfil_tecnico, estado, fechaCreacion, fechaCierra, departamento FROM Vista_Tickets_Tecnicos WHERE perfil_tecnico = @perfil_tecnico AND estado = 'Cerrado'", conn);
                            cmd.Parameters.AddWithValue("@perfil_tecnico", perfilUsuario);
                        }

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            // Mostrar mensaje si no hay tickets cerrados
                            lblNoTickets.Text = "No tienes tickets cerrados.";
                            lblNoTickets.Visible = true;
                            GridViewTickets.Visible = false;
                        }
                        else
                        {
                            // Enlazar datos al GridView si hay tickets
                            GridViewTickets.DataSource = dt;
                            GridViewTickets.DataBind();

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
                // Manejar el caso donde el idUsuario, nombreUsuario o perfilUsuario no es válido
                lblNoTickets.Text = "Error: El ID de usuario, el nombre de usuario o el perfil de usuario no es válido.";
                lblNoTickets.Visible = true;
                GridViewTickets.Visible = false;
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
    }
}
