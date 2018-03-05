$("#SQLConnection").hide();
//
// SQL type selection
//
function SelectSQL() {
    var _Selection = $("#SelectSQL input[type='radio']:checked").val();

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