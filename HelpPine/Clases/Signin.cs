using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace HelpPine.Clases
{
    public class Signin
    {
        readonly Utilitarios util = new Utilitarios();

        public string ValidarUsuario(string UserName, string Password)
        {
            string id = "", nombre = "", apellido = "", email = "", usuario = "", departamento = "", perfil = "", estado = "", fechaRegistro = "";

            // Consulta para obtener los datos del usuario
            string sql = "SELECT * FROM V_Login WHERE usuario = '" + UserName + "' AND contrasena = '" + Password + "' AND estado = 'Sí'";

            // Se llena un data set con la información extraída por medio de la consulta
            DataSet dataset = util.ObtenerDS(sql, "Tabla");

            // Verificamos que el dataset tenga al menos una fila
            if (dataset.Tables["Tabla"].Rows.Count != 0)
            {
                // Recorremos cada fila del dataset aislando los campos que contienen
                foreach (DataRow dr in dataset.Tables["Tabla"].Rows)
                {
                    id = dr["idUsuario"].ToString();
                    nombre = dr["nombre"].ToString();
                    apellido = dr["apellido"].ToString();
                    email = dr["email"].ToString();
                    usuario = dr["usuario"].ToString();
                    departamento = dr["departamento"].ToString();
                    perfil = dr["perfil"].ToString();
                    estado = dr["estado"].ToString();
                    fechaRegistro = dr["fechaRegistro"].ToString();
                }
            }

            // Almacenamos los datos del usuario obtenido en la sesión
            HttpContext IdUsuario = HttpContext.Current;
            IdUsuario.Session["IdUser"] = id;
            IdUsuario.Session["Nombre"] = nombre;
            IdUsuario.Session["Apellido"] = apellido;
            IdUsuario.Session["Email"] = email;
            IdUsuario.Session["Usuario"] = usuario;
            IdUsuario.Session["Departamento"] = departamento;
            IdUsuario.Session["Perfil"] = perfil;
            IdUsuario.Session["Estado"] = estado;
            IdUsuario.Session["FechaRegistro"] = fechaRegistro;

            // Devolvemos el id del usuario obtenido
            return id;
        }

        public void ListadoFormularios(string Id)
        {
            // Preparamos una lista de clases formulario
            List<FormulariosUsuario> ListadoFormularios = new List<FormulariosUsuario>();

            // Creamos la consulta que obtendrá los formularios a los que el usuario tendrá acceso
            DataSet dataset = util.ObtenerDS(@"SELECT * FROM V_FormulariosUsuario WHERE UserId = '" + Id + "'", "Tabla");

            // Recorremos las filas del dataset de los formularios
            foreach (DataRow row in dataset.Tables["Tabla"].Rows)
            {
                // Asignamos a la clase `FormulariosUsuario` la información de cada una de las filas obtenidas
                var Formularios = new FormulariosUsuario
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    FormId = Convert.ToInt32(row["FormId"]),
                    FormDesc = row["FormDesc"].ToString()
                };

                // Agregamos la clase `FormulariosUsuario` con los datos de las filas de la consulta y lo agregamos a la lista de formularios que preparamos anteriormente
                ListadoFormularios.Add(Formularios);
            }

            // Almacenamos la lista de formularios en la sesión del usuario logueado
            HttpContext SessionFormularios = HttpContext.Current;
            SessionFormularios.Session["Formularios"] = ListadoFormularios;
        }

        public string Encriptar(string texto)
        {
            //arreglo de bytes donde guardaremos la llave
            byte[] keyArray;
            //arreglo de bytes donde guardaremos el texto
            //que vamos a encriptar
            byte[] Arreglo_a_Cifrar = Encoding.UTF8.GetBytes(texto);

            //se utilizan las clases de encriptación
            //provistas por el Framework
            //Algoritmo MD5
            MD5CryptoServiceProvider hashmd5 =
            new MD5CryptoServiceProvider();
            //se guarda la llave para que se le realice
            //hashing
            keyArray = hashmd5.ComputeHash(
            Encoding.UTF8.GetBytes("=kjk"));

            hashmd5.Clear();

            //Algoritmo 3DAS
            TripleDESCryptoServiceProvider tdes =
            new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            //se empieza con la transformación de la cadena
            ICryptoTransform cTransform = tdes.CreateEncryptor();

            //arreglo de bytes donde se guarda la
            //cadena cifrada
            byte[] ArrayResultado =
            cTransform.TransformFinalBlock(Arreglo_a_Cifrar,
            0, Arreglo_a_Cifrar.Length);

            tdes.Clear();

            //se regresa el resultado en forma de una cadena
            return Convert.ToBase64String(ArrayResultado,
            0, ArrayResultado.Length);
        }
        public string Desencriptar(string textoEncriptado)
        {
            //arreglo de bytes donde guardaremos la llave
            byte[] keyArray;
            //convierte el texto en una secuencia de bytes
            byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);

            //se utilizan las clases de encriptación
            //provistas por el Framework
            //Algoritmo MD5
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("=kjk"));

            hashmd5.Clear();

            //Algoritmo 3DES
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            //se empieza con la transformación de la cadena
            ICryptoTransform cTransform = tdes.CreateDecryptor();

            //arreglo de bytes donde se guarda la
            //cadena descifrada
            byte[] resultadoArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);
            tdes.Clear();

            //se regresa el resultado en forma de una cadena
            return Encoding.UTF8.GetString(resultadoArray);
        }
    }

}






