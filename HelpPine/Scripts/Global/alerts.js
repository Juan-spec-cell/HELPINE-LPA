
//Mostrar alertas
let alerts = $("#messages").val();

if (alerts != '') {
    alerts = alerts.split('|')
    iziToast.show({
        timeout: 10000,
        title: alerts[0],
        color: alerts[1], // blue, red, green, orange,
        message: alerts[2],
        position: 'topRight', // bottomRight, bottomLeft, topRight, topLeft, topCenter, bottomCenter
        icon: 'far fa-fw fa-bell',
        pauseOnHover: true,
        transitionIn: 'fadeInDown', // bounceInLeft, bounceInRight, bounceInUp, bounceInDown, fadeIn, fadeInDown, fadeInUp, fadeInLeft, fadeInRight or flipInX.
        transitionOut: 'fadeOutUp', // fadeOut, fadeOutUp, fadeOutDown, fadeOutLeft, fadeOutRight, flipOutX
        transitionInMobile: 'fadeInDown',
        transitionOutMobile: 'fadeOutUp',
    });
}




