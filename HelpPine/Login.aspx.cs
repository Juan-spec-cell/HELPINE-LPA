using HelpPine.Clases;
using System;
using System.Web;

namespace HelpPine
{
    public partial class Login : System.Web.UI.Page
    {
        readonly Signin sign = new Signin();
        readonly Utilitarios util = new Utilitarios();
        protected void Page_Load(object sender, EventArgs e)
        {
            //Eliminamos los datos que se almacenan en las sesiones
            Session.Remove("User");
            Session.Remove("Formularios");
            Session.Remove("Departamento");
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                //Enviamos el nombre del usuario y su contraseña encriptada
                string Id = sign.ValidarUsuario(txtUser.Text, sign.Encriptar(txtPass.Text));
                //Con el id obtenido de la validacion del usuario obtenemos la lista de formularios a las que el usuario logueado podra acceder
                sign.ListadoFormularios(Id);
                string departamento = HttpContext.Current.Session["Departamento"]?.ToString();
                Session["Departamento"] = departamento;
                // Verificamos si el Id está vacío para mostrar una alerta o para enviarlo a la pantalla principal
                if (string.IsNullOrEmpty(Id))
                {
                    messages.Value = "Error|red|Usuario o Contraseña Incorrectos| Usuario Desactivado.";
                    txtPass.Focus();
                }
                else
                {
                    // Aseguramos que el Id y el nombre de usuario se almacenen en la sesión
                    Session["IdUser"] = Id;
                    Session["Usuario"] = txtUser.Text;
                    Response.Redirect("~/");
                }
            }
            catch (Exception ex)
            {
                // Mostramos un mensaje de error en caso que haya ocurrido un error en el segmento analizado
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                messages.Value = "Fatal error|red|" + ex.Message;
            }
        }
    }
}






