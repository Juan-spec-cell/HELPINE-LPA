<%@ Page Title="Errores por Fecha" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ErroresPorFecha.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Reportes.ErroresPorFecha" %>

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
                <h4 class="text-white"><strong>Errores por Fecha</strong></h4>
            </div>
        </div>

        <div class="card pt-2">
            <div class="card-header">
                <asp:UpdatePanel ID="upControls" runat="server">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-md-3">
                                <label class="form-label">Fecha Inicio:</label>
                                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-3">
                                <label class="form-label">Fecha Fin:</label>
                                <asp:TextBox ID="txtFechaFin" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                            </div>
                            <div class="col-md-2 d-flex align-items-end">
                                <asp:Button ID="btnBuscar" CssClass="btn btn-outline-primary" Text="Buscar" runat="server" OnClick="btnBuscar_Click" ClientIDMode="Static" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="card p-3 mt-3">
                <div class="card-body">
                    <asp:UpdatePanel ID="upGridView" runat="server">
                        <ContentTemplate>
                            <div class="table-responsive">
                                <asp:GridView ID="gvErroresFecha" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover" Width="100%">
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="#" />
                                        <asp:BoundField DataField="Fecha" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd}" />
                                        <asp:BoundField DataField="Departamento" HeaderText="Departamento" />
                                        <asp:BoundField DataField="TituloTicket" HeaderText="Título Ticket" />
                                        <asp:BoundField DataField="Clasificacion" HeaderText="Clasificación" />
                                        <asp:BoundField DataField="Prioridad" HeaderText="Prioridad" />
                                        <asp:BoundField DataField="Estado" HeaderText="Estado" />
                                        <asp:BoundField DataField="fechaCierra" HeaderText="Fecha Cierra" DataFormatString="{0:yyyy-MM-dd}" />
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

            $('#<%= gvErroresFecha.ClientID %>').DataTable(config);
        });

        function applyDataTable(gridViewName) {
            $(gridViewName).DataTable(config);
        }
    </script>
</asp:Content>
