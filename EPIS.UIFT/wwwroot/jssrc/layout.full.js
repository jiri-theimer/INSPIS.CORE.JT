/* pri prvnim nahrati stranky - onready */
$(window).load(function () {
    CKEDITOR.disableAutoInline = true;
    
    /* set page elements */
    UIFT.Controls.scrollIndicator = $('#scrollIndicator')
    UIFT.Controls.contentContainer = $("#contentContainerInner");
    UIFT.Controls.segmentsTree = $("#segmentsTree");
    UIFT.Controls.window = $(window);

    /* check browser version */
    var ua = navigator.userAgent.toLowerCase();
    if (ua.indexOf('msie 6.0') >= 0 || ua.indexOf('msie 7.0') >= 0) {
        var warning = $('<div id="browserWarning">Prohlížeč Internet Explorer verze 7 a nižší není v systému EPIS podporován!</div>').prependTo("body");
        var wleft = parseInt($("body").width() / 2) - parseInt(warning.width() / 2);
        warning.css({ "left": wleft + "px" }).slideDown("slow");
        warning = null;
    }

    /* init mobile section menu */
    $("#mobileMenu").mobileMenu();

    /* workflow dialog */
    $("#headerWF_move").click(function (e) {
        e.preventDefault();
        var html = '<div id="workflowWindow"><iframe src="' + this.href + '" style="width:620px;height:580px;" /></div>';

        $(html).dialog({
            modal: true,
            width: 646,
            height: 640,
            title: UIFT.tra("Posunout / doplnit"),
            close: function (event, ui) {
                /* odstranit dialog z dom */
                $(this).dialog("destroy").remove();
            }
        });
    });

    /* export to Word */
    $("#aExportWord").click(function (e) {
        e.preventDefault();
        
        if ($("#exportToWordTemplate").length == 0) {
            $.post($(this).attr("href"), function (data) {
                $(data).dialog({
                    modal: true,
                    width: 400,
                    height: 250,
                    title: "Export",
                    close: function (event, ui) {
                        /* odstranit dialog z dom */
                        $(this).dialog("destroy").remove();
                    },
                    buttons: [
                        {
                            text: UIFT.tra("Včetně odpovědí"),
                            click: function () {
                                $(this).loading();
                                location.href = $(this).data("url") + "?showAnswers=true&templateId=" + $("#exportToWordTemplate").val();
                                $(this).dialog("close");
                            }
                        },
                        {
                            text: UIFT.tra("Prázdný dotazník"),
                            click: function () {
                                $(this).loading();
                                location.href = $(this).data("url") + "?showAnswers=false&templateId=" + $("#exportToWordTemplate").val();
                                $(this).dialog("close");
                            }
                        }
                    ]
                });
            });
        }
    });

    /* ukazat ikonu oznamujici moznost skrolovani nahoru */
    if (UIFT.Controls.scrollIndicator.length > 0) {
        UIFT.Controls.window.scroll(function () {
            var y = UIFT.GetScrollTop();
            if (y > 1) {
                UIFT.Controls.scrollIndicator.fadeIn(300);
            } else {
                UIFT.Controls.scrollIndicator.fadeOut(300);
            }
        });
    }

    if (UIFT.Controls.segmentsTree.length > 0) {
        /* vyska menu sekci + scrollbary */
        UIFT.adjustSegmentsTree();

        /* kliknuti na uzel ve stromu sekci */
        UIFT.Controls.segmentsTree.find("a").click(function (e) {
            e.preventDefault();

            /* je otevreno mobilni menu - zavri ho */
            if ($("html").hasClass("menuOpened")) {
                $("#menuOverlay").click();
            }

            /* loading overlay */
            UIFT.Controls.contentContainer.loading();

            /* musi byt v timoutu, aby se provedla nejdriv onchange udalost opousteneho prvku */
            window.setTimeout(function (obj) {
                /* odstranit vsechny Html Editory kvuli vycisteni pameti */
                for (var i = CKEDITOR.instances.length - 1; i >= 0; i--) {
                    CKEDITOR.instances[i].destroy();
                }

                /* odstranit fileupload widgety */
                $(".qControl_FileUpload .odpovedContainer input").fileupload('destroy');

                /* nahrat novy obsah hlavni casti */
                UIFT.LoadSegment($(obj).parent().data("id"));
                obj = null;
            }, 200, this);
        });

        /* podle tridy .sel nahrat aktualni obsah hlavni casti */
        /* loading overlay */
        UIFT.Controls.contentContainer.loading();
        UIFT.LoadSegment(UIFT.Controls.segmentsTree.find("li.sel").data("id"), UIFT.ActualQuestion);
    }
});