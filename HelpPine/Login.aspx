<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="HelpPine.Login" %>

<!DOCTYPE html>
<html lang="en"<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link rel="shortcut icon" href="~/Content/img/Logo.jpg" />
    <link href="~/Content/css/app.css" rel="stylesheet" />
    <link href="/Content/css/iziToasst.min.css" rel="stylesheet" />
    <title>Inicio de Sesión | LPA DelpDesk</title>
    <script src="https://code.jquery.com/jquery-3.7.0.min.js"></script>
</head>  </style>
</head>

<body class="bg-success" style="background-color: #729d86 !important;">
    <main class="d-flex w-100 h-100">
        <div class="container d-flex flex-column">
            <div class="row vh-100">
                <div class="col-sm-10 col-md-8 col-lg-6 mx-auto d-table h-100">
                    <div class="d-table-cell align-middle">
                        <div class="card">
                            <div class="card-body pt-0">
                                <div class="m-sm-4">
                                    <div class="text-center">
                                        <img src="/Content/img/Logo.jpg" alt="Charles Hall" class="img-fluid" width="145" height="132" />
                                    </div>

                                    <h4 class="h4 mb-2 text-center"><strong runat="server" id="pruebas"></strong></h4>
                                    <h4 class="h4 mb-5 mt-2 text-center"><strong>LPA | HelpDesk</strong></h4>

                                    <form runat="server" class="container d-flex flex-column align-items-center w-100">
                                        <asp:HiddenField ID="messages" ClientIDMode="Static" runat="server" />

                                        <div class="w-100 mb-3">
                                            <asp:TextBox
                                                ID="txtUser" runat="server" placeholder="Usuario" autocomplete="off"
                                                Style="background-color: var(--bs-success);" CssClass="form-control form-control-lg text-white border-white"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server"
                                                ErrorMessage="Escriba el usuario para continuar..." ControlToValidate="txtUser"
                                                CssClass="text-danger"></asp:RequiredFieldValidator>
                                          <div class="w-100 mb-3">
                                            <asp:TextBox CssClass="form-control form-control-lg text-white border-white"
                                                ID="txtPass" runat="server" placeholder="Contraseña" TextMode="Password"
                                                Style="background-color: var(--bs-success);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server"
                                                ErrorMessage="Escriba la contraseña para continuar..." ControlToValidate="txtPass"
                                                CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>

                                        <div class="text-center w-100 mt-3">
                                            <asp:Button CssClass="btn btn-lg text-white border-white w-100"
                                                ID="BtnLogin" runat="server" Text="Iniciar Sesión" OnClick="BtnLogin_Click" 
                                                OnClientClick="return validarLogin();"
                                                Style="background-color: var(--bs-success);"></asp:Button>
                                        </div>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                ErrorMessage="Escriba la contraseña para continuar..." ControlToValidate="txtPass"
                                                CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </form>

                                </div>
                            </div>
                        </div>
                        <div class="text-center">
                            <strong class="text-white">&copy;<%: DateTime.Now.Year %> | Los Pinos Apparel</strong>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script src="/Content/js/app.js"></script>
    <script src="/Content/js/iziToast.min.js"></script>
    <script>
        function validarLogin() {
            var usuario = document.getElementById('<%= txtUser.ClientID %>').value.trim();
            var contrasena = document.getElementById('<%= txtPass.ClientID %>').value.trim();

            if (usuario === "" || contrasena === "") {
                Swal.fire({
                    icon: 'warning',
                    title: 'Datos incompletos',
                    text: 'Por favor, complete todos los campos para continuar.',
                });
                return false;
            }
            return true;
        }

        let alerts = $("#messages").val();
        console.log(alerts);

        if (alerts != '') {
            alerts = alerts.split('|')

            Swal.fire({
                icon: alerts[0] === 'Error' ? 'error' : 'warning',
                title: alerts[0],
                text: alerts[2],
                confirmButtonColor: alerts[2]
            });
        }
    </script>
</body>
</html>
