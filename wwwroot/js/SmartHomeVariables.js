"use strict";

function SmartHomeVariables() {
    //Controller
    this.DeconzHeader = document.getElementById("DeconzHeader"); //Header UI
    this.DeconzContent = document.getElementById("DeconzContent");// Content ui
    this.DeconzContentWrapper = document.getElementById("DeconzContentWrapper");// Content ui
    this.PowerUnit = "On/Off plug-in unit"; //Identifiziert die Steckdosen
}

var SHV = new SmartHomeVariables();