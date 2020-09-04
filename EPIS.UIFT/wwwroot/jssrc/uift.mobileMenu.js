(function ($) {
    var animationTimeout = 500;

    $.fn.mobileMenu = function () {
        $(this).click(_open);
    };

    /* window width */
    function getWidth() {
        var w = Math.max(UIFT.Controls.window.innerWidth(), window.innerWidth);
        if (w <= 575) {
            return "80%";
        } else {
            return "50%";
        }
    };

    /* open menu */
    function _open(e) {
        e.preventDefault();
        e.stopImmediatePropagation();

        var menuWidth = getWidth();
        
        /* remove scrollbars */
        $("html").addClass("menuOpened");

        /* move elements */
        $("#menuContainer").animate({ "left": "0px" }, animationTimeout);
        $("#contentContainer").animate({ "margin-left": menuWidth }, animationTimeout);
        $("#header").animate({ "left": menuWidth }, animationTimeout);

        /* add overlay */
        $('<div id="menuOverlay" />').prependTo("body").click(_close);
    };

    /* close menu */
    function _close(e) {
        e.preventDefault();
        e.stopImmediatePropagation();

        var menuWidth = getWidth();

        $("#menuContainer").animate({ "left": "-" + menuWidth }, animationTimeout, function () {
            /* remove overlay */
            $("#menuOverlay").remove();
            /* return scrollbars */
            $("html").removeClass("menuOpened");
        });
        $("#header").animate({ "left": "0px" }, animationTimeout);
        $("#contentContainer").animate({ "margin-left": "0px" }, animationTimeout);
    };

})(jQuery);