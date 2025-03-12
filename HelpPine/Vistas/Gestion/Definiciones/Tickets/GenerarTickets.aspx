<%@ Page Title="Generar Tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GenerarTickets.aspx.cs" Inherits="HelpPine.Vistas.Gestion.Definiciones.Tickets.GenerarTickets" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <div class="row mb-4 text-center">
            <div class="col">
                <h3 class="text-white fw-bold">Generar Tickets</h3>
            </div>
        </div>

        <div class="card shadow-sm p-4">
            <div class="row g-3">
                <div class="col-md-6">
                    <label class="form-label" for="txtTitulo">Título Ticket</label>
                    <asp:TextBox ID="txtTitulo" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-6">
                    <label class="form-label" for="txtDescripcion">Descripción del Ticket</label>
                    <asp:TextBox ID="txtDescripcion" CssClass="form-control" runat="server"></asp:TextBox>
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="ddlPrioridad">Prioridad</label>
                    <asp:DropDownList ID="ddlPrioridad" CssClass="form-select" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="ddlEstado">Estado</label>
                    <asp:DropDownList ID="DropDownList1" CssClass="form-select" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="col-md-4">
                    <label class="form-label" for="ddlClasificacion">Clasificación</label>
                    <asp:DropDownList ID="ddlClasificacion" CssClass="form-select" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="col-12">
                    <label class="form-label" for="txtComentario">Comentario</label>
                    <asp:TextBox ID="txtComentario" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="4"></asp:TextBox>
                </div>
                <div class="col-12">
                    <label class="form-label" for="FileUpload">Seleccionar Archivo</label>
                    <div class="dropzone" id="dropzone" onclick="document.getElementById('<%= FileUpload.ClientID %>').click();">
                        <asp:FileUpload ID="FileUpload" runat="server" CssClass="form-control" AllowMultiple="true" style="display:none;" onchange="mostrarArchivos(this.files)" />
                        <div class="dz-message">Arrastra y suelta archivos aquí o haz clic para subir</div>
                        <div class="dz-preview"></div>
                    </div>
                </div>
                <div class="col-12 text-center">
                    <asp:Button ID="btnGenerarTicket" CssClass="btn btn-outline-primary" runat="server" Text="Guardar Ticket" OnClientClick="return validarFormulario();" OnClick="btnGenerarTicket_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="CSS" runat="server">
    <link href="/Content/css/fileupload.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="JS" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10"></script>
    <script src="/Scripts/Global/alerts.js"></script>
    <script type="text/javascript">
        function validarFormulario() {
            var titulo = document.getElementById('<%= txtTitulo.ClientID %>').value.trim();
            var descripcion = document.getElementById('<%= txtDescripcion.ClientID %>').value.trim();
            var prioridad = document.getElementById('<%= ddlPrioridad.ClientID %>').value;
            var estado = document.getElementById('<%= DropDownList1.ClientID %>').value;
            var clasificacion = document.getElementById('<%= ddlClasificacion.ClientID %>').value;

            if (titulo === "" || descripcion === "" || prioridad === "" || estado === "" || clasificacion === "") {
                mostrarAlertaError("Por favor, complete todos los campos obligatorios.");
                return false;
            }
            return true;
        }

        function mostrarAlertaError(mensaje) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: mensaje,
                timer: 2000,
                showConfirmButton: false
            });
        }

        function mostrarAlertaExito(mensaje) {
            Swal.fire({
                icon: 'success',
                title: 'Éxito',
                text: mensaje,
                timer: 2000,
                showConfirmButton: false
            });
        }

        // Funciones para arrastrar y soltar archivos
        const dropzone = document.getElementById('dropzone');
        const fileUpload = document.getElementById('<%= FileUpload.ClientID %>');

        dropzone.addEventListener('dragover', (e) => {
            e.preventDefault();
            dropzone.classList.add('dragover');
        });

        dropzone.addEventListener('dragleave', (e) => {
            e.preventDefault();
            dropzone.classList.remove('dragover');
        });

        dropzone.addEventListener('drop', (e) => {
            e.preventDefault();
            dropzone.classList.remove('dragover');
            const files = e.dataTransfer.files;
            fileUpload.files = files;
            mostrarArchivos(files);
        });

        function mostrarArchivos(files) {
            const preview = document.querySelector('.dz-preview');
            preview.innerHTML = '';
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                const fileElement = document.createElement('div');
                fileElement.classList.add('dz-filename');
                fileElement.textContent = file.name;
                preview.appendChild(fileElement);
            }
        }
    </script>
</asp:Content>








