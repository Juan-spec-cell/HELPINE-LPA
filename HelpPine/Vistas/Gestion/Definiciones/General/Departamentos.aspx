<%@ Page Title="Departamentos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Departamentos.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.General.Departamentos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <!-- Agrega la referencia a SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="messages" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="ModalActivo" ClientIDMode="Static" runat="server" />

    <div class="row mb-3 text-center">
        <div class="col">
            <h4 class="text-white"><strong>Crear Departamentos</strong></h4>
        </div>
    </div>

    <div class="card mt-3">
        <!-- Formulario para Agregar Departamento -->
        <div class="card pt-3 pb-3">
            <div class="card-header">
                <h5 class="mb-0">Agregar Departamento</h5>
            </div>
            <div class="card-body">
                <asp:UpdatePanel ID="upAgregarDepartamento" runat="server">
                    <ContentTemplate>
                        <div class="row g-3">
                            <div class="col-12 col-md-6">
                                <label class="form-label">Descripción:</label>
                                <asp:TextBox ID="txtGuardarDescripcion" runat="server" CssClass="form-control" required="required"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server" ControlToValidate="txtGuardarDescripcion" CssClass="text-danger" Display="Dynamic" />
                                <asp:ValidationSummary ID="vsGuardar" runat="server" CssClass="text-danger" />
                            </div>
                            <div class="col-12 col-md-6 d-flex align-items-center">
                                <label class="form-check-label me-2">Activo</label>
                                <input class="form-check-input" type="checkbox" runat="server" id="chkGuardarActivo" checked="checked" />
                            </div>
                        </div>
                        <div class="row mt-3">
                            <div class="col text-end">
                                <!-- Se añade OnClientClick para validar antes de enviar -->
                                <asp:Button ID="BtnGuardar" CssClass="btn btn-outline-primary" Text="Guardar" runat="server" 
                                    OnClick="BtnGuardar_Click" ValidationGroup="Global" ClientIDMode="Static" 
                                    OnClientClick="return validarAgregar();" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        <!-- GridView de Departamentos -->
        <div class="card-body">
            <asp:UpdatePanel ID="upGridView" runat="server">
                <ContentTemplate>
                    <div class="row">
                        <asp:GridView CssClass="table table-sm" ID="gvDepartamentos" runat="server" ClientIDMode="Static" 
                            AutoGenerateColumns="False" BorderStyle="None" Width="80%" Style="min-width: 73rem" OnRowCommand="gvDepartamentos_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="row" HeaderText="#" ItemStyle-Width="5%" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDetalle" CommandArgument='<%# Eval("[IdDepartamento]") %>' runat="server" CommandName="Detalle" ToolTip="Detalle">
                                            <i class="fa-solid fa-circle-info fa-2x"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnEdit" Visible='<%# Eval("[Activo]").ToString() == "False" ? false : true %>' 
                                            CommandArgument='<%# Eval("[IdDepartamento]") %>' runat="server" CommandName="Editar" ToolTip="Editar" formId="14">
                                            <i class="fa-solid fa-pen-to-square fa-2x" style="color: yellow;"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnActivate" Visible='<%# Eval("[Activo]").ToString() == "True" ? false : true %>' 
                                            CommandArgument='<%# Eval("[IdDepartamento]") %>' runat="server" CommandName="Activar" ToolTip="Activar">
                                            <i class="fa-solid fa-check fa-2x" style="color: green;"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="text-center" />
                            <RowStyle CssClass="text-center" />
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <!-- Botón oculto para abrir el modal de editar -->
    <input type="button" hidden="hidden" id="btnEditar" data-toggle="modal" data-target="#ModalAgregar" />
    <div class="modal fade" id="ModalAgregar" tabindex="-1" data-backdrop="static" data-keyboard="false" aria-modal="true" role="dialog">
        <div class="modal-dialog " role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" runat="server" id="modalTitle"></h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body m-2">
                    <div class="col">
                        <label class="form-label">Descripción:</label>
                        <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                        <asp:RequiredFieldValidator SetFocusOnError="true" CssClass="text-danger" runat="server" 
                            ErrorMessage="Campo requerido." ControlToValidate="txtDescripcion" ValidationGroup="Global"></asp:RequiredFieldValidator>
                    </div>
                    <div class="col">
                        <label class="form-check form-check-inline">
                            Activo
                            <input class="form-check-input" type="checkbox" runat="server" id="chkActivo" checked="checked" />
                        </label>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:HiddenField ID="idDepartamento" ClientIDMode="Static" runat="server" />
                    <a class="btn btn-outline-secondary" data-dismiss="modal">Cancelar</a>
                    <!-- Se modifica OnClientClick para validar el modal -->
                    <asp:Button ID="Button1" CssClass="btn btn-outline-primary" Text="Guardar" runat="server" 
                        OnClientClick="return validarModal();" ValidationGroup="Global" ClientIDMode="Static" />
                </div>
            </div>
        </div>
    </div>

    <input type="button" hidden="hidden" id="btnDetalles" data-toggle="modal" data-target="#ModalDetalles" />
    <div class="modal fade" id="ModalDetalles" tabindex="-1" data-backdrop="static" data-keyboard="false" aria-modal="true" role="dialog">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Detalles del Departamento</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>

                <div class="modal-body m-2">
                    <div class="row">
                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Descripción: </strong><span id="lbDescripcion" runat="server"></span>
                            </label>
                        </div>

                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Fecha creación: </strong><span id="lbFechaC" runat="server"></span>
                            </label>
                        </div>

                        <div class="col-md-6">
                            <label class="h5 mb-2" id="lbModificado" runat="server"></label>
                        </div>

                        <div class="col-md-6">
                            <label class="h5 mb-2" id="lbFechaM" runat="server"></label>
                        </div>

                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Activo: </strong><span id="lbActivo" runat="server"></span>
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
    <script>
        // Función para validar el formulario de "Agregar Departamento"
        function validarAgregar(){
            var descripcion = $("#<%= txtGuardarDescripcion.ClientID %>").val().trim();
            if(descripcion === ""){
                Swal.fire({
                    icon: 'error',
                    title: 'Campo vacío',
                    text: 'La descripción es requerida.',
                    timer: 2000,
                    showConfirmButton: false
                });
                return false;
            }
            return true;
        }

        // Función para validar el modal de edición
        function validarModal(){
            var descripcionModal = $("#txtDescripcion").val().trim();
            if(descripcionModal === ""){
                Swal.fire({
                    icon: 'error',
                    title: 'Campo vacío',
                    text: 'La descripción es requerida.',
                    timer: 2000,
                    showConfirmButton: false
                });
                return false;
            }
            if(Page_ClientValidate('Global')){
                __doPostBack('Add', $('#idDepartamento').val());
            }
            return false;
        }

        const isModified = () => { };

        // Se mantiene la función Add original (por si se requiere en otros flujos)
        const Add = () => {
            Page_ClientValidate('Global') ? __doPostBack('Add', $('#idDepartamento').val()) : false;
        }

        // Si existiera un botón con id "btnAgregar" para reiniciar el modal, se puede agregar aquí el código correspondiente.
        $('#btnAgregar').click(() => {
            $('#idDepartamento').val('0');
            $('#MainContent_modalTitle').text('Agregar Departamento');
            $('#txtDescripcion').val('');
        });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script src="/Content/js/dataTables.bootstrap5.min.js"></script>
    <script>
        //// Desactivar ordenamiento en la columna de acciones
        config.columnDefs = [{
            'targets': [2],
            'orderable': false,
        }];

        config.drawCallback = function (settings) {
            feather.replace();
        };

        let ModalActivo = $("#ModalActivo").val();

        if (ModalActivo != '')
            $("#btn" + ModalActivo).click();

        let table = $('#gvDepartamentos').DataTable(config);
        if (table.data().length == 1 && table.row(0).data()[0] == "&nbsp;")
            table.clear().draw();
    </script>
</asp:Content>



