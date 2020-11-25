(function ($) {
    $.fn.initUpload = function () {
        /* jiz existuji uploady - moznost Delete */
        $(this).parent().find(".btnDelete").click(_deleteUploadedFile);
        
        $(this).click(function () {
            var dataAtt = $(this).data("upload"),
                $input = $('<input type="file" />'),
                f19id = $(this).parents(".otazkaContainer").data("question").f19id,
                maxFiles = parseInt($(this).parent().find(".attachments").length);
            
            /* limit mnozstvi souboru */
            if (maxFiles >= dataAtt.maxFiles && maxFiles > 0) {
                alert(UIFT.tra("Dosáhli jste maximálního možného množství přiložených souborů k této otázce!"));
                return false;
            }

            /* allow multiple files to be selected? */
            if (dataAtt.multiple) {
                $input.attr("multiple", true);
            }
            /* set file extension filter */
            if (dataAtt.extensions != "") {
                $input.attr("accept", dataAtt.extensions);
            }

            $input.fileupload({
                url: "/Files/Save/" + f19id,
                dataType: 'html',
                singleFileUploads: true,
                add: function (e, data) {
                    var fileName = data.files[0].name,
                        errors = [];
                    
                    /* check file extension */
                    if (this.accept != "") {
                        var extensionsArray = this.accept.replace(/\./g, "").split(",");
                        var patt = new RegExp(".*\.(" + extensionsArray.join("|") + ")$", "i");

                        if (!patt.test(fileName)) {
                            errors.push(fileName + UIFT.tra("Soubor nemá požadovaný formát!"));
                        }
                    }

                    /* check file size */
                    
                    if (data.originalFiles[0]['size']) {
                        var max = UIFT.maxUploadFileSize;
                        if (!isNaN(parseInt(dataAtt.maxSize))) {
                            max = parseInt(dataAtt.maxSize);
                        }
                        
                        if (data.originalFiles[0]['size'] > max) {
                            errors.push(UIFT.tra("Soubor je příliš velký, maximální povolená velikost je (kB): ") + parseInt(max / 1000));
                        }
                    }

                    /* zobrazeni chybove hlasky */
                    if (errors.length > 0) {
                        var divId = Math.random().toString(36).slice(2);
                        $(this).after('<div class="uploadValidation" id="' + divId + '">' + errors[0] + '</div>');

                        /* remove fail info after some timeout */
                        window.setTimeout(function () {
                            $("#" + divId).slideUp("fast");
                        }, 6000, divId);
                        return false;
                    }

                    /* everything is ok */
                    data.context = $('<div class="uploadProgress"><span><span class="uploadProgress2"></span><i>Uploading ' + fileName + '</i></span></div>').insertAfter(this);

                    data.submit();
                },
                /* callback pro progressbar */
                progress: function (e, data) {
                    var progress = parseFloat(data.loaded / data.total),
                        w = data.context.children("span").width();
                    data.context.find(".uploadProgress2").width(parseInt(w * progress, 10) + "px");
                },
                /* failed upload */
                fail: function (e, data) {
                    data.context.replaceWith('<div class="uploadValidation">' + UIFT.tra("Nahrávaný soubor překračuje povolenou velikost 42MB.") + '</div>');
                },
                /* upload single souboru uspesne dokoncen */
                done: function (e, data) {
                    /* zasranej IE8 neumi poradne json */
                    var idx1 = data.result.indexOf("]");
                    var html = data.result.substr(idx1 + 1);
                    data.result = JSON.parse(data.result.substr(0, idx1 + 1));
                    data.result[0].html = html;

                    if (data.result[0].success) {
                        $html = $(data.result[0].html);
                        $html.find(".btnDelete").click(_deleteUploadedFile);
                        
                        data.context.parent().find(":file").remove();
                        data.context.replaceWith($html);

                        $html = null;
                    } else {
                        data.context.replaceWith('<div class="uploadValidation">' + UIFT.tra("Soubor se nepodařilo nahrát: ") + data.result[0].message + '</div>');

                        /* zobrazit upload button */
                        $("#Answer_" + data.result[0].f19id).show();
                    }
                }
            });

            /* vytvorit file upload button a automaticky na nej kliknout - zobrazi se file dialog */
            /* pro zkurvenej IE8,9 tam nemuze byt click, protoze ta jebka nepodporuje programaticke kliknuti na file input */
            if ($("html").hasClass("ie8")) {
                if ($("#uploadArea :file").length === 0)
                    $input.appendTo($(this).siblings("form")).show();
            } else {
                $input.appendTo($(this).siblings("form")).click();
            }
        });

        $input = null;
    };

    /* delete previously uploaded file */
    function _deleteUploadedFile() {
        if (!confirm(UIFT.tra("Opravdu si přejete smazat tento soubor?"))) {
            return false;
        }
        var guid = $(this).parent().data("guid");

        /* odstranit soubor z db */
        $.post("/Files/Delete", { id: guid });

        /* odstranit radek z html */
        $(this).parent().slideUp("normal", function () {
            $(this).remove();
        });
    };
})(jQuery);
