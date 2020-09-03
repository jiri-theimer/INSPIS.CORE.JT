(function ($) {
    $.fn.initSummaryOverview = function () {
        var $this = $(this);

        /* autogrow for summary textareas */
        autosize($this.find(".summary2 textarea"));

        /* enable summary overview question - click at default text region */
        $this.find(".summary1").click(function (ev) {
            var btn = $(this).next().find(".btnSummaryEdit");
            if (btn.is(":visible")) {
                btn.click();
            } else {
                $(this).siblings(".summary2").children("textarea").focus();
            }
            btn = null;
        });

        /* enable summary overview question - click at edit button */
        $this.find(".btnSummaryEdit").click(function (ev) {
            var qContainer = $(this).parents(".otazkaContainer"),
                questionData = qContainer.data("question"),
                /* automaticky generovany text */
                text = qContainer.find(".summary1").html();

            /* nastavit text custom editoru na text automatickeho editoru */
            CKEDITOR.instances["Answer_" + questionData.f19id].setData(text);

            /* zobrazit cudlik pro skryti editacniho editoru */
            $(this).hide().siblings(".btnSummaryDefault").show();

            /* zobrazit editacni editor */
            qContainer.find(".summary2").slideDown("fast");

            qContainer = questionData = null;
        });

        /* enable summary overview question - click at default button */
        $this.find(".btnSummaryDefault").click(function (ev) {
            var $t = $(this),
                qContainer = $t.parents(".otazkaContainer"),
                answerData = qContainer.find(".editor").data("answer"),
                questionData = qContainer.data("question"),
                $defaultText = $("#Answer_" + questionData.f19id + "_default", qContainer);

            /* switch buttons */
            $t.hide().siblings(".btnSummaryEdit").show();
            /* hide editor */
            qContainer.find(".summary2").slideUp("fast");
            CKEDITOR.instances["Answer_" + questionData.f19id].setData($defaultText.html());

            /* ulozit default text otazky */
            var dataToSave = {
                inputID: $defaultText.attr("id"),
                f19id: questionData.f19id,
                f21id: answerData.f21id,
                value: $defaultText.html(),
                filledByEval: true
            };
            
            UIFT.SaveAnswerToDb(dataToSave);

            dataToSave = qContainer = $t = answerData = questionData = $defaultText = null;
        });

        $this = null;
    };
})(jQuery);