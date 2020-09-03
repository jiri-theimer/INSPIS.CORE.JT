/* global javascript error handler */
window.onerror = function (msg, url, line, col, error) {
    // Note that col & error are new to the HTML 5 spec and may not be 
    // supported in every browser.  It worked for me in Chrome.
    var extra = !col ? '' : '\ncolumn: ' + col;
    extra += !error ? '' : '\nerror: ' + error;
    if (!url) url = location.href;

    // You can view the information in an alert to see things working like this:
    var message = "V aplikaci nastala chyba. \n";
    message += "Error: " + msg + "\nurl: " + url + "\nline: " + line + extra;
    alert(message);

    var suppressErrorAlert = true;
    // If you return true, then error alerts (like in older versions of 
    // Internet Explorer) will be suppressed.
    return suppressErrorAlert;
};

/* Czech initialisation for the jQuery UI date picker plugin. */
jQuery(function ($) {
    $.datepicker.regional['cs'] = {
        closeText: 'Zavřít',
        prevText: '&#x3c;Dříve',
        nextText: 'Později&#x3e;',
        currentText: 'Nyní',
        monthNames: ['leden', 'únor', 'březen', 'duben', 'květen', 'červen',
        'červenec', 'srpen', 'září', 'říjen', 'listopad', 'prosinec'],
        monthNamesShort: ['led', 'úno', 'bře', 'dub', 'kvě', 'čer',
		'čvc', 'srp', 'zář', 'říj', 'lis', 'pro'],
        dayNames: ['neděle', 'pondělí', 'úterý', 'středa', 'čtvrtek', 'pátek', 'sobota'],
        dayNamesShort: ['ne', 'po', 'út', 'st', 'čt', 'pá', 'so'],
        dayNamesMin: ['ne', 'po', 'út', 'st', 'čt', 'pá', 'so'],
        weekHeader: 'Týd',
        dateFormat: 'dd.mm.yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: '',
        changeMonth: true,
        changeYear: true
    };
    $.datepicker.setDefaults($.datepicker.regional['cs']);
});

/* nastaveni jQuery ajax */
$.ajaxSetup({
    type: "POST",
    beforeSend: function (jqXHR, settings) {
        /* normalize url */
        var url = settings.url;
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
        
        settings.url = url;

        return settings;
    },
    error: function (event, jqXHR, ajaxSettings, thrownError) {
        alert(thrownError);
        if (event.status == 403) {  /* odhlasit uzivatele */
            window.open("", "_top");
        } else {
            alert("V aplikaci nastala chyba: " + event.status);
        }
    }
});
