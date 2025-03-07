using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Tickets
{
    public partial class GestionarTickets : System.Web.UI.Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarTickets();
            }
        }

        private void CargarTickets()
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT idTicket, titulo, descripcion, clasificacion, prioridad, estado, creadoPor, tecnicoAsignado, fechaCierra, departamento FROM V_GetTickets WHERE estado != 'Cerrado'", conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                RepeaterTickets.DataSource = dt;
                RepeaterTickets.DataBind();
            }
        }

        protected void RepeaterTickets_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Enlazar técnicos con el idUsuario obtenido a partir de la tabla UsuariosPorPerfil
                DropDownList ddlTecnicos = (DropDownList)e.Item.FindControl("ddlTecnicos");
                if (ddlTecnicos != null)
                {
                    DataTable dtTecnicos = ObtenerTecnicos();
                    ddlTecnicos.DataSource = dtTecnicos;
                    ddlTecnicos.DataTextField = "descripcion";
                    ddlTecnicos.DataValueField = "idUsuario"; // Ahora se utiliza el idUsuario
                    ddlTecnicos.DataBind();

                    string tecnicoAsignado = DataBinder.Eval(e.Item.DataItem, "tecnicoAsignado").ToString();
                    if (!string.IsNullOrEmpty(tecnicoAsignado))
                    {
                        ddlTecnicos.SelectedValue = tecnicoAsignado;
                    }
                }

                // Actualizar el estado del ticket según la base de datos
                DropDownList ddlEstado = (DropDownList)e.Item.FindControl("ddlEstado");
                if (ddlEstado != null)
                {
                    string estadoTicket = DataBinder.Eval(e.Item.DataItem, "estado").ToString();
                    if (ddlEstado.Items.FindByValue(estadoTicket) != null)
                    {
                        ddlEstado.SelectedValue = estadoTicket;
                    }
                }

                // Deshabilitar el botón de guardar si el estado es distinto de "Abierto"
                Button btnGuardar = (Button)e.Item.FindControl("btnGuardar");
                string estadoTicketParaBoton = DataBinder.Eval(e.Item.DataItem, "estado").ToString();
                if (btnGuardar != null && estadoTicketParaBoton != "Abierto")
                {
                    btnGuardar.Enabled = false;
                }
            }
        }

        private DataTable ObtenerTecnicos()
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                // Se modifica la consulta para obtener el idUsuario a partir del idPerfil
                string query = @"
                    SELECT up.idUsuario, t.descripcion 
                    FROM V_Tecnicos t
                    INNER JOIN UsuariosPorPerfil up ON t.idPerfil = up.idPerfil";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Mensaje de depuración
                Console.WriteLine("Número de técnicos recuperados: " + dt.Rows.Count);

                return dt;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            // Obtener controles
            HiddenField hfTicketId = (HiddenField)item.FindControl("hfTicketId");
            DropDownList ddlClasificacion = (DropDownList)item.FindControl("ddlClasificacion");
            DropDownList ddlPrioridad = (DropDownList)item.FindControl("ddlPrioridad");
            DropDownList ddlEstado = (DropDownList)item.FindControl("ddlEstado");
            DropDownList ddlTecnicos = (DropDownList)item.FindControl("ddlTecnicos");
            TextBox txtFechaCierre = (TextBox)item.FindControl("txtFechaCierre");
            Label lblTitulo = (Label)item.FindControl("lblTitulo");
            Label lblDescripcion = (Label)item.FindControl("lblDescripcion");

            // Verificar que se haya seleccionado un técnico
            if (string.IsNullOrEmpty(ddlTecnicos.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Debe seleccionar un técnico.');", true);
                return;
            }

            int idTicket = Convert.ToInt32(hfTicketId.Value);
            string titulo = lblTitulo.Text;
            string descripcion = lblDescripcion.Text;
            string clasificacion = ddlClasificacion.SelectedValue;
            string prioridad = ddlPrioridad.SelectedValue;
            string estado = ddlEstado.SelectedValue;
            int idUsuario = Convert.ToInt32(ddlTecnicos.SelectedValue); // Ahora se obtiene el idUsuario correcto
            DateTime? fechaCierra = string.IsNullOrEmpty(txtFechaCierre.Text)
                ? (DateTime?)null
                : Convert.ToDateTime(txtFechaCierre.Text);

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand("usp_EditarTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idTicket", idTicket);
                cmd.Parameters.AddWithValue("@titulo", titulo);
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@clasificacion", clasificacion);
                cmd.Parameters.AddWithValue("@prioridad", prioridad);
                cmd.Parameters.AddWithValue("@estado", estado);
                cmd.Parameters.AddWithValue("@tecnicoAsignado", idUsuario);
                cmd.Parameters.AddWithValue("@fechaCierra", (object)fechaCierra ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();

                // Obtener el correo electrónico del técnico seleccionado
                SqlCommand cmdEmail = new SqlCommand("SELECT Email FROM V_GetUsuarios WHERE idUsuario = @idUsuario", conn);
                cmdEmail.Parameters.AddWithValue("@idUsuario", idUsuario);
                string emailTecnico = cmdEmail.ExecuteScalar()?.ToString();

                // Enviar notificación al técnico
                if (!string.IsNullOrEmpty(emailTecnico))
                {
                    string asunto = "Nuevo ticket asignado";
                    Ticket mensaje = new Ticket
                    {
                        TicketID = idTicket,
                        Titulo = titulo,
                        Descripcion = descripcion,
                        Prioridad = prioridad,
                        Clasificacion = clasificacion,
                        Estado = estado
                    };
                    try
                    {
                        string nombreRemitente = "Sistema de Gestión de Tickets"; // Añadir el nombre del remitente
                        EmailHelper.EnviarNotificacion(emailTecnico, asunto, mensaje, nombreRemitente);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Error al enviar la notificación al técnico: " + ex.Message);
                    }
                }
            }

            // Deshabilitar el botón guardado en este card
            btn.Enabled = false;

            // Redireccionar a la misma página para refrescar la vista completa con los datos actualizados
            Response.Redirect(Request.RawUrl);
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            // Obtener el ID del ticket
            int idTicket = Convert.ToInt32(((HiddenField)item.FindControl("hfTicketId")).Value);

            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                SqlCommand cmd = new SqlCommand("UPDATE Tickets SET estado = 'Cerrado', fechaCierra = @fechaCierra WHERE idTicket = @idTicket", conn);
                cmd.Parameters.AddWithValue("@idTicket", idTicket);
                cmd.Parameters.AddWithValue("@fechaCierra", DateTime.Now);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Ocultar la tarjeta del ticket
            item.Visible = false;
        }

        protected void ddlEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlEstado = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddlEstado.NamingContainer;

            TextBox txtFechaCierre = (TextBox)item.FindControl("txtFechaCierre");
            if (txtFechaCierre != null)
            {
                if (ddlEstado.SelectedValue == "Cerrado")
                {
                    txtFechaCierre.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    txtFechaCierre.Text = string.Empty;
                }
            }
        }
    }
}


