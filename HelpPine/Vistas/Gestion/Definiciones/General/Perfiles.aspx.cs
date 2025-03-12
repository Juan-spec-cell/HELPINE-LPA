using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.General
{
    public partial class Perfiles : Page
    {
        readonly Utilitarios util = new Utilitarios();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Session.Remove("idPerfil");
                CargarDatos();
            }
            else
            {
                // Si se lanza un evento desde el cliente (por ejemplo, Detalle)
                string eventTarget = Request["__EVENTTARGET"];
                string eventArgument = Request["__EVENTARGUMENT"];
                if (eventTarget == "Detalles")
                    MostrarDetalles(eventArgument);
            }
        }

        private void CargarDatos()
        {
            try
            {
                DataSet ds = util.ObtenerDS("SELECT * FROM V_Perfiles2", "T");
                ViewState["Table"] = ds.Tables[0];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Refresh(ds.Tables[0], ref gvPerfiles);
                }
                else
                {
                    ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                    gvPerfiles.DataSource = ds.Tables[0];
                    gvPerfiles.DataBind();
                    gvPerfiles.HeaderRow.TableSection = TableRowSection.TableHeader;
                }

                Refresh(ds.Tables[0], ref gvPerfiles);
            }
            catch (Exception ex)
            {
                messages.Value = "Error|red|" + ex.Message;
            }
        }

        protected void gvPerfiles_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            messages.Value = "";

            if (e.CommandName.Equals("Editar"))
            {
                Session["idPerfil"] = e.CommandArgument.ToString();
                Response.Redirect("PerfilesAdmin.aspx", true);
            }
            else if (e.CommandName.Equals("Detalle"))
            {
                try
                {
                    MostrarDetalles(e.CommandArgument.ToString());
                    // No se usa ScriptManager para abrir el modal; se confiará en la lógica jQuery que evalúa ModalActivo
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en comando Detalle: {ex.Message}");
                }
            }

            Refresh((DataTable)ViewState["Table"], ref gvPerfiles);
        }

        private void MostrarDetalles(string idPerfil)
        {
            // Asigna el valor que disparará el modal en el cliente
            ModalActivo.Value = "Detalles";
            DataSet ds = util.ObtenerDS($"SELECT * FROM V_Perfiles WHERE idPerfil = '{idPerfil}'", "T");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lbDescripcion.InnerText = dr["Descripcion"].ToString();
                bool activo;
                // Intenta parsear el valor de "Activo" a booleano
                if (bool.TryParse(dr["Activo"].ToString(), out activo))
                {
                    lbActivo.InnerText = activo ? "Si" : "No";
                }
                else
                {
                    // Si no se pudo parsear, se puede usar otra lógica o asignar un valor por defecto
                    lbActivo.InnerText = "No";
                }
                lbCreado.InnerText = dr["UsuarioCreador"].ToString();
                lbFechaC.InnerText = dr["FechaCreacion"].ToString();
            }
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
    }
}


