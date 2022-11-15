//Dokument Ready
$(document).ready(function () {
    window.SV = new SmartHomeVariables();
    window.DS = new DeconzSystem();
    DS.Init();
});     //ENDE DOK READY


function ErrorMessages(source, data) {
    console.log(source);
    console.log(data);
}