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
            DataSet dsCheck = util.ObtenerDS("SELECT COUNT(*) FROM V_GetDeptos WHERE descripcion = '" + txtDescripcion.Text.Trim() + "' AND idDepartamento != " + idDepartamento, "T");
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

                // Agrega los parámetros necesarios
                comand.Parameters.Add(new SqlParameter("@idDepartamento", int.Parse(idDepartamento)));
                comand.Parameters.Add(new SqlParameter("@descripcion", txtDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkActivo.Checked));
                comand.Parameters.Add(new SqlParameter("@usuario", "Administrador"));

                // Mensajes de depuración
                System.Diagnostics.Debug.WriteLine("Descripción: " + txtDescripcion.Text.Trim());
                System.Diagnostics.Debug.WriteLine("Activo: " + chkActivo.Checked);

                string mensaje = util.EjecutarProcedimiento(comand);
                if (mensaje == "1")
                {
                    // Actualización exitosa, refresca la página o la grilla
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
                messages.Value = "Fatal error|red| Ha ocurrido un error al actualizar el departamento.|" + ex.Message;
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

                    // Agrega los parámetros necesarios
                    comand.Parameters.Add(new SqlParameter("@idDepartamento", int.Parse(e.CommandArgument.ToString())));
                    comand.Parameters.Add(new SqlParameter("@activo", true));
                    comand.Parameters.Add(new SqlParameter("@usuario", "Administrador"));

                    string mensaje = util.EjecutarProcedimiento(comand);
                    if (mensaje == "1")
                    {
                        // Activación exitosa, refresca la grilla
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
                    messages.Value = "Fatal error|red| Ha ocurrido un error al activar el departamento.|" + ex.Message;
                }
            }

            Refresh((DataTable)ViewState["Table"], ref gvDepartamentos);
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Validación para no guardar datos en blanco
            if (string.IsNullOrWhiteSpace(txtGuardarDescripcion.Text))
            {
                messages.Value = "Error|Red|La descripción es obligatoria.";
                return;
            }

            // Verificar si el nombre del departamento ya existe
            DataSet dsCheck = util.ObtenerDS("SELECT COUNT(*) FROM V_GetDeptos WHERE descripcion = '" + txtGuardarDescripcion.Text.Trim() + "'", "T");
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
                    CommandText = "usp_InsertDepto"
                };

                // Agrega los parámetros necesarios
                comand.Parameters.Add(new SqlParameter("@descripcion", txtGuardarDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkGuardarActivo.Checked));
                comand.Parameters.Add(new SqlParameter("@usuario", "Administrador"));

                string mensaje = util.EjecutarProcedimiento(comand);
                if (mensaje == "1")
                {
                    // Inserción exitosa, refresca la página o la grilla
                    Response.Redirect(Page.Request.Url.ToString(), false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    messages.Value = "Error|Red|Ha ocurrido un error al guardar. " + mensaje;
                }
            }
            catch (SqlException ex)
            {
                messages.Value = "Fatal error|red| Ha ocurrido un error al guardar el departamento.|" + ex.Message;
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
                if (dt != null)
                    if (dt.Rows.Count > 0)
                        grid.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            catch (Exception ex)
            {
                messages.Value = "Error|red|" + ex.Message;
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


    }
}