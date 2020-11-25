/* global javascript error handler */
window.onerror = function (msg, url, line, col, error) {
    // Note that col & error are new to the HTML 5 spec and may not be 
    // supported in every browser.  It worked for me in Chrome.
    var extra = !col ? '' : '\n column: ' + col;
    extra += !error ? '' : '\n error: ' + error;
    if (!url) url = location.href;

    // You can view the information in an alert to see things working like this:
    var message = UIFT.tra("V aplikaci nastala chyba ") + " \n ";
    message += "Error: " + msg + "\n url: " + url + "\n line: " + line + extra;
    alert(message);

    var suppressErrorAlert = true;
    // If you return true, then error alerts (like in older versions of 
    // Internet Explorer) will be suppressed.
    return suppressErrorAlert;
};

/* Czech initialisation for the jQuery UI date picker plugin. */
jQuery(function ($) {
    var lang = localizationLanguageIndex == 2 ? "ua" : "cs";
    $.datepicker.regional[lang] = datepickerLocalization.find(t => t.k == lang).v;
    $.datepicker.setDefaults($.datepicker.regional[lang]);
});

/* nastaveni jQuery ajax */
$.ajaxSetup({
    type: "POST",
    beforeSend: function (jqXHR, settings) {
        /* normalize url */
        var url = settings.url;
        if (!url.startsWith("http")) {
            /* odstranit root url, pokud je soucasti url */
            if (url.substr(0, UIFT.WebRoot.length) == UIFT.WebRoot && UIFT.WebRoot != "") {
                url = url.substr(UIFT.WebRoot.length);
            }
            /* odstranit a11id pokud je soucasti url */
            if (url.substr(1, UIFT.a11id.toString().length) == UIFT.a11id) {
                url = url.substr(UIFT.a11id.toString().length + 1);
            }
            /* vytvorit nove url */
            url = UIFT.WebRoot + "/" + UIFT.a11id + url;
        }

        settings.url = url;
        
        var e = $('input[name="__UiftCsrfToken"]').val();
        
        jqXHR.setRequestHeader("UiftCsrf", e);

        return settings;
    },
    error: function (event, jqXHR, ajaxSettings, thrownError) {
        alert(thrownError);
        if (event.status == 403) {  /* odhlasit uzivatele */
            window.open("", "_top");
        } else {
            alert(UIFT.tra("V aplikaci nastala chyba ") + event.status);
        }
    }
});
