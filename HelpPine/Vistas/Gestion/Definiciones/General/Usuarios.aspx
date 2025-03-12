<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.General.Usuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
     <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="HiddenFieldUsuarioID" runat="server" />
    <asp:HiddenField ID="HiddenFieldEmail" runat="server" />
    <asp:HiddenField ID="messages" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="ModalActivo" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hfIsEditMode" runat="server" Value='<%= IsEditMode %>' />


    <div class="row mb-3 text-center">
        <div class="col">
            <h4 class="text-white"><strong>Listado de Usuarios</strong></h4>
        </div>
    </div>

    <!-- Card para creación/edición de usuario -->
    <div class="card p-3 mt-3">
        <div class="text-center">
            <!-- Este título se actualizará dinámicamente -->
            <h5 class="mb-0" id="formTitle">Crear Usuario</h5>
        </div>
        <div class="card-body">
            <div class="row g-3">
                <!-- Nombre -->
                <div class="col-md-4">
                    <label for="txtNombre" class="form-label">Nombre</label>
                    <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ingrese nombre"></asp:TextBox>
                </div>
                <!-- Apellido -->
                <div class="col-md-4">
                    <label for="txtApellido" class="form-label">Apellido</label>
                    <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" placeholder="Ingrese apellido"></asp:TextBox>
                </div>
                <!-- Email -->
                <div class="col-md-4">
                    <label for="txtEmail" class="form-label">Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="Ingrese email" TextMode="Email"></asp:TextBox>
                </div>
                <!-- Contraseña -->
                <div class="col-md-4">
                    <label for="txtContrasena" class="form-label">Contraseña</label>
                    <asp:TextBox ID="txtPass" runat="server" CssClass="form-control" placeholder="Ingrese la contraseña" TextMode="Password"></asp:TextBox>
                </div>
                <!-- Perfil -->
                <div class="col-md-4">
                    <label for="ddlPerfil" class="form-label">Perfil</label>
                    <asp:DropDownList ID="ddlPerfil" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <!-- Departamento -->
                <div class="col-md-4">
                    <label for="ddlDepartamento" class="form-label">Departamento</label>
                    <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <!-- Estado -->
                <div class="col-md-4">
                    <label for="ddlEstado" class="form-label">Estado</label>
                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="card-body text-end">
            <asp:Button ID="btnGuardar" runat="server" CssClass="btn btn-success" Text="Guardar Usuario"
                OnClick="btnGuardar_Click" OnClientClick="return validarFormulario();" />
            <asp:Button ID="btnActualizar" runat="server" CssClass="btn btn-primary" Text="Actualizar Usuario"
                OnClick="btnActualizar_Click" OnClientClick="return validarFormulario();" Visible="false" />
            <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-secondary" Text="Cancelar"
                OnClick="btnCancelar_Click" Visible="false" />
        </div>


        <!-- Tabla de usuarios -->
        <div class="table-responsive">
            <asp:GridView ID="gvUsuarios" runat="server"
                CssClass="table table-sm table-bordered text-center"
                AutoGenerateColumns="false"
                ClientIDMode="Static"
                Width="100%"
                OnRowCommand="gvUsuarios_RowCommand">
                <Columns>
                    <asp:BoundField DataField="idUsuario" HeaderText="#" ItemStyle-Width="5%" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="Login" HeaderText="Usuario" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="Perfil" HeaderText="Perfil" />
                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" />
                    <asp:TemplateField HeaderText="Activo">
                        <ItemTemplate>
                            <asp:Label ID="lblActivo" runat="server"
                                Text='<%# Eval("Activo").ToString().Trim().ToUpper() == "ACTIVO" ? "ACTIVO" : "INACTIVO" %>'>
                </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="10%">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnDetalle" runat="server"
                                CommandName="Detalle"
                                CommandArgument='<%# Eval("idUsuario") %>'
                                ToolTip="Ver Detalles">
                    <i class="fa-solid fa-circle-info fa-2x"></i>
                </asp:LinkButton>
                            <asp:LinkButton ID="btnEdit" runat="server"
                                CommandName="Editar"
                                CommandArgument='<%# Eval("idUsuario") %>'
                                ToolTip="Editar"
                                Visible='<%# Eval("Activo").ToString().Trim().ToUpper() == "ACTIVO" %>'>
                    <i class="fa-solid fa-pen-to-square fa-2x" style="color: yellow;"></i>
                </asp:LinkButton>
                            <asp:LinkButton ID="btnActivar" runat="server"
                                CommandName="Activar"
                                CommandArgument='<%# Eval("idUsuario") %>'
                                ToolTip="Activar"
                                OnClientClick="return confirm('¿Estás seguro de que deseas activar este usuario?');"
                                Visible='<%# Eval("Activo").ToString().Trim().ToUpper() == "INACTIVO" %>'>
                    <i class="fa-solid fa-check-circle fa-2x" style="color: green;"></i>
                </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <HeaderStyle CssClass="text-center" />
                <RowStyle CssClass="text-center" />
            </asp:GridView>
        </div>
    </div>


