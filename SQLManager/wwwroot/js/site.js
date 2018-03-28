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
    var _insert = '"' + document.getElementById("addLineForm").elements[0].value + '"';

    for(i = 1; i < document.getElementById("addLineForm").elements.length - 1; i++)
    {
        _insert += ', "' + document.getElementById("addLineForm").elements[i].value + '"';
    }

    $("#addLineForm").hide();
    $("#addLineWork").removeClass("hidden");
    $("#addLineWork").show();

    // $.ajax({

    //     url:$(this).attr("href"), // comma here instead of semicolon   
    //     success: function(){
    //     alert("Value Added");  // or any other indication if you want to show
    //     }
    // });

    var ToSend = 
    {
        InsertData: _insert
    };

    $.ajax({
        url: "Add",
        type: "POST",
        data: JSON.stringify(ToSend),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(data) {
            alert("Komple");
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