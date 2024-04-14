var slideIndex = 1;

//Neu
var sendroot = "/receipt/";
var OptionListSource = "";
var RezeptListe = [];
var ZutatenListe = [];
var KategorieListe = [];
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
        RezeptListe.forEach(re => {
            var divEntry = document.createElement("DIV");
            divEntry.classList.add("listenEintrag")
            divEntry.textContent = re.title;
            divEntry.setAttribute("onclick", "Rezept.GetRezept(" + re.id + ")");
            divEntry.dataset.name = re.title.toLowerCase();
            DOMRezeptliste.appendChild(divEntry);
        });
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
        ZutatenListe.forEach(ing => {
            let ingDom = document.createElement("DIV");
            ingDom.classList.add("listenEintrag");
            ingDom.textContent = ing.ingredient;
            ingDom.dataset.name = ing.ingredient.toLowerCase();
            ingDom.setAttribute("OnClick", "Rezept.GetZutatenPerID(" + ing.id + ",'" + ing.ingredient +"')");
            DOMZutatenListe.appendChild(ingDom);
        });
        DOMZutatenListe.style.display = "block";
    }
    this.GetZutatenPerID = function (id,text) {
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
        KategorieListe.forEach(cat => {
            let catDom = document.createElement("DIV");
            catDom.classList.add("listenEintrag");
            catDom.textContent = cat.category;
            catDom.dataset.name = cat.category.toLowerCase();
            catDom.setAttribute("OnClick", "Rezept.GetKategoriePerID(" + cat.id + ",'"+cat.category+"')");
            DOMKategorieListe.appendChild(catDom);
        });
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



}//Ende Class

/*

this.EditRezept = function(id) {
    if (id === "neu") {
        AdminRezept.AddRezept();
    } else {
        AdminRezept.EditRezept(this.AjaxCall("r", id));
    }
}
this.EditZutat = function (id) {
    if (id ==="neu") {
        AdminRezept.AddType("Zutat");
    } else {
        AdminRezept.EditType(id, "Zutat");
    }
}
this.EditKategorie = function (id) {
    if (id ==="neu") {
        AdminRezept.AddType("Kategorie");
    } else {
        AdminRezept.EditType(id, "Kategorie");
    }
}
this.EditEinheit = function(id) {
    if (id ==="neu") {
        AdminRezept.AddType("Einheit");
    } else {
        AdminRezept.EditType(id, "Einheit");
    }
}
this.ReloadZutatenListe = function() {
    ZutatenListeVar = [];
    var el = document.getElementById("ZutatenListe");
    while (el.firstChild) el.removeChild(el.firstChild);
    this.GetZutatenListe();
}
this.ReloadKategorienListe = function () {
    KategorienListeVar = [];
    var el = document.getElementById("KategorieListe");
    while (el.firstChild) el.removeChild(el.firstChild);
    this.GetKategorienListe();
}
this.ReloadEinheitenListe = function () {
    EinheitenListeVar = [];
    var el = document.getElementById("EinheitenListe");
    while (el.firstChild) el.removeChild(el.firstChild);
    this.GetEinheitenListe();
}
this.LoadAllList = function() {
    if (ZutatenListeVar.length === 0) {
        this.AjaxCall("zl").done(function(data) {
            for (var i = 0; i < data.length; i++) {
                ZutatenListeVar[data[i].id] = data[i].titel;
            }
            ZutatenListeData = data;
        });
    }
    if (KategorienListeVar.length === 0) {
        this.AjaxCall("kl").done(function(data) {
            for (var i = 0; i < data.length; i++) {
                KategorienListeVar[data[i].id] = data[i].titel;
            }
            KategorienListeData = data;
        });
    }
    if (EinheitenListeVar.length === 0) {
        this.AjaxCall("el").done(function(data) {
            for (var i = 0; i < data.length; i++) {
                EinheitenListeVar[data[i].id] = data[i].titel;
            }
            EinheitenListeData = data;
        });
    }
}
*/
//}



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