<input type="button" hidden="hidden" id="btnDetalles" data-toggle="modal" data-target="#ModalDetalles" />

<!-- Modal de detalles del usuario -->
<div class="modal fade" id="ModalDetalles" tabindex="-1" data-backdrop="static" data-keyboard="false" aria-modal="true" role="dialog">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">Detalles del Usuario</h4>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">×</span>
                </button>
            </div>
            <div class="modal-body m-2">
                <div class="row">
                    <div class="col-md-6">
                        <label class="h5 mb-2">
                            <strong>Nombre completo: </strong><span id="lbNombre" runat="server"></span>
                        </label>
                    </div>
                    <div class="col-md-6">
                        <label class="h5 mb-2">
                            <strong>Usuario: </strong><span id="lbUsuario" runat="server"></span>
                        </label>
                    </div>
                    <div class="col-md-6">
                        <label class="h5 mb-2">
                            <strong>Rol: </strong><span id="lbPerfil" runat="server"></span>
                        </label>
                    </div>
                    <div class="col-md-6">
                        <label class="h5 mb-2">
                            <strong>Departamento asignado: </strong><span id="lbDepartamento" runat="server"></span>
                        </label>
                    </div>
                    <div class="col-md-6">
                        <label class="h5 mb-2">
                            <strong>Activo: </strong><span id="lbActivo" runat="server"></span>
                        </label>
                    </div>
                    <div class="col-md-6">
                        <label class="h5 mb-2">
                            <strong>Fecha creación: </strong><span id="lbFechaC" runat="server"></span>
                        </label>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <a class="btn btn-outline-secondary" data-dismiss="modal">Cerrar</a>
            </div>
        </div>
    </div>
</div>



</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <script src="/Scripts/Global/alerts.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script src="/Content/js/dataTables.bootstrap5.min.js"></script>
    <script>
        $(document).ready(function () {
            let config = {
                columnDefs: [{
                    'targets': [3], 'orderable': false,
                }],
                drawCallback: function (settings) {
                    feather.replace();
                },
                columns: [
                    { data: 'idUsuario' },
                    { data: 'Nombre' },
                    { data: 'Login' },
                    { data: 'Email' },
                    { data: 'Perfil' },
                    { data: 'Departamento' },
                    { data: 'Activo' },
                    { data: 'Acciones' }
                ]
            };

            let table = $('#gvUsuarios').DataTable(config);
            if (table.data().length == 1 && table.row(0).data()[0] == "&nbsp;")
                table.clear().draw();
            let ModalActivo = $("#ModalActivo").val();

            if (ModalActivo != '')
                $("#btn" + ModalActivo).click();
        });

        function mostrarAlertaGuardado() {
            Swal.fire({
                icon: 'success',
                title: 'Usuario guardado',
                text: 'El usuario ha sido guardado exitosamente.',
                timer: 2000,
                showConfirmButton: false
            });
        }

        function mostrarAlertaEditado() {
            Swal.fire({
                icon: 'success',
                title: 'Usuario actualizado',
                text: 'El usuario ha sido actualizado exitosamente.',
                timer: 2000,
                showConfirmButton: false
            });
        }

        function mostrarAlertaError(mensaje) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: mensaje,
                timer: 2000,
                showConfirmButton: false
            });
        }

        function validarFormulario() {
            let nombre = $('#<%= txtNombre.ClientID %>').val().trim();
            let apellido = $('#<%= txtApellido.ClientID %>').val().trim();
            let email = $('#<%= txtEmail.ClientID %>').val().trim();
            let contrasena = $('#<%= txtPass.ClientID %>').val().trim();
            let perfil = $('#<%= ddlPerfil.ClientID %>').val();
            let departamento = $('#<%= ddlDepartamento.ClientID %>').val();
            let estado = $('#<%= ddlEstado.ClientID %>').val();
            let isEditMode = $('#<%= hfIsEditMode.ClientID %>').val() === 'true';

            if (nombre === "" || apellido === "" || email === "" || perfil === "" || departamento === "" || estado === "") {
                mostrarAlertaError("Todos los campos son obligatorios.");
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
