function SMAjax(_url, _para1, _para2, _data) {
    /*
	_url= URL als interner Interpreter
	_data= Daten, die an den Server sollen. Meherere als Objekt: {variable1: "value1", variable2:"value2"}
    _para1 und 2 Optionale Parameter, die nur verwendung finden bei speziellen Cases.
	*/
    if (typeof _url === "undefined" || _url === null || _url === "") {
        return false;
    }
    var url;
    var type;
    switch (_url) {
        case "GetGroups":
            url = SV.DeconzControl + "/GetGroups/";
            break;
        case "GetLightsbyGroup":
            url = SV.DeconzControl + "/GetLightsbyGroup/" + _para1;
            break;
        case "ToggleGroupPowerStateTo":
        case "ToggleLightPowerStateTo":
            url = SV.DeconzControl + "/"+_url+"/" + _para1 + "/" + _para2;
            break;
        case "GetLights":
            url = SV.DeconzControl + "/GetLights/";
            break;
        case "GetLight":
            url = SV.DeconzControl + "/GetLight/" + _para1;
            break;
        default:
            alert("Übergeben url ist nicht definiert und kann somit nicht interpretiert werden.");
            return false;

    }

    //Es werden aktuell nur Get und Post Supportet
    if (type !== "GET" && type !== "POST") {
        type = "GET";
    }
    //Wenn keine Daten definiert sind, dann leer überegeben
    if (typeof _data === "undefined") {
        _data = "";
    }
    if (typeof _dataType === "undefined") {
        _dataType = "json";
    }
    return $.ajax({
        type: type,
        url: url,
        data: _data,
        dataType: _dataType
    });
}