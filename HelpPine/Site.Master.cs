using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine
{
    public partial class SiteMaster : MasterPage
    {
        public string url;
        //Inicializamos una coleccionpublica que almacenara los formulario epecificos para el usuario
        public List<FormulariosUsuario> formularios = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Si la sesion del usuario no esta definida lo retorna al login
            if (Session["Usuario"] == null)
            {
                Response.Redirect("~/Login");

            }
                

            //Llenamos la coleccion de formularios que inicializamos como publica con la lista de formularios que se almacenan en la sesion del usuario
            formularios = (List<FormulariosUsuario>)Session["Formularios"];

            // Mostrar el mensaje solo si aún no se ha mostrado
            if (Session["BienvenidaMostrada"] == null)
            {
                lbUser.InnerText = "Bienvenido " + Session["Usuario"].ToString();
                Session["BienvenidaMostrada"] = true; // Indicar que ya se mostró la bienvenida
            }
            else
            {
                // Si ya se mostró, puedes dejarlo vacío o mostrar otra cosa
                lbUser.InnerText = "" + Session["Usuario"].ToString(); ;
            }

            if (Session["Departamento"] != null)
            {

            }
            url = HttpContext.Current.Request.Url.AbsolutePath;
            if (Session["Keyboard"] != null)
                Keyboard.Text = bool.Parse(Session["Keyboard"].ToString()) ? "Usar teclado en pantalla" : "Usar teclado de windows";

        }

        protected void Control_Load(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            int formId = int.Parse(control.Attributes["formId"]);
            control.Visible = false;

            var form_req = formularios.Where(f => f.FormId == formId).FirstOrDefault();
        }

        protected void Salir_Click(object sender, EventArgs e)
        {
            //Removemos la sesion del usuario cuando seleccion cerrar sesion
            Session.Remove("User");
            Session.Remove("Formularios");
            Session.Remove("Departamento");
            Session.Abandon();
            //Dirigimos al usuario que cerro sesión al login nuevamente
            Response.Redirect("~/Login");
        }

        protected void Keyboard_Click(object sender, EventArgs e)
        {
            if (Session["Keyboard"] == null)
            {
                Session["Keyboard"] = false;
                Keyboard.Text = "Usar teclado de windows";
            }
            else
            {
                Session["Keyboard"] = !bool.Parse(Session["Keyboard"].ToString());
                Keyboard.Text = bool.Parse(Session["Keyboard"].ToString()) ? "Usar teclado en pantalla" : "Usar teclado de windows";
            }
        }
    }
}
