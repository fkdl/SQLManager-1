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
    } else if (_Selection == "MySQL") {
        $("#MySQL").removeClass("hidden");
        $("#MySQL").show();
    } else {
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
function ResetLine(type) {
    if (type == Add) {
        document.getElementById("addLineForm").reset();
        $("#addLineForm").show();
        $("#addLineWork").addClass("hidden");
    } else {
        document.getElementById("editLineForm").reset();
        $("#editLineForm").show();
        $("#editLineWork").addClass("hidden");
    }
}
//
// Modal data to method
//
function Line(type) {
    var _insert = new Array();
    var _id = "";

    alert(type);

    if (type == "Add") {
        var _form = document.getElementById("addLineForm");
        var _url = "/Table/Add";

        $("#addLineForm").hide();
        $("#addLineWork").removeClass("hidden");
        $("#addLineWork").show();
    } else {
        var _form = document.getElementById("editLineForm");
        var _url = "/Table/Edit";

        $("#editLineForm").hide();
        $("#editLineWork").removeClass("hidden");
        $("#editLineWork").show();
    }
    //
    // Get elements with values
    //
    for (i = 0; i < _form.elements.length - 1; i++) {
        if (_form.elements[i].value.length > 0) {
            _insert.push(_form.elements[i].value);
            _id += _form.elements[i].id + ", ";
        }
    }

    var ToSend = {
        InsertData: _insert,
        FieldNames: _id,
        TableName: document.getElementById("TableName").value
    };

    $.ajax({
            url: _url,
            type: "POST",
            data: ToSend,
            success: function (data) {
                location.reload();
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
            if (type == "Add") {
                $("#addLineForm").show();
                $("#addLineWork").addClass("hidden");
            } else {
                $("#editLineForm").show();
                $("#editLineWork").addClass("hidden");
            }
        });

    return false;
}

function EditLineData(line) {
    var _table = document.getElementById("LinesTable").rows[line];

    var _tableArray = [];

    for (i = 1; i < _table.cells.length; i++) {
        _tableArray[$("#LinesTable th").eq(i).text()] = _table.cells[i].innerHTML;
    }

    var _form = document.getElementById("editLineForm");

    for (i = 0; i < _form.elements.length - 1; i++) {
        _form.elements[i].value = _tableArray[_form.elements[i].id];
    }
}