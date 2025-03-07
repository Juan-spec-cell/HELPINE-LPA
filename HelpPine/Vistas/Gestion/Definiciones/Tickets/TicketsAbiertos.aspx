<%@ Page Title="Tickets Abiertos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TicketsAbiertos.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.TicketsAbiertos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="row mb-3 text-center">
            <div class="col">
                <h4 class="text-white">
                    <strong>Tickets Abiertos</strong>
                </h4>
            </div>
        </div>

        <asp:Label ID="lblNoTickets" runat="server" CssClass="text-danger" Visible="false"></asp:Label>

        <div class="card p-3 mt-3">
            <div class="card-body">
                <div class="table-responsive">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:GridView 
                                ID="GridViewTickets" 
                                runat="server"
                                CssClass="table table-sm table-bordered text-center"
                                AutoGenerateColumns="False" 
                                Visible="false"
                                OnRowDataBound="GridViewTickets_RowDataBound"
                                OnRowCommand="GridViewTickets_RowCommand">
                                <Columns>
                                    <asp:BoundField DataField="idTicket" HeaderText="#" />
                                    <asp:BoundField DataField="Departamento" HeaderText="Depto" />
                                    <asp:BoundField DataField="TecnicoAsignado" HeaderText="Técnico Asignado" />
                                    <asp:BoundField DataField="TipoTecnico" HeaderText="Tipo Técnico" />
                                    <asp:BoundField DataField="titulo" HeaderText="Título" />
                                    <asp:BoundField DataField="Creador" HeaderText="Creador" />
                                    <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                                    <asp:BoundField DataField="clasificacion" HeaderText="Clasificacion" />
                                    <asp:BoundField DataField="prioridad" HeaderText="Prioridad" />
                                    <asp:BoundField DataField="estado" HeaderText="Estado" />
                                    <asp:BoundField 
                                        DataField="fechaCreacion" 
                                        HeaderText="Fecha Creación" 
                                        DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton 
                                                ID="btnResponder" 
                                                runat="server" 
                                                CommandName="Responder"
                                                CommandArgument='<%# Eval("idTicket") %>' 
                                                CssClass="btn btn-primary" 
                                                Visible="false" 
                                                ToolTip="Responder">
                                                <i class="bi bi-reply-fill"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton
                                                ID="btnEnviarMensaje"
                                                runat="server"
                                                CommandName="EnviarMensaje"
                                                CommandArgument='<%# Eval("idTicket") %>'
                                                CssClass="btn btn-primary"
                                                Visible="false"
                                                ToolTip="Enviar Mensaje">
                                                <i class="bi bi-send"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton
                                                ID="btnCerrarTicket"
                                                runat="server"
                                                CommandName="CerrarTicket"
                                                CommandArgument='<%# Eval("idTicket") %>'
                                                CssClass="btn btn-danger"
                                                Visible="false"
                                                ToolTip="Cerrar Ticket">
                                                <i class="bi bi-x-circle"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton
                                                ID="btnCancelarTicket"
                                                runat="server"
                                                CommandName="CancelarTicket"
                                                CommandArgument='<%# Eval("idTicket") %>'
                                                CssClass="btn btn-warning"
                                                Visible="false"
                                                ToolTip="Cancelar Ticket">
                                                <i class="bi bi-x-square"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <HeaderStyle CssClass="text-center" />
                                <RowStyle CssClass="text-center" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <script src="/Scripts/Global/alerts.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script src="/Content/js/dataTables.bootstrap5.min.js"></script>
    <script>
        $(document).ready(function () {
            var config = {
                // Configuración de DataTable
                drawCallback: function (settings) {
                    feather.replace();
                }
            };

            $('#<%= GridViewTickets.ClientID %>').DataTable(config);
        });

        function applyDataTable(gridViewName) {
            $(gridViewName).DataTable(config);
        }
    </script>
</asp:Content>




