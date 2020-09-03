UIFT.IE = (function () {
    "use strict";

    var ret, isTheBrowser,
        actualVersion,
        jscriptMap, jscriptVersion;

    isTheBrowser = false;
    jscriptMap = {
        "5.5": "5.5",
        "5.6": "6",
        "5.7": "7",
        "5.8": "8",
        "9": "9",
        "10": "10"
    };
    jscriptVersion = new Function("/*@cc_on return @_jscript_version; @*/")();

    if (jscriptVersion !== undefined) {
        isTheBrowser = true;
        actualVersion = jscriptMap[jscriptVersion];
    }

    ret = {
        isTheBrowser: isTheBrowser,
        actualVersion: actualVersion
    };

    return ret;
}());

/*
Vraci scrollTop pozici pro vsechny prohlizece.
*/
UIFT.GetScrollTop = function () {
    var html = $("html").scrollTop(),
      body = $("body").scrollTop(),
      doc = $(document).scrollTop();
    var x1 = (html > body ? html : body);
    return (doc > x1 ? doc : x1);
};

UIFT.SetScrollTop = function (top) {
    $("html:not(:animated), body:not(:animated)").animate({ scrollTop: top }, 400, "swing");
};

/* workaround pro chybejici moznost parametru pro setTimeout v IE */
(function (f) {
    window.setTimeout = f(window.setTimeout);
    window.setInterval = f(window.setInterval);
})(function (f) {
    return function (c, t) {
        var a = [].slice.call(arguments, 2);
        return f(function () {
            c.apply(this, a);
        }, t);
    };
});

/* indexOf in array for IE */
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
        "use strict";
        if (this == null) {
            throw new TypeError();
        }
        var t = Object(this);
        var len = t.length >>> 0;

        if (len === 0) {
            return -1;
        }
        var n = 0;
        if (arguments.length > 1) {
            n = Number(arguments[1]);
            if (n != n) { // shortcut for verifying if it's NaN
                n = 0;
            } else if (n != 0 && n != Infinity && n != -Infinity) {
                n = (n > 0 || -1) * Math.floor(Math.abs(n));
            }
        }
        if (n >= len) {
            return -1;
        }
        var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
        for (; k < len; k++) {
            if (k in t && t[k] === searchElement) {
                return k;
            }
        }
        return -1;
    }
}