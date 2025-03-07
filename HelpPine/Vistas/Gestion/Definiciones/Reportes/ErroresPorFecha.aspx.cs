using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Reportes
{
    public partial class ErroresPorFecha : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarErrores();
            }
        }


        private void CargarErrores()
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                // Consulta base a la vista
                string query = "SELECT * FROM vwErroresPorDepartamentoClasificacion";
                string fechaInicio = txtFechaInicio.Text;
                string fechaFin = txtFechaFin.Text;


                var condiciones = new System.Collections.Generic.List<string>();

                // Si se selecciona una fecha de inicio, se filtra por ésta
                if (!string.IsNullOrEmpty(fechaInicio))
                {
                    condiciones.Add("Fecha >= @fechaInicio");
                }
                // Si se selecciona una fecha de fin, se filtra por ésta
                if (!string.IsNullOrEmpty(fechaFin))
                {
                    condiciones.Add("Fecha <= @fechaFin");
                }
                // Agregar las condiciones a la consulta si existen
                if (condiciones.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", condiciones);
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(fechaInicio))
                    {
                        cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                    }
                    if (!string.IsNullOrEmpty(fechaFin))
                    {
                        cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvErroresFecha.DataSource = dt;
                        gvErroresFecha.DataBind();
                    }

                    // Forzar que el encabezado se coloque en la sección <thead>
                    if (gvErroresFecha.HeaderRow != null)
                    {
                        gvErroresFecha.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }

                    string script = "applyDataTable('#gvErroresFecha');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "applyDataTable", script, true);
                }
            }
        }

        // Evento del botón "Buscar" para refrescar los datos
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarErrores();
        }
    }
}






