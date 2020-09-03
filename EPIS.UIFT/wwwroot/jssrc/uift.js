/* zavreni okna s workflow UI dialogem bez refresh stranky */
UIFT.WorkflowWindowClose = function () {
    $("#workflowWindow").dialog("close");
};

/* funkce volana z UI po zmene v modalnim okne s workflow. Mela by refreshnout stranku. */
function hardrefresh() {
    location.reload();
};

/* Zavrit okno s formularem. Kontroluje, zda se volani window.close() podarilo, pokud ne - da hlasku uzivateli. */
UIFT.CloseWindow = function () {
    window.close();
    if (!window.closed) {
        alert("Okno nebylo možné zavřít. Prosím, zavřete okno standardním způsobem.");
    }
};

/* odhlasit uzivatele - navrat na poll login */
UIFT.Logout = function () {
    location.href = UIFT.WebRoot + "/Login/Logout";
};

/*
Nastaveni scrollbars pro menu sekci
*/
UIFT.adjustSegmentsTree = function () {
    var windowHeight = UIFT.Controls.window.height(),
        titleHeight = $("#menuTitle").outerHeight(),
        height = windowHeight - titleHeight - parseInt($("#menuContainer").css("top"));

    /* set new height */
    $("#segmentsTreeContainer").height(height);

    if (UIFT.segmentsScrollbar == null) {
        UIFT.segmentsScrollbar = $("#segmentsTreeContainer").tinyscrollbar({ axis: 'y' });

        /* update scrollbars when window resize */
        UIFT.Controls.window.resize(function () {
            UIFT.adjustSegmentsTree();
        });
    } else {
        UIFT.segmentsScrollbar.tinyscrollbar_update('relative');
    }
};

/*
zobrazit dany segment v hlavnim editacnim okne
id = f18id
vybranaOtazka (optional) = f19id otazky, na kterou se ma naskrolovat
vybranaOtazkaClass (optional) = nazev CSS tridy, ktera se ma aplikovat na otazku zadanou v "vybranaOtazka"
*/
UIFT.LoadSegment = function (id, vybranaOtazka, vybranaOtazkaClass) {
    /* nastavi .sel tridu pro aktualni uzel ve stromu */
    var url = UIFT.Controls.segmentsTree.find("li").removeClass("sel").filter("#menusekce-" + id).addClass("sel").children("a").attr("href");
    /* optional question id */
    var scrollToOtazka = parseInt(vybranaOtazka);
    var scrollToOtazkaClass = vybranaOtazkaClass;
    
    $.ajax({
        url: url,
        success: function (data) {
            /* load new data */
            UIFT.Controls.contentContainer.empty().html(data);

            /* init questions */
            UIFT.Controls.contentContainer.find(".otazkaContainer").not(".preview").each(function () {
                $(this).initQuestion();
            });

            /* nascrolovat na konkretni otazku */
            if (!isNaN(scrollToOtazka) && scrollToOtazka > 0) {
                var y = $("#otazka-" + scrollToOtazka).addClass(vybranaOtazkaClass).offset().top;

                /* scroll to question */
                UIFT.SetScrollTop(y - 62);

                if (vybranaOtazkaClass != "") {
                    window.setTimeout(function (obj) {
                        $("#otazka-" + obj.a).removeClass(obj.b);
                    }, 2500, { a: scrollToOtazka, b: vybranaOtazkaClass });
                }
            } else {
                /* scroll uplne nahoru */
                UIFT.SetScrollTop(0);
            }
        }
    });
};

/*
Navigacni cudliky mezi sekcemi.
direction = (next/prev)
*/
UIFT.Navigation = function (direction) {
    var coll = $("#segmentsTree li:visible:has(> a)"),
        idx = coll.index($("#segmentsTree li.sel"));

    if ((direction == "next" && idx == coll.length - 1) || (direction == "prev" && idx <= 0)) {
        return;
    } else {
        coll.eq(direction == "next" ? idx + 1 : idx - 1).children("a").click();
    }

    coll = null;
};

/*
Inicializace stranky Shrnuti.
*/
UIFT.InitShrnuti = function () {
    /* kliknuti na chybove hlasky */
    $("#shrnutiErrors a").click(function (e) {
        e.preventDefault();

        /* loading overlay */
        UIFT.Controls.contentContainer.loading();

        /* nahrat novy obsah hlavni casti */
        if ($(this).data("otazka") == undefined) {
            UIFT.LoadSegment($(this).data("sekce"));
        } else {
            UIFT.LoadSegment($(this).data("sekce"), $(this).data("otazka"), "error");
        }
    });

    /* ukoncovaci tlacitko */
    $(".shrnutiButton button").click(function (e) {
        if (confirm("Opravdu si přejete uzamknout tento formulář?")) {
            $("#contentContainerInner").loading();

            $.ajax({
                url: "/Formular/Zamknuti",
                success: function (data) {
                    if (data.success) {
                        location.href = data.url;
                    } else {
                        alert(data.message);
                        $("#contentContainerInner").loading("hide");
                    }
                }
            });
        }
    });
};

/* 
Doplnit nove default values do elementu na strance
defaultValues = { f21id, f19id, value }
*/
UIFT.ProccessDefaultValues = function (defaultValues, f19id) {
    if (defaultValues == null) {
        return;
    }

    for (var i = 0; i < defaultValues.length; i++) {
        /* pokud se jedna o def.value pro aktualne ukladanou otazku, nechod dal */
        if (defaultValues[i].f19id == f19id) {
            continue;
        }

        var otazka = $("#otazka-" + defaultValues[i].f19id);

        if (otazka.length > 0) {
            switch (otazka.data("question").control) {
                case UIFT.ReplyKeyEnum.Checkbox:
                    document.getElementById("Answer_" + defaultValues[i].f19id).checked = defaultValues[i].f21id == 1;
                    break;
                case UIFT.ReplyKeyEnum.RadiobuttonList:
                case UIFT.ReplyKeyEnum.CheckboxList:
                    document.getElementById("Answer_" + defaultValues[i].f19id + "_" + defaultValues[i].f21id).checked = true;
                    break;
                case UIFT.ReplyKeyEnum.DropdownList:
                    otazka.find('select option[value="' + defaultValues[i].f21id + '"]').attr("selected", true);
                    break;
                case UIFT.ReplyKeyEnum.SummaryOverview:
                    $("#Answer_" + defaultValues[i].f19id + "_default").html(defaultValues[i].value)
                        .siblings(".summary2:hidden").children("textarea").html(defaultValues[i].value);
                    break;
                default:
                    document.getElementById("Answer_" + defaultValues[i].f19id).value = defaultValues[i].value;
                    break;
            }
        }

        otazka = null; /* clean up */
    }
};

/*
Ulozeni odpovedi do databaze - Ajax.
dataToSave = { inputID: "", f19id: number, f21id: number, value: "" };
*/
UIFT.SaveAnswerToDb = function(dataToSave) {
    $.ajax({
        url: "/Otazka/Ulozeni",
        data: dataToSave,
        success: function (data) {
            var $input = $("#" + data.inputID),
                inputId = data.inputID;

            if (data.success) { /* ulozeni se povedlo */
                /* zapsat zmenu stavu na ulozeno */
                $input.removeClass("error")
                    .addClass("success");
                if ($input.data("answer") != undefined) {
                    $input.data("answer").state = UIFT.AnswerState.Saved;
                }

                /* info hlaska */
                $.jGrowl("Odpověď byla uložena");

                /* doplnit nove default values do elementu na strance */
                UIFT.ProccessDefaultValues(data.defaultValues, data.f19id);

                /* Zobrazit / skryt segmenty */
                UIFT.Controls.segmentsTree.find('li').each(function () {
                    var $this = $(this),
                        f18id = $this.data("id");
                    
                    if (data.hiddenSegments.indexOf(f18id) === -1) {
                        if (!$this.is(":visible")) {
                            $this.slideDown("fast");
                        }
                    } else {
                        if ($this.is(":visible")) {
                            $this.slideUp("fast");
                        }
                    }
                    $this = null;
                });

                $(".otazkaContainer").each(function () {
                    var $this = $(this),
                        f19id = $this.data("question").f19id
                        control = $this.data("question").control;
                    
                    /* nastavit readonly otazky */
                    $this.readOnlyQuestion(data.readOnly.indexOf(f19id) !== -1);

                    /* nastavit required flag */
                    $this.requiredQuestion(data.required.indexOf(f19id) !== -1);

                    /* Zobrazit / skryt otazky na formulari */
                    var shouldBeVisible = true;

                    if (control == 22) { /* jedna se o baterii */
                        var count_visible = 0;
                        
                        $this.find("tr[data-question]").each(function () {
                            var $battery_this = $(this),
                                battery_f19id = $battery_this.data("question").f19id;

                            if (data.hiddenQuestions.indexOf(battery_f19id) === -1) {
                                if (!$battery_this.is(":visible")) {
                                    $battery_this.css("display", document.all ? "block" : "table-row");
                                }
                                count_visible++;
                            } else {
                                if ($battery_this.is(":visible")) {
                                    $battery_this.css("display", "none");
                                }
                            }

                            $battery_this = null;
                        });

                        /* pokud je alespon jedna podotazka viditelna, musi byt videt cela baterie */
                        shouldBeVisible = count_visible > 0;
                    } else { /* normalni otazka */
                        shouldBeVisible = data.hiddenQuestions.indexOf(f19id) === -1;
                    }

                    /* pokud ma byt cela otazka viditelna */
                    if (shouldBeVisible) {
                        if (!$this.is(":visible")) {
                            $this.slideDown("fast");
                        }
                    } else { /* nebo skryta */
                        if ($this.is(":visible")) {
                            $this.slideUp("fast");
                        }
                    }

                    $this = null; /* clean up */
                });

                /* odstranit success class po chvili */
                window.setTimeout(function () {
                    $("#" + inputId).removeClass("success");
                }, 1500);
            } else {
                /* zobrazit tooltip, error class */
                $input.attr("title", data.message)
                    .addClass("error")
                    .tooltip({ tooltipClass: "errorTooltip", position: { my: "left top", at: "left bottom" } })
                    .focus();

                /* odstranit tooltip a error class po chvili */
                window.setTimeout(function () {
                    $("#" + inputId)
                        .tooltip("destroy")
                        .removeAttr("title");
                }, 3000);
            }
            data = $input = null; /* clean up */
        }
    });
};
