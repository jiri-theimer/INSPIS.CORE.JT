var _last_clicker;

(function ($, window) {
    var menus = {};

    $.fn.contextMenu = function (settings) {
        var $menu = $(settings.menuSelector);
        var clicker = settings.menuClicker;
        var menuEntity = settings.menuEntity;
        var menuPid = settings.menuPid;
        var menuFlag = settings.menuFlag;
        var menuLoadByServer = settings.menuLoadByServer;

        $menu.data("menuSelector", settings.menuSelector);
        if ($menu.length === 0) return;
        
        menus[settings.menuSelector] = { $menu: $menu, settings: settings };
        
        //make sure menu closes on any click
        $(document).click(function (e) {            
            if (e.target.id === "divZoomHeader") return;
            hideAll();
        });
        $(document).on("click", function (e) {
            
            if (e.target.id === "divZoomHeader") return;

            var $ul = $(e.target).closest("ul");
            if ($ul.length === 0 || !$ul.data("menuSelector")) {
                hideAll();

                
                var b = false;
                if (e.target === clicker) b = true;
                if (b === false && (e.target.parentNode)) {
                    if (e.target.parentNode === clicker) b = true;
                }

                if (b === true) {        //rovnou při úvodní incializaci vyvolat click a zobrazit menu                       
                    _last_clicker = clicker;
                    return handler_show_menu(e, settings.menuSelector);

                }
            }
        });
        $(document).on("keydown", function (e) {
            if (e.keyCode === 27) {
                hideAll();  //ESCAPE klávesa

            }
            
        });

        

        // Open context menu
        (function (element, menuSelector) {
            element.on("click", function (e) {
                
                // return native menu if pressing control
                if (clicker === _last_clicker) {
                    var m = getMenu(menuSelector);
                    if (m.$menu.css("display") === "block") {
                        hideAll();                        
                        return false;
                    }
                }
                _last_clicker = clicker;
                
                return handler_show_menu(e, menuSelector);

            });
        })($(this), settings.menuSelector);

        function handler_show_menu(e, menuSelector) {            
            if (e.ctrlKey) return;            
            hideAll();
            var menu = getMenu(menuSelector);
            if (menuLoadByServer === false) {
                return false;   //statické menu
            }
            $.post("/Menu/ContextMenu", { entity: menuEntity, pid: menuPid, flag: menuFlag }, function (data) {
                $(menuSelector).html(data);

                //až nyní je menu stažené ze serveru
                menu.$menu
                    .data("invokedOn", $(e.target))
                    .show()
                    .css({
                        position: "absolute",
                        left: getMenuPosition(e.clientX, "width", "scrollLeft"),
                        top: getMenuPosition(e.clientY, "height", "scrollTop")
                    })
                    .off('click');

                $('.cm_submenu').each(function () {
                    if ($(this).height() + $(this).offset().top > $(window).height()) {
                        $(this).css("margin-top", -10+$(window).height() - ($(this).height() + $(this).offset().top));
                    }
                    
                });
                
               
                callOnMenuShow(menu);
                return false;
            });
            

            
        }

        function getMenu(menuSelector) {
            var menu = null;
            $.each(menus, function (i_menuSelector, i_menu) {
                if (i_menuSelector === menuSelector) {
                    menu = i_menu
                    return false;
                }
            });
            return menu;
        }
        function hideAll() {            
            $.each(menus, function (menuSelector, menu) {
                menu.$menu.hide();
                callOnMenuHide(menu);
            });
            if (self === top) return;
            //if (window.parent.$("#site_loading1").length > 0) {
            //    window.parent.$("#site_loading1").click();
            //}




        }

        function callOnMenuShow(menu) {
            var $invokedOn = menu.$menu.data("invokedOn");
            if ($invokedOn && menu.settings.onMenuShow) {
                menu.settings.onMenuShow.call(this, $invokedOn);
            }
        }
        function callOnMenuHide(menu) {
            var $invokedOn = menu.$menu.data("invokedOn");
            menu.$menu.data("invokedOn", null);
            if ($invokedOn && menu.settings.onMenuHide) {
                menu.settings.onMenuHide.call(this, $invokedOn);
            }
        }

        function getMenuPosition(mouse, direction, scrollDir) {
            var win = $(window)[direction](),
                scroll = $(window)[scrollDir](),                
                menu = $(settings.menuSelector)[direction](),   //výška/šířka menu
                position = mouse + scroll;                      //souřadnice

            
            if (direction === "width") {
                position = position + 20;
                if (_device.type === "Phone") {
                    position = 0;
                }
                
            }
           
            
            if (direction === "height" && menu + position > $(window).height()) {
                position = $(window).height() - menu - 10;
                if (_device.type === "Phone") {
                    position = 0;
                }
                
                return position;
        
            }

            if (menu === 0 || menu === win) {                
                menu = 300;
            }
            // opening menu would pass the side of the page
            if (mouse + menu > win && menu < mouse) {
                position -= menu;
            }



            return position;
        }
        return this;
    };
})(jQuery, window);