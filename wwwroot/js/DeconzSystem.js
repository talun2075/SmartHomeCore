"use strict";
function ErrorMessages(source, data) {
    console.log(source);
    console.log(data);
}
function DeconzSystem() {
    this.rooms = [];
    window.BasePath = "/deconz/"
    this.Lights = [];
    this.MergedRoomLights = [];
    var t = this;
    var lastRoomChildShowedid = 0;
    this.ColorPickers = [];
    this.ColorLights = [];
    var UpdateID = 0;
    this.Init = function (update = false) {
        //Lade Räume und Lichter
        clearTimeout(UpdateID)
        UpdateID = window.setTimeout("DS.RefreshData()", 10000)
        try {
            Send("GetGroups").then(function (data) {
                if (data === null) {
                    console.log("DeconzSystem:Init:GetGroups:Null");
                }
                t.MergedRoomLights = data;
                Send("GetLights").then(function (data) {
                    if (data === null) {
                        console.log("DeconzSystem:Init:GetLights:Null");
                    }
                    t.Lights = data;
                    t.MergeData();
                    if (update === true) {
                        t.UpdateRendering();
                    } else {
                        t.RenderDeconzSystem();
                    }
                }).catch(function (data) { ErrorMessages("DeconzSystem:Init:GetLights", data); });
            }).catch(function (data) { ErrorMessages("DeconzSystem:Init:GetGroups", data); });
        }
        catch (ex) {
            alert("Beim init ist folgender Fehler aufgetreten:" + ex.message);
        }
    }
    this.ColorPickerInit = function () {
        for (var i = 0; i < this.ColorLights.length; i++) {
            let lightid = this.ColorLights[i];
            if (typeof this.ColorPickers[lightid] === "undefined") {


                let light = this.Lights.find(x => x.id === lightid);
                var lvalue = light.hexColor;
                var cp = new iro.ColorPicker('#color_' + lightid, { color: lvalue, width: 250 });
                this.ColorPickers[lightid] = cp;

                cp.on('input:end', function () {
                    // log the current color as a HEX string
                    t.UpdateColor(lightid);

                });
            }
        }
    };
    this.UpdateColor = function (id) {
        //console.log(id);
        var color = this.ColorPickers[id].color.hexString
        Send("SetColor/" + id, color, "POST");
    }

    this.RenderDeconzSystem = function () {
        if (this.MergedRoomLights.length === 0) {
            alert("Keine Lampen oder Räume vorhanden");
            return;
        }
        for (let roomGroups of this.MergedRoomLights) {
            if (roomGroups.hide === true || roomGroups.room.hidden === true || roomGroups.room.lights.length === 0) {
                console.log("room Wird übersprungen:" + roomGroups.room.name);
                continue;
            }
            var item = roomGroups.room;
            try {
                let DOMLights = document.createElement("DIV");
                DOMLights.classList.add("roomLightsWrapper")
                var counter = 0;
                for (let itemLight of item.lightsMerged) {
                    counter++;
                    var check = "";
                    if ((itemLight.state.on === true && itemLight.state.isReachable === true && itemLight.type !== SV.PowerUnit) || (itemLight.state.on === true && itemLight.type === SV.PowerUnit)) {
                        check = 'checked="checked"';
                    }
                    //wenn eine Lampe nicht erreichbar ist, dann deaktivieren
                    var dis = "";
                    var disclass = "";
                    if (itemLight.state.isReachable === false && itemLight.type !== SV.PowerUnit) {
                        dis = "disabled=disabled";
                        disclass = " disabledCheckbox";
                    }
                    let colorbox = document.createElement("DIV");
                    if (itemLight.hasColor && itemLight.state.colorCoordinates !== null && itemLight.state.isReachable) {
                        this.ColorLights.push(itemLight.id);
                        colorbox.id = "color_" + itemLight.id
                        colorbox.classList.add("iroColor");
                    }

                    let DOMSwitch = document.createElement("DIV");
                    DOMSwitch.classList.add("switch")
                    DOMSwitch.innerHTML = '<input type="checkbox" onClick="DS.TogglePowerState(\'l\',' + itemLight.id + ', this.checked, ' + item.id + ');" id="LightCheckbox_' + itemLight.id + '" name="LightCheckbox_' + itemLight.id + '" ' + check + ' ' + dis + '><label for="LightCheckbox_' + itemLight.id + '"><i class="bulb"><span class="bulb-center"></span><span class="filament-1"></span><span class="filament-2"></span><span class="reflections"><span></span></span><span class="sparks"><i class="spark1"></i><i class="spark2"></i><i class="spark3"></i><i class="spark4"></i></span></i></label>';
                    let DOMName = document.createElement("DIV");
                    DOMName.classList.add("lightName");
                    DOMName.textContent = itemLight.name;
                    let DOMlightWrapperNoColor = document.createElement("DIV");
                    DOMlightWrapperNoColor.classList.add("lightWrapperNoColor");
                    DOMlightWrapperNoColor.appendChild(DOMName);
                    DOMlightWrapperNoColor.appendChild(DOMSwitch);
                    let DOMLight = document.createElement("DIV");
                    DOMLight.id = "light_" + itemLight.id;
                    DOMLight.classList.add("light");
                    DOMLight.appendChild(DOMlightWrapperNoColor);
                    DOMLight.appendChild(colorbox);
                    DOMLights.appendChild(DOMLight);
                }
                var check = "";
                var dis = "";
                var disclass = "";
                if (item.state.allOn === true) {
                    check = 'checked="checked"';
                }
                if (counter === 1 && item.lightsMerged[0].state.isReachable === false && item.lightsMerged[0].type !== SV.PowerUnit) {
                    dis = "disabled=disabled";
                    disclass = "disabledCheckbox";
                }
                var anyClass = "";
                if (item.state.anyOn === true && item.state.allOn === false) {
                    anyClass = "anyRommLightsOnClass";
                }
                var itemclass = "";
                if (item.type === "LightGroup") {
                    itemclass = "lightGroup";
                }
                let DOMRoomSwitch = document.createElement("DIV");
                if (anyClass !=="")
                    DOMRoomSwitch.classList.add(anyClass);
                if (disclass !=="")
                    DOMRoomSwitch.classList.add(disclass);

                DOMRoomSwitch.classList.add("switch");
                DOMRoomSwitch.innerHTML = '<input type="checkbox" onClick="DS.TogglePowerState(\'r\',' + item.id + ',this.checked,' + item.id + ');"id="RoomCheckbox_' + item.id + '" name="RoomCheckbox_' + item.id + '" ' + check + dis + '><label for="RoomCheckbox_' + item.id + '"><i class="bulb"><span class="bulb-center"></span><span class="filament-1"></span><span class="filament-2"></span><span class="reflections"><span></span></span><span class="sparks"><i class="spark1"></i><i class="spark2"></i><i class="spark3"></i><i class="spark4"></i></span></i></label>';
                let DOMroomEveryTimeShowed = document.createElement("DIV");
                DOMroomEveryTimeShowed.classList.add("roomEveryTimeShowed");
                let DOMroomName = document.createElement("DIV");
                DOMroomName.classList.add("roomName");
                DOMroomName.setAttribute("OnClick", "DS.ShowRoomChilds(" + item.id + ")");
                let DOMroomNameSpan = document.createElement("SPAN");
                DOMroomNameSpan.classList.add("roomNameSpan");
                DOMroomNameSpan.textContent = item.name;
                DOMroomName.appendChild(DOMroomNameSpan);
                DOMroomEveryTimeShowed.appendChild(DOMroomName);
                let DOMRoom = document.createElement("DIV");
                let DOMroomPowerState = document.createElement("DIV");
                DOMroomPowerState.dataset.all = item.state.allOn;
                DOMroomPowerState.dataset.any = item.state.anyOn;
                DOMroomPowerState.classList.add("roomPowerState");
                DOMroomPowerState.appendChild(DOMRoomSwitch)
                DOMroomEveryTimeShowed.appendChild(DOMroomPowerState);
                DOMRoom.id = "room_" + item.id;
                DOMRoom.classList.add("room");
                if(itemclass !=="")
                    DOMRoom.classList.add(itemclass);

                DOMRoom.appendChild(DOMroomEveryTimeShowed);
                DOMRoom.appendChild(DOMLights);
                SV.DeconzContentWrapper.appendChild(DOMRoom);
            }
            catch (ex) {
                console.log(ex);
            }
        }
        this.ColorPickerInit();
    }
    //Verbindet die Räume und Lampen zu einem Objekt
    this.MergeData = function () {
        if (this.MergedRoomLights.length == 0) {
            console.log("Merge kann nicht durchgeführt werden");
            return;
        }
        for (let item of this.MergedRoomLights) {
            var roomData = item.room;
            //hier nun die Lampen zufügen und den StateMerken/setzen.
            let tempState = roomData.state.allOn;
            let tempStateAny = false;
            roomData.lightsMerged = [];
            for (let roomDataLights of roomData.lights) {
                //hier nun die einzelnelampe
                let lightToAdd = this.Lights.find(x => x.id === roomDataLights);
                if (lightToAdd.state.isReachable === true && lightToAdd.state.on === true) {
                    tempStateAny = true;
                }
                if ((lightToAdd.state.isReachable === false && lightToAdd.type !== SV.PowerUnit) || lightToAdd.state.on === false) {
                    tempState = false;
                }
                roomData.lightsMerged.push(lightToAdd);
            }
            roomData.state.allOn = tempState;
            roomData.state.anyOn = tempStateAny;
        }

    }
    this.TogglePowerState = function (SourceType, id, powerstate, roomID) {
        //console.log("Type:" + SourceType);
        //console.log("ID:" + id);
        //raum laden
        var room = this.MergedRoomLights.find(x => x.room.id == roomID).room;
        if (typeof room === "undefined" || room === null) {
            console.log("roomID ist unbekannt");
            return;
        }
        switch (SourceType) {
            case "r":
                //console.log("Room");
                room.state.allOn = powerstate;
                Send("ToggleGroupPowerStateTo/" + id + "/" + powerstate);
                //Alle Lampen auch ändern.
                for (let light of room.lightsMerged) {
                    light.state.on = room.state.allOn;
                }
                break;
            case "l":
                //console.log("Light");
                var light = room.lightsMerged.find(x => x.id == id);
                light.state.on = powerstate;
                Send("ToggleLightPowerStateTo/" + id + "/" + powerstate);
                break;
            default:
                alert("Übergabe TogglePower ist fehlerhaft:" + SourceType + " ID:" + id);

        }
        this.UpdateRendering();
    }
    this.ShowRoomChilds = function (id) {
        let roomLightsWrapper = document.querySelectorAll('.roomLightsWrapper')
        roomLightsWrapper.forEach(el => {

            if (el.style.display === "block") {
                el.style.display = "none";
                console.log("elehide");
            }
        })
        if (lastRoomChildShowedid == id) {
            lastRoomChildShowedid = -1;
            return;
        }
        var temproom = (this.MergedRoomLights.find(x => x.room.id == id)).room;
        if (temproom.lightsMerged.length < 2 && temproom.lightsMerged[0].hasClass === false) {
            return;
        }
        lastRoomChildShowedid = id;
        document.querySelector("#room_" + id + ">.roomLightsWrapper").style.display = "block";
    }
    this.UpdateRendering = function () {
        for (let item of this.MergedRoomLights) {
            try {
                if (item.hide === true || item.room.lights.length === 0) {
                    continue;
                }
                var room = item.room;
                //hier nun die Räume und Lichter durchgehen und das RenderingUpdaten
                let tempState = room.state.allOn;
                let tempStateAny = false;
                for (let light of room.lightsMerged) {
                    if (light.state.isReachable === true && light.state.on === true) {
                        tempStateAny = true;
                    }
                    if (light.state.isReachable === true && light.state.on === false) {
                        tempState = false;
                    }
                    //colorPicker Update
                    if (this.ColorLights.includes(light.id)) {
                        //todo: check for change and update if needed.
                        if (typeof this.ColorPickers[light.id] !== "undefined") {
                            let cp = this.ColorPickers[light.id];
                            if (light.hexColor.toLowerCase() !== cp.color.hexString) {
                                cp.color.hexString = light.hexColor;
                            }

                        } else {
                            console.log("Licht nicht als Picker definiert:" + light.id);
                        }
                    }
                    //colorPicker Update End
                    var domLight = document.getElementById("LightCheckbox_"+light.id)
                    if (domLight.checked !== light.state.on) {
                        //hier nun auch die Erreichbarkeit prüfen
                        if ((light.state.isReachable === false && light.type !== SV.PowerUnit)) {
                            if (domLight.checked) {
                                domLight.checked = false
                            }
                        } else {
                            domLight.checked = light.state.on
                        }
                    }
                }
                room.state.allOn = tempState;
                room.state.anyOn = tempStateAny;
                var domRoomInput = document.getElementById("RoomCheckbox_" + room.id);
                var domRoomSwitch = domRoomInput.parentElement;
                if (room.state.allOn === false && room.state.anyOn === true) {
                    //anyclass adden und check deaktivieren
                    if (domRoomInput.checked) {
                        domRoomInput.checked = room.state.allOn;
                    }
                    if (!domRoomSwitch.classList.contains("anyRommLightsOnClass")) {
                        domRoomSwitch.classList.add("anyRommLightsOnClass");
                    }
                }
                if (room.state.allOn === true) {
                    //anyclass löschen und check aktivieren
                    if (domRoomInput.checked) {
                        domRoomInput.checked = room.state.allOn;
                    }
                    if (domRoomSwitch.classList.contains("anyRommLightsOnClass")) {
                        domRoomSwitch.classList.remove("anyRommLightsOnClass");
                    }
                }
                if (room.state.allOn === false && room.state.anyOn === false) {
                    //anyclass löschen und uncheck
                    if (domRoomInput.checked) {
                        domRoomInput.checked = room.state.allOn;
                    }
                    if (domRoomSwitch.classList.contains("anyRommLightsOnClass")) {
                        domRoomSwitch.classList.remove("anyRommLightsOnClass");
                    }
                }
            }
            catch (error) {
                console.log("Fehler beim Update.")
                console.log(error);
                continue;
            }
        }
    }
    this.RefreshData = function () {
        this.Init(true);
    }
}