"use strict";
var activeClass = "active"

function OverViewVariables(_ov) {
    this.OverView = _ov;
    this.activeClass = "active";
}
var OV = new OverViewVariables("init");
const request = new XMLHttpRequest();
function ToggleRoom(DomElement, index) {
    if (DomElement.classList.contains(activeClass)) {
        //Same clicked
        DomElement.classList.remove(activeClass)
        DomElement.nextElementSibling.style.display = "none";
        return;
    }
    (document.querySelectorAll('.room')).forEach(function (el) {
        if (el.classList.contains(activeClass)) {
            el.classList.remove(activeClass);
            el.nextElementSibling.style.display = "none";
        }
    });
    OV.OverView[index].controllers.forEach((controller) => {
        let currentvalue = [];
        let currentcontrollerDOM = document.getElementById(controller.guid);
        controller.getCurrentValue.forEach((uri) => {
            if (uri !== "ALL") {
                request.open("GET", uri, false); // `false` makes the request synchronous
                request.send(null);
                currentvalue.push(JSON.parse(request.responseText));
            }
        });

        if (currentvalue.length > 0 && currentvalue.every(v => v === true)) {
            currentcontrollerDOM.checked = true;
        } else {
            currentcontrollerDOM.checked = false;
        }
    });
    //set ALL

    DomElement.classList.add(activeClass)
    DomElement.nextElementSibling.style.display = "block";
    if (DomElement.nextElementSibling.querySelectorAll('input[type="checkbox"]:checked').length + 1 == OV.OverView[index].controllers.length) {
        console.log("alle"); //todo: nun alle checked
    } else {
        console.log("wenige");
    }

}
function CheckBoxOverViewChanged(room, controller, inputDom) {
    console.log(inputDom.checked);
    console.log(room);
    console.log(controller);
    var selecetedController = OV.OverView[room].controllers[controller];
    console.log(selecetedController);
    var caller
    if (inputDom.checked) {
        caller = selecetedController.on;
        if (selecetedController.getCurrentValue == "ALL") {
            document.getElementById(OV.OverView[room].room).nextElementSibling.querySelectorAll('input[type="checkbox"]:not(:checked)').forEach((e) => { e.checked = true });
        }
    } else {
        caller = selecetedController.off;
        if (selecetedController.getCurrentValue == "ALL") {
            document.getElementById(OV.OverView[room].room).nextElementSibling.querySelectorAll('input[type="checkbox"]:checked').forEach((e) => { e.checked = false });
        }
    }
    //todo: wenn der letzte auch aktiv is, den ganzen raum aktiv machen.
    caller.forEach(async(uri) => {
        request.open("GET", uri, false); // `false` makes the request synchronous
        request.send(null);
    });

}