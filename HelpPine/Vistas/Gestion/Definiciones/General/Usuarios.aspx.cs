using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using HelpPine.Clases;

namespace HelpPine.Vistas.Gestion.Definiciones.General
{
    public partial class Usuarios : System.Web.UI.Page
    {
        public List<FormulariosUsuario> formularios = null;
        private int hide = 0;

        readonly Utilitarios util = new Utilitarios();
        readonly Signin us = new Signin();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CargarRoles();
                CargarDepartamentos();
                CargarEstados();
                CargarDatos();
            }
            else
            {
                ModalActivo.Value = "";
            }

            // Mantener el botón correcto visible después del PostBack
            if (ViewState["ModoEdicion"] != null && (bool)ViewState["ModoEdicion"])
            {
                btnGuardar.Visible = false;
                btnActualizar.Visible = true;
                btnCancelar.Visible = true;

            }
            else
            {
                btnGuardar.Visible = true;
                btnActualizar.Visible = false;
                btnCancelar.Visible = false;
            }

            // Pasar el estado de edición al frontend
            ScriptManager.RegisterStartupScript(this, GetType(), "SetEditMode", $"var isEditMode = {(ViewState["ModoEdicion"] != null && (bool)ViewState["ModoEdicion"]).ToString().ToLower()};", true);
        }

        private void CargarDatos()
        {
                DataSet ds = util.ObtenerDS("SELECT idUsuario, Nombre, Login, Email, Perfil, Departamento, Activo FROM V_GetUsuarios", "T");
                ViewState["Table"] = ds.Tables[0];
            if (ds.Tables[0].Rows.Count > 0)
            {
                Refresh(ds.Tables[0], ref gvUsuarios);
            }
            else
            {
                ds.Tables[0].Rows.Add(ds.Tables[0].NewRow());
                gvUsuarios.DataSource = ds.Tables[0];
                gvUsuarios.DataBind();
                gvUsuarios.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            Refresh(ds.Tables[0], ref gvUsuarios);


        }
        private void CargarRoles()
        {
            Debug.WriteLine("Cargando roles...");
            DataSet ds = util.ObtenerDS("SELECT idPerfil, descripcion FROM V_Perfiles WHERE activo = 1", "T");
            util.BindDdlUniversal(ref ddlPerfil, ds.Tables[0], "idPerfil", "descripcion");
            Debug.WriteLine("Roles cargados.");
        }
        private void CargarEstados()
        {
            Debug.WriteLine("Cargando estados...");
            ddlEstado.Items.Clear();
            ddlEstado.Items.Add(new ListItem("Activo", "1"));
            ddlEstado.Items.Add(new ListItem("Inactivo", "0"));
            Debug.WriteLine("Estados cargados.");
        }
        private void CargarDepartamentos()
        {
            Debug.WriteLine("Cargando departamentos...");
            DataSet ds = util.ObtenerDS("SELECT IdDepartamento, Descripcion FROM V_GetDeptos WHERE Activo = 1", "T");
            util.BindDdlUniversal(ref ddlDepartamento, ds.Tables[0], "IdDepartamento", "Descripcion");
            Debug.WriteLine("Departamentos cargados.");
        }
        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                // Asegurar que el CommandArgument sea el idUsuario real
                string idUsuario = e.CommandArgument.ToString();
                Debug.WriteLine($"RowCommand ejecutado. CommandName: {e.CommandName}, ID recibido: {idUsuario}");

                // Validar que el ID sea correcto
                if (string.IsNullOrEmpty(idUsuario) || idUsuario == "0")
                {
                    Debug.WriteLine("Error: idUsuario no es válido.");
                    return;
                }

                // Convertir el ID a entero
                if (!int.TryParse(idUsuario, out int usuarioId))
                {
                    Debug.WriteLine("Error: idUsuario no es un número válido.");
                    return;
                }

                if (e.CommandName.Equals("Editar"))
                {
                    try
                    {
                        Debug.WriteLine($"Ejecutando comando Editar para el usuario: {usuarioId}");

                        DataSet ds = util.ObtenerDS($@"
                     SELECT idUsuario, Nombre, Email, Perfil, Departamento 
                     FROM V_GetUsuarios 
                     WHERE idUsuario = {usuarioId}", "T");

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];

                            string nombreCompleto = dr["Nombre"].ToString().Trim();
                            string[] partes = nombreCompleto.Split(' ');

                            string nombre = partes.Length > 1 ? partes[0] : nombreCompleto;
                            string apellido = partes.Length > 1 ? string.Join(" ", partes.Skip(1)) : "";

                            HiddenFieldUsuarioID.Value = usuarioId.ToString();
                            txtNombre.Text = nombre;
                            txtApellido.Text = apellido;
                            txtEmail.Text = dr["Email"].ToString();

                            if (dr["Perfil"] != DBNull.Value)
                            {
                                string perfil = dr["Perfil"].ToString().Split('|')[0];
                                if (ddlPerfil.Items.FindByText(perfil) != null)
                                    ddlPerfil.SelectedValue = ddlPerfil.Items.FindByText(perfil).Value;
                                else
                                    Debug.WriteLine($"Perfil '{perfil}' no encontrado en ddlPerfil.");
                            }
                            else
                            {
                                ddlPerfil.ClearSelection();
                            }

                            if (ddlDepartamento.Items.FindByText(dr["Departamento"].ToString()) != null)
                                ddlDepartamento.SelectedValue = ddlDepartamento.Items.FindByText(dr["Departamento"].ToString()).Value;
                            else
                                Debug.WriteLine($"Departamento '{dr["Departamento"]}' no encontrado en ddlDepartamento.");

                            ViewState["ModoEdicion"] = true;

                            Debug.WriteLine($"Datos cargados: Nombre='{nombre}', Apellido='{apellido}', Email='{txtEmail.Text}'");

                            ScriptManager.RegisterStartupScript(this, GetType(), "UpdateFormTitle",
                                "document.getElementById('formTitle').innerText='Editar Usuario';", true);

                            ScriptManager.RegisterStartupScript(this, GetType(), "ScrollToForm",
                                "document.getElementById('formTitle').scrollIntoView({ behavior: 'smooth' });", true);

                            ScriptManager.RegisterStartupScript(this, GetType(), "TriggerPostBack", "__doPostBack('', '');", true);
                        }
                        else
                        {
                            Debug.WriteLine($"No se encontró ningún usuario con id: {usuarioId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error en comando Editar: {ex.Message}");
                    }
                }
                else if (e.CommandName.Equals("Activar"))
                {
                    try
                    {
                        Debug.WriteLine($"ID recibido para activar: {usuarioId}");

                        using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
                        {
                            conn.Open();

                            using (SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Usuarios WHERE idUsuario = @idUsuario", conn))
                            {
                                cmdCheck.Parameters.AddWithValue("@idUsuario", usuarioId);
                                int existe = Convert.ToInt32(cmdCheck.ExecuteScalar());

                                if (existe == 0)
                                {
                                    Debug.WriteLine("Error: No se encontró el usuario en la base de datos.");
                                    return;
                                }
                            }

                            using (SqlCommand cmd = new SqlCommand("usp_ActivateUser", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idUsuario", usuarioId);

                                object resultado = cmd.ExecuteScalar();
                                if (resultado != null && int.TryParse(resultado.ToString(), out int result) && result == 1)
                                {
                                    Debug.WriteLine("Usuario activado correctamente.");
                                }
                                else
                                {
                                    Debug.WriteLine("Error al activar usuario: No se recibió el resultado esperado.");
                                }
                            }
                        }

                        CargarDatos(); // Recargar la tabla después de la activación
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error al activar usuario: {ex.Message}");
                    }
                }
                else if (e.CommandName.Equals("Detalle"))
                {
                    try
                    {
                        

                        MostrarDetallesUsuario(e.CommandArgument.ToString());

                        // Mostrar el modal
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowDetailsModal", "$('#btnDetalles').click();", true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error en comando Detalle: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en RowCommand: {ex.Message}");
            }
        }

        private void MostrarDetallesUsuario(string idUsuario)
        {
            ModalActivo.Value = "Detalles";
            DataSet ds = util.ObtenerDS($"SELECT * FROM V_GetUsuarios WHERE idUsuario = '{idUsuario}'", "T");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                lbNombre.InnerText = dr["Nombre"].ToString();
                lbUsuario.InnerText = dr["Login"].ToString();
                lbPerfil.InnerText = dr["Perfil"].ToString();
                lbDepartamento.InnerText = dr["Departamento"].ToString();
                lbActivo.InnerText = dr["Activo"].ToString();
                lbFechaC.InnerText = dr["FechaRegistro"].ToString();
            }
        }


        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnGuardar_Click ejecutado.");

            if (string.IsNullOrEmpty(txtNombre.Text) ||
                string.IsNullOrEmpty(txtApellido.Text) ||
                string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(ddlPerfil.SelectedValue))
            {
                Debug.WriteLine("btnGuardar_Click: Datos incompletos.");
                return;
            }

            try
            {
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower();
                string pass = txtPass.Enabled ? us.Encriptar(txtPass.Text.Trim()) : "";
                // Ya no se usa "rolDescripcion" porque ahora se utiliza idPerfil
                int estado = int.Parse(ddlEstado.SelectedValue);
                int departamento = int.Parse(ddlDepartamento.SelectedValue);

                // Extraer el idPerfil desde el DropDownList
                int idPerfil;
                if (!int.TryParse(ddlPerfil.SelectedValue, out idPerfil))
                {
                    Debug.WriteLine($"Error al convertir el valor de ddlPerfil a entero. Valor: {ddlPerfil.SelectedValue}");
                    return;
                }

                Debug.WriteLine($"Datos a insertar: Nombre={nombre}, Apellido={apellido}, Email={email}, idPerfil={idPerfil}, Estado={estado}, Departamento={departamento}");

                using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_GuardarUsuario", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@apellido", apellido);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@contrasena", pass);
                        cmd.Parameters.AddWithValue("@estado", estado);
                        cmd.Parameters.AddWithValue("@idDepartamento", departamento);
                        cmd.Parameters.AddWithValue("@idPerfil", idPerfil); // Agregado

                        Debug.WriteLine("Ejecutando procedimiento almacenado...");
                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Debug.WriteLine($"Procedimiento ejecutado. Filas afectadas: {filasAfectadas}");
                    }
                }

                Debug.WriteLine("Usuario guardado correctamente.");
                CargarDatos();
                LimpiarFormulario();
            }
            catch (SqlException sqlEx)
            {
                Debug.WriteLine($"Error SQL: {sqlEx.Message}");
                foreach (SqlError error in sqlEx.Errors)
                {
                    Debug.WriteLine($"Código: {error.Number}, Mensaje: {error.Message}, Línea: {error.LineNumber}");
                }
                Response.Write($"<script>alert('Error SQL: {sqlEx.Message}');</script>");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error general: {ex.Message}");
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnActualizar_Click ejecutado.");

            if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtApellido.Text) || string.IsNullOrEmpty(txtEmail.Text))
            {
                Debug.WriteLine("Datos incompletos para actualizar usuario.");
                return;
            }

            try
            {
                string idUsuario = HiddenFieldUsuarioID.Value;
                string nombre = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower();
                int estado;
                int departamento;
                int idPerfil;

                if (!int.TryParse(ddlEstado.SelectedValue, out estado))
                {
                    Debug.WriteLine("Error al convertir el valor de ddlEstado a entero.");
                    return;
                }

                if (!int.TryParse(ddlDepartamento.SelectedValue, out departamento))
                {
                    Debug.WriteLine($"Error al convertir el valor de ddlDepartamento a entero. Valor: {ddlDepartamento.SelectedValue}");
                    return;
                }

                if (!int.TryParse(ddlPerfil.SelectedValue, out idPerfil))
                {
                    Debug.WriteLine($"Error al convertir el valor de ddlPerfil a entero. Valor: {ddlPerfil.SelectedValue}");
                    return;
                }

                string usuarioGenerado = $"{nombre[0].ToString().ToLower()}{apellido.Replace(" ", "").ToLower()}";

                Debug.WriteLine($"Actualizando usuario ID={idUsuario}, Nombre='{nombre}', Apellido='{apellido}', Email={email}, UsuarioGenerado='{usuarioGenerado}', Perfil={idPerfil}");

                using (SqlConnection conn = new SqlConnection(ConexionBD.sConexion))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("usp_EditarUsuario", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.Parameters.AddWithValue("@nombre", nombre);
                        cmd.Parameters.AddWithValue("@apellido", apellido);
                        cmd.Parameters.AddWithValue("@email", email);

                        // Si el campo de contraseña está vacío, se envía DBNull.Value; de lo contrario se envía el valor encriptado.
                        if (string.IsNullOrWhiteSpace(txtPass.Text))
                        {
                            cmd.Parameters.AddWithValue("@contrasena", DBNull.Value);
                        }
                        else
                        {
                            string pass = us.Encriptar(txtPass.Text.Trim());
                            cmd.Parameters.AddWithValue("@contrasena", pass);
                        }

                        cmd.Parameters.AddWithValue("@estado", estado);
                        cmd.Parameters.AddWithValue("@idDepartamento", departamento);
                        cmd.Parameters.AddWithValue("@usuario", usuarioGenerado);
                        cmd.Parameters.AddWithValue("@idPerfil", idPerfil);

                        int filasAfectadas = cmd.ExecuteNonQuery();
                        Debug.WriteLine($"Usuario actualizado. Filas afectadas: {filasAfectadas}");
                    }
                }

                ViewState["ModoEdicion"] = false;
                CargarDatos();
                LimpiarFormulario();

                ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarVisibilidadBotones",
                    "document.getElementById('" + btnGuardar.ClientID + "').style.display = 'block'; " +
                    "document.getElementById('" + btnActualizar.ClientID + "').style.display = 'none'; " +
                    "document.getElementById('" + btnCancelar.ClientID + "').style.display = 'none';", true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al actualizar usuario: {ex.Message}");
            }
        }






        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btnCancelar_Click ejecutado.");
            LimpiarFormulario();
            ViewState["ModoEdicion"] = false;

            // Forzar un postback para actualizar la visibilidad de los botones
            ScriptManager.RegisterStartupScript(this, GetType(), "ActualizarVisibilidadBotones", "document.getElementById('" + btnGuardar.ClientID + "').style.display = 'block'; document.getElementById('" + btnActualizar.ClientID + "').style.display = 'none'; document.getElementById('" + btnCancelar.ClientID + "').style.display = 'none';", true);
        }

        private void LimpiarFormulario()
        {
            Debug.WriteLine("Limpiando formulario...");
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtEmail.Text = "";
            txtPass.Text = "";
            ddlPerfil.SelectedIndex = 0;
            ddlDepartamento.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;

            // Restablecer botones: mostrar "Guardar" y ocultar "Actualizar" y "Cancelar"
            btnGuardar.Visible = true;
            btnActualizar.Visible = false;
            btnCancelar.Visible = false;

            // Cambiar el título del formulario de vuelta a "Crear Usuario"
            ScriptManager.RegisterStartupScript(this, GetType(), "ResetFormTitle", "document.getElementById('formTitle').innerText='Crear Usuario';", true);
            Debug.WriteLine("Formulario restablecido.");
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
        protected void Control_Load(object sender, EventArgs e)
        {
            var control = (WebControl)sender;
            int idUsuario = Convert.ToInt32(Session["IdUser"].ToString());
            int idFormulario = int.Parse(control.Attributes["FormId"]);
            control.Visible = false;
            string btn = control.ClientID;

            if (hide == 0)
            {
                control.Visible = util.FormulariosAccion(idFormulario, idUsuario);
                var formularios = (List<FormulariosUsuario>)Session["Formularios"];
                var form_req = formularios.Where(f => f.FormId == idFormulario).FirstOrDefault();
            }
        }
    }
}



