using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.Reportes
{
    public partial class ErroresPorDepartamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDepartamentos();
                CargarErrores();
            }
        }

        // Método para cargar el DropDownList con los departamentos activos
        private void CargarDepartamentos()
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                string query = "SELECT idDepartamento, descripcion FROM Departamentos WHERE Activo = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    ddlDepartamentos.DataSource = cmd.ExecuteReader();
                    ddlDepartamentos.DataTextField = "descripcion";
                    ddlDepartamentos.DataValueField = "idDepartamento";
                    ddlDepartamentos.DataBind();
                }
            }
            // Agregar opción para ver todos los departamentos
            ddlDepartamentos.Items.Insert(0, new ListItem("Todos", "0"));
        }

        // Método para cargar los errores desde la vista
        private void CargarErrores()
        {
            using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
            {
                // Consulta base a la vista
                string query = "SELECT * FROM vwErroresPorDepartamentoClasificacion";
                int idDept = Convert.ToInt32(ddlDepartamentos.SelectedValue);
                string clasificacion = ddlClasificacion?.SelectedValue; // Usar el operador de condicional nulo

                // Lista para almacenar condiciones
                var condiciones = new System.Collections.Generic.List<string>();

                // Si se selecciona un departamento específico, se filtra por éste
                if (idDept > 0)
                {
                    condiciones.Add("Departamento = (SELECT descripcion FROM Departamentos WHERE idDepartamento = @idDept)");
                }
                // Si se selecciona una clasificación (Hardware o Software), se filtra por ésta
                if (!string.IsNullOrEmpty(clasificacion))
                {
                    condiciones.Add("Clasificacion = @clasificacion");
                }
                // Agregar las condiciones a la consulta si existen
                if (condiciones.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", condiciones);
                }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (idDept > 0)
                    {
                        cmd.Parameters.AddWithValue("@idDept", idDept);
                    }
                    if (!string.IsNullOrEmpty(clasificacion))
                    {
                        cmd.Parameters.AddWithValue("@clasificacion", clasificacion);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        gvErroresDepartamento.DataSource = dt;
                        gvErroresDepartamento.DataBind();
                    }

                    // Forzar que el encabezado se coloque en la sección <thead>
                    if (gvErroresDepartamento.HeaderRow != null)
                    {
                        gvErroresDepartamento.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }

                    string script = "applyDataTable('#gvErroresDepartamento');";
                    ScriptManager.RegisterStartupScript(this, GetType(), "applyDataTable", script, true);
                }
            }
        }

        // Evento del botón "Buscar" para refrescar los datos
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarErrores();
        }

        // Evento al cambiar la selección del DropDownList
        protected void ddlDepartamentos_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarErrores();
        }
    }
}



