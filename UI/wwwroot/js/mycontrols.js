function mycheckboxlist_checked(chk, hidden_id, value) {

    if (chk.checked === true) {

        $("#" + hidden_id).val(value);

    } else {

        $("#" + hidden_id).val("0");
    }


}
function myradiolist_checked(hidden_id, value, event_after_changevalue) {
    $("#" + hidden_id).val(value);

    if (event_after_changevalue !== "") {
        eval(event_after_changevalue + "('" + value + "')");
    }
}
function mynumber_blur(ctl, decimaldigits) {
    var num = 0;
    if (ctl.getAttribute("type") === "text") {
        var s = $(ctl).val().replace(/\s/g, '').replace(",", ".");
        num = Number(s);
    } else {
        num = Number($(ctl).val());
    }

    //val = num.toFixed(decimaldigits).replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    val = num.toFixed(decimaldigits);

    var forid = $(ctl).attr("for-id");   //hidden id prvku pro spojení s hostitelským view   
    $("#" + forid).val(val.replace(".", ","));    //pro uložení na server v rámci hostitelského view, je třeba předávat desetinnou čárku a nikoliv tečku!

    //dále pro zformátování čísla navenek    
    if (val.indexOf(".") > 0) {
        arr = val.split(".");
        arr[0] = arr[0].replace(/\B(?=(\d{3})+(?!\d))/g, " ");
        val = arr[0] + "," + arr[1];
    } else {
        val = val.replace(/\B(?=(\d{3})+(?!\d))/g, " ");
    }


    val = val.replace(/[.]/g, ',');

    ctl.setAttribute("type", "text");
    ctl.value = val;





}

function formatNumber(num) {
    return num.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
}


function mynumber_focus(ctl) {

    var s = ctl.value;
    s = s.replace(/\s/g, '');
    s = s.replace(/[,]/g, '.');
    ctl.value = s;

    var strBrowser = getBrowser();
    if (strBrowser !== "firefox" && strBrowser !== "edge") {
        ctl.setAttribute("type", "number");
    }
    ctl.select();



}



function getBrowser() {
    var browser_name = '';
    isIE = /*@cc_on!@*/false || !!document.documentMode;
    isEdge = !isIE && !!window.StyleMedia;
    if (navigator.userAgent.indexOf("Chrome") !== -1 && !isEdge) {
        browser_name = "chrome";
    }
    else if (navigator.userAgent.indexOf("Safari") !== -1 && !isEdge) {
        browser_name = "safari";
    }
    else if (navigator.userAgent.indexOf("Firefox") !== -1) {
        browser_name = "firefox";
    }
    else if ((navigator.userAgent.indexOf("MSIE") !== -1) || (!!document.documentMode === true)) //IF IE > 10
    {
        browser_name = "ie";
    }
    else if (isEdge) {
        browser_name = "edge";
    }
    else {
        browser_name = "other-browser";
    }

    return browser_name;
}


function datepicker_init(inputid) {

    $("#" + inputid).datepicker({
        format: "dd.mm.yyyy",
        todayBtn: "linked",
        clearBtn: true,
        language: "cs",
        todayHighlight: true,
        autoclose: true
    });

}

function datepicker_button_click(inputid) {
    $("#" + inputid).focus();
}

function datepicker_change(ctl) {
    var forid = $(ctl).attr("for-id");   //hidden id prvku pro spojení s hostitelským view   
    var s = ctl.value;
    if (document.getElementById(forid + "_Time")) {
        s = s + " " + document.getElementById(forid + "_Time").value;
    }
    if (s.trim() === "00:00") s = "";   //je třeba to vyčistit, aby se do db uložila null value

    $("#" + forid).val(s);    //pro uložení na server v rámci hostitelského view


}
function datepicker_time_change(ctl) {

    var forid = $(ctl).attr("for-id");   //hidden id prvku pro spojení s hostitelským view   
    var s = $("#" + forid + "helper").val() + " " + $(ctl).val();

    $("#" + forid).val(s);    //pro uložení na server v rámci hostitelského view

}
function datepicker_get_value(ctlClientID) {
    var value = $("#" + ctlClientID + "helper").datepicker("getDate");
    return (value);
}
function datepicker_set_value(ctlClientID, datValue) {
    $("#" + ctlClientID + "helper").datepicker("setDate", datValue);
}




