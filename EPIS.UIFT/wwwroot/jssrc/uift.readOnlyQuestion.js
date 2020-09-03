/*
Nastavuje otazku do readOnly rezimu nebo zpet.
*/
(function ($) {
    $.fn.readOnlyQuestion = function (setReadOnly) {
        var $this = $(this),
            data = $this.data("question");

        if (setReadOnly) { /* nastavit RO */
            /* disable all controls */
            switch (data.control) {
                case UIFT.ReplyKeyEnum.TextBox:
                    $('.odpovedContainer input', $this).prop("readOnly", true);

                    if (data.type == UIFT.DataTypeEnum.tDate || data.type == UIFT.DataTypeEnum.tDateTime) {
                        $(".hasDatepicker").datepicker("option", "disabled", true);
                    }
                    break;
                case UIFT.ReplyKeyEnum.DropdownList:
                case UIFT.ReplyKeyEnum.Checkbox:
                case UIFT.ReplyKeyEnum.RadiobuttonList:
                case UIFT.ReplyKeyEnum.CheckboxList:
                case UIFT.ReplyKeyEnum.Button:
                    $('.odpovedContainer', $this).find("input, select").attr("disabled", "disabled");
                    break;
                case UIFT.ReplyKeyEnum.SummaryOverview:
                    CKEDITOR.instances["Answer_" + data.f19id].setReadOnly(true);
                    $(".summary3", $this).hide();
                    break;
                case UIFT.ReplyKeyEnum.HtmlEditor:
                    CKEDITOR.instances["Answer_" + data.f19id].setReadOnly(true);
                    break;
            }

            /* set visual style */
            $this.addClass("readOnly").find(".clearAnswer").hide();

        } else { /* odstranit readonly */

            /* otazka je opravdu readonly */
            if ($this.hasClass("readOnly")) {
                /* enable all controls */
                switch (data.control) {
                    case UIFT.ReplyKeyEnum.TextBox:
                        $('.odpovedContainer input', $this).prop("readOnly", false);

                        if (data.type == UIFT.DataTypeEnum.tDate || data.type == UIFT.DataTypeEnum.tDateTime) {
                            $(".hasDatepicker").datepicker("option", "disabled", false);
                        }
                        break;
                    case UIFT.ReplyKeyEnum.DropdownList:
                    case UIFT.ReplyKeyEnum.Checkbox:
                    case UIFT.ReplyKeyEnum.RadiobuttonList:
                    case UIFT.ReplyKeyEnum.CheckboxList:
                    case UIFT.ReplyKeyEnum.Button:
                        $('.odpovedContainer', $this).find("input, select").not(".prevented").removeAttr("disabled");
                        break;
                    case UIFT.ReplyKeyEnum.SummaryOverview:
                        $('.odpovedContainer input', $this).removeAttr("disabled");
                        $(".summary3", $this).show();
                        break;
                    case UIFT.ReplyKeyEnum.HtmlEditor:
                        CKEDITOR.instances["Answer_" + data.f19id].setReadOnly(false);
                        break;
                }

                /* set visual style */
                $this.removeClass("readOnly").find(".clearAnswer").show();
            }
        }

        $this = data = null;
    };
})(jQuery);
