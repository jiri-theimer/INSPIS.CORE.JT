(function ($) {
    var _disableOnAnswerChange = false;

    $.fn.initQuestion = function () {
        var $this = $(this),
            questionData = $this.data("question"),
            readOnly = $this.hasClass("readOnly");
        
        switch (questionData.control) {
            /* checkbox list */
            case UIFT.ReplyKeyEnum.CheckboxList:
                $this.initCheckboxList();
                break;

            /* baterie */
            case UIFT.ReplyKeyEnum.BatteryBoard:
                $this.find(".tabBattery tr").not(".batteryFirstRow").each(function () {
                    switch ($(this).data("question").control) {
                        case UIFT.ReplyKeyEnum.TextBox:
                            _setupTextBoxes($(this).data("question").type, $this);
                            break;
                        case UIFT.ReplyKeyEnum.CheckboxList:
                            $(this).initCheckboxList();
                            break;
                    }
                });
                break;

            /* sachovnice */
            case UIFT.ReplyKeyEnum.ChessBoard:
                _setupTextBoxes(questionData.childType, $this);
                break;

            /* textboxes */
            case UIFT.ReplyKeyEnum.TextBox:
                _setupTextBoxes(questionData.type, $this);
                break;

                /* enable html editors */
            case UIFT.ReplyKeyEnum.HtmlEditor:
            case UIFT.ReplyKeyEnum.SummaryOverview:
                /* jen pro summary */
                if (questionData.control == UIFT.ReplyKeyEnum.SummaryOverview) {
                    $this.initSummaryOverview();
                }

                $this.find(".editor").each(function () {
                    CKEDITOR.inline(this.id, {
                        customConfig: '',
                        skin: 'moono',
                        width: '500px',
                        height: '180px',
                        defaultLanguage: 'cs',
                        language: 'cs',
                        readOnly: readOnly,
                        pasteFromWordRemoveFontStyles: true,
                        pasteFromWordRemoveStyles: false,
                        toolbar: [
                            ['Bold', 'Italic', 'Subscript', 'Superscript', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink', '-', 'SpecialChar', 'RemoveFormat', 'Table', 'tabletools']
                        ],
                        on: {
                            instanceReady: function (ev) {
                                /* kvuli chrome - kdyz to tu nebude, budou pri hidden parent elementu disablovane toolbary */
                                var obj = $(ev.editor.container.$.parentElement);
                                if (!obj.data("visible") && obj.data("visible") != undefined) obj.css("display", "none");
                                obj = null;
                            },
                            blur: function (ev) {
                                if (!ev.editor.readOnly) {
                                    _onAnswerChange.call(document.getElementById(ev.editor.name), ev);
                                }
                            }
                        }
                    });
                });

                break;

            /* enable file upload */
            case UIFT.ReplyKeyEnum.FileUpload:
                $this.find("button.btnUpload").initUpload();
                break;

            /* button */
            case UIFT.ReplyKeyEnum.Button:
                $this.find("button").click(function () {
                    /* disable button to prevent double click */
                    this.disabled = true;

                    var dataToSave = { f19id: $(this).parents(".otazkaContainer").data("question"), inputID : "" };

                    $.ajax({
                        url: "/Otazka/Button",
                        data: dataToSave,
                        success: function (data) {
                            if (data.success) {
                                var sep = (data.url.indexOf("?") > 0 ? "&" : "?");
                                location.href = data.url + sep + "guid=" + data.guid; 
                            } else {
                                alert(data.message);
                            }
                        }
                    });
                });
                break;
        }

        /* enable tooltips */
        $this.find(".otazkaNameHint").tooltip({ position: { my: "left+24 top-6", at: "left bottom" } });

        /* publikovani otazky */
        $this.find(".publishQuestion input").change(function (ev) {
            var dataToSave = { f25id: 0, f26id: 0, f19id: 0, value: (this.checked ? "true" : "false") };

            switch ($(this).parents(".otazkaContainer").data("question").control) {
                case UIFT.ReplyKeyEnum.ChessBoard:
                    dataToSave.f25id = this.value;
                    break;
                case UIFT.ReplyKeyEnum.BatteryBoard:
                    dataToSave.f26id = this.value;
                    break;
                default:
                    dataToSave.f19id = this.value;
                    break;
            }

            var checked = this.checked;
            $.ajax({
                url: "/Otazka/PublikovaniOtazky",
                data: dataToSave,
                success: function(data) {
                    /* info hlaska */
                    $.jGrowl(checked ? UIFT.tra("Otázka byla označena jako publikovatelná") : UIFT.tra("Otázka již není publikovatelná"));
                }
            });
        });

        /* ulozeni komentaru */
        $this.find(".commentContainer textarea").change(_onCommentChange);

        /* ulozeni odpovedi */
        $this.find(".odpovedContainer").find('input, select, textarea').not(".comment").change(_onAnswerChange);

        /* vycistit otazku button pro checkbox list, radio button list */
        $this.find("a.clearAnswer").click(function (e) {
            e.preventDefault();

            if (confirm(UIFT.tra("Opravdu si přejete vrátit otázku do výchozího stavu?"))) {
                _disableOnAnswerChange = true;

                var $coll = $(this).parent(),
                    questionData = $coll.parents(".otazkaContainer").data("question");

                $coll.find("input").prop("checked", false);
                $coll.find("input").removeAttr("disabled");
                /* komentare */
                $coll.find(".commentContainer").hide().find("textarea").val("");
                $coll = null;

                /* post data odeslana na server. f19id musi byt zadano vzdy. V pripade baterie musi byt jeste f26id. */
                var ajaxData = { f19id: 0 };
                switch (questionData.control) {
                    case UIFT.ReplyKeyEnum.ChessBoard:
                        ajaxData.f25id = questionData.f25id;
                        break;
                    case UIFT.ReplyKeyEnum.BatteryBoard:
                        ajaxData.f26id = questionData.f26id;
                        break;
                    default:
                        ajaxData.f19id = questionData.f19id;
                        break;
                }

                /* zavolat vycisteni odpovedi na serveru */
                $.ajax({
                    url: "/Otazka/Vycisteni",
                    data: ajaxData,
                    success: function (data) {
                        if (!data.success) alert(data.message);

                        /* refresh segmentu kvuli zmene stavu otazek zavyslich na teto otazce */
                        $("#segmentsTree li.sel > a").click();
                    }
                });

                _disableOnAnswerChange = false;
            }
        });

        $this = null;
    };

    /* nastaveni spravne funkcnosti textboxu / textovych odpovedi
    type = typ textove odpovedi (cislo, datum....)
    obj = kontajner otazky
    */
    function _setupTextBoxes(type, obj) {
        switch (type) {
            /* integer */
            case UIFT.DataTypeEnum.tInteger:
                obj.find("input.numeric").numericInput({ allowFloat: true });
                break;

                /* decimal */
            case UIFT.DataTypeEnum.tDecimal:
                obj.find("input.numeric").numericInput();
                break;

                /* enable date pickers */
            case UIFT.DataTypeEnum.tDate:
            case UIFT.DataTypeEnum.tDateTime:
            case UIFT.DataTypeEnum.tTime:
                obj.find('input.date').each(function () {
                    var $t = $(this),
                        min = parseInt($t.attr("min")),
                        max = parseInt($t.attr("max")),
                        args = {};

                    /* min and max dates */
                    if (!isNaN(min)) {
                        args.minDate = new Date(min);
                    }
                    if (!isNaN(max)) {
                        args.maxDate = new Date(max);
                    }
                    /* disabled */
                    if (this.readOnly) {
                        args.disabled = true;
                    }

                    $t.datepicker(args);
                    $t = null;
                });

                /* enable time pickers */
                obj.find('[id$="_time"]').numericInput({ floatSeparator: ":" }).timepicker({
                    timeFormat: "H:i",
                    lang: { decimal: ',', mins: 'min', hr: 'h', hrs: 'hod' }
                });
                break;

            /* cisty textbox nebo textarea */
            default:
                autosize(obj.find("textarea"));
                break;
        }
    };

    /* zobrazeni / skryti komentaru k otazce */
    function _commentToggle(qContainer, questionData, current_f21id) {
        var setFocusTo = "";

        switch (questionData.control) {
            case UIFT.ReplyKeyEnum.RadiobuttonList:
            case UIFT.ReplyKeyEnum.CheckboxList:
                $('input[data-comment="True"]', qContainer).each(function () {
                    $("#commentContainer_" + questionData.f19id + "_" + this.value, qContainer).css("display", this.checked ? "block" : "none");
                    if (this.value == current_f21id && this.checked) {
                        setFocusTo = this.value;
                    }
                });
                break;
            case UIFT.ReplyKeyEnum.DropdownList:
                $('option[data-comment="True"]', qContainer).each(function () {
                    $("#commentContainer_" + questionData.f19id + "_" + this.value, qContainer).css("display", this.selected ? "block" : "none");
                    if (this.value == current_f21id && this.selected) {
                        setFocusTo = this.value;
                    }
                });
                break;
        }

        /* pokud se ma hodit focus na otevreny koment */
        if (setFocusTo != "") {
            $("#commentContainer_" + questionData.f19id + "_" + setFocusTo + " textarea", qContainer).focus();
        }
    };

    /* ulozeni zmeny komentare */
    function _onCommentChange(ev) {
        var $input = $(this),
            questionData = $input.parents(".otazkaContainer").data("question"),
            f21id = $(this).data("comment-id");

        $.ajax({
            url: "/Otazka/Komentar",
            data: { f19id: questionData.f19id, f21id: f21id, value: this.value },
            success: function (data) {
                if (data.success) {
                    /* zapsat zmenu stavu na ulozeno */
                    $input.addClass("success");

                    /* doplnit nove default values do elementu na strance */
                    UIFT.ProccessDefaultValues(data.defaultValues, data.f19id);

                    /* info hlaska */
                    $.jGrowl(UIFT.tra("Komentář byl uložen"));

                    /* odstranit success class po chvili */
                    window.setTimeout(function () {
                        $input.removeClass("success");
                        $input = null;
                    }, 1500);
                } else {
                    $.jGrowl(UIFT.tra("Komentář se nepodařilo uložit: ") + data.message, { theme: "error", life: 5000 });
                }
            }
        });
    };

    /* ulozeni zmeny odpovedi */
    function _onAnswerChange(ev) {
        /* pokud je docasne vypnuta obsluha udalosti */
        if (_disableOnAnswerChange) return;

        var $this = $(this),
            answerData = $this.data("answer"),
            qContainer = $this.parents(".otazkaContainer"),
            questionData = qContainer.data("question"),
            dataToSave = { inputID: $this.attr("id"), f19id: questionData.f19id, f21id: answerData.f21id, value: "" };

        /* jedna se o baterii */
        if (questionData.control == UIFT.ReplyKeyEnum.BatteryBoard) {
            questionData = $this.parents("tr").data("question"),
            dataToSave.f19id = questionData.f19id;
        }

        /* jedna se o sachovnici */
        if (questionData.control == UIFT.ReplyKeyEnum.ChessBoard) {
            dataToSave.f19id = answerData.f19id;
        }
        
        /* ulozit hodnotu odpovedi */
        switch (questionData.control) {
            case UIFT.ReplyKeyEnum.EvalList:
                if ($this.prop("tagName").toLowerCase() == "select") {
                    dataToSave.value = $this.children(":selected").data("value");
                    dataToSave.alias = $this.children(":selected").html();
                } else { /* multiselect */
                    $coll = $this.closest("div").find(":checked");

                    if ($coll.length > 0) {
                        var arrValues = jQuery.map($coll, function (a) {
                            return a.value;
                        });
                        var arrAliasis = jQuery.map($coll, function (a) {
                            return $(a).parent().text().trim();
                        });

                        dataToSave.value = arrValues.join("|");
                        dataToSave.alias = arrAliasis.join("|");
                    } else {
                        dataToSave.value = "";
                        dataToSave.alias = "";
                    }

                    $coll = null;
                }
                break;
            case UIFT.ReplyKeyEnum.Checkbox:
                dataToSave.value = $this.val();
                break;
            case UIFT.ReplyKeyEnum.CheckboxList:
            case UIFT.ReplyKeyEnum.RadiobuttonList:
                dataToSave.value = (this.checked ? "true" : "false");

                /* zobrazit / skryt komentar */
                _commentToggle(qContainer, questionData, dataToSave.f21id);
                break;
            case UIFT.ReplyKeyEnum.DropdownList:
                dataToSave.f21id = parseInt($this.val());
                if (isNaN(dataToSave.f21id)) {
                    dataToSave.f21id = 0;
                }

                /* zobrazit / skryt komentar */
                _commentToggle(qContainer, questionData, dataToSave.f21id);
                break;
            case UIFT.ReplyKeyEnum.TextBox:
                if (UIFT.DataTypeEnum.tDateTime == questionData.type) {
                    dataToSave.value = $("#Answer_" + dataToSave.f19id).val() + " " + $("#Answer_" + dataToSave.f19id + "_time").val();
                } else {
                    dataToSave.value = $this.val();
                }
                break;
            case UIFT.ReplyKeyEnum.HtmlEditor:
            case UIFT.ReplyKeyEnum.SummaryOverview:
                dataToSave.value = ev.editor.getData();
                break;
            case UIFT.ReplyKeyEnum.ChessBoard:
                if (questionData.childControl == UIFT.DataTypeEnum.tDateTime) {
                    dataToSave.value = $("#Answer_" + dataToSave.f19id).val() + " " + $("#Answer_" + dataToSave.f19id + "_time").val();
                } else {
                    dataToSave.value = $this.val();
                }
                break;
            case UIFT.ReplyKeyEnum.BatteryBoard:
                dataToSave.value = (this.checked ? "true" : "false");
                dataToSave.f19id = answerData.f19id;
                break;
            default:
                dataToSave.value = $this.val();
                break;
        }
        
        /* zmenit stav */
        answerData.state = UIFT.AnswerState.Saving;

        /* odeslat pozadavek na server */
        UIFT.SaveAnswerToDb(dataToSave);

        $this = dataToSave = answerData = questionData = qContainer = null; /* clean up */
    };
})(jQuery);