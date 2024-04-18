var slideIndex = 1;

//Neu
var sendroot = "/receipt/";
var OptionListSource = "";
var RezeptListe = [];
var ZutatenListe = [];
var KategorieListe = [];
var EinheitenListe = [];
var adminmodus = false;
//Dom Elements
var DOMRezeptliste;
var DOMRezept;
var DOMRezeptPermaLinkA;
var DOMRezeptTitel;
var DOMRezeptBeschreibung;
var DOMRezeptBilder;
var DOMRezeptDauer;
var DOMRezeptRuheZeit;
var DOMRezeptRuheZeitWrapper;
var DOMRezeptZutaten;
var DOMRezeptKategorien;
var DOMZutatenListe;
var DOMKategorieListe;
var DOMSuche;
var DOMSucheInput;
var DOMSucheKeineErgebnisse;
var DOMOptionsListe;
var DOMRezeptOptionInformation;
var DOMEinheitenListe;
var DOMEinheitenListeButton;
function RezeptObject(adminstyle) {
    var t = this;
    this.adminStyle = (typeof adminstyle === "undefined") ? false : adminstyle;
    this.Init = function () {
        DOMRezeptliste = document.getElementById("RezeptListe");
        DOMRezept = document.getElementById("Rezept");
        DOMRezeptPermaLinkA = document.getElementById("RezeptPermaLinkA");
        DOMRezeptTitel = document.getElementById("RezeptTitel");
        DOMRezeptBeschreibung = document.getElementById("RezeptBeschreibung");
        DOMRezeptBilder = document.getElementById("RezeptBilder");
        DOMRezeptDauer = document.getElementById("RezeptDauer");
        DOMRezeptRuheZeit = document.getElementById("RezeptRuheZeit");
        DOMRezeptRuheZeitWrapper = document.getElementById("RezeptRuheZeitWrapper");
        DOMRezeptZutaten = document.getElementById("RezeptZutaten");
        DOMRezeptKategorien = document.getElementById("RezeptKategorien");
        DOMZutatenListe = document.getElementById("ZutatenListe");
        DOMKategorieListe = document.getElementById("KategorieListe");
        DOMSuche = document.getElementById("Suche");
        DOMSucheInput = document.getElementById("SucheInput");
        DOMSucheKeineErgebnisse = document.getElementById("SucheEintragKeineErgebnisse");
        DOMOptionsListe = document.getElementById("OptionsListe");
        DOMRezeptOptionInformation = document.getElementById("RezeptOptionInformation");
        DOMEinheitenListe = document.getElementById("EinheitenListe");
        DOMEinheitenListeButton = document.getElementById("EinheitenListeButton");
        this.GetRezeptListe();
        DOMSucheInput.addEventListener('input', function (evt) {
            Rezept.Suche("change")
        });
    }
    this.GetRezeptListe = function () {
        Send(sendroot + "GetList").then(function (data) {
            RezeptListe = data;
            if (RezeptListe !== "") {
                t.RenderRezepListe();
            } else {
                alert("Keine Rezepte Gefunden");
            }
        });

    }
    this.RenderRezepListe = function () {
        this.HideLists();
        this.ResetSearch();
        this.OptionListClear();
        if (DOMRezeptliste.hasChildNodes()) {
            DOMRezeptliste.replaceChildren();
        }
        if (adminmodus === true) {
            var divEntry = document.createElement("DIV");
            divEntry.classList.add("listenEintrag")
            divEntry.textContent = "Neu";
            divEntry.setAttribute("onclick", "Rezept.EditRezept('neu')");
            divEntry.dataset.name = "neu";
            DOMRezeptliste.appendChild(divEntry);
            RezeptListe.forEach(re => {
                var divEntry = document.createElement("DIV");
                divEntry.classList.add("listenEintrag")
                divEntry.textContent = re.title;
                divEntry.setAttribute("onclick", "Rezept.EditRezept(" + re.id + ")");
                divEntry.dataset.name = re.title.toLowerCase();
                DOMRezeptliste.appendChild(divEntry);
            });
        } else {
            RezeptListe.forEach(re => {
                var divEntry = document.createElement("DIV");
                divEntry.classList.add("listenEintrag")
                divEntry.textContent = re.title;
                divEntry.setAttribute("onclick", "Rezept.GetRezept(" + re.id + ")");
                divEntry.dataset.name = re.title.toLowerCase();
                DOMRezeptliste.appendChild(divEntry);
            });
        }
        DOMRezeptliste.style.display = "block";
    }
    this.HideLists = function () {
        var listen = document.getElementsByClassName("listen");
        for (var i = 0; i < listen.length; i++) {
            if (listen[i].style.display === "block") {
                listen[i].style.display = "none";
            }
        }
    }
    this.HideListsEnttries = function () {
        DOMListenEinträge = document.getElementsByClassName("listenEintrag");
        Array.from(DOMListenEinträge).forEach(le => {
            if (le.style.display === "block")
                le.style.display = "none";
        });
    }
    this.GetRezept = function (id) {
        this.HideLists();
        //Optionlist
        if (id != DOMRezept.dataset.id) {
            DOMRezept.dataset.id = id;
            let rez = RezeptListe.find(x => x.id === id);
            if (rez !== undefined) {
                DOMRezeptPermaLinkA.setAttribute("href", "/receipt/?rezept=" + id);
                const qrperma = document.getElementById('Qrperma');
                QRCode.toCanvas(qrperma, document.location.origin + "/receipt/?rezept=" + id);
                DOMRezeptTitel.textContent = rez.title;
                if (rez.decription === "") {
                    if (DOMRezeptBeschreibung.style.display === "block")
                        DOMRezeptBeschreibung.style.display = "none";
                } else {
                    DOMRezeptBeschreibung.innerHTML = rez.decription;
                    if (DOMRezeptBeschreibung.style.display !== "block")
                        DOMRezeptBeschreibung.style.display = "block";
                }
                //Picture
                if (DOMRezeptBilder.hasChildNodes()) {
                    DOMRezeptBilder.replaceChildren();
                    if (DOMRezeptBilder.style.display !== "none")
                        DOMRezeptBilder.style.display = "none";
                }
                if (rez.pictures.length > 0) {
                    document.getElementById('RezeptBilder').addEventListener('touchstart', TouchHandler.HandleTouchstart, false);
                    document.getElementById('RezeptBilder').addEventListener('touchmove', TouchHandler.HandleTouchMove, false);
                    var t = "";

                    if (rez.pictures.length === 1) {
                        let imagediv = document.createElement("DIV");
                        let imageDOM = document.createElement("IMAGE");
                        imageDOM.setAttribute("src", "/rImages/" + rez.pictures[0].image);
                        imagediv.appendChild(imageDOM);
                        DOMRezeptBilder.appendChild(imagediv);
                    } else {
                        let prev = document.createElement("a");
                        prev.classList.add("prev");
                        prev.setAttribute("onClick", "plusSlides(-1)");
                        prev.innerHTML = "&#10094;"
                        DOMRezeptBilder.appendChild(prev);
                        let next = document.createElement("a");
                        next.classList.add("next");
                        next.setAttribute("onClick", "plusSlides(-1)");
                        next.innerHTML = "&#10095;"
                        DOMRezeptBilder.appendChild(next);
                        for (var i = 0; i < rez.pictures.length; i++) {
                            let myslide = document.createElement("DIV");
                            myslide.classList.add("mySlides");
                            myslide.classList.add("fade");
                            myslide.innerHTML = '<div class="numbertext"><span class="numbertextformat">' + (i + 1) + ' / ' + rez.pictures.length + '</span></div><img src="/rImages/' + rez.pictures[i].image + '">';
                            DOMRezeptBilder.appendChild(myslide);
                        }
                        currentSlide(1);
                    }
                    DOMRezeptDauer.textContent = rez.duration + " Minuten";
                    if (rez.restTime.restTime !== "" && rez.restTime.restTime !== "0") {
                        let einheit = rez.restTime.unit == "1" ? "Minuten" : "Stunden";
                        DOMRezeptRuheZeit.textContent = rez.restTime.restTime + " " + einheit;
                        if (DOMRezeptRuheZeitWrapper.style.display !== "block")
                            DOMRezeptRuheZeitWrapper.style.display = "block"
                    } else {
                        if (DOMRezeptRuheZeitWrapper.style.display === "block")
                            DOMRezeptRuheZeitWrapper.style.display = "none"
                    }
                    if (DOMRezeptZutaten.hasChildNodes()) {
                        DOMRezeptZutaten.replaceChildren();
                    }
                    if (rez.ingredients.length > 0) {
                        rez.ingredients.forEach(ingre => {
                            let ingdom = document.createElement("DIV");
                            ingdom.classList.add("zutateneintrag")
                            let ingdomchild = document.createElement("DIV");
                            ingdomchild.textContent = ingre.amount + " " + ingre.unit + " ";
                            let spandom = document.createElement("SPAN");
                            spandom.textContent = ingre.ingredient;
                            spandom.setAttribute("OnClick", "Rezept.GetZutatenPerID(" + ingre.id + ")");
                            spandom.classList.add("zeiger");
                            ingdomchild.appendChild(spandom);
                            ingdom.appendChild(ingdomchild);
                            DOMRezeptZutaten.appendChild(ingdom);
                        });
                    }
                    if (DOMRezeptKategorien.hasChildNodes()) {
                        DOMRezeptKategorien.replaceChildren();
                    }
                    if (rez.categories.length > 0) {
                        rez.categories.forEach(cat => {
                            let catdom = document.createElement("DIV");
                            catdom.classList.add("zeiger");
                            catdom.classList.add("kategorieneintrag");
                            catdom.textContent = cat.category;
                            catdom.setAttribute("OnClick", "Rezept.GetKategoriePerID(" + cat.id + ")");
                            DOMRezeptKategorien.appendChild(catdom);
                        });
                    }

                    DOMRezeptBilder.style.display = "block";
                }
            } else {
                alert("konnte das rezept nicht finden.");
                return;
            }
        }
        if (DOMOptionsListe.hasChildNodes()) {
            DOMRezeptOptionInformation.textContent = "Liste der ausgewählten " + OptionListSource + " wieder anzeigen.";
            if (DOMRezeptOptionInformation.style.display !== "block")
                DOMRezeptOptionInformation.style.display = "block"
        } else {
            if (DOMRezeptOptionInformation.style.display === "block")
                DOMRezeptOptionInformation.style.display = "none"
        }
        DOMRezept.style.display = "block";
    }
    this.RenderZutatenListe = function () {
        this.HideLists();
        this.ResetSearch();
        this.OptionListClear();
        if (ZutatenListe.length === 0) {
            this.GetZutatenListe(true);
            return;
        }
        if (DOMZutatenListe.hasChildNodes()) {
            DOMZutatenListe.replaceChildren();
        }
        if (adminmodus === true) {
            let ingDom = document.createElement("DIV");
            ingDom.classList.add("listenEintrag");
            ingDom.textContent = "Neu";
            ingDom.dataset.name = "neu";
            ingDom.setAttribute("OnClick", "Rezept.AddType('Zutat')");
            DOMZutatenListe.appendChild(ingDom);
            ZutatenListe.forEach(ing => {
                let ingDom = document.createElement("DIV");
                ingDom.classList.add("listenEintrag");
                ingDom.textContent = ing.ingredient;
                ingDom.dataset.name = ing.ingredient.toLowerCase();
                ingDom.setAttribute("OnClick", "Rezept.EditType(" + ing.id + ",'Zutat')");
                DOMZutatenListe.appendChild(ingDom);
            });
        } else {
            ZutatenListe.forEach(ing => {
                let ingDom = document.createElement("DIV");
                ingDom.classList.add("listenEintrag");
                ingDom.textContent = ing.ingredient;
                ingDom.dataset.name = ing.ingredient.toLowerCase();
                ingDom.setAttribute("OnClick", "Rezept.GetZutatenPerID(" + ing.id + ",'" + ing.ingredient + "')");
                DOMZutatenListe.appendChild(ingDom);
            });
        }
        DOMZutatenListe.style.display = "block";
    }
    this.GetZutatenPerID = function (id, text) {
        let foundedre = RezeptListe.filter(obj => { return obj.ingredients.find(y => y.id == id); });
        if (foundedre.length === 0) {
            return;
        }
        OptionListSource = "Zutat";
        this.OptionListClear();
        let ueber = document.createElement("DIV");
        ueber.classList.add("ueberschrift");
        ueber.textContent = "Alle Rezepte mit der Zutat: " + text;
        DOMOptionsListe.appendChild(ueber);
        foundedre.forEach(re => {
            var divEntry = document.createElement("DIV");
            divEntry.classList.add("listenEintrag")
            divEntry.textContent = re.title;
            divEntry.setAttribute("onclick", "Rezept.GetRezept(" + re.id + ")");
            divEntry.dataset.name = re.title.toLowerCase();
            DOMOptionsListe.appendChild(divEntry);
        });
        this.OptionListShow();
    }
    this.GetKategoriePerID = function (id, text) {
        let foundedre = RezeptListe.filter(obj => { return obj.categories.find(y => y.id == id); });
        if (foundedre.length === 0 || foundedre === undefined) {
            console.log(id);
            console.log(text);
            return;
        }
        OptionListSource = "Kategorie";
        this.OptionListClear();
        let ueber = document.createElement("DIV");
        ueber.classList.add("ueberschrift");
        ueber.textContent = "Alle Rezepte mit der Kategorie: " + text;
        DOMOptionsListe.appendChild(ueber);
        foundedre.forEach(re => {
            var divEntry = document.createElement("DIV");
            divEntry.classList.add("listenEintrag")
            divEntry.textContent = re.title;
            divEntry.setAttribute("onclick", "Rezept.GetRezept(" + re.id + ")");
            divEntry.dataset.name = re.title.toLowerCase();
            DOMOptionsListe.appendChild(divEntry);
        });
        this.OptionListShow();
    }
    this.OptionListShow = function () {
        this.HideLists();
        DOMOptionsListe.style.display = "block";
    }
    this.OptionListClear = function () {
        if (DOMOptionsListe.hasChildNodes()) {
            DOMOptionsListe.replaceChildren();
        }
    }
    this.GetZutatenListe = function (withRender = false) {
        if (ZutatenListe.length === 0) {
            Send(sendroot + "GetIngrediens").then(function (data) {
                ZutatenListe = data;
                if (withRender === true) {
                    t.RenderZutatenListe();
                }
            });

        }
    }
    this.RenderKategorieListe = function () {
        this.HideLists();
        this.ResetSearch();
        this.OptionListClear();
        if (KategorieListe.length === 0) {
            this.GetKategorieListe(true);
            return;
        }
        if (DOMKategorieListe.hasChildNodes()) {
            DOMKategorieListe.replaceChildren();
        }
        if (adminmodus === true) {
            let catDom = document.createElement("DIV");
            catDom.classList.add("listenEintrag");
            catDom.textContent = "Neu";
            catDom.dataset.name = "neu";
            catDom.setAttribute("OnClick", "Rezept.AddType('Kategorie')");
            DOMKategorieListe.appendChild(catDom);
            KategorieListe.forEach(cat => {
                let catDom = document.createElement("DIV");
                catDom.classList.add("listenEintrag");
                catDom.textContent = cat.category;
                catDom.dataset.name = cat.category.toLowerCase();
                catDom.setAttribute("OnClick", "Rezept.EditType(" + cat.id + ",'Kategorie')");
                DOMKategorieListe.appendChild(catDom);
            });
        } else {
            KategorieListe.forEach(cat => {
                let catDom = document.createElement("DIV");
                catDom.classList.add("listenEintrag");
                catDom.textContent = cat.category;
                catDom.dataset.name = cat.category.toLowerCase();
                catDom.setAttribute("OnClick", "Rezept.GetKategoriePerID(" + cat.id + ",'" + cat.category + "')");
                DOMKategorieListe.appendChild(catDom);
            });
        }
        DOMKategorieListe.style.display = "block";
    }
    this.GetKategorieListe = function (withRender = false) {
        if (KategorieListe.length === 0) {
            Send(sendroot + "GetCategories").then(function (data) {
                KategorieListe = data;
                if (withRender === true) {
                    t.RenderKategorieListe();
                }
            });

        }
    }
    this.ResetSearch = function (withInput) {
        let DOMListenEinträge = document.getElementsByClassName("listenEintrag");
        if (withInput !== false) {
            DOMSuche.style.display = "none";
            DOMSucheInput.value = "";
        }
        Array.from(DOMListenEinträge).forEach(le => {
            le.style.display = "block";
        });
        if (DOMSucheKeineErgebnisse.style.display === "block") {
            DOMSucheKeineErgebnisse.style.display = "none"
        }

    }
    this.Suche = function (source) {
        let DOMListenEinträge = document.getElementsByClassName("listenEintrag");
        if (DOMSuche.style.display === "block" && source === "button") {
            this.ResetSearch();
        } else {
            if (source !== "LoadListInfos") {
                if (DOMSuche.style.display !== "block") {
                    DOMSuche.style.display = "block";
                }
            }
            var ival = DOMSucheInput.value;
            if (typeof ival === "undefined") {
                return;
            }
            if (ival.length > 2) {
                document.querySelectorAll('[data-name*="' + ival.toLowerCase() + '"]')
                let results = Array.from(DOMListenEinträge).filter(elem => elem.matches('[data-name*="' + ival.toLowerCase() + '"]'));
                if (results.length > 0) {
                    this.HideListsEnttries();
                    if (DOMSucheKeineErgebnisse.style.display === "block") {
                        DOMSucheKeineErgebnisse.style.display = "none"
                    }
                    results.forEach(re => {
                        re.style.display = "block";
                    })
                } else {
                    if (DOMSucheKeineErgebnisse.style.display !== "block") {
                        DOMSucheKeineErgebnisse.style.display = "block"
                    }
                }
            } else if (ival.length < 3) {
                this.ResetSearch(false);
            }
        }
    }

    this.RenderEinheitenListe = function () {
        this.HideLists();
        this.ResetSearch();
        this.OptionListClear();
        if (EinheitenListe.length === 0) {
            this.GetEinheitenListe(true);
            return;
        }
        if (DOMEinheitenListe.hasChildNodes()) {
            DOMEinheitenListe.replaceChildren();
        }
        if (adminmodus === true) {
            let catDom = document.createElement("DIV");
            catDom.classList.add("listenEintrag");
            catDom.textContent = "Neu";
            catDom.dataset.name = "neu";
            catDom.setAttribute("OnClick", "Rezept.AddType('Einheit')");
            DOMEinheitenListe.appendChild(catDom);
            EinheitenListe.forEach(cat => {
                let catDom = document.createElement("DIV");
                catDom.classList.add("listenEintrag");
                catDom.textContent = cat.unit;
                catDom.dataset.name = cat.unit.toLowerCase();
                catDom.setAttribute("OnClick", "Rezept.EditType(" + cat.id + ",'Einheit')");
                DOMEinheitenListe.appendChild(catDom);
            });
        } else {
            
        }
        DOMEinheitenListe.style.display = "block";
    }
    this.GetEinheitenListe = function (withRender = false) {
        if (EinheitenListe.length === 0) {
            Send(sendroot + "GetUnits").then(function (data) {
                EinheitenListe = data;
                if (withRender === true) {
                    t.RenderEinheitenListe();
                }
            });
        }
    }
    this.ToggleAdminModus = function(){
        adminmodus = !adminmodus;
        if (adminmodus === true) {
            DOMEinheitenListeButton.style.display = "block";
        } else {
            DOMEinheitenListeButton.style.display = "none";
        }
        this.HideLists();
        this.RenderRezepListe();
    }
    this.AddType = function(typ) {
        var value = prompt("Bitte den Wert f" + unescape("%FC") + "r die " + typ + " eingeben:");
        if (value != null) {
            switch (typ) {
                case "Einheit":
                    Send(sendroot + "AddUnit", value, "POST").then(function () {
                        EinheitenListe = []
                        t.RenderEinheitenListe();
                    }).catch(function (jqXHR, textStatus) {
                        alert("Es ist ein Fehler aufgetreten:" + textStatus + " " + jqXHR.responseJSON.error);
                    });

                    break;
                case "Zutat":
                    Send(sendroot + "AddIngredient", value, "POST").then(function () {
                        ZutatenListe = []
                        t.RenderZutatenListe();
                    }).catch(function (jqXHR, textStatus) {
                        alert("Es ist ein Fehler aufgetreten:" + textStatus + " " + jqXHR.responseJSON.error);
                    });
                    break;
                case "Kategorie":
                    Send(sendroot + "AddCategory", value, "POST").then(function () {
                        KategorieListe = []
                        t.RenderKategorieListe();
                    }).catch(function (jqXHR, textStatus) {
                        alert("Es ist ein Fehler aufgetreten:" + textStatus + " " + jqXHR.responseJSON.error);
                    });
                    break;
                default:
                    this.ErrorMessage("Unkowing AddType");
                    return;
                    break;

            }
        }
    }
    this.EditType = function(id, typ) {
        if (isNaN(id)) {
            alert("ID has wrong Syntax");
            return;
        };
        let listentryt;
        switch (typ) {
            case "Einheit":
                listentryt = EinheitenListe.find(x => x.id == id).unit;
                if (typeof listentryt === "undefined") {
                    alert("ID for Edit not Found");
                    return;
                }
                break;
            case "Zutat":
                listentryt = ZutatenListe.find(x => x.id == id).ingredient;
                if (typeof listentryt === "undefined") {
                    alert("ID for Edit not Found");
                    return;
                }
                break;
            case "Kategorie":
                listentryt = KategorieListe.find(x => x.id == id).category;
                if (typeof listentryt === "undefined") {
                    alert("ID for Edit not Found");
                    return;
                }
                break;
        }
        listentryid = id;
        //Prompt for new Value
        var value = prompt("Bitte den neuen Wert f" + unescape("%FC") + "r " + listentryt + " eingeben", listentryt);
        if (value != null) {
                switch (typ) {
                    case "Einheit":
                        Send(sendroot + "UpdateUnit/" + id, value, "POST").then(function () {
                            EinheitenListe = []
                            t.RenderEinheitenListe();
                        }).catch(function (jqXHR, textStatus) {
                            alert("Es ist ein Fehler aufgetreten:" + textStatus + " " + jqXHR.responseJSON.error);
                        });
                        break;
                    case "Zutat":
                        Send(sendroot + "UpdateIngredient/" + id, value, "POST").then(function () {
                            ZutatenListe = []
                            t.RenderZutatenListe();
                        }).catch(function (jqXHR, textStatus) {
                            alert("Es ist ein Fehler aufgetreten:" + textStatus + " " + jqXHR.responseJSON.error);
                        });
                        break;
                    case "Kategorie":
                        Send(sendroot + "UpdateCategory/" + id, value, "POST").then(function () {
                            KategorieListe = []
                            t.RenderKategorieListe();
                        }).catch(function (jqXHR, textStatus) {
                            alert("Es ist ein Fehler aufgetreten:" + textStatus + " " + jqXHR.responseJSON.error);
                        });
                        break;
                }
            

        }
    }


}//Ende Class


