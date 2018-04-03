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
    document.getElementById("addLineForm").reset();
    $("#addLineForm").show();
    $("#addLineWork").addClass("hidden");
}
//
// Modal data to method
//
function Line(type) {
    var _insert = new Array();
    var _id = "";

    if (type == "Add") {
        var _form = document.getElementById("addLineForm");
        var _url = "/Table/Add";
        var _formLength = _form.elements.length - 1;

        $("#addLineForm").hide();
        $("#addLineWork").removeClass("hidden");
        $("#addLineWork").show();
    } else {
        var _form = document.getElementById("editLineForm");
        var _url = "/Table/Edit";
        var _formLength = _form.elements.length - 3;

        $("#editLineForm").hide();
        $("#editLineWork").removeClass("hidden");
        $("#editLineWork").show();
    }
    //
    // Get elements with values
    //
    for (i = 0; i < _formLength; i++) {
        if (_form.elements[i].value.length > 0) {
            _insert.push(_form.elements[i].value);
            _id += _form.elements[i].id + ", ";
        }
    }

    if (type == "Add") {
        var ToSend = {
            InsertData: _insert,
            FieldNames: _id,
            TableName: document.getElementById("TableName").value
        };
    } else {
        var ToSend = {
            InsertData: _insert,
            FieldNames: _id,
            TableName: document.getElementById("TableName").value,
            PrimaryKey: [document.getElementById("ElementName").value, document.getElementById("ElementKey").value]
        };
    }

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
    var _primaryKey = "";
    var _primaryName = "";
    var _tableArray = [];

    for (i = 1; i < _table.cells.length; i++) {
        _tableArray[$("#LinesTable th").eq(i).text()] = _table.cells[i].innerHTML;
        //
        // get first primary key
        //
        if (_table.cells[i].id == "PrimaryKey" && _primaryKey.length == 0) {
            _primaryKey = _table.cells[i].innerHTML;
            _primaryName = $("#LinesTable th").eq(i).text();
        }
    }

    var _form = document.getElementById("editLineForm");

    for (i = 0; i < _form.elements.length - 1; i++) {
        _form.elements[i].value = _tableArray[_form.elements[i].id];
    }

    document.getElementById("ElementKey").value = _primaryKey;
    document.getElementById("ElementName").value = _primaryName;
}

function RemoveLineData(line) {
    var _table = document.getElementById("LinesTable").rows[line];
    var _form = document.getElementById("removeLineForm");

    for (i = 0; i < _table.cells.length; i++) {
        if (_table.cells[i].id == "PrimaryKey") {
            _form.elements["RemoveName"].value = $("#LinesTable th").eq(i).text();
            _form.elements["RemoveValue"].value = _table.cells[i].innerHTML;
            break;
        }
    }
}

function RemoveLine() {
    var _form = document.getElementById("removeLineForm");

    $("#removeLineForm").hide();
    $("#removeLineWork").removeClass("hidden");
    $("#removeLineWork").show();

    var ToSend = {
        RemoveKey: [_form.elements["RemoveName"].value, _form.elements["RemoveValue"].value],
        TableName: document.getElementById("TableName").value
    };

    $.ajax({
            url: "/Table/Remove",
            type: "POST",
            data: ToSend,
            success: function (data) {
                location.reload();
            }
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert(errorThrown);
            $("#removeLineForm").show();
            $("#removeLineWork").addClass("hidden");
        });

    return false;
}
//
// Create Table
//
function ResetTable() {
    var _form = document.getElementById("createTableForm");

    _form.reset();
}

function AddField(field) {
    var _builder = [];

    _builder.push('<div class="form-group">');
    _builder.push('<div class="form-check col-xs-3">');
    _builder.push('<input type="checkbox" class="form-check-input" id="PK' + field.toString() + '">');
    _builder.push('<label class="form-check-label" for="PK' + field.toString() + '">Primary Key</label>');
    _builder.push('</div><div class="col-xs-3">');
    _builder.push('<input class="form-control" id="Name' + field.toString() + '" placeholder="Name"></div>');
    _builder.push('<div class="col-xs-3"><input class="form-control" id="Type' + field.toString() + '" placeholder="Type">');
    _builder.push('</div><div class="col-xs-3">');
    _builder.push('<input class="form-control" id="Length' + field.toString() + '" placeholder="Lentgh"></div></div>');

    
    var _htmlToAdd = _builder.join('');

    alert(_htmlToAdd);
}