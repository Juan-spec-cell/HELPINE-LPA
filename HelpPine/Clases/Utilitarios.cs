using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Npgsql;
using MySql.Data.MySqlClient;

namespace HelpPine.Clases
{
    public class Utilitarios
    {
        /// CONECTAR CON POSTGRESQL 
        /// 
        /*NpgsqlConnection conex = new NpgsqlConnection();

        static string servidor = "10.10.20.17";
        static string bdC = "biotimec";
        static string bdP = "biotimep";
        static string usuario = "postgres";
        static string pass = "postgres123";
        static string puerto = "5432";/*


        /// CONECTAR CON POSTGRESQL CAMISA
        /// 
        string ConexionCamisa = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + pass + ";" + "database=" + bdC + ";";

        /*public DataTable BiometricoCamisa(string query)
        {
            conex.ConnectionString = ConexionCamisa;
            NpgsqlCommand ds = new NpgsqlCommand(query, conex);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(ds);
            DataTable tabla = new DataTable();
            da.Fill(tabla);
            conex.Close();
            return tabla;
        }*/

        /// CONECTAR CON POSTGRESQL PANTALON
        /// 
        //string ConexionPantalon = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + pass + ";" + "database=" + bdP + ";";

        /*public DataTable BiometricoPantalon(string query)
        {
            conex.ConnectionString = ConexionPantalon;
            NpgsqlCommand ds = new NpgsqlCommand(query, conex);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(ds);
            DataTable tabla = new DataTable();
            da.Fill(tabla);
            conex.Close();
            return tabla;
        }*/
        /////////////////////////////////////////////////////////////////////

        /// CONECTAR CON MYSQL 
        /// 
        /*MySqlConnection conexionMySQL = new MySqlConnection();

        static string servidorMySQL = "10.10.20.21";
        static string bdMySQL = "lpapparel";
        static string usuarioMySQL = "lpamin";
        static string passMySQL = "Gu4rd14n2024";
        static string puertoMySQL = "3306";
        static string SslModeMySQL = "none";

        string ConexionMySQL = "server=" + servidorMySQL + ";" + "port=" + puertoMySQL + ";" + "user id=" + usuarioMySQL + ";" + "password=" + passMySQL + ";" + "database=" + bdMySQL + ";" + "SslMode=" + SslModeMySQL + ";";

        /*public DataTable CalidadMySQL(string query)
        {
            conexionMySQL.ConnectionString = ConexionMySQL;
            MySqlCommand ds = new MySqlCommand(query, conexionMySQL);
            MySqlDataAdapter da = new MySqlDataAdapter(ds);
            DataTable tabla = new DataTable();
            da.Fill(tabla);
            conexionMySQL.Close();
            return tabla;
        }*/
        ///////////////////////////////////////////////////////////////////    

        //CONECTAR CON SQL SERVER
        ConexionBD cnx = new ConexionBD();
        SqlConnection con = new SqlConnection();

        public DataSet ObtenerDS(string query, string tabla)
        {
            con = cnx.GetConnection();
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.SelectCommand.CommandTimeout = 0;
            da.Fill(ds, tabla);
            con.Close();
            return ds;
        }

        //public DataSet ObtenerDSAP(string query, string tabla)
        //{
        //    con = cnx.GetConnectionSAP();
        //    DataSet ds = new DataSet();
        //    SqlDataAdapter da = new SqlDataAdapter(query, con);
        //    da.SelectCommand.CommandTimeout = 0;
        //    da.Fill(ds, tabla);
        //    con.Close();
        //    return ds;
        //}

        public string EjecutarProcedimiento(SqlCommand cmd)
        {
            con = cnx.GetConnection();
            cmd.Connection = con;
            con.Open();
            object result = cmd.ExecuteScalar();
            string mensaje = result != null ? result.ToString() : "No result returned";
            con.Close();
            return mensaje;
        }

        public string EjecutarProcedimientosalmacenados(SqlCommand cmd)
        {
            string mensaje;
            con = cnx.GetConnection();
            try
            {
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteReader();
                mensaje = "Correcto";
                con.Close();
            }
            catch (SqlException error)
            {
                con.Close();
                mensaje = error.Message;
            }

            return mensaje;
        }

