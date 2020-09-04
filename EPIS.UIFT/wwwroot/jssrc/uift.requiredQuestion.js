/*
Nastavuje otazku do required rezimu nebo zpet.
*/
(function ($) {
    $.fn.requiredQuestion = function (setRequired) {
        if (setRequired) {
            $(this).addClass("isRequired");
        } else {
            $(this).removeClass("isRequired");
        }
    };
})(jQuery);
