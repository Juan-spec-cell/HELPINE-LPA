using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace HelpPine.Clases
{
    public class ConexionBD
    {
        public static string sConexion = Properties.Settings.Default.ConexionHP_Prueba.Trim();
        // public static string sConexion = Properties.Settings.Default.ConexionHD_Produccion.Trim(); 

        public SqlConnection GetConnection()
        {
            Debug.WriteLine("Cadena de conexión utilizada: " + sConexion);
            SqlConnection con = new SqlConnection(sConexion);
            return con;
        }
    }
}
