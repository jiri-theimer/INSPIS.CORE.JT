(function ($) {
    $.fn.initCheckboxList = function () {
        /* inicializace otazky */
        var currentTagName = this.prop("tagName").toLowerCase(),
            $allColl = this.find(currentTagName == "tr" ? ":checkbox" : ".odpovedContainer :checkbox"),
            $coll = $allColl.filter(":checked");
        
        /* pokud je zaskrtnut alespon jeden checkbox, zkontroluj zda nema nastavene max. mnozstvi odpovedi */
        if ($coll.length > 0) {
            var max = parseInt($coll.eq(0).data("answer").prevent);
            if (isNaN(max)) max = 0;
            
            /* pokud je zaskrtnuto max. mnozstvi odpovedi, znepristupni ostatni checkboxy */
            if ($coll.length == max) {
                $allColl.not(":checked")
                    .attr("disabled", "disabled")
                    .addClass("prevented");
            }
        }

        /* pri kliknuti na checkbox */
        $allColl.change(function (e) {
            var max = parseInt($(this).data("answer").prevent),
                $coll = $(this).parent().siblings().children(":checkbox");
            
            /* pokud ma checkbox nastaveny max. pocet odpovedi */
            if (max > 0) {
                if (this.checked) { /* checkbox zaskrtnut - zkontroluj, zda neni treba disablovat ostatni checkboxy */
                    var checked = $coll.filter(":checked").length + 1;
                    
                    /* pokud je zaskrtnutych vice checkboxu, nez je maximum - muze se stat v pripade isNegation */
                    if (max < checked) {
                        $coll.filter(":checked").not(this).each(function () {
                            $(this).prop("checked", false).change();
                        });
                    }

                    /* pokud jsme dosahli max. poctu zaskrtnutych checkboxu */
                    if (max <= checked) {
                        $coll.not(":checked")
                            .attr("disabled", "disabled")
                            .addClass("prevented");
                    }
                } else { /* checkbox odskrtnut - zapni ostatni checkboxy, pokud byly vypnute */
                    $coll
                        .removeAttr("disabled")
                        .removeClass("prevented");
                }
            }

            $allColl = $coll = null;
        });
    };
})(jQuery);