/*_MyAutoComplete*/
function myautocomplete_init(c) {
    var _tableid = c.controlid + "tab1";
    var _combo_currentFocus = -1;


    $("#" + c.controlid).click(function () {
        $("#cmdCombo" + c.controlid).dropdown("toggle");        //click na textbox se má chovat stejně jako click na tlačítko cmdCombo        
        $("#" + c.controlid).focus();
    });

    $("#" + c.controlid).on("focus", function () {
        document.getElementById(c.controlid).select();
    });

    $("#" + c.controlid).on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#" + _tableid + " tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#" + c.controlid).on("keydown", function (e) {
        if (e.keyCode === 27) {  //ESC
            $("#divDropdown" + c.controlid).dropdown("hide");
            return;
        }
        var rows_count = $("#" + _tableid).find("tr").length;

        if ($("#divDropdown" + c.controlid).css("display") === "none") {
            $("#cmdCombo" + c.controlid).dropdown("toggle");
            //$("#divDropdown" + c.controlid).dropdown("show");

            document.getElementById(c.controlid).focus();
        }
        if (e.keyCode === 13) {//ENTER            
            var row = $("#" + _tableid).find(".selrow");
            if (row.length > 0) {
                autocomplete_was_selected(row[0]);
            }
        }

        var destrowindex;
        if (e.keyCode === 40) {  //down

            if (rows_count - 1 <= _combo_currentFocus) {
                _combo_currentFocus = 0

            } else {
                _combo_currentFocus++;

            }
            destrowindex = get_first_visible_rowindex(_combo_currentFocus, "down");
            if (destrowindex === -1) destrowindex = get_first_visible_rowindex(0, "down");
            _combo_currentFocus = destrowindex;
            if (destrowindex === -1) return;

            update_selected_row();
        }
        if (e.keyCode === 38) {  //up            
            _combo_currentFocus--;
            destrowindex = get_first_visible_rowindex(_combo_currentFocus, "up");
            if (destrowindex === -1) destrowindex = get_first_visible_rowindex(0, "down");
            _combo_currentFocus = destrowindex;
            if (destrowindex === -1) _combo_currentFocus = 0;

            update_selected_row();
        }



    });

    $("#divDropdownContainer" + c.controlid).on("show.bs.dropdown", function () {
        $("#divDropdown" + c.controlid).css("margin-left", 25 + $("#divDropdown" + c.controlid).width() * -1);      //šírka dropdown oblasti má být zleva 100% jako celý usercontrol

        if ($("#" + c.controlid).prop("filled") === true) return;    //combo už bylo dříve otevřeno


        $.post(c.posturl, { o15flag: c.o15flag, tableid: _tableid }, function (data) {

            $("#divData" + c.controlid).html(data);

            $("#" + c.controlid).prop("filled", true);

            $("#" + _tableid).css("width", $("#divDropdown" + c.controlid).width());


            $("#" + _tableid + " .txz").on("click", function () {
                s = $(this).find("td:first").text();

                $("#" + c.controlid).val(s);
                $("#" + c.controlid).select();
                _toolbar_warn2save_changes();
            });

        });


    })


    function get_first_visible_rowindex(fromindex, direction) {
        var rows_count = $("#" + _tableid).find("tr").length;
        var row;
        if (direction === "down") {
            for (i = fromindex; i < rows_count; i++) {
                row = $("#" + _tableid).find("tr").eq(i);
                if ($(row).css("display") !== "none") {
                    return i;
                }
            }
        }
        if (direction === "up") {
            for (i = fromindex; i >= 0; i--) {
                row = $("#" + _tableid).find("tr").eq(i);
                if ($(row).css("display") !== "none") {
                    return i;
                }
            }
        }

        return -1;
    }
    function update_selected_row() {
        $("#" + _tableid).find("tr").removeClass("selrow");
        if (_combo_currentFocus > -1) {
            var row = $("#" + _tableid).find("tr").eq(_combo_currentFocus);
            if (row.length === 0) {
                return;
            }
            $(row).addClass("selrow");


            var element = row[0];
            element.scrollIntoView({ block: "end", inline: "nearest" });

        }

    }

    function autocomplete_was_selected(row) {
        var t = $(row).attr("data-t");
        if (t === undefined) {
            t = $(row).find("td:first").text();
        }

        $("#" + c.controlid).val(t);
    }

}


