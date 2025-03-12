<%@ Page Title="Tickets Cerrados" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TicketsCerrados.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.TicketsCerrados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="row mb-3 text-center">
            <div class="col">
                <h4 class="text-white"><strong>Tickets Cerrados</strong></h4>
            </div>
        </div>

        <div class="col">
            <h4 class="text-white">
                <asp:Label ID="lblNoTickets" runat="server" Style="color: white;" Visible="false"></asp:Label>
            </h4>
        </div>

        <div class="card p-3 mt-3">
            <div class="card-body">
                <div class="table-responsive">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="GridViewTickets" runat="server" CssClass="table table-sm table-bordered text-center" AutoGenerateColumns="False" Visible="false">
                                <Columns>
                                    <asp:BoundField DataField="idTicket" HeaderText="#" />
                                    <asp:BoundField DataField="titulo" HeaderText="Título" />
                                    <asp:BoundField DataField="email_creador" HeaderText="Email Reportador" />
                                    <asp:BoundField DataField="departamento" HeaderText="Depto" />
                                    <asp:BoundField DataField="tecnico_asignado" HeaderText="Técnico" />
                                    <asp:BoundField DataField="perfil_tecnico" HeaderText="Perfil" />
                                    <asp:BoundField DataField="email_tecnico" HeaderText="Email Técnico" />
                                    <asp:BoundField DataField="estado" HeaderText="Estado" />
                                    <asp:BoundField DataField="fechaCreacion" HeaderText="Fecha Creación" DataFormatString="{0:yyyy-MM-dd}" />
                                    <asp:BoundField DataField="fechaCierra" HeaderText="Fecha Cierre" DataFormatString="{0:yyyy-MM-dd}" />
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

