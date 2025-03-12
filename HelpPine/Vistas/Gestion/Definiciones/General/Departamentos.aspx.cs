using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;
using System.ComponentModel;

namespace HelpPine.Vistas.Gestion.Definiciones.General
{
    public partial class Departamentos : System.Web.UI.Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataSet ds = util.ObtenerDS("SELECT * FROM V_GetDeptos", "T");
                ViewState["Table"] = ds.Tables[0];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Refresh(ds.Tables[0], ref gvDepartamentos);
                }
                else
                {
                    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                    gvDepartamentos.DataSource = ds.Tables[0];
                    gvDepartamentos.DataBind();
                    gvDepartamentos.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                Refresh(ds.Tables[0], ref gvDepartamentos);
            }
            else
            {
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];
                if (eventTarget == "Add")
                    AddDepto(eventArgument);
                else if (eventTarget == "Detalles")
                    AddDepto(eventArgument);
            }
        }

        private void AddDepto(string idDepartamento)
        {
            // Validación para no guardar datos en blanco
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                messages.Value = "Error|Red|La descripción es obligatoria.";
                return;
            }

            DataTable ds = (DataTable)ViewState["Table"];

            // Verificar si el nombre del departamento ya existe
            DataSet dsCheck = util.ObtenerDS("SELECT COUNT(*) FROM V_GetDeptos WHERE descripcion = '"
                + txtDescripcion.Text.Trim() + "' AND idDepartamento != " + idDepartamento, "T");
            if (dsCheck.Tables[0].Rows[0][0].ToString() != "0")
            {
                messages.Value = "Error|Red|El nombre del departamento ya existe.";
                return;
            }

            try
            {
                SqlCommand comand = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_UpdateDeptos"
                };

                comand.Parameters.Add(new SqlParameter("@idDepartamento", int.Parse(idDepartamento)));
                comand.Parameters.Add(new SqlParameter("@descripcion", txtDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkActivo.Checked));
                comand.Parameters.Add(new SqlParameter("@usuario", "Administrador"));

                System.Diagnostics.Debug.WriteLine("Descripción: " + txtDescripcion.Text.Trim());
                System.Diagnostics.Debug.WriteLine("Activo: " + chkActivo.Checked);

                string mensaje = util.EjecutarProcedimiento(comand);
                if (mensaje == "1")
                {
                    Response.Redirect(Page.Request.Url.ToString(), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    messages.Value = "Error|Red|Ha ocurrido un error al actualizar. " + mensaje;
                    ModalActivo.Value = "Editar";
                    modalTitle.InnerText = "Editar Departamento";
                }
            }
            catch (SqlException ex)
            {
                messages.Value = "Fatal error|Red|Ha ocurrido un error al actualizar el departamento.|" + ex.Message;
                ModalActivo.Value = "Editar";
                modalTitle.InnerText = "Editar Departamento";
            }

            Refresh(ds, ref gvDepartamentos);
        }

        protected void gvDepartamentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            messages.Value = "";

            if (e.CommandName.Equals("Editar"))
            {
                DataSet ds = util.ObtenerDS("SELECT * FROM V_GetDeptos WHERE idDepartamento = '" + e.CommandArgument.ToString() + "'", "T");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    idDepartamento.Value = dr["idDepartamento"].ToString();
                    txtDescripcion.Text = dr["descripcion"].ToString();
                    chkActivo.Checked = bool.Parse(dr["activo"].ToString());
                }

                ModalActivo.Value = "Editar";
                modalTitle.InnerText = "Editar Departamento";
            }
            else if (e.CommandName.Equals("Detalle"))
            {
                ModalActivo.Value = "Detalles";
                DataSet ds = util.ObtenerDS("SELECT * FROM V_GetDeptos WHERE idDepartamento = '" + e.CommandArgument.ToString() + "'", "T");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    lbDescripcion.InnerText = dr["Descripcion"].ToString();
                    lbActivo.InnerText = bool.Parse(dr["Activo"].ToString()) ? "Si" : "No";
                    if (dr["UsuarioModificador"].ToString() != "")
                    {
                        lbModificado.InnerHtml = "<strong>Modificado por: </strong>" + dr["UsuarioModificador"].ToString();
                        lbFechaM.InnerHtml = "<strong>Fecha modificado: </strong>" + dr["FechaModificado"].ToString();
                    }
                    else
                    {
                        lbModificado.InnerHtml = "";
                        lbFechaM.InnerHtml = "";
                    }
                }
            }
            else if (e.CommandName.Equals("Activar"))
            {
                try
                {
                    SqlCommand comand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "usp_ActivateDepto"
                    };

                    comand.Parameters.Add(new SqlParameter("@idDepartamento", int.Parse(e.CommandArgument.ToString())));
                    comand.Parameters.Add(new SqlParameter("@activo", true));
                    comand.Parameters.Add(new SqlParameter("@usuario", "Administrador"));

                    string mensaje = util.EjecutarProcedimiento(comand);
                    if (mensaje == "1")
                    {
                        DataSet ds = util.ObtenerDS("SELECT * FROM V_GetDeptos", "T");
                        ViewState["Table"] = ds.Tables[0];
                        Refresh(ds.Tables[0], ref gvDepartamentos);
                    }
                    else
                    {
                        messages.Value = "Error|Red|Ha ocurrido un error al activar. " + mensaje;
                    }
                }
                catch (SqlException ex)
                {
                    messages.Value = "Fatal error|Red|Ha ocurrido un error al activar el departamento.|" + ex.Message;
                }
            }

            Refresh((DataTable)ViewState["Table"], ref gvDepartamentos);
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGuardarDescripcion.Text))
            {
                // Establecemos el mensaje y registramos el script inmediatamente
                messages.Value = "Error|Red|La descripción es obligatoria.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert",
                    "Swal.fire({ icon: 'error', title: 'Error', text: 'La descripción es obligatoria.', timer: 2000, showConfirmButton: false });", true);
                return;
            }

            DataSet dsCheck = util.ObtenerDS("SELECT COUNT(*) FROM V_GetDeptos WHERE descripcion = '" + txtGuardarDescripcion.Text.Trim() + "'", "T");
            if (dsCheck.Tables[0].Rows[0][0].ToString() != "0")
            {
                messages.Value = "Error|Red|El nombre del departamento ya existe.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert",
                    "Swal.fire({ icon: 'error', title: 'Error', text: 'El nombre del departamento ya existe.', timer: 2000, showConfirmButton: false });", true);
                return;
            }

            try
            {
                SqlCommand comand = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_InsertDepto"
                };

                comand.Parameters.Add(new SqlParameter("@descripcion", txtGuardarDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkGuardarActivo.Checked));
                comand.Parameters.Add(new SqlParameter("@usuario", "Administrador"));

                string mensaje = util.EjecutarProcedimiento(comand);
                if (mensaje == "1")
                {
                    Response.Redirect(Page.Request.Url.ToString(), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    messages.Value = "Error|Red|Ha ocurrido un error al guardar. " + mensaje;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert",
                        $"Swal.fire({{ icon: 'error', title: 'Error', text: 'Ha ocurrido un error al guardar. {mensaje}', timer: 2000, showConfirmButton: false }});", true);
                }
            }
            catch (SqlException ex)
            {
                messages.Value = "Fatal error|Red|Ha ocurrido un error al guardar el departamento.|" + ex.Message;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert",
                    $"Swal.fire({{ icon: 'error', title: 'Fatal error', text: 'Ha ocurrido un error al guardar el departamento. {ex.Message}', timer: 2000, showConfirmButton: false }});", true);
            }

            DataSet ds = util.ObtenerDS("SELECT * FROM V_GetDeptos", "T");
            ViewState["Table"] = ds.Tables[0];
            Refresh(ds.Tables[0], ref gvDepartamentos);
        }

        private void Refresh(DataTable dt, ref GridView grid)
        {
            try
            {
                grid.DataSource = dt;
                grid.DataBind();
                if (dt != null && dt.Rows.Count > 0)
                    grid.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                messages.Value = "Error|Red|" + ex.Message;
            }
        }

        protected void Control_Load(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            int idUsuario = Convert.ToInt32(Session["IdUser"].ToString());
            int idFormulario = int.Parse(control.Attributes["FormId"]);
            control.Visible = false;
            string btn = control.ClientID;

            if (hide == 0)
            {
                control.Visible = util.FormulariosAccion(idFormulario, idUsuario);
                var formularios = (List<FormulariosUsuario>)Session["Formularios"];
                var form_req = formularios.Where(f => f.FormId == idFormulario).FirstOrDefault();
            }
        }

        // Este evento se ejecuta justo antes de renderizar la página y permite inyectar el script del Sweet Alert.
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(messages.Value))
            {
                // Suponiendo el formato "Error|Red|Mensaje" o "Exito|Green|Mensaje"
                string[] parts = messages.Value.Split('|');
                if (parts.Length >= 3)
                {
                    string title = parts[0];
                    string text = parts[2];
                    string icon = "error";
                    if (title.ToLower().Contains("exito"))
                        icon = "success";
                    if (title.ToLower().Contains("fatal"))
                        icon = "error";
                    string script = $"Swal.fire({{ icon: '{icon}', title: '{title}', text: '{text}', timer: 2000, showConfirmButton: false }});";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowAlert", script, true);
                }
            }
        }
    }
}

