<%@ Page Title="Perfiles" Language="C#" MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" CodeBehind="Perfiles.aspx.cs" 
    Inherits="HelpPine.Vistas.Gestion.Definiciones.General.Perfiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="messages" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="ModalActivo" ClientIDMode="Static" runat="server" />

    <div class="row mb-3 text-center">
        <div class="col">
            <h4 class="text-white"><strong>Listado de Perfiles</strong></h4>
        </div>
    </div>

    <div class="card p-3">
        <div class="card-header d-flex justify-content-between">
            <asp:HyperLink CssClass="btn btn-outline-primary" runat="server" 
                NavigateUrl="~/Vistas/Gestion/Definiciones/General/PerfilesAdmin.aspx">
                Agregar Perfil
            </asp:HyperLink>
        </div>

        <div class="card-body">
            <div class="table-responsive">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <asp:GridView CssClass="table table-sm table-bordered text-center" 
                            ID="gvPerfiles" runat="server" BorderStyle="None" 
                            RowStyle-Wrap="False" ClientIDMode="Static" AutoGenerateColumns="False" 
                            Width="100%" Style="min-width: 45rem" OnRowCommand="gvPerfiles_RowCommand">
                            <Columns>
                                <asp:BoundField DataField="idPerfil" HeaderText="#" ItemStyle-Width="5%" />
                                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                <asp:BoundField DataField="UsuarioCreador" HeaderText="Creado Por" />
                                <asp:TemplateField HeaderText="Acciones" ItemStyle-Width="10%">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDetalle" CommandArgument='<%# Eval("idPerfil") %>' 
                                            runat="server" CommandName="Detalle" ToolTip="Detalle">
                                            <i class="fa-solid fa-circle-info fa-2x"></i>
                                        </asp:LinkButton>
                                        <asp:HyperLink ID="hlEdit" runat="server" 
                                            NavigateUrl='<%# "PerfilesAdmin.aspx?idPerfil=" + Eval("idPerfil") %>' 
                                            ToolTip="Editar">
                                            <i class="fa-solid fa-pen-to-square fa-2x" style="color: yellow;"></i>
                                        </asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="table-light text-center" />
                            <RowStyle CssClass="text-center" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <!-- Botón oculto para disparar el modal (se usa la misma lógica que en Formularios) -->
    <input type="button" hidden="hidden" id="btnDetalles" data-toggle="modal" data-target="#ModalDetalles" />

    <!-- Modal de Detalles -->
    <div class="modal fade" id="ModalDetalles" tabindex="-1" data-backdrop="static" data-keyboard="false" aria-modal="true" role="dialog">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Detalles del perfil</h4>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">×</span>
                    </button>
                </div>
                <div class="modal-body m-2">
                    <div class="row">
                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Descripción: </strong>
                                <span id="lbDescripcion" runat="server"></span>
                            </label>
                        </div>
                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Activo: </strong>
                                <span id="lbActivo" runat="server"></span>
                            </label>
                        </div>
                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Creado por: </strong>
                                <span id="lbCreado" runat="server"></span>
                            </label>
                        </div>
                        <div class="col-md-6">
                            <label class="h5 mb-2">
                                <strong>Fecha creación: </strong>
                                <span id="lbFechaC" runat="server"></span>
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
                    'targets': [2], 'orderable': false,
                }],
                drawCallback: function (settings) {
                    feather.replace();
                },
                columns: [
                    { data: 'idPerfil' },
                    { data: 'Descripcion' },
                    { data: 'UsuarioCreador' },
                    { data: 'Acciones' }
                ]
            };

            let table = $('#gvPerfiles').DataTable(config);
            if (table.data().length == 1 && table.row(0).data()[0] == "&nbsp;")
                table.clear().draw();

            let ModalActivo = $("#ModalActivo").val();
            if (ModalActivo != '')
                $("#btn" + ModalActivo).click();
        });
    </script>
</asp:Content>