function mystitky_multiselect(event, entity) {
    var o51ids = $("#TagPids").val();
    _zoom(event, null, null, 250, "★Zatřídit do kategorií...", "/o51/MultiSelect?entity=" + entity + "&o51ids=" + o51ids);

}


/*taghelper mycombochecklist*/
function mycombochecklist_init(c) {

    $("#divDropdown" + c.controlid).on("click.bs.dropdown", function (e) {
        e.stopPropagation();                                    //click na dropdown oblast nemá zavírat dropdown div
    });

    $("#value_alias_" + c.controlid).click(function () {
        $("#cmdCombo" + c.controlid).dropdown("toggle");        //click na textbox se má chovat stejně jako click na tlačítko cmdCombo
    });
    if ($("#value_alias_" + c.controlid).val() !== "") {
        $("#value_alias_" + c.controlid).css("font-weight", "bold");
        $("#value_alias_" + c.controlid).css("color", "navy");
    }

    $("#divDropdownContainer" + c.controlid).on("show.bs.dropdown", function (e) {

        $("#divDropdown" + c.controlid).css("margin-left", 25 + $("#divDropdown" + c.controlid).width() * -1);      //šírka dropdown oblasti má být zleva 100% jako celý usercontrol


        if ($("#divDropdown" + c.controlid).prop("filled") === true) return;    //combo už bylo dříve otevřeno

        $.post(c.posturl, { controlid: c.controlid, entity: c.entity, selectedvalues: c.selectedvalues, masterprefix: c.masterprefix, masterpid: c.masterpid, param1: c.param1 }, function (data) {

            $("#divData" + c.controlid).html(data);

            var xx = document.getElementById("divData" + c.controlid);
            var ofs = $(xx).offset();
            if (xx.scrollHeight > xx.clientHeight && $(xx).height() < 500 && window.innerHeight - ofs.top > 500) {        //je vidět scrollbara -> zvýšit výšku divData                      
                $("#divData" + c.controlid).css("height", "500px");
            }

            $("#divDropdown" + c.controlid).prop("filled", true);

            $("input:checkbox[name='chk" + c.controlid + "']").click(function () {
                var checked = $(this).prop("checked");

                var vals = [];
                var lbls = [];
                var strLabel = "";
                $("input:checkbox[name=chk" + c.controlid + "]:checked").each(function () {
                    vals.push($(this).val());
                    strLabel = $("label[for=" + $(this).attr("id") + "]").text();
                    lbls.push(strLabel);

                });
                var s = vals.join(",");

                $("#" + c.controlid).val(s);

                if (s !== "") {
                    $("#value_alias_" + c.controlid).css("font-weight", "bold");
                    $("#value_alias_" + c.controlid).css("color", "navy");
                } else {
                    $("#value_alias_" + c.controlid).css("font-weight", "");
                    $("#value_alias_" + c.controlid).css("color", "");
                }

                s = lbls.join(",");
                $("#value_alias_" + c.controlid).val(s);
                $("#value_alias_" + c.controlid).attr("title", s.replace(",", "★"));
                

                if (c.on_after_change !== "") {
                    eval(c.on_after_change + "('" + vals.join(",") + "')");
                }

            });

            $("#cmdCheckAll" + c.controlid).click(function () {
                var vals = [];
                var lbls = [];
                var strLabel = "";
                $("input:checkbox[name=chk" + c.controlid + "]").each(function () {
                    $(this).prop("checked", true);
                    vals.push($(this).val());
                    strLabel = $("label[for=" + $(this).attr("id") + "]").text();
                    lbls.push(strLabel);
                });
                var s = vals.join(",");
                $("#" + c.controlid).val(s);
                s = lbls.join(",");
                $("#value_alias_" + c.controlid).val(s);
                $("#value_alias_" + c.controlid).attr("title", s.replace(",", "★"));

                if (c.on_after_change !== "") {
                    eval(c.on_after_change + "('" + vals.join(",") + "')");
                }
            });

            $("#cmdUnCheckAll" + c.controlid).click(function () {
                $("input:checkbox[name=chk" + c.controlid + "]").each(function () {
                    $(this).prop("checked", false);

                });
                $("#" + c.controlid).val("");
                $("#value_alias_" + c.controlid).val("");
                $("#value_alias_" + c.controlid).attr("title", "");

                if (c.on_after_change !== "") {
                    eval(c.on_after_change + "('')");
                }
            });


        });


    });



}




