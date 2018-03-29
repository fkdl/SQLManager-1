$("#SQLConnection").hide();
//
// SQL type selection
//
function SelectSQL() {
    var _Selection = $("#SelectSQL input[type='radio']:checked").val();

    $("#HomeError").hide();
    $("#SQLType").hide();

    if (_Selection == "SQLServer") {
        $("#SQLServer").removeClass("hidden");
        $("#SQLServer").show();
    }
    else if (_Selection == "MySQL") {
        $("#MySQL").removeClass("hidden");
        $("#MySQL").show();
    }
    else {
        $("#SQLite").removeClass("hidden");
        $("#SQLite").show();
    }

    return false;
}

function BackToServerTypes() {
    $("#SQLType").show();
    $("#SQLServer").addClass("hidden");
    $("#MySQL").addClass("hidden");
    $("#SQLite").addClass("hidden");
}
//
// Add data to table
//
function ResetAddLine() {
    document.getElementById("addLineForm").reset();
    $("#addLineForm").show();
    $("#addLineWork").addClass("hidden");
}

function AddLine() {
    var _form = document.getElementById("addLineForm");
    var _insert = "";
    var _id = "";
    var counter = 0;
    //
    // Get first element with value
    //
    for(i = 0; i < _form.elements.length - 1; i++)
    {
        if (_form.elements[i].value.length > 0) {
            _insert = _form.elements[i].value;
            _id = _form.elements[i].id;
            counter = i + 1;
            break;
        }
    }
    //
    // Get rest elements with values
    //
    for(i = counter; i < _form.elements.length - 1; i++)
    {
        if (_form.elements[i].value.length > 0) {
            _insert += ", " + _form.elements[i].value;
            _id += ", " + _form.elements[i].id;
        }
    }

    $("#addLineForm").hide();
    $("#addLineWork").removeClass("hidden");
    $("#addLineWork").show();

    var ToSend = 
    {
        InsertData: _insert,
        FieldNames: _id
    };

    $.ajax({
        url: "/Table/Add",
        type: "POST",
        data: ToSend,
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        success: function(data) {
            //alert("Komple");
            location.reload();
        }
    })
    .fail(function (jqXHR, textStatus, errorThrown){
        alert(errorThrown);
        $("#addLineForm").show();
        $("#addLineWork").addClass("hidden");
    });

    return false;
}