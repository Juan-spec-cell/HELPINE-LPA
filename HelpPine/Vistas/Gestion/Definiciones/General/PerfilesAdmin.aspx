<%@ Page Title="Perfiles Admin" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PerfilesAdmin.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.General.PerfilesAdmin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="messages" ClientIDMode="Static" runat="server" />

    <div class="row mb-2">
        <h4 class="text-white"><strong>Administración de perfiles</strong></h4>
    </div>

    <div class="card pt-3 pb-3">
        <div class="card-header">
            <h4><strong>Agregar perfil</strong></h4>
        </div>

        <div class="card-body">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="col-6 ">
                        <div class="col">
                            <label class="form-label">Descripción:</label>
                            <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                            <asp:RequiredFieldValidator SetFocusOnError="true" CssClass="text-danger" runat="server" ErrorMessage="Campo requerido." ControlToValidate="txtDescripcion" ValidationGroup="Global"></asp:RequiredFieldValidator>
                        </div>

                        <div class="col">
                            <label class="form-check form-check-inline">
                                <input class="form-check-input" type="checkbox" runat="server" id="chkActivo" checked="checked" />
                                Activo
                            </label>
                        </div>
                    </div>

                    <div class=" d-flex flex-column">
                        <div class="col text-center border-bottom">
                            <h4><strong>Asignar Permisos</strong></h4>
                        </div>

                        <div class="col pt-5">
                            <asp:GridView CssClass="table table-sm" ID="Gridview1" Width="100%" runat="server" AutoGenerateColumns="False" ClientIDMode="Static" BorderStyle="None" DataKeyNames="IdFormulario">
                                <Columns>
                                    <asp:BoundField DataField="row" HeaderText="#" ItemStyle-Width="5%" />
                                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" ItemStyle-Width="80%" />
                                    <asp:TemplateField HeaderText="Opciones" ItemStyle-Width="2%">
                                        <ItemTemplate>
                                            <label class="form-check form-check-inline">
                                                <input class="form-check-input" type="checkbox" idformulario='<%#Eval("IdFormulario") %>' runat="server" id="check" onclick="Add(this);" checked='<%#(bool)Eval("Asignado") %>'>
                                            </label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="table-light text-center" />
                                <RowStyle CssClass="text-center" />
                            </asp:GridView>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div class="card-footer d-flex justify-content-end pb-4 pt-4">
            <div class="footer">
                <asp:HyperLink CssClass="btn btn-outline-secondary" runat="server" NavigateUrl="~/Vistas/Gestion/Definiciones/General/Perfiles">Cancelar</asp:HyperLink>
                <asp:Button ID="BtnGuardar" CssClass="btn btn-outline-primary " Text="Guardar" runat="server" OnClientClick="Guardar();return false;" ValidationGroup="Global" ClientIDMode="Static" />
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <script src="/Scripts/Global/alerts.js"></script>
    <script>
        const isModified = () => { }

        let formularios = [];
        const Add = (e) => {
            let idFormulario = $(e).attr('idFormulario');

            formularios.push(idFormulario);
            formularios = formularios.filter((el, index) => formularios.indexOf(el) === index);
            //console.log(permisos)
        }

        const Guardar = () => {
            if (Page_ClientValidate('Global')) {
                formularios = formularios == "" ? '<%=ViewState["formularios"]%>' : formularios;
                __doPostBack('Add', formularios)
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script src="/Content/js/dataTables.bootstrap5.min.js"></script>
    <script>

        //desactivar ordenamineto
        config.columnDefs = [{
            'targets': [2], 'orderable': false,
        }]

        const table = $('#Gridview1').DataTable(config);

        const CheckAll = () => {

            let dat = table.column(2).data();
            dat.each((e, i) => {
                let $chk = $(e).find('input:checkbox');

                $chk.click();
                $chk.attr('checked')

            })
        }
    </script>
</asp:Content>
