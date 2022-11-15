﻿"use strict";


function ColoLight() {
    this.Off = function () {
        Send("Off");
    }
    this.On = function () {
        Send("On");
    }
    this.SetColor = function (data) {
        this.On();
        Send("SetColor/" + data);
    }
    this.Brighness = function (data) {
        this.On();
        Send("SetBrightness/" + data);
    }
    this.SetEffect = function (data) {
        this.On();
        Send("SetEffect/" + data);
    }
    async function Send(url = '', data = {}, t = 'GET') {
        // Default options are marked with *
        var fetchparams = {
            method: t, // *GET, POST, PUT, DELETE, etc.
            mode: 'cors', // no-cors, *cors, same-origin
            cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            credentials: 'same-origin', // include, *same-origin, omit
            headers: {
                'Content-Type': 'application/json'
            },
            redirect: 'follow', // manual, *follow, error
            referrerPolicy: 'no-referrer' // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        };
        if (t === "POST") {
            fetchparams.body = JSON.stringify(data); // body data type must match "Content-Type" header
        }
        const response = await fetch(url, fetchparams);
        var res = await response.text(); //take text

        try {
            return JSON.parse(res);// parses JSON response into native JavaScript objects
        } catch (e) {
            return res;
        }
    }
}