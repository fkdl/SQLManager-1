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
        var _url = "/TableMethods/Add";
        var _formLength = _form.elements.length - 1;

        $("#addLineForm").hide();
        $("#addLineWork").removeClass("hidden");
        $("#addLineWork").show();
    } else {
        var _form = document.getElementById("editLineForm");
        var _url = "/TableMethods/Edit";
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
            if (data == "OK") {
                location.reload();
            } else {
                if (type == "Add") {
                    $("#addLineForm").show();
                    $("#addLineWork").addClass("hidden");
                    document.getElementById("addModalError").innerHTML =
                        '<div class="row">' +
                        '<div class="alert alert-danger">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">' +
                        '&times;' +
                        '</button>' +
                        '<strong>' + data + '</strong>' +
                        '</div>' +
                        '</div>';
                } else {
                    $("#editLineForm").show();
                    $("#editLineWork").addClass("hidden");
                    document.getElementById("editModalError").innerHTML =
                        '<div class="row">' +
                        '<div class="alert alert-danger">' +
                        '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">' +
                        '&times;' +
                        '</button>' +
                        '<strong>' + data + '</strong>' +
                        '</div>' +
                        '</div>';
                }

            }
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
        url: "/TableMethods/Remove",
        type: "POST",
        data: ToSend,
        success: function (data) {
            if (data == "OK") {
                location.reload();
            } else {
                $("#removeLineForm").show();
                $("#removeLineWork").addClass("hidden");
                document.getElementById("removeLineError").innerHTML =
                    '<div class="row">' +
                    '<div class="alert alert-danger">' +
                    '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">' +
                    '&times;' +
                    '</button>' +
                    '<strong>' + data + '</strong>' +
                    '</div>' +
                    '</div>';
            }

        }
    });

    return false;
}
//
// Create Table
//
function ResetTable() {
    var _form = document.getElementById("createTableForm");

    _form.reset();

    document.getElementById("createTableFields").innerHTML = LineFields();
}

function AddField() {
    var _htmlToAdd = LineFields();

    $("#createTableFields").append(_htmlToAdd);

    return false;
}

function LineFields() {
    var _builder = [];

    if (document.getElementById('SqlType').value == 'SQLServer') {
        _type = '<datalist id="types">' +
            '<option value="INT">' +
            '<option value="BIGINT">' +
            '<option value="SMALLINT">' +
            '<option value="NUMERIC">' +
            '<option value="DECIMAL">' +
            '<option value="FLOAT">' +
            '<option value="REAL">' +
            '<option value="DATE">' +
            '<option value="TIME">' +
            '<option value="DATETIME">' +
            '<option value="DATETIME2">' +
            '<option value="CHAR">' +
            '<option value="VARCHAR">' +
            '<option value="TEXT">' +
            '<option value="NCHAR">' +
            '<option value="NVARCHAR">' +
            '<option value="NTEXT">' +
            '</datalist>';
    } else if (document.getElementById('SqlType').value == 'SQLite') {
        _type = '<datalist id="types">' +
            '<option value="INTEGER">' +
            '<option value="REAL">' +
            '<option value="TEXT">' +
            '<option value="BLOB">' +
            '</datalist>';
    }

    _builder.push(' <div class="form-group">' +
        '<div class="form-check col-xs-2">' +
        '<input type="checkbox" class="form-check-input" name="PK">' +
        '<label class="form-check-label">PK</label>' +
        '</div>' +
        '<div class="form-check col-xs-2">' +
        '<input type="checkbox" class="form-check-input" name="AI">' +
        '<label class="form-check-label">AI</label>' +
        '</div>' +
        '<div class="col-xs-3">' +
        '<input class="form-control" name="Name" placeholder="Name">' +
        '</div>' +
        '<div class="col-xs-3">' +
        '<input class="form-control" name="Type" list="types" placeholder="Type">' +
        _type +
        '</div>' +
        '<div class="col-xs-2">' +
        '<input class="form-control" name="Length" placeholder="Lentgh">' +
        '</div>' +
        '</div>'
    );

    return _builder.join('');
}

function CreateTable() {
    var _form = document.getElementById("createTableForm");

    var _PK = document.getElementsByName("PK");
    var _AI = document.getElementsByName("AI")
    var _Name = document.getElementsByName("Name");
    var _Type = document.getElementsByName("Type");
    var _Length = document.getElementsByName("Length");

    $("#createTableForm").hide();
    $("#createTableWork").removeClass("hidden");
    $("#createTableWork").show();

    var FieldData = [];

    for (i = 0; i < _Name.length; i++) {
        FieldData.push(_PK[i].checked + ";" + _AI[i].checked + ";" + _Name[i].value + ";" +
            _Type[i].value + ";" + _Length[i].value);
    }

    var ToSend = {
        TableName: document.getElementById("TableName").value,
        FieldData: FieldData
    };

    $.ajax({
        url: "/DatabaseMethods/Create",
        type: "POST",
        data: ToSend,
        success: function (data) {
            if (data == "OK") {
                location.reload();
            } else {
                $("#createTableForm").show();
                $("#createTableWork").addClass("hidden");
                document.getElementById("createTableError").innerHTML =
                    '<div class="row">' +
                    '<div class="alert alert-danger">' +
                    '<button type="button" class="close" data-dismiss="alert" aria-hidden="true">' +
                    '&times;' +
                    '</button>' +
                    '<strong>' + data + '</strong>' +
                    '</div>' +
                    '</div>';
            }
        }
    });

    return false;
}