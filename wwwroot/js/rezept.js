var slideIndex = 1;

//Neu
var sendroot = "/receipt/";
var imageroot = "/rImages/";
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
var DOMRezeptRuheZeitIN;
var DOMRezeptTitelIN;
var DOMRezeptBeschreibungIN;
var DOMRezeptDauerIN;
var DOMRezeptRuheDauerSelection;
var DOMRezeptZutatenDIV;
var DOMRezeptKategorienDIV;
var DOMRezeptBildIN;
var DOMBilderWrapperHideShowClick;
var DOMBilderWrapper;
var DOMAdminRezept;
var DOMAdminRezeptBeschreibungDIV;
var DOMUploadImageList;

function RezeptObject(adminstyle) {
    var t = this;
    this.adminStyle = (typeof adminstyle === "undefined") ? false : adminstyle;
    this.descriptionChange = false;
    this.EditReceiptID = 0;
    this.CurrentReceipt = 0;
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
        DOMRezeptDauerIN = document.getElementById("RezeptDauerIN");
        DOMRezeptRuheZeitIN = document.getElementById("RezeptRuheZeitIN");
        DOMRezeptTitelIN = document.getElementById("RezeptTitelIN");
        DOMRezeptBeschreibungIN = document.getElementById("RezeptBeschreibungIN");
        DOMRezeptRuheDauerSelection = document.getElementById("RezeptRuheDauerSelection");
        DOMRezeptZutatenDIV = document.getElementById("RezeptZutatenDIV");
        DOMRezeptKategorienDIV = document.getElementById("RezeptKategorienDIV");
        DOMRezeptBildIN = document.getElementById("RezeptBildIN");
        DOMBilderWrapperHideShowClick = document.getElementById("BilderWrapperHideShowClick");
        DOMBilderWrapper = document.getElementById("BilderWrapper");
        DOMAdminRezept = document.getElementById("AdminRezept");
        DOMAdminRezeptBeschreibungDIV = document.getElementById("RezeptBeschreibungDIV");
        DOMAdminRezeptZutatenDIV = document.getElementById("RezeptZutatenDIV");
        DOMUploadImageList = document.getElementById("UploadImageList");
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
            divEntry.setAttribute("onclick", "Rezept.AddReceipt()");
            divEntry.dataset.name = "neu";
            DOMRezeptliste.appendChild(divEntry);
            RezeptListe.forEach(re => {
                var divEntry = document.createElement("DIV");
                divEntry.classList.add("listenEintrag")
                divEntry.textContent = re.title;
                divEntry.setAttribute("onclick", "Rezept.EditReceipt(" + re.id + ")");
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
        window.scrollTo({ top: 0, behavior: 'smooth' });
        this.CurrentReceipt = 0;
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
        this.ResetSearch();
        //Optionlist
        if (id != DOMRezept.dataset.id) {
            DOMRezept.dataset.id = id;
            this.CurrentReceipt = id;
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
                        let imageDOM = document.createElement("IMG");
                        imageDOM.classList.add("singleImage");
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
                        next.setAttribute("onClick", "plusSlides(1)");
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
                }
                DOMRezeptDauer.textContent = rez.duration + " Minuten";
                if (rez.restTime.restTime !== "" && rez.restTime.restTime !== "0") {
                    let einheit = rez.restTime.unit === "1" ? "Minuten" : "Stunden";
                    DOMRezeptRuheZeit.textContent = rez.restTime.restTime + " " + einheit;
                    if (DOMRezeptRuheZeitWrapper.style.display !== "flex")
                        DOMRezeptRuheZeitWrapper.style.display = "flex"
                } else {
                    if (DOMRezeptRuheZeitWrapper.style.display === "flex")
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
                        spandom.setAttribute("OnClick", "Rezept.GetZutatenPerID(" + ingre.id + ",'" + ingre.ingredient + "')");
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
        setTimeout(() => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }, 200)
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
                DOMSucheInput.focus({ focusVisible: true });
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
    //AdminReceipt Part
    this.ToggleAdminModus = function () {
        adminmodus = !adminmodus;
        if (adminmodus === true) {
            DOMEinheitenListeButton.style.display = "block";
        } else {
            DOMEinheitenListeButton.style.display = "none";
        }
        Send(sendroot + "GetUnits").then(function (data) {
            EinheitenListe = data;
            Send(sendroot + "GetCategories").then(function (data2) {
                KategorieListe = data2;
                Send(sendroot + "GetIngrediens").then(function (data3) {
                    ZutatenListe = data3;
                    let curtemp = t.CurrentReceipt;

                    t.HideLists();
                    t.RenderRezepListe();
                    if (curtemp !== 0) {
                        t.EditReceipt(curtemp);
                    }
                });
            });
        });
    }
    this.AddType = function (typ) {
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
    this.EditType = function (id, typ) {
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
    this.AddReceipt = function () {
        var value = prompt("Bitte den Titel f" + unescape("%FC") + "r das Rezept eingeben:");
        if (value != null) {
            Send(sendroot + "AddReceipt", value, "POST").then(function (data) {
                RezeptListe = [];
                Send(sendroot + "GetList").then(function (data2) {
                    RezeptListe = data2;
                    if (RezeptListe !== "") {
                        t.RenderRezepListe();
                        t.EditReceipt(data);
                    } else {
                        alert("Keine Rezepte Gefunden");
                    }
                });
            }).catch(function (jqXHR, textStatus) {
                alert("Es ist ein Fehler aufgetreten:" + jqXHR.message);
            });
        }
    }
    this.EditReceipt = function (id) {
        this.HideLists();
        let receipt = RezeptListe.find(x => x.id === id);
        if (receipt === undefined) {
            alert("Rezept konnte nicht geladen werden!");
            return;
        }
        this.ClearRezeptFields();
        this.HideLists();
        this.EditReceiptID = id;
        this.RenderAdminReceipt(receipt);
    }
    this.RenderAdminReceipt = function (receipt) {

        if (receipt === undefined) {
            return;
        }
        DOMRezeptDauerIN.value = receipt.duration
        DOMRezeptRuheZeitIN.value = receipt.restTime.restTime
        DOMRezeptTitelIN.value = receipt.title
        DOMRezeptBeschreibungIN.value = receipt.decription
        DOMRezeptRuheDauerSelection.value = receipt.restTime.unit;
        DOMAdminRezept.style.display = "block";
        tinymce.init({
            selector: '#RezeptBeschreibungIN',
            plugins: "lists autoresize",
            toolbar: "bold italic underline numlist bullist fontsizeselect",
            branding: false,
            menubar: false,
            statusbar: false,
            license_key: "gpl",
            language: "de",
            setup: function (ed) {
                ed.on('input', function (e) {
                    t.ChangeDescription("start");
                });
                ed.on('blur', function (e) {
                    t.ChangeDescription("lost");
                });
            }
        });
        this.RenderEditReceiptIngridiens(receipt);
        this.RenderEditReceiptCategories(receipt);
        this.RenderEditReceiptPictures(receipt);
        this.BindEventListener();
    }
    this.RenderEditReceiptPictures = function (receipt) {
        if (DOMBilderWrapper.hasChildNodes()) {
            DOMBilderWrapper.replaceChildren();
        }
        receipt.pictures.forEach(pic => {
            let DOMElement = document.createElement("DIV");
            DOMElement.dataset.picid = pic.id;
            DOMElement.id = "Image_" + pic.id;
            DOMElement.classList.add("rezeptBilderListe");
            let im = document.createElement("IMG");
            im.setAttribute("src", imageroot + pic.image);

            DOMElement.appendChild(im);
            let sort = this.CreateImageSortSelection(pic.sortOrder, pic.id)
            DOMElement.appendChild(sort)
            let s = document.createElement("SPAN");
            s.innerHTML = " (L&ouml;schen)";
            s.setAttribute("OnClick", "Rezept.DeleteImage(" + pic.id + ",'" + pic.image + "'," + receipt.id + ")");
            DOMElement.appendChild(s);
            DOMBilderWrapper.appendChild(DOMElement);
        });
        Array.from(document.getElementsByClassName("imagesort")).forEach(function (element) {
            element.addEventListener("change", function (event) {
                Rezept.ChangeImageSortOrder(event);
            })
        });
    }
    this.CreateImageSortSelection = function (sortorder, imageid) {
        let sortselection = document.createElement("SELECT");
        sortselection.dataset.picid = imageid;
        sortselection.id = "SortSelection_" + imageid;
        sortselection.classList.add("imagesort");
        for (var i = 1; i < 11; i++) {
            let opt = document.createElement("OPTION");
            opt.value = i;
            opt.textContent = i;
            if (sortorder == i) {
                opt.setAttribute("selected", "selected");
            }
            sortselection.appendChild(opt);
        }
        return sortselection;
    }
    this.RenderEditReceiptCategories = function (receipt) {
        if (DOMRezeptKategorienDIV.hasChildNodes()) {
            DOMRezeptKategorienDIV.replaceChildren();
        }
        receipt.categories.forEach(cat => {
            let DOMElement = document.createElement("DIV");
            DOMElement.classList.add("kategorieWrapper");
            DOMElement.id = "Category_" + cat.id;
            DOMElement.innerHTML = cat.category + " (L&ouml;schen)";
            DOMElement.setAttribute("OnClick", "Rezept.DeleteCategory(" + cat.id + "," + receipt.id + ")");
            DOMRezeptKategorienDIV.appendChild(DOMElement);
        })
        let DOMElement = document.createElement("DIV");
        DOMElement.classList.add("kategorieWrapper");
        DOMElement.id = "new_CategoryLink";
        let selectionElement = document.createElement("SELECT");
        selectionElement.classList.add("categoriesSelection");
        selectionElement.id = "NewCategoriesSelection"
        let opt1 = document.createElement("OPTION");
        opt1.value = "";
        selectionElement.appendChild(opt1);
        KategorieListe.forEach(cat => {
            let opt = document.createElement("OPTION");
            opt.value = cat.id;
            opt.textContent = cat.category;
            selectionElement.appendChild(opt);
        });
        DOMElement.appendChild(selectionElement)
        let span = document.createElement("SPAN");
        span.setAttribute("OnClick", "Rezept.AddCategory(" + receipt.id + ")");
        span.textContent = " Weitere";
        DOMElement.appendChild(span);
        DOMRezeptKategorienDIV.appendChild(DOMElement)
    }
    this.RenderEditReceiptIngridiens = function (receipt) {
        if (DOMRezeptZutatenDIV.hasChildNodes()) {
            Array.from(document.getElementsByClassName("zutatMenge")).forEach(function (element) {
                element.removeEventListener("change", function (event) { });
            });
            DOMRezeptZutatenDIV.replaceChildren();
        }
        receipt.ingredients.forEach(ing => {
            let DOMElement = document.createElement("DIV");
            DOMElement.classList.add("zutatWrapper");
            DOMElement.id = "zuei" + ing.ingredientUnitID;
            DOMElement.dataset.zuei = ing.ingredientUnitID
            DOMElement.innerHTML = '<input type="text" data-zuei=' + ing.ingredientUnitID + ' class="zutatMenge" value="' + ing.amount + '"> ' + ing.unit + " " + ing.ingredient + ' <span data-receipt="' + receipt.id + '" data-zuei="' + ing.ingredientUnitID + '" OnClick="Rezept.DeleteIngridUnit(this)">(L&ouml;schen)</span>';
            DOMRezeptZutatenDIV.appendChild(DOMElement);
        })
        Array.from(document.getElementsByClassName("zutatMenge")).forEach(function (element) {
            element.addEventListener("change", function (event) {
                Rezept.ChangeIngridUnit(event);
            })
        });
        let DOMElementNew = document.createElement("DIV");
        DOMElementNew.id = "new_zuei";
        DOMElementNew.classList.add("zutatWrapper")
        DOMElementNew.innerHTML = '<input id="IngredientNewValue" type="text" value="" /> <select id="UnitSelection" class="unitSelection">' + this.CreateUnitsSelection() + '</select> <select id="IngredientsSelection" class="ingredientsSelection">' + this.CreateIngridientsSelection() + '</select><span id="new_ZuEiLink" onClick="Rezept.AddIngridUnit(' + receipt.id + ')"> Weitere </span>'
        DOMRezeptZutatenDIV.appendChild(DOMElementNew);
    }
    this.CreateIngridientsSelection = function () {
        let ingri = document.createElement("SELECTION");
        let option = document.createElement("OPTION");
        option.value = "";
        ingri.appendChild(option);
        for (var i = 0; i < ZutatenListe.length; i++) {
            let chele = document.createElement("OPTION");
            chele.value = ZutatenListe[i].id;
            chele.textContent = ZutatenListe[i].ingredient
            ingri.appendChild(chele);
        }
        return ingri.innerHTML;
    }
    this.CreateUnitsSelection = function () {
        let units = document.createElement("SELECTION");
        let option = document.createElement("OPTION");
        option.value = "";
        units.appendChild(option);
        for (var i = 0; i < EinheitenListe.length; i++) {
            let chele = document.createElement("OPTION");
            chele.value = EinheitenListe[i].id;
            chele.textContent = EinheitenListe[i].unit
            units.appendChild(chele);
        }
        return units.innerHTML;
    }
    this.BindEventListener = function () {
        DOMRezeptDauerIN.addEventListener("change", function (event) { t.ChangeDuration(event) }, false);
        DOMRezeptRuheZeitIN.addEventListener("change", function (event) { t.ChangeRestTimeDuration(event) }, false);
        DOMRezeptTitelIN.addEventListener("change", function (event) { t.ChangeTitle(event) }, false);
        DOMRezeptRuheDauerSelection.addEventListener("change", function (event) { t.ChangeRestTimeArt(event) }, false);
        DOMRezeptBildIN.addEventListener("change", function (event) { t.AddImage(event) }, false);
    }
    this.TogglePictures = function () {
        if (DOMBilderWrapper.classList.contains("hideChild")) {
            DOMBilderWrapper.classList.remove("hideChild");
        } else {
            DOMBilderWrapper.classList.add("hideChild");
        }
    }
    this.ClearRezeptFields = function () {
        if (tinyMCE.activeEditor !== null) {
            tinyMCE.activeEditor.off();
        }
        tinyMCE.remove();
        DOMRezeptDauerIN.removeEventListener("change", function () { });
        DOMRezeptDauerIN.value = "";
        DOMRezeptRuheZeitIN.removeEventListener("change", function () { });
        DOMRezeptRuheZeitIN.value = "";
        DOMRezeptTitelIN.removeEventListener("change", function () { });
        DOMRezeptTitelIN.value = "";
        DOMRezeptBeschreibungIN.value = "";
        DOMRezeptBildIN.value = "";
        DOMRezeptRuheDauerSelection.removeEventListener("change", function () { });
        DOMRezeptRuheDauerSelection.value = "";
        if (DOMRezeptZutatenDIV.hasChildNodes()) {
            DOMRezeptZutatenDIV.replaceChildren();
        }
        if (DOMRezeptKategorienDIV.hasChildNodes()) {
            DOMRezeptKategorienDIV.replaceChildren();
        }

    }
    this.ChangeDescription = function (event) {
        if (event === "start") {
            this.descriptionChange = true;
            return;
        }
        if (event === "lost" && this.descriptionChange === true) {
            this.descriptionChange = false;
            var content = tinyMCE.get('RezeptBeschreibungIN').getContent();
            if (content) {
                let obj = { Type: ReceiptUpdateType.Description, Value: content };
                Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
                    if (!DOMAdminRezeptBeschreibungDIV.classList.contains("changedDone")) {
                        DOMAdminRezeptBeschreibungDIV.classList.add("changedDone")
                        setTimeout(() => {
                            DOMAdminRezeptBeschreibungDIV.classList.remove("changedDone");
                        }, 2000)
                    }
                }).catch(function (err) {
                    if (DOMAdminRezeptBeschreibungDIV.classList.contains("changedDone")) {
                        DOMAdminRezeptBeschreibungDIV.classList.remove("changedDone")
                        DOMAdminRezeptBeschreibungDIV.classList.add("error")
                        setTimeout(() => {
                            DOMAdminRezeptBeschreibungDIV.classList.remove("error");
                        }, 2000)
                    }
                    alert("Es ist ein Fehler aufgetreten:" + err.message);
                });
            }
        }
    }
    this.ChangeDuration = function (event) {
        let obj = { Type: ReceiptUpdateType.Duration, Value: DOMRezeptDauerIN.value };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            if (!DOMRezeptDauerIN.classList.contains("changedDone")) {
                DOMRezeptDauerIN.classList.add("changedDone")
                setTimeout(() => {
                    DOMRezeptDauerIN.classList.remove("changedDone");
                }, 2000)
            }
        }).catch(function (err) {
            if (DOMRezeptDauerIN.classList.contains("changedDone")) {
                DOMRezeptDauerIN.classList.remove("changedDone")
                DOMRezeptDauerIN.classList.add("error")
                setTimeout(() => {
                    DOMRezeptDauerIN.classList.remove("error");
                }, 2000)
            }
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.ChangeRestTimeDuration = function () {
        let obj = { Type: ReceiptUpdateType.RestTime, Value: DOMRezeptRuheZeitIN.value };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            if (!DOMRezeptRuheZeitIN.classList.contains("changedDone")) {
                DOMRezeptRuheZeitIN.classList.add("changedDone")
                setTimeout(() => {
                    DOMRezeptRuheZeitIN.classList.remove("changedDone");
                }, 2000)
            }
        }).catch(function (err) {
            if (DOMRezeptRuheZeitIN.classList.contains("changedDone")) {
                DOMRezeptRuheZeitIN.classList.remove("changedDone")
                DOMRezeptRuheZeitIN.classList.add("error")
                setTimeout(() => {
                    DOMRezeptRuheZeitIN.classList.remove("error");
                }, 2000)
            }
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.ChangeRestTimeArt = function () {
        let obj = { Type: ReceiptUpdateType.ResTimeUnit, Value: DOMRezeptRuheDauerSelection.value };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            if (!DOMRezeptRuheDauerSelection.classList.contains("changedDone")) {
                DOMRezeptRuheDauerSelection.classList.add("changedDone")
                setTimeout(() => {
                    DOMRezeptRuheDauerSelection.classList.remove("changedDone");
                }, 2000)
            }
        }).catch(function (err) {
            if (DOMRezeptRuheDauerSelection.classList.contains("changedDone")) {
                DOMRezeptRuheDauerSelection.classList.remove("changedDone")
                DOMRezeptRuheDauerSelection.classList.add("error")
                setTimeout(() => {
                    DOMRezeptRuheDauerSelection.classList.remove("error");
                }, 2000)
            }
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.ChangeTitle = function () {
        let obj = { Type: ReceiptUpdateType.Title, Value: DOMRezeptTitelIN.value };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            if (!DOMRezeptTitelIN.classList.contains("changedDone")) {
                DOMRezeptTitelIN.classList.add("changedDone")
                setTimeout(() => {
                    DOMRezeptTitelIN.classList.remove("changedDone");
                }, 2000)
            }
        }).catch(function (err) {
            if (DOMRezeptTitelIN.classList.contains("changedDone")) {
                DOMRezeptTitelIN.classList.remove("changedDone")
                DOMRezeptTitelIN.classList.add("error")
                setTimeout(() => {
                    DOMRezeptTitelIN.classList.remove("error");
                }, 2000)
            }
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.ChangeImageSortOrder = function (event) {
        let obj = { Type: ReceiptUpdateType.ImageSortOrder, Value: event.originalTarget.value, UnitID: event.originalTarget.dataset.picid };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            event.originalTarget.classList.add("changedDone");
            setTimeout(() => {
                event.originalTarget.classList.remove("changedDone");
            }, 2000)

        }).catch(function (err) {
            event.originalTarget.classList.add("error");
            setTimeout(() => {
                event.originalTarget.classList.add("error");
            }, 2000)
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.DeleteImage = function (id, image, receiptID) {
        let check = confirm("Willst Du das Bild wirklich l" + unescape("%F6") + "schen?");
        if (!check) {
            return;
        }
        let obj = { Type: ReceiptUpdateType.ImageDelete, UnitID: id, Value: image };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            let delimg = document.getElementById("Image_" + id);
            delimg.remove();
            let receipt = RezeptListe.find(x => x.id == receiptID);
            let pics = receipt.pictures;
            let t = pics.find(y => y.id == id);
            let tin = pics.indexOf(t);
            pics.splice(tin, 1);


        }).catch(function (err) {
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.AddImage = function (event) {
        var fi = DOMRezeptBildIN.files;
        for (var i = 0; i < fi.length; i++) {
            if (fi[i].type.indexOf("image") !== -1) {
                let dimid = fi[i].lastModified + "_" + fi[i].size;
                let newp = document.createElement("DIV");
                newp.id = "newpic_" + dimid;
                newp.classList.add("uploadpic");
                newp.textContent = fi[i].name
                DOMUploadImageList.appendChild(newp);
                let formData = new FormData();
                formData.append('file', fi[i], fi[i].name);
                formData.append('ReceiptID', this.EditReceiptID);
                formData.append('ImgName', fi[i].name);
                let url = window.location.protocol + "//" + window.location.host + "/receipt/UploadImage/" + this.EditReceiptID;
                fetch(url, { method: 'POST', body: formData })
                    .then(function (data) {
                        let returnvalue = data.text();
                        returnvalue.then(function (retpic) {
                            let newpic = JSON.parse(retpic);

                            let newpi = document.getElementById("newpic_" + dimid);
                            newpi.classList.add("changedDone");
                            //Add New Pic
                            let DOMElement = document.createElement("DIV");
                            DOMElement.dataset.picid = newpic.id;
                            DOMElement.id = "Image_" + newpic.id;
                            DOMElement.classList.add("rezeptBilderListe");
                            let im = document.createElement("IMG");
                            im.setAttribute("src", imageroot + newpic.image);
                            DOMElement.appendChild(im);
                            let sort = Rezept.CreateImageSortSelection(1, newpic.id)
                            DOMElement.appendChild(sort)
                            let s = document.createElement("SPAN");
                            s.innerHTML = " (L&ouml;schen)";
                            s.setAttribute("OnClick", "Rezept.DeleteImage(" + newpic.id + ",'" + newpic.image + "'," + newpic.receiptID + ")");
                            DOMElement.appendChild(s);
                            DOMBilderWrapper.appendChild(DOMElement);
                            let sortstuff = document.getElementById("SortSelection_" + newpic.id);
                            sortstuff.addEventListener("change", function (event) {
                                Rezept.ChangeImageSortOrder(event);
                            })
                            let receipt = RezeptListe.find(x => x.id = newpic.receiptID);
                            receipt.pictures.push(newpic);
                        });

                        setTimeout(function () {
                            Rezept.RemoveUploadImage(dimid);
                        }, 1300)
                    })
                    .catch(function (err) { /* Error. Inform the user */
                        console.log("ImageUpload ERRRRRROOOORR");
                        console.log(err);
                    });
            }
        }



    }
    this.RemoveUploadImage = function (id) {
        console.log("RemoveUploadImage" + id);
        let newpi = document.getElementById("newpic_" + id);
        newpi.remove();
    }
    this.DeleteIngridUnit = function (id) {
        let check = confirm("Willst Du diesen Eintrag wirklich l" + unescape("%F6") + "schen?");
        if (!check) {
            return;
        }
        let obj = { Type: ReceiptUpdateType.IngridientUnitDelete, UnitID: id.dataset.zuei };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            let zuei = document.getElementById("zuei" + id.dataset.zuei);
            zuei.remove();
            let receipt = RezeptListe.find(x => x.id == id.dataset.receipt);
            let ings = receipt.ingredients;
            let t = ings.find(y => y.ingredientUnitID == id.dataset.zuei);
            let tin = ings.indexOf(t);
            ings.splice(tin, 1);
        }).catch(function (err) {
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
    this.ChangeIngridUnit = function (event) {
        let obj = { Type: ReceiptUpdateType.IngridientUnitUpdate, Value: event.originalTarget.value, UnitID: event.originalTarget.dataset.zuei };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            if (!event.originalTarget.classList.contains("changedDone")) {
                event.originalTarget.classList.add("changedDone")
                setTimeout(() => {
                    event.originalTarget.classList.remove("changedDone");
                }, 2000)
            }
        }).catch(function (err) {
            if (event.originalTarget.classList.contains("changedDone")) {
                event.originalTarget.classList.remove("changedDone")
                event.originalTarget.classList.add("error")
                setTimeout(() => {
                    event.originalTarget.classList.remove("error");
                }, 2000)
            }
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });

    }
    this.AddIngridUnit = function (rid) {
        let IngredientNewValue = document.getElementById("IngredientNewValue");
        let UnitSelection = document.getElementById("UnitSelection");
        let IngredientsSelection = document.getElementById("IngredientsSelection");
        let obj = { Type: ReceiptUpdateType.IngridientUnitADD, Value: IngredientNewValue.value, UnitID: UnitSelection.value, UnitID2: IngredientsSelection.value };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function (data) {
            console.log(data);
            let receipt = RezeptListe.find(x => x.id == rid);
            let ing = ZutatenListe.find(x => x.id == IngredientsSelection.value)
            let uni = EinheitenListe.find(x => x.id == UnitSelection.value);
            let inobj = { amount: IngredientNewValue.value, id: ing.id, ingredient: ing.ingredient, ingredientUnitID: data, unit: uni.unit }
            receipt.ingredients.push(inobj);
            Rezept.RenderEditReceiptIngridiens(receipt);
        }).catch(function (err) {
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });


    }
    this.DeleteCategory = function (id, rid) {
        let check = confirm("Willst Du diese Kategorie wirklich l" + unescape("%F6") + "schen?");
        if (!check) {
            return;
        }
        let obj = { Type: ReceiptUpdateType.CategoryDelete, UnitID: id };
        Send(sendroot + "UpdateReceipt/" + rid, obj, "POST").then(function () {
            let c = document.getElementById("Category_" + id);
            c.remove();
            let receipt = RezeptListe.find(x => x.id == rid);
            let ca = receipt.categories;
            let t = ca.find(y => y.id == id);
            let tin = ca.indexOf(t);
            ca.splice(tin, 1);
        }).catch(function (err) {
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });

    }
    this.AddCategory = function (receiptid) {
        let NewCategoriesSelection = document.getElementById("NewCategoriesSelection");
        let obj = { Type: ReceiptUpdateType.CategoryAdd, UnitID: NewCategoriesSelection.value };
        Send(sendroot + "UpdateReceipt/" + this.EditReceiptID, obj, "POST").then(function () {
            let receipt = RezeptListe.find(x => x.id == receiptid);
            let cat = KategorieListe.find(x => x.id == NewCategoriesSelection.value)
            receipt.categories.push(cat);
            Rezept.RenderEditReceiptCategories(receipt);
        }).catch(function (err) {
            alert("Es ist ein Fehler aufgetreten:" + err.message);
        });
    }
}//Ende Class
const delay = (delayInms) => {
    return new Promise(resolve => setTimeout(resolve, delayInms));
};


function ReceiptUpdateTypeClass() {
    this.Title = 0;
    this.Duration = 1;
    this.Description = 2;
    this.RestTime = 3;
    this.ResTimeUnit = 4;
    this.Ingridient = 5;
    this.CategoryAdd = 6;
    this.IngridientUnitUpdate = 7;
    this.IngridientUnitADD = 8;
    this.IngridientUnitDelete = 9;
    this.ImageSortOrder = 10;
    this.ImageDelete = 11;
    this.CategoryDelete = 12;
}

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