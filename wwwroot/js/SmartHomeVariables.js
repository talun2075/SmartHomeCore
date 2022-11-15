"use strict";

function SmartHomeVariables() {
    //Controller
    this.DeconzHeader = $("#DeconzHeader"); //Header UI
    this.DeconzContent = $("#DeconzContent");// Content ui
    this.DeconzContentWrapper = $("#DeconzContentWrapper");// Content ui
    this.PowerUnit = "On/Off plug-in unit"; //Identifiziert die Steckdosen
}

var SHV = new SmartHomeVariables();