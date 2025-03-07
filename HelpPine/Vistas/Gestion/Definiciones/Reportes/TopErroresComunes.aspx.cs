using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Reportes
{
    public partial class TopErroresComunes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarTopErroresComunes();
            }
        }

        private void CargarTopErroresComunes()
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                // Consulta para obtener el top 10 de errores más comunes
                string query = "SELECT * FROM vwTop10ErroresMasComunes";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvTopErroresComunes.DataSource = dt;
                        gvTopErroresComunes.DataBind();
                    }

                    // Forzar que el encabezado se coloque en la sección <thead>
                    if (gvTopErroresComunes.HeaderRow != null)
                    {
                        gvTopErroresComunes.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }

                    string script = "applyDataTable('#gvTopErroresComunes');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "applyDataTable", script, true);
                }
            }
        }
    }
}