function ChangeNavigationMenu(t) {
    t.classList.toggle("changeShowNav");
    document.getElementById('Navigation').classList.toggle("noDisplaySmart");

}
// Next/previous controls
function plusSlides(n) {
    showSlides(slideIndex += n);
}

// Thumbnail image controls
function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    var i;
    var slides = document.getElementsByClassName("mySlides");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
    }
    slides[slideIndex - 1].style.display = "block";
}


function TouchHandlerCL() {
    this.xDown = null; //Touch x Koordinaten
    this.yDown = null; //Touch Y Koordinaten
    this.HandleTouchstart = function (evt) {
        this.xDown = evt.touches[0].clientX;
        this.yDown = evt.touches[0].clientY;
    };
    this.HandleTouchMove = function (evt) {
        if (!this.xDown || !this.yDown) {
            return;
        }

        var xUp = evt.touches[0].clientX;
        var yUp = evt.touches[0].clientY;
        var xDiff = this.xDown - xUp;
        var yDiff = this.yDown - yUp;

        if (Math.abs(xDiff) > Math.abs(yDiff)) { /*most significant*/
            if (xDiff > 0) {
                /* left swipe */
                plusSlides(1);
            } else {
                plusSlides(-1);
            }
        } else {
            if (yDiff > 0) {
                /* up swipe */
            }
        }
        /* reset values */
        this.xDown = null;
        this.yDown = null;
    };
}
var TouchHandler = new TouchHandlerCL();

function GetURLParameter(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        var devicesParameterName = sParameterName[0].toLowerCase();
        if (devicesParameterName === sParam) {
            return decodeURIComponent(sParameterName[1]);
        }
    }
    return "leer";
}
function LoadParamRezept() {
    var rpara = GetURLParameter("rezept");

    if (rpara !== "leer") {
        let rez = RezeptListe.find(x => x.id == rpara);
        if (rez !== undefined) {
            Rezept.GetRezept(rez.id);
        }
    }
}