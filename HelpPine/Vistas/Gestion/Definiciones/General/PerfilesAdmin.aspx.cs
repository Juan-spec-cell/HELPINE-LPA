using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.General
{
    public partial class PerfilesAdmin : Page
    {
        readonly Utilitarios util = new Utilitarios();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    // Obtener lista de formularios activos
                    DataSet ds = util.ObtenerDS("SELECT * FROM V_Formularios WHERE Activo = 1", "T");
                    ds.Tables[0].Columns.Add(new DataColumn("Asignado", typeof(bool)));
                    ViewState["Table"] = ds.Tables[0];

                    // Obtener idPerfil de la URL y almacenarlo en la sesión
                    if (Request.QueryString["idPerfil"] != null)
                    {
                        Session["idPerfil"] = Request.QueryString["idPerfil"];
                    }

                    // Modo edición: Si hay un perfil en sesión, cargar su información
                    if (Session["idPerfil"] != null)
                    {
                        ds = util.ObtenerDS($"usp_GetInfoPerfiles '{Session["idPerfil"].ToString()}'", "T");
                        ds.Tables[1].Columns.Add(new DataColumn("Asignado", typeof(bool)));

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            txtDescripcion.Text = ds.Tables[0].Rows[0][0].ToString();
                            chkActivo.Checked = bool.Parse(ds.Tables[0].Rows[0][1].ToString());

                            // Marcar los formularios que ya están asignados al perfil
                            foreach (DataRow dr in ds.Tables[1].Rows)
                            {
                                dr["Asignado"] = false;
                                foreach (DataRow dr2 in ds.Tables[2].Rows)
                                {
                                    if (dr2["IdFormulario"].ToString() == dr["IdFormulario"].ToString())
                                        dr["Asignado"] = true;
                                }
                            }
                        }
                        ViewState["Table"] = ds.Tables[1];
                    }
                    else
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                            dr["Asignado"] = false;
                    }

                    // Añadir columna "row" para numerar las filas
                    DataTable dt = (DataTable)ViewState["Table"];
                    if (!dt.Columns.Contains("row"))
                    {
                        dt.Columns.Add("row", typeof(int));
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i]["row"] = i + 1;
                        }
                    }

                    Gridview1.DataSource = dt;
                    Gridview1.DataBind();

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        Gridview1.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                else
                {
                    string eventTarget = Request["__EVENTTARGET"];
                    string eventArgument = Request["__EVENTARGUMENT"];
                    if (eventTarget == "Add")
                        Guardar(eventArgument);
                }
            }
            catch (Exception ex)
            {
                // Registrar el error en la consola
                Console.WriteLine("Error en Page_Load: " + ex.Message);
                messages.Value = "Error|red|" + ex.Message;
            }
        }
        private void Guardar(string idFormularios)
        {
            DataTable ds = (DataTable)ViewState["Table"];
            try
            {
                Console.WriteLine("idFormularios recibido: " + idFormularios);

                if (!string.IsNullOrEmpty(idFormularios))
                {
                    string[] formularios = idFormularios.Split(',');
                    foreach (string item in formularios)
                    {
                        if (int.TryParse(item, out int idFormulario))
                        {
                            DataRow row = ds.Select($"IdFormulario = '{idFormulario}'").FirstOrDefault();
                            if (row != null)
                                row["Asignado"] = !bool.Parse(row["Asignado"].ToString());
                        }
                        else
                        {
                            messages.Value = "Error|red|El valor de formulario no es válido.";
                            return;
                        }
                    }
                }

                string asignar = string.Join(",", ds.Rows.Cast<DataRow>()
                    .Where(dr => dr["Asignado"].ToString() == "True")
                    .Select(dr => dr["IdFormulario"].ToString()));

                Console.WriteLine("Formularios asignados: " + asignar);

                SqlCommand comand = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "USP_InsertPerfiles"
                };
                comand.Parameters.Add(new SqlParameter("@idPerfil", Session["idPerfil"] == null ? 0 : int.Parse(Session["idPerfil"].ToString())));
                comand.Parameters.Add(new SqlParameter("@descripcion", txtDescripcion.Text.Trim()));
                comand.Parameters.Add(new SqlParameter("@activo", chkActivo.Checked)); // Capturar el estado del checkbox aquí
                comand.Parameters.Add(new SqlParameter("@formularios", asignar));
                comand.Parameters.Add(new SqlParameter("@usuario", Session["usuario"].ToString()));

                Console.WriteLine("Parámetros enviados al procedimiento almacenado:");
                foreach (SqlParameter param in comand.Parameters)
                {
                    Console.WriteLine($"{param.ParameterName}: {param.Value}");
                }

                string mensaje = util.EjecutarProcedimiento(comand);
                if (mensaje == "1")
                {
                    Response.Redirect("Perfiles.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                    messages.Value = "Error|red|Ha ocurrido un error al guardar. " + mensaje;
            }
            catch (SqlException ex)
            {
                // Registrar el error en la consola
                Console.WriteLine("Error en Guardar: " + ex.Message);
                messages.Value = "Fatal error|red|" + ex.Message;
            }

            ViewState["formularios"] = idFormularios;
            ViewState["Table"] = ds;
            Gridview1.DataSource = ds;
            Gridview1.DataBind();
            Gridview1.HeaderRow.TableSection = TableRowSection.TableHeader;
        }



    }
}







