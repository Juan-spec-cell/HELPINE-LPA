
//Datatable
var config = {
    "language": {
        "processing": "Procesando...",
        "lengthMenu": "Mostrar _MENU_ registros",
        "zeroRecords": "No se encontraron resultados",
        "emptyTable": "Ningún dato disponible en esta tabla",
        "infoEmpty": "No se encontraron resultados",
        "infoFiltered": "",
        "search": "Buscar:",
        "infoThousands": ",",
        "loadingRecords": "Cargando...",
        "paginate": {
            "first": "Primero",
            "last": "Último",
            "next": "Siguiente",
            "previous": "Anterior"
        },
        "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
    },
    stateSave: true,
    scrollX: true,
    scrollCollapse: true,
}


const Save = () => {
    $("#BtnGuardar").attr("disabled", "disabled");
    __doPostBack('Guardar');
}

const Save2 = () => {
    if (Page_ClientValidate('Global')) {
        $("#BtnGuardar").attr("disabled", "disabled");
        __doPostBack('Guardar');
    }
}

$('input').attr('autocomplete', 'off');

//Mantener la posicion del scroll
window.scrollTo(0, $("#__SCROLLPOSITIONY").val());

$(".select").change(function () {
    var $input = $(this),
        val = $input.val(),
        list = $input.attr('list'),
        match = $('#' + list + ' option').filter(function () {
            return ($(this).val() === val.toUpperCase());
        });

    if (match.length == 0)
        $input.val('');
});

