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

        public static string sConexion = Properties.Settings.Default.ConexionHD.Trim(); 

        public SqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(sConexion))
            {
                throw new Exception("Error: La cadena de conexión no está configurada correctamente.");
            }
            return new SqlConnection(sConexion);
        }
    }

}