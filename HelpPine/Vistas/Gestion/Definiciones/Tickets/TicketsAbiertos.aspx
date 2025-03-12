<%@ Page Title="Tickets Abiertos - Galería" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TicketsAbiertosGaleria.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.TicketsAbiertosGaleria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link href="/Content/css/TicketsAbiertosGaleria.css" rel="stylesheet" />
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

        <div class="col">
            <h4 class="text-white">
                <asp:Label ID="lblNoTickets" runat="server" Style="color: white;" Visible="false"></asp:Label>
            </h4>
        </div>


        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="row equal-height">
                    <asp:Repeater ID="RepeaterTickets" runat="server" OnItemDataBound="RepeaterTickets_ItemDataBound" OnItemCommand="RepeaterTickets_ItemCommand">
                        <ItemTemplate>
                            <div class="col-12 col-md-3">
                                <div class="card card-small mb-3 card-container">
                                    <div class="card-header">
                                        <h5 class="card-title"><%# Eval("titulo") %></h5>
                                    </div>
                                    <div class="card-body">
                                        <p><strong>Departamento:</strong> <%# Eval("Departamento") %></p>
                                        <p><strong>Descripción:</strong> <%# Eval("descripcion") %></p>
                                        <p><strong>Clasificación:</strong> <%# Eval("clasificacion") %></p>
                                        <p><strong>Prioridad:</strong> <%# Eval("prioridad") %></p>
                                        <p><strong>Estado:</strong> <%# Eval("estado") %></p>
                                        <p><strong>Fecha Creación:</strong> <%# Eval("fechaCreacion", "{0:yyyy-MM-dd}") %></p>
                                    </div>
                                    <div class="card-footer text-end">
                                        <asp:LinkButton ID="btnResponder" runat="server" CommandName="Responder" CommandArgument='<%# Eval("idTicket") %>' CssClass="btn btn-primary" ToolTip="Responder">
                                            <i class="bi bi-reply-fill"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnEnviarMensaje" runat="server" CommandName="EnviarMensaje" CommandArgument='<%# Eval("idTicket") %>' CssClass="btn btn-primary" ToolTip="Enviar Mensaje">
                                            <i class="bi bi-send"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnCerrarTicket" runat="server" CommandName="CerrarTicket" CommandArgument='<%# Eval("idTicket") %>' CssClass="btn btn-danger" ToolTip="Cerrar Ticket">
                                            <i class="bi bi-x-circle"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnCancelarTicket" runat="server" CommandName="CancelarTicket" CommandArgument='<%# Eval("idTicket") %>' CssClass="btn btn-warning" ToolTip="Cancelar Ticket">
                                            <i class="bi bi-x-square"></i>
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <script src="/Scripts/Global/alerts.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <!-- Scripts adicionales si son necesarios -->
</asp:Content>

