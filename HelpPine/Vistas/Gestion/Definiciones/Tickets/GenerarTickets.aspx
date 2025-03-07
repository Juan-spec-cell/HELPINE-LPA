<%@ Page Title="Generar Tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenerarTickets.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.GenerarTickets" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="row mb-3 text-center">
            <div class="col">
                <h4 class="text-white"><strong>Generar Tickets</strong></h4>
            </div>
        </div>
        <div class="card shadow mt-3 p-4">
            <div class="row g-3">
                <div class="col-12 col-md-4">
                    <label for="txtTitulo">Título Ticket</label>
                    <asp:TextBox ID="txtTitulo" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-12 col-md-4">
                    <label for="txtDescripcion">Descripción del Ticket</label>
                    <asp:TextBox ID="txtDescripcion" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-12 col-md-4">
                    <label for="ddlPrioridad">Prioridad</label>
                    <asp:DropDownList ID="ddlPrioridad" CssClass="form-select" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-4">
                    <label for="ddlEstado">Estado</label>
                    <asp:DropDownList ID="DropDownList1" CssClass="form-select" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="col-12 col-md-4">
                    <label for="ddlClasificacion">Clasificación</label>
                    <asp:DropDownList ID="ddlClasificacion" CssClass="form-select" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="row g-3 mt-3">
                <div class="col-12">
                    <label for="txtComentario">Comentario</label>
                    <asp:TextBox ID="txtComentario" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4"></asp:TextBox>
                </div>
            </div>
            <!-- Control de selección de archivo -->
            <div class="mt-3">
                <label for="FileUpload">Seleccionar Archivo</label>
                <asp:FileUpload ID="FileUpload" runat="server" CssClass="form-control" AllowMultiple="true" />
            </div>
            <div class="row mt-3 text-center">
                <div class="col">
                    <asp:Button ID="btnGenerarTicket" CssClass="btn btn-outline-primary" runat="server" Text="Guardar Ticket" OnClick="btnGenerarTicket_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>








