<%@ Page Title="Top 10 Errores Comunes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TopErroresComunes.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Reportes.TopErroresComunes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="messages" ClientIDMode="Static" runat="server" />

    <div class="container py-4">
        <div class="row mb-3 text-center">
            <div class="col">
                <h4 class="text-white"><strong>Top 10 Errores Más Comunes</strong></h4>
            </div>
        </div>

        <div class="card p-3 mt-3">
            <div class="card-body">
                <asp:UpdatePanel ID="upGridView" runat="server">
                    <ContentTemplate>
                        <div class="table-responsive">
                            <asp:GridView ID="gvTopErroresComunes" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover" Width="100%">
                                <Columns>
                                    <asp:BoundField DataField="TituloTicket" HeaderText="Título Ticket" />
                                    <asp:BoundField DataField="Clasificacion" HeaderText="Clasificación" />
                                    <asp:BoundField DataField="Prioridad" HeaderText="Prioridad" />
                                    <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                    <asp:BoundField DataField="Departamento" HeaderText="Departamento" />
                                    <asp:BoundField DataField="Cantidad" HeaderText="Cantidad de Errores" />
                                </Columns>
                                <HeaderStyle CssClass="text-center" />
                                <RowStyle CssClass="text-center" />
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <script src="/Scripts/Global/alerts.js"></script>
    <script>
        const isModified = () => { }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script src="/Content/js/dataTables.bootstrap5.min.js"></script>
    <script>
        $(document).ready(function () {
            var config = {
                drawCallback: function (settings) {
                    feather.replace();
                }
            };

            $('#<%= gvTopErroresComunes.ClientID %>').DataTable(config);
        });

        function applyDataTable(gridViewName) {
            $(gridViewName).DataTable(config);
        }
    </script>
</asp:Content>
