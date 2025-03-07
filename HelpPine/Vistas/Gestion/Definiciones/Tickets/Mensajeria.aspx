<%@ Page Title="Mensajeria" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Mensajeria.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.Mensajeria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/css/Mensajeria.css" rel="stylesheet" type="text/css" />
    <div class="container py-4">
        <div class="row mb-3 text-center">
            <div class="col">
                <h4 class="text-white"><strong>Mensajería del Ticket</strong></h4>
            </div>
        </div>

        <!-- Detalles del ticket -->
        <div class="card p-3 mt-3">
            <div class="card-body">
                <h5 class="card-title">Detalles del Ticket</h5>
                <p>
                    <strong>ID del Ticket:</strong>
                    <asp:Label ID="lblTicketId" runat="server" />
                </p>
                <p>
                    <strong>Nombre:</strong>
                    <asp:Label ID="lblNombreCreador" runat="server" />
                </p>
                <p>
                    <strong>Título:</strong>
                    <asp:Label ID="lblTitulo" runat="server" />
                </p>
                <p>
                    <strong>Estado:</strong>
                    <asp:Label ID="lblEstado" runat="server" />
                </p>
                <p>
                    <strong>Prioridad:</strong>
                    <asp:Label ID="lblPrioridad" runat="server" />
                </p>
                <p>
                    <strong>Descripcion:</strong>
                    <asp:Label ID="lblDescripcion" runat="server" />
                </p>

                <h5 class="card-title">Archivos Adjuntos</h5>
                <asp:Label ID="lblMensajeArchivos" runat="server" Visible="false" ForeColor="Red"></asp:Label>
                <asp:Repeater ID="rptArchivos" runat="server">
                    <ItemTemplate>
                        <a href='<%# Eval("RutaArchivo") %>' target="_blank"><%# Eval("NombreArchivo") %></a><br />
                    </ItemTemplate>
                </asp:Repeater>


            </div>
        </div>


        <!-- Mensajes y área de envío -->
        <div class="card p-3 mt-3">
            <div class="card-body">
                <h5 class="card-title">Mensajes</h5>
                <asp:UpdatePanel ID="UpdatePanelMensajes" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Timer ID="TimerMensajes" runat="server" Interval="5000" OnTick="TimerMensajes_Tick" />
                        <!-- Se agrega id para el control del scroll -->
                        <div id="mensajesContainer" class="mensajes-container">
                            <asp:Repeater ID="rptMensajes" runat="server">
                                <ItemTemplate>
                                    <div class='<%# Eval("ClaseMensaje") %>'>
                                        <div class="mensaje">
                                            <strong><%# Eval("NombreRemitente") %> (<%# Eval("RolRemitente") %>):</strong>
                                            <%# Eval("Mensaje") %>
                                            <em>(<%# Eval("FechaEnvio") %>)</em>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                        <!-- Se agregan los eventos onfocus y onblur para controlar el Timer -->
                        <asp:TextBox ID="txtMensaje" runat="server" TextMode="MultiLine" Rows="4" Columns="50"
                            CssClass="form-control mt-3" onfocus="disableTimer()" onblur="enableTimer()"></asp:TextBox>
                        <asp:Button ID="btnEnviar" runat="server" Text="Enviar" CssClass="btn btn-primary mt-3" OnClick="btnEnviar_Click" OnClientClick="return validarMensaje();" />
                        <asp:Button ID="btnVolverLista" runat="server" Text="Volver a la lista" CssClass="btn btn-secondary mt-3" OnClick="btnVolverLista_Click" />

                        <!-- Control de subida de archivos -->
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnEnviar" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="TimerMensajes" EventName="Tick" />
                    </Triggers>
                </asp:UpdatePanel>

                <asp:UpdateProgress ID="UpdateProgressMensajes" runat="server" AssociatedUpdatePanelID="UpdatePanelMensajes">
                    <ProgressTemplate>
                        <div class="d-flex justify-content-center align-items-center" style="min-height: 150px;">
                            <div class="text-center">
                                <img src="/Content/img/loading2.gif" alt="Cargando..." style="width: 3rem; height: 3rem;" />
                                <div class="mt-2">
                                    <strong>Enviando Mensaje.</strong>
                                </div>
                            </div>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JS" runat="server">
    <script src ="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="/Scripts/Global/alerts.js"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="JS2" runat="server">
    <script type="text/javascript">
        var scrollPos = 0;
        var focusedControlId = "";
        var caretStart = 0;
        var caretEnd = 0;

        function saveScrollPosAndFocus() {
            var mensajes = document.getElementById("mensajesContainer");
            if (mensajes) {
                scrollPos = mensajes.scrollTop;
            }
            if (document.activeElement) {
                focusedControlId = document.activeElement.id;
                // Si el control es el TextBox, guardamos la posición del caret
                if (document.activeElement.id === "<%= txtMensaje.ClientID %>") {
                    if (typeof document.activeElement.selectionStart === "number") {
                        caretStart = document.activeElement.selectionStart;
                        caretEnd = document.activeElement.selectionEnd;
                    }
                }
            }
        }

        function restoreScrollPosAndFocus() {
            var mensajes = document.getElementById("mensajesContainer");
            if (mensajes) {
                mensajes.scrollTop = scrollPos;
            }
            if (focusedControlId) {
                var control = document.getElementById(focusedControlId);
                if (control) {
                    control.focus();
                    // Si es el TextBox, restaurar la posición del caret
                    if (control.id === "<%= txtMensaje.ClientID %>") {
                        if (typeof control.setSelectionRange === "function") {
                            control.setSelectionRange(caretStart, caretEnd);
                        }
                    }
                }
            }
        }

        Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function () {
            saveScrollPosAndFocus();
        });
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            restoreScrollPosAndFocus();
        });

        function disableTimer() {
            var timer = $find('<%= TimerMensajes.ClientID %>');
            if (timer) {
                timer.set_enabled(false);
            }
        }
        function enableTimer() {
            var timer = $find('<%= TimerMensajes.ClientID %>');
            if (timer) {
                timer.set_enabled(true);
            }
        }

        function validarMensaje() {
            var mensaje = document.getElementById('<%= txtMensaje.ClientID %>').value.trim();
            if (mensaje === "") {
                Swal.fire({
                    icon: 'warning',
                    title: 'Mensaje vacío',
                    text: 'No puedes enviar un mensaje vacío.',
                });
                return false;
            }
            return true;
        }
    </script>
</asp:Content>
