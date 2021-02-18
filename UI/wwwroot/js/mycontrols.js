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
    var _listid = c.controlid + "_list";

    $("#" + c.controlid).on("mouseover", function () {
        if ($("#" + _listid).prop("filled") === true || $("#" + _listid).prop("filling") === true) return;    //datalist už bylo dříve načten

        $("#" + _listid).prop("filling", true); //momentálně běží plnění

        handle_load_options();

    });
    $("#" + c.controlid).on("focus", function () {
       

        if ($("#" + _listid).prop("filled") === true) return;    //datalist už bylo dříve načten

        handle_load_options();

        

    });


    function handle_load_options() {
        var strCurPlaceHolder = $("#" + c.controlid).attr("placeholder");
        $("#" + c.controlid).attr("placeholder", "Loading...");
        $.post(c.posturl, { o15flag: c.o15flag }, function (data) {

            $("#" + _listid).html(data);

            $("#" + _listid).prop("filled", true);

            $("#" + c.controlid).attr("placeholder", strCurPlaceHolder);


            

        });
    }

}


function mystitky_multiselect(event, entity) {
    var o51ids = $("#TagPids").val();
    
    _zoom(event, null, null, 250, $(event.target).text(), "/o51/MultiSelect?entity=" + entity + "&o51ids=" + o51ids);

}


/*taghelper mycombochecklist*/
function mycombochecklist_init(c) {

    $("#divDropdown" + c.controlid).on("click.bs.dropdown", function (e) {
        e.stopPropagation();                                    //click na dropdown oblast nemá zavírat dropdown div
    });


    if ($("#value_alias_" + c.controlid).val() !== "") {

        var deftext = $("#value_alias_" + c.controlid).val();
        deftext = deftext.replace(/,/g, "★");
        $("#cmdCombo" + c.controlid).text(deftext);
        $("#cmdCombo" + c.controlid).attr("title", deftext);

    }

    var myDropdown = document.getElementById("divDropdownContainer" + c.controlid);

    myDropdown.addEventListener("show.bs.dropdown", function () {

        $("#divDropdown" + c.controlid).css("margin-left", 25 + $("#divDropdown" + c.controlid).width() * -1);      //šírka dropdown oblasti má být zleva 100% jako celý usercontrol


        if ($("#divDropdown" + c.controlid).prop("filled") === true) return;    //combo už bylo dříve otevřeno

        $.post(c.posturl, { controlid: c.controlid, entity: c.entity, selectedvalues: c.selectedvalues, masterprefix: c.masterprefix, masterpid: c.masterpid, myqueryinline: c.myqueryinline }, function (data) {

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

                if (s === "") {
                    $("#cmdCombo" + c.controlid).text(c.placeholder);
                    $("#cmdCombo" + c.controlid).attr("title", "");
                    $("#value_alias_" + c.controlid).val("");
                } else {
                    s = lbls.join(",");
                    s = s.replace(/,/g, "★");
                    $("#value_alias_" + c.controlid).val(s);
                    $("#cmdCombo" + c.controlid).text(s);
                    $("#cmdCombo" + c.controlid).attr("title", s);
                }



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
                s = s.replace(/,/g, "★");
                $("#value_alias_" + c.controlid).val(s);
                $("#cmdCombo" + c.controlid).text(s);
                $("#cmdCombo" + c.controlid).attr("title", s);


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
                $("#cmdCombo" + c.controlid).text(c.placeholder);
                $("#cmdCombo" + c.controlid).attr("title", "");


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
   

    var myDropdown = document.getElementById("divDropdownContainer" + c.controlid);

    myDropdown.addEventListener("show.bs.dropdown", function () {

        var dd = document.getElementById("divDropdown" + c.controlid);
        var x = $("#divDropdownContainer" + c.controlid).offset().left;        
        if ($(dd).width() < 300) {
            $(dd).css("width", _device.innerWidth - x - 10);            
            if (_device.innerWidth < 500) {                
                $(dd).css("width","600px");
            }
        }
        var expr = $("#" + c.controlid).val();
        if (expr !== "") {
            setTimeout(function () {
                //focus až po 300ms
                $("#" + c.controlid).focus();
                $("#" + c.controlid).select();                
            }, 200);
            return;
        }

        $("#divData" + c.controlid).html("Loading...");

        $.post(c.posturl + "Setting", { controlid: c.controlid, entity: c.entity }, function (data) {

            $("#divData" + c.controlid).html(data);

            setTimeout(function () {
                //focus až po 300ms
                $("#" + c.controlid).focus();
                $("#" + c.controlid).select();

            }, 200);

        });

        $("#divDropdown" + c.controlid).css("top", "500px");

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


function myselect_focus(e, cbx, entity, textfield,orderfield) { //zpracování událost focus pro taghelper mySelect
    
    if ($(cbx).prop("naplneno")) {
        return; //dropdown byl již dříve naplněn daty
    }

    var firstrow = "";
    var curval = $(cbx).val();
    
    if (cbx.options.length > 0) {
        firstrow = "<option value='" + cbx.options[0].value + "'>" + cbx.options[0].text + "</option>"; //hack kvůli vizualiazci dropdown
    }

    
    for (var i = 1; i <= 10; i++) {
        $(cbx).append(new Option("Loading..."));
        
    }   
    $(cbx).val("0");
    
    //$(cbx).load("/TheCombo/GetMySelectHtmlOptions?textfield=" + textfield + "&entity=" + entity + "&orderfield=" + orderfield);
    
    
    //$(cbx).load("/TheCombo/GetMySelectHtmlOptions?entity=" + entity + "&textfield=" + textfield);
   
    $.post("/TheCombo/GetMySelectHtmlOptions", { textfield: textfield, entity: entity, orderfield: orderfield }, function (data) {

        $(cbx).html(firstrow + data);


        $(cbx).prop("naplneno", "1");               

        $(cbx).html(firstrow + data);

        $(cbx).val(curval);
       
        var x = $(cbx).find(":selected").index();
        //$(cbx).attr("size", 10);
        cbx.options[x].scrollIntoView(false);
        //$(cbx).attr("size", 1);

        
        
    });
    

}