        /*public string EjecutarProcedimientoAlmacenadoConTransaccion(SqlCommand cmd)
        {
            string mensaje;
            using (SqlConnection con = cnx.GetConnection())
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmd.Connection = con;
                cmd.Transaction = transaction;

                try
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    mensaje = "Correcto";
                }
                catch (SqlException error)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollbackError)
                    {
                        // Manejar cualquier error durante el rollback
                        mensaje = "Error durante el rollback: " + rollbackError.Message;
                        return mensaje;
                    }
                    mensaje = "Error: " + error.Message;
                }
                finally
                {
                    con.Close();
                }
            }

            return mensaje;
        }*/

        /*public Boolean EjecutarINSERTS(SqlCommand cmd)
        {
            Boolean res = false;
            con = cnx.GetConnection();
            cmd.Connection = con;
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();

                res = true;

            }
            catch (Exception)
            {
                con.Close();
                res = false;

            }
            finally
            {
                con.Close();
            }

            return res;
        }*/

        public bool ValidarFormularios(int id)
        {
            var formularios = (List<FormulariosUsuario>)HttpContext.Current.Session["Formularios"];
            var form = formularios.Where(f => f.FormId == id).FirstOrDefault();
            if (form != null)
                return true;

            return false;
        }

        //LLENAR DDLS
        /*public void BindDdl(ref DropDownList ddl, DataTable dt, string value)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = "Descripcion";
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", ""));
        }*/

        public void BindDdlUniversal(ref DropDownList ddl, DataTable dt, string value, string text)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = text;
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", "0"));
        }


        //LLENAR DDLS
        public void BindDdlNombre(ref DropDownList ddl, DataTable dt, string value)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = "Nombre";
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", ""));
        }

        //LLENAR Lote
        public void BindDdlCod(ref DropDownList ddl, DataTable dt, string value)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = "Codigo";
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", ""));
        }

        public void BindDdlOpe(ref DropDownList ddl, DataTable dt, string value)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = "Operacion";
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", ""));
        }

        //LLENAR DDLS SAP
        /*public void BindDdlSAP(ref DropDownList ddl, DataTable dt, string value)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = "CardName";
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", ""));
        }*/

        //LLENAR ORDENES WIP
        /*public void BindDdlWIP(ref DropDownList ddl, DataTable dt, string value)
        {
            ddl.DataSource = dt;
            ddl.DataTextField = "CPON";
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("Seleccionar", ""));
        }*/

        //LLENAR LISTAS
       /* public void BindLists(ref HtmlGenericControl lista, DataTable dt)
        {
            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
                builder.Append("<option label='" + dt.Rows[i][1] + "' value='" + dt.Rows[i][0] + "'>");

            lista.InnerHtml = builder.ToString();
        }*/

        public void BindGrid(DataTable dt, ref GridView grid)
        {
            int rowIndex = 1;
            if (dt.Rows.Count > 0)
            {
                for (int m = 0; m < dt.Rows.Count; m++)
                {
                    dt.Rows[m][0] = rowIndex;
                    rowIndex++;
                }

                grid.DataSource = dt;
                grid.DataBind();
            }
            else
            {
                dt.Rows.Add(dt.NewRow());
                grid.DataSource = dt;
                grid.DataBind();
                grid.Rows[0].Cells.Clear();
                dt.Clear();
            }
        }

        /*public byte[] GeneraQR(String CodigoAGenerar)
        {
            byte[] yourByteArray;
            QRCoder.QRCodeGenerator CodigoQR = new QRCodeGenerator();
            QRCoder.QRCodeData EsteCodigoQR = CodigoQR.CreateQrCode(CodigoAGenerar, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCode MandaQR = new QRCode(EsteCodigoQR);
            Bitmap imgbmp = MandaQR.GetGraphic(7);

            using (MemoryStream ms = new MemoryStream())
            {
                imgbmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                yourByteArray = ms.ToArray();
            }

            return yourByteArray;
        }*/

        /*public Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pdata = blob;
            mStream.Write(pdata, 0, Convert.ToInt32(pdata.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }*/

        public bool FormulariosAccion(int idFormulario, int idUsuario)
        {
            bool value = false;

            var formularios = (List<FormulariosUsuario>)HttpContext.Current.Session["Formularios"];
            var form_req = formularios.Where(f => f.FormId == idFormulario && f.UserId == idUsuario).FirstOrDefault();

            if (form_req != null)
            {
                value = true;
            }

            return value;
        }
    }
}