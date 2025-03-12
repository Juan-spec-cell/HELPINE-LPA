using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;
using System.Linq;

namespace HelpPine.Vistas.Gestion.Definiciones.General
{
    public partial class Formularios : System.Web.UI.Page
    {
        readonly Utilitarios util = new Utilitarios();
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataSet ds = util.ObtenerDS("SELECT * FROM V_Formularios", "T");
                ViewState["Table"] = ds.Tables[0];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Refresh(ds.Tables[0], ref gvFormularios);
                }
                else
                {
                    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                    gvFormularios.DataSource = ds.Tables[0];
                    gvFormularios.DataBind();
                    gvFormularios.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                Refresh(ds.Tables[0], ref gvFormularios);
            }
            else
            {
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];
                if (eventTarget == "Add")
                    AddFormulario(eventArgument);
                else if (eventTarget == "Detalles")
                    MostrarDetalles(eventArgument);
            }
        }

        private void AddFormulario(string idFormulario)
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                messages.Value = "Error|Red|La descripción es obligatoria.";
                return;
            }

            DataTable dt = (DataTable)ViewState["Table"];

            DataSet dsCheck = util.ObtenerDS($"SELECT COUNT(*) FROM V_Formularios WHERE descripcion = '{txtDescripcion.Text.Trim()}' AND idFormulario != {idFormulario}", "T");
            if (dsCheck.Tables[0].Rows[0][0].ToString() != "0")
            {
                messages.Value = "Error|Red|El nombre del formulario ya existe.";
                return;
            }

            try
            {
                SqlCommand comand = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_UpdateFormulario"
                };

                comand.Parameters.Add(new SqlParameter("@idFormulario", int.Parse(idFormulario)));
                comand.Parameters.Add(new SqlParameter("@descripcion", txtDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkActivo.Checked));


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
                    modalTitle.InnerText = "Editar Formulario";
                }
            }
            catch (SqlException ex)
            {
                messages.Value = "Fatal error|red| Ha ocurrido un error al actualizar el formulario.|" + ex.Message;
                ModalActivo.Value = "Editar";
                modalTitle.InnerText = "Editar Formulario";
            }

            Refresh(dt, ref gvFormularios);
        }

        protected void gvFormularios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            messages.Value = "";

            if (e.CommandName.Equals("Editar"))
            {
                DataSet ds = util.ObtenerDS($"SELECT * FROM V_Formularios WHERE idFormulario = '{e.CommandArgument.ToString()}'", "T");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    idFormulario.Value = dr["idFormulario"].ToString();
                    txtDescripcion.Text = dr["descripcion"].ToString();
                    chkActivo.Checked = bool.Parse(dr["activo"].ToString());
                }

                ModalActivo.Value = "Editar";
                modalTitle.InnerText = "Editar Formulario";
            }
            else if (e.CommandName.Equals("Detalle"))
            {
                MostrarDetalles(e.CommandArgument.ToString());
            }
            else if (e.CommandName.Equals("Activar"))
            {
                try
                {
                    SqlCommand comand = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "usp_ActivateFormulario"
                    };

                    comand.Parameters.Add(new SqlParameter("@idFormulario", int.Parse(e.CommandArgument.ToString())));
                    comand.Parameters.Add(new SqlParameter("@activo", true));

                    string mensaje = util.EjecutarProcedimiento(comand);
                    if (mensaje == "1")
                    {
                        DataSet ds = util.ObtenerDS("SELECT * FROM V_Formularios", "T");
                        ViewState["Table"] = ds.Tables[0];
                        Refresh(ds.Tables[0], ref gvFormularios);
                    }
                    else
                    {
                        messages.Value = "Error|Red|Ha ocurrido un error al activar. " + mensaje;
                    }
                }
                catch (SqlException ex)
                {
                    messages.Value = "Fatal error|red| Ha ocurrido un error al activar el formulario.|" + ex.Message;
                }
            }

            Refresh((DataTable)ViewState["Table"], ref gvFormularios);
        }

        private void MostrarDetalles(string idFormulario)
        {
            ModalActivo.Value = "Detalles";
            DataSet ds = util.ObtenerDS($"SELECT * FROM V_Formularios WHERE idFormulario = '{idFormulario}'", "T");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lbDescripcion.InnerText = dr["descripcion"].ToString();
                lbActivo.InnerText = bool.Parse(dr["activo"].ToString()) ? "Sí" : "No";
                lbFechaC.InnerText = dr["FechaCreacion"].ToString();
            }
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGuardarDescripcion.Text))
            {
                messages.Value = "Error|Red|La descripción es obligatoria.";
                return;
            }

            DataSet dsCheck = util.ObtenerDS($"SELECT COUNT(*) FROM V_Formularios WHERE descripcion = '{txtGuardarDescripcion.Text.Trim()}'", "T");
            if (dsCheck.Tables[0].Rows[0][0].ToString() != "0")
            {
                messages.Value = "Error|Red|El nombre del formulario ya existe.";
                return;
            }

            try
            {
                SqlCommand comand = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_InsertFormulario"
                };

                comand.Parameters.Add(new SqlParameter("@descripcion", txtGuardarDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkGuardarActivo.Checked));

                string mensaje = util.EjecutarProcedimiento(comand);
                if (mensaje == "1")
                {
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
                messages.Value = "Fatal error|red| Ha ocurrido un error al guardar el formulario.|" + ex.Message;
            }

            DataSet ds = util.ObtenerDS("SELECT * FROM V_Formularios", "T");
            ViewState["Table"] = ds.Tables[0];
            Refresh(ds.Tables[0], ref gvFormularios);
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
