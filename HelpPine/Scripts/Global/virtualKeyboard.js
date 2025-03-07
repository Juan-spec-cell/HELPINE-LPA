
if (x != '') {
    if (x == 'False') {
        //Teclado numerico
        $(".number").keyboard({
            display: {
                bksp: "\u2190",
                accept: "Aceptar",
                cancel: "Cancelar",
                next: "\u21e8",
                prev: "\u21e6",
            },
            layout: "custom",
            customLayout: {
                normal: ["9 8 7 {bksp}", "6 5 4 .", "3 2 1 0", "{prev} {next}"],
            },
            maxLength: 15,
            // Prevent keys not in the displayed keyboard from being typed in
            restrictInput: true,
            // don't use combos or A+E could become a ligature
            useCombos: false,
            // activate the "validate" callback function
            acceptValid: true,
            autoAccept: true,
            validate: function (kb, value, isClosing) {
                let x = [...value].filter(l => l === '.').length

                if (value.includes('..') || x > 1) {
                    value = parseFloat(value)
                    kb.setValue(value, $.el)
                }

                if (value.toString().startsWith(".")) {
                    kb.setValue('0', $.el)
                }

                if (value.toString().endsWith(".") && x > 1)
                    kb.setValue(parseFloat(value), $.el)

                return true
            },
            tabNavigation: true,
        });

        //Teclado completo
        $(".text").keyboard({
            display: {
                bksp: "\u2190",
                accept: "Aceptar",
                normal: "abc",
                meta1: "123",
                meta2: "#+=",
                next: "\u21e8",
                prev: "\u21e6",
            },

            layout: "custom",
            customLayout: {
                normal: [
                    "q w e r t y u i o p {bksp}",
                    "a s d f g h j k l ñ ;",
                    "{s} z x c v b n m , . {s}",
                    "{meta1} {prev} {space} {next}",
                ],
                shift: [
                    "Q W E R T Y U I O P {bksp}",
                    "A S D F G H J K L Ñ",
                    "{s} Z X C V B N M ! ? {s}",
                    "{meta1} {prev} {space} {next}",
                ],
                meta1: [
                    "1 2 3 4 5 6 7 8 9 0 {bksp}",
                    "- / : ; ( ) \u20ac & @ ",
                    "{meta2} . , ? ! ' \"",
                    "{normal} {prev} {space} {next}",
                ],
                meta2: [
                    "[ ] { } # % ^ * + = {bksp}",
                    "_ \\ | ~ &lt; &gt; $ \u00a3 \u00a5",
                    ". , ? ! ' \"",
                    "{normal} {prev} {space} {next}",
                ],
            },
            autoAccept: true,
            tabNavigation: true,
        });
    }
    else {
        $(".number").attr('type', 'number')
    }
}
else {
    $(".number").attr('type', 'number')
    $('.select2-search__field').attr('type', 'text');
}