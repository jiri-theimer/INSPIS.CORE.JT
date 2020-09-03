/*
Parametry:
action:
    hide = odstrani loading
    show (nebo nic) = zobrazi loading
*/
(function (jQuery) {
    jQuery.fn.loading = function (action, settings) {

        /* nastaveni */
        var settings = jQuery.extend({
            "image": UIFT.WebRoot + "/css/images/loading3.gif",
            "overlayClass": "dvLoading",
            "imageClass": "dvLoading2",
            "loadingImageHeight": 40
        }, settings);

        /* zpracovat kazdy element v sade */
        return this.each(function () {
            var isBody = this.tagName.toLowerCase() == "body";

            /* odstrani loadovaci objekty z rodice */
            switch (action) {
                case "hide":
                    if (isBody) {
                        jQuery("#jquery_loading_1, #jquery_loading_2").remove();
                    } else {
                        jQuery("." + settings.overlayClass + ", ." + settings.imageClass, this).remove();
                        /* position pred loadingem */
                        var pos = jQuery(this).prop("data-loading").toString();
                        if (pos.length > 0) {
                            jQuery(this).css("position", pos);
                        }
                    }
                    break;

                default: /* zobrazit loading */
                    var obj1 = jQuery(this);
                    var obj2;
                    /* v pripade "body" pridat do divu i ID kvuli rychlejsimu hledani */
                    var id1 = id2 = "";
                    var isFieldset;

                    if (!isBody) {
                        /* ulozit puvodni pozici */
                        obj1.prop("data-loading", obj1.css("position")).css("position", "relative");
                        obj2 = obj1;
                        isFieldset = (obj2[0].tagName.toLowerCase() == "fieldset");
                    } else {
                        id1 = ' id="jquery_loading_1"';
                        id2 = ' id="jquery_loading_2"';
                        obj2 = jQuery(window);
                        isFieldset = false;
                    }

                    /* sirka rodice vcetne padding atd. */
                    var width = parseInt(obj2.outerWidth());
                    if (isNaN(width)) {
                        width = parseInt(obj2.width());
                    }
                    /* vyska rodice vcetne padding atd. */
                    var height = parseInt(obj2.outerHeight());
                    if (isNaN(height)) {
                        height = parseInt(obj2.height());
                    }

                    /* umisteni overlay */
                    var padLeft, padTop, offsetTop;
                    if (isFieldset) {
                        /* fix legend problem */
                        offsetTop = parseInt(jQuery("legend", obj2).outerHeight());
                        padLeft = parseInt(obj2.css("padding-left"));
                        padTop = parseInt(obj2.css("padding-top"));
                    } else {
                        offsetTop = padLeft = padTop = 0;
                    }
                    var left = (isNaN(padLeft) ? 0 : -padLeft);
                    var top = (isNaN(padTop) ? 0 : -padTop) - offsetTop;

                    /* umisteni loading obrazku */
                    var leftp = parseInt((width / 2) - (settings.loadingImageHeight / 2)) - padLeft;
                    var topp = parseInt((height / 2) - (settings.loadingImageHeight / 2)) - offsetTop - padTop;

                    /* loadovaci overlay */
                    obj1.prepend('<div' + id1 + ' class="' + settings.overlayClass + '" style="left:' + left + 'px;top:' + top + 'px;width:' + width + 'px;height:' + height + 'px;"></div>');
                    /* loadovaci obrazek */
                    obj1.prepend('<div' + id2 + ' class="' + settings.imageClass + '" style="left:' + leftp + 'px;top:' + topp + 'px;"><img src="' + settings.image + '" /></div>');

                    /* vycistit */
                    obj1 = obj2 = null;
                    break;
            }
        });
    };
})(jQuery);