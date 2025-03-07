<%@ Page Title="Galeria" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Galeria.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Reportes.Galeria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <!-- Bootstrap y estilos personalizados -->
    <link href="/Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <link href="/Content/css/Galeria.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row mb-3 text-center">
        <div class="col">
            <h4 class="text-white"><strong>Galeria</strong></h4>
        </div>
    </div>
    <!-- Se asume que el ScriptManager ya está en la MasterPage -->
    <div class="container py-4">
        <div class="card shadow mb-4">
            <!-- Repeater de Tickets -->
            <div class="card-body">
                <asp:Repeater ID="RepeaterGallery" runat="server" OnItemCommand="RepeaterGallery_ItemCommand">
                    <HeaderTemplate>
                        <div class="row row-cols-1 row-cols-md-2 row-cols-lg-4 g-4">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="col">
                            <div class="card h-100 shadow card-custom">
                                <img src='<%# Eval("Imagen") %>' class="card-img-top" alt="Imagen Ticket">
                                <div class="card-body">
                                    <h5 class="card-title text-truncate"><strong><%# Eval("Titulo") %></strong></h5>
                                    <p class="badge bg-primary"><%# Eval("Estado") %></p>
                                    <p class="card-text">
                                        <small class="text-muted"><strong>Tiempo de Solución:</strong> <%# Eval("TiempoSolucion") %> días</small>
                                    </p>
                                    <p class="card-text"><strong>Descripción:</strong> <%# Eval("Descripcion") %></p>
                                    <asp:LinkButton ID="btnVerDetalles" runat="server" CommandName="VerDetalles"
                                        CommandArgument='<%# Eval("IdTicket") %>' CssClass="btn btn-outline-info w-100">
                                        <i class="bi bi-search"></i> Ver Detalles
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <!-- Paginación -->
            <div class="card-footer text-center">
                <asp:Literal ID="litPaging" runat="server"></asp:Literal>
            </div>
        </div>

        <!-- UpdatePanel para refrescar solo el modal -->
        <asp:UpdatePanel ID="upModal" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Modal de Detalles -->
                <div class="modal fade" id="ModalDetalles" tabindex="-1" aria-hidden="true">
                    <div class="modal-dialog modal-lg modal-dialog-centered">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title"><strong>Detalles del Ticket</strong></h4>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <!-- Menú de pestañas: Detalles, Mensajes y Archivos -->
                                <ul class="nav nav-tabs" id="detallesTab" role="tablist">
                                    <li class="nav-item" role="presentation">
                                        <button class="nav-link active" id="detalles-tab" data-bs-toggle="tab" data-bs-target="#detalles" type="button" role="tab" aria-controls="detalles" aria-selected="true">
                                            <strong>Detalles</strong>
                                        </button>
                                    </li>
                                    <li class="nav-item" role="presentation">
                                        <button class="nav-link" id="mensajes-tab" data-bs-toggle="tab" data-bs-target="#mensajes" type="button" role="tab" aria-controls="mensajes" aria-selected="false">
                                            <strong>Mensajes</strong>
                                        </button>
                                    </li>
                                    <li class="nav-item" role="presentation">
                                        <button class="nav-link" id="archivos-tab" data-bs-toggle="tab" data-bs-target="#archivos" type="button" role="tab" aria-controls="archivos" aria-selected="false">
                                            <strong>Archivos</strong>
                                        </button>
                                    </li>
                                </ul>
                                <div class="tab-content mt-3" id="detallesTabContent">
                                    <div class="tab-pane fade show active" id="detalles" role="tabpanel" aria-labelledby="detalles-tab">
                                        <asp:Literal ID="litDetalles" runat="server"></asp:Literal>
                                    </div>
                                    <div class="tab-pane fade" id="mensajes" role="tabpanel" aria-labelledby="mensajes-tab">
                                        <ul class="list-group">
                                            <asp:Literal ID="litMensajes" runat="server"></asp:Literal>
                                        </ul>
                                    </div>
                                    <div class="tab-pane fade" id="archivos" role="tabpanel" aria-labelledby="archivos-tab">
                                        <ul class="list-group">
                                            <asp:Literal ID="litArchivos" runat="server"></asp:Literal>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button class="btn btn-outline-secondary" data-bs-dismiss="modal">
                                    <strong>Cerrar</strong>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="RepeaterGallery" EventName="ItemCommand" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <link href="/Content/css/Galeria.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
</asp:Content>







