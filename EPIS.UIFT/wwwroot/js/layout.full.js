$(window).load((function(){CKEDITOR.disableAutoInline=!0,UIFT.Controls.scrollIndicator=$("#scrollIndicator"),UIFT.Controls.contentContainer=$("#contentContainerInner"),UIFT.Controls.segmentsTree=$("#segmentsTree"),UIFT.Controls.window=$(window);var o=navigator.userAgent.toLowerCase();if(o.indexOf("msie 6.0")>=0||o.indexOf("msie 7.0")>=0){var t=$('<div id="browserWarning">Prohlížeč Internet Explorer verze 7 a nižší není v systému EPIS podporován!</div>').prependTo("body"),e=parseInt($("body").width()/2)-parseInt(t.width()/2);t.css({left:e+"px"}).slideDown("slow"),t=null}$("#mobileMenu").mobileMenu(),$("#headerWF_move").click((function(o){o.preventDefault();var t='<div id="workflowWindow"><iframe src="'+this.href+'" style="width:620px;height:580px;" /></div>';$(t).dialog({modal:!0,width:646,height:640,title:UIFT.tra("Posunout / doplnit"),close:function(o,t){$(this).dialog("destroy").remove()}})})),$("#aExportWord").click((function(o){o.preventDefault(),0==$("#exportToWordTemplate").length&&$.post($(this).attr("href"),(function(o){$(o).dialog({modal:!0,width:400,height:250,title:"Export",close:function(o,t){$(this).dialog("destroy").remove()},buttons:[{text:UIFT.tra("Včetně odpovědí"),click:function(){$(this).loading(),location.href=$(this).data("url")+"?showAnswers=true&templateId="+$("#exportToWordTemplate").val(),$(this).dialog("close")}},{text:"Prázdný dotazník",click:function(){$(this).loading(),location.href=$(this).data("url")+"?showAnswers=false&templateId="+$("#exportToWordTemplate").val(),$(this).dialog("close")}}]})}))})),UIFT.Controls.scrollIndicator.length>0&&UIFT.Controls.window.scroll((function(){UIFT.GetScrollTop()>1?UIFT.Controls.scrollIndicator.fadeIn(300):UIFT.Controls.scrollIndicator.fadeOut(300)})),UIFT.Controls.segmentsTree.length>0&&(UIFT.adjustSegmentsTree(),UIFT.Controls.segmentsTree.find("a").click((function(o){o.preventDefault(),$("html").hasClass("menuOpened")&&$("#menuOverlay").click(),UIFT.Controls.contentContainer.loading(),window.setTimeout((function(o){for(var t=CKEDITOR.instances.length-1;t>=0;t--)CKEDITOR.instances[t].destroy();$(".qControl_FileUpload .odpovedContainer input").fileupload("destroy"),UIFT.LoadSegment($(o).parent().data("id")),o=null}),200,this)})),UIFT.Controls.contentContainer.loading(),UIFT.LoadSegment(UIFT.Controls.segmentsTree.find("li.sel").data("id"),UIFT.ActualQuestion))}));