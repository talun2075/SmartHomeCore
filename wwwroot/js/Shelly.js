
function TogglePower(v) {
        Send("TogglePower/" + v.dataset.name + "/" + v.checked+"/true").then(function () { console.log("sendDone"); });
}
var updateid;
function Update() {
    clearTimeout(updateid);
    Send("GetUpdates").then(function (data) {
        var shelly = data;
        for (var i = 0; i < shelly.length; i++) {
            var input = document.getElementById("sheylly_" + shelly[i].Mac);
            var ison = shelly[i].IsOn == "true";
            if (input.checked !== ison) {
                input.checked = ison;
            }
        }
        updateid = setTimeout("Update()", 3000);
    }).catch(function (data) { console.log(data); });
}
function Init() {
    window.BasePath = "/Shelly/";
    let toggle = document.getElementsByClassName("powerinput");
    for (var i = 0; i < toggle.length; i++) {
        let el = toggle[i];
        el.addEventListener("click", function (el) {
            TogglePower(el.target);
        })
    }
    Update();

}
var clicketName = "";
var activeClass = "active"
function ToggleDescription(domEle) {
    let currentactive = document.getElementsByClassName(activeClass);
    if (currentactive.length > 0) {
        //HTML Collection no forEach
        for (a of currentactive) {
            a.classList.remove(activeClass);
        }
    }


    if (clicketName === domEle) {
        clicketName = "";
        return;
    }
    clicketName = domEle;
    domEle.nextElementSibling.classList.add(activeClass)
}