<%@ Page Title="Gestionar Tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionarTickets.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.GestionarTickets" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="Content/css/dataTables.bootstrap5.min.css" rel="stylesheet" />
    <link href="/Content/css/Tickets.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="row mb-4 text-center">
            <div class="col">
                <h3 class="text-white fw-bold">Gestión de Tickets</h3>
            </div>
        </div>

        <div class="row row-cols-1 row-cols-md-3 g-4">
            <asp:Repeater ID="RepeaterTickets" runat="server" OnItemDataBound="RepeaterTickets_ItemDataBound">
                <ItemTemplate>
                    <div class="col">
                        <!-- Se aplica la clase personalizada para el diseño -->
                        <div class="card shadow-lg card-custom-tickets h-100">
                            <div class="card-header text-black text-center fs-1">
                                <asp:Label ID="lblTitulo" runat="server" Text='<%# Eval("titulo") %>' CssClass="m-0 fs-1"></asp:Label>
                            </div>
                            <div class="card-body">
                                <asp:HiddenField ID="hfTicketId" runat="server" Value='<%# Eval("idTicket") %>' />
                                <h6 class="text-muted">Ticket ID: <%# Eval("idTicket") %></h6>
                                <p><strong>Descripción:</strong>
                                    <asp:Label ID="lblDescripcion" runat="server" Text='<%# Eval("descripcion") %>'></asp:Label>
                                </p>
                                <p><strong>Departamento:</strong> <%# Eval("departamento") %></p>

                                <div class="mb-2">
                                    <label class="fw-bold">Clasificación:</label>
                                    <asp:DropDownList ID="ddlClasificacion" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Soporte" Value="Soporte"></asp:ListItem>
                                        <asp:ListItem Text="Mantenimiento" Value="Mantenimiento"></asp:ListItem>
                                        <asp:ListItem Text="Incidencia" Value="Incidencia"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="mb-2">
                                    <label class="fw-bold">Prioridad:</label>
                                    <asp:DropDownList ID="ddlPrioridad" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Baja" Value="Baja"></asp:ListItem>
                                        <asp:ListItem Text="Media" Value="Media"></asp:ListItem>
                                        <asp:ListItem Text="Alta" Value="Alta"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="mb-2">
                                    <label class="fw-bold">Estado:</label>
                                    <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEstado_SelectedIndexChanged">
                                        <asp:ListItem Text="Abierto" Value="Abierto"></asp:ListItem>
                                        <asp:ListItem Text="En Proceso" Value="En Proceso"></asp:ListItem>
                                        <asp:ListItem Text="Cerrado" Value="Cerrado"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <p><strong>Creado Por:</strong> <%# Eval("creadoPor") %></p>

                                <div class="mb-2">
                                    <label class="fw-bold">Técnico Asignado:</label>
                                    <asp:DropDownList ID="ddlTecnicos" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>

                                <div class="mb-3">
                                    <label class="fw-bold">Fecha Cierre:</label>
                                    <asp:TextBox ID="txtFechaCierre" runat="server" Text='<%# Eval("fechaCierra") %>' CssClass="form-control"></asp:TextBox>
                                </div>
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" 
                                    CssClass="btn btn-success btn-sm w-100 btnGuardar" 
                                    OnClick="btnGuardar_Click" 
                                    OnClientClick="disableButton(this);" />
                                <asp:Button ID="btnCerrar" runat="server" Text="Cerrar Ticket" CssClass="btn btn-danger btn-sm w-100 mt-2" OnClick="btnCerrar_Click" />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <link href="/Content/css/Tickets.css" rel="stylesheet" />
    <script src="/Scripts/Global/alerts.js"></script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script src="/Scripts/jquery.dataTables.min.js"></script>
    <script src="/Content/js/dataTables.bootstrap5.min.js"></script>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // Para cada tarjeta se reactiva el botón "Guardar" cuando cambia un input/select
            var cards = document.querySelectorAll('.card');
            cards.forEach(function (card) {
                var inputs = card.querySelectorAll('input, select, textarea');
                inputs.forEach(function (input) {
                    input.addEventListener('change', function () {
                        var btnGuardar = card.querySelector('.btnGuardar');
                        if (btnGuardar) {
                            btnGuardar.disabled = false;
                        }
                    });
                });
            });
        });
    </script>
</asp:Content>

