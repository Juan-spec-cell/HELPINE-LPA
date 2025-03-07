<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="HelpPine._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="row mb-2">
        <h4><strong class="text-white">Inicio</strong></h4>
    </div>


    <div class="card pt-3 pb-3">
        <div class="card-header d-flex justify-content-center">
            <h3 class="mb-0"><strong>HelpDesk LPA | Control de Errores</strong></h3>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="text-center">
                    <img src="/Content/img/Logo.jpg" class="img-fluid " style="width: 30rem" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="JS">
    <script>
        const isModified = () => { }
    </script>
</asp:Content>