/*taghelper mysearch*/
function mysearch_init(c) {
    var _mysearch_serverfiltering_timeout;

    $("#divDropdown" + c.controlid).on("click.bs.dropdown", function (e) {
        e.stopPropagation();                                    //click na dropdown oblast nemá zavírat dropdown div
    });
    $("#" + c.controlid).click(function () {
        $("#cmdCombo" + c.controlid).dropdown("toggle");        //click na textbox se má chovat stejně jako click na tlačítko cmdCombo
        $("#" + c.controlid).focus();
    });

    $("#divDropdownContainer" + c.controlid).on("show.bs.dropdown", function (e) {

        $("#divDropdown" + c.controlid).css("margin-left", 25 + $("#divDropdown" + c.controlid).width() * -1);      //šírka dropdown oblasti má být zleva 100% jako celý usercontrol

        var expr = $("#" + c.controlid).val();
        if (expr !== "") {

            return;
        }

        $.post(c.posturl + "Setting", { controlid: c.controlid, entity: c.entity }, function (data) {

            $("#divData" + c.controlid).html(data);

        });

    });

    $("#" + c.controlid).focus(function () {
        //$(this).select();
    });

    $("#" + c.controlid).on("input", function (e) {
        if ($("#divDropdown" + c.controlid).css("display") === "none") {
            $("#divDropdown" + c.controlid).dropdown("show");   //zobrazit dropdown, pokud je skrytý
        }


        if (typeof _mysearch_serverfiltering_timeout !== "undefined") {
            clearTimeout(_mysearch_serverfiltering_timeout);
        }
        _mysearch_serverfiltering_timeout = setTimeout(function () {
            //čeká se 500ms až uživatel napíše všechny znaky
            handle_mysearch_server_filtering();

        }, 500);


    });

    function handle_mysearch_server_filtering() {
        var expr = $("#" + c.controlid).val();

        $.post(c.posturl, { entity: c.entity, searchstring: expr }, function (data) {
            $("#divData" + c.controlid).html(data);

            var xx = document.getElementById("divData" + c.controlid);
            var ofs = $(xx).offset();

            if (xx.scrollHeight > xx.clientHeight && $(xx).height() < 500 && window.innerHeight - ofs.top > 500) {        //je vidět scrollbara -> zvýšit výšku divData                      
                $("#divData" + c.controlid).css("height", "500px");
            }

            $("#hovado .txs").on("click", function () {                
                var pid = $(this).attr("data-v");
                
                if (c.on_after_search !== "") {
                    eval(c.on_after_search + "('" + pid + "')");
                } else {
                    location.replace("/" + c.entity.substring(0, 3) + "/Recpage?pid=" + pid);
                }

                
            });


        });



    }


}

function mysearch_save_setting() {
    var strTopRecs = $("#mysearch_toprecs").val();
    var keys = [];
    var vals = [];

    keys.push("mysearch_toprecs");
    vals.push($("#mysearch_toprecs").val());
    keys.push("mysearch_closedrecs");
    vals.push($("#mysearch_closedrecs").val());

    $.post("/Common/SetUserParams", { keys: keys, values: vals }, function (data) {

        _notify_message("Nastavení bylo uloženo.", "info");

    });

    alert(s);
}


function mytree_init(expandall) {

    var toggler = document.getElementsByClassName("caret");
    var i;
    var s = $("#treeState1").val();
    var arr = [];
    if (s !== "") {
        arr = s.split(",");
    }


    for (i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function () {
            if (event.target.id === "") {
                return; //je třeba klikat na id=rx...
            }
            this.parentElement.querySelector(".branch_nested").classList.toggle("branch_active");

            this.classList.toggle("caret-down");


            var intIndex = arr.indexOf(this.id);

            if ($(this).hasClass("caret-down")) {
                if (intIndex === -1) {
                    arr.push(this.id);
                }
            } else {
                if (intIndex > -1) {
                    arr.splice(intIndex, 1);
                }

            }

            $("#treeState1").val(arr.join(","));


        });
    }

    if (expandall === true) {
        $(".caret").each(function () {

            $(this).click();


        });
    }
    


}