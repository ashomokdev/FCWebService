$(document).ready(function () {
    $("#inputDocumentDefinition").change(function () {
        if (!$(this).val()) {
            $("#documentDefinitionWarning").show();
        }
        else {
            $("#documentDefinitionWarning").hide();
        }
    });
});

$(document).ready(function () {
    $('#inputDocumentDefinition').change(function () {
        var ext = $('#inputDocumentDefinition').val().split('.').pop().toLowerCase();
        if (ext != "xml" && ext != "") {
            $('#documentDefinitionExtensionError').show();
        }
        else {
            $('#documentDefinitionExtensionError').hide();
        }
    });
});

$(document).ready(function () {
    $("#inputImages").change(function () {
        var ext = $('#inputImages').val().split('.').pop().toLowerCase();

        if (ext == "") {
            $('#imageErrorBadExtension').hide();
            $('#submitButton').attr("disabled", true);
        }
        else if (ext != "jpg")
        {
            $('#imageErrorBadExtension').show();
            $('#submitButton').attr("disabled", true);
        }
        else {
            $('#imageErrorBadExtension').hide();
            $('#submitButton').removeAttr("disabled");
        }
    });
});



