// common initialization
$(function () {
    $('#searchButton').click(function (e) {
        e.preventDefault();
        $(this).parents('form').submit();
    });
});

// search suggestions
(function ($) {
    var currentId = 0;
    var selectedIndex = -1;
    var cache = new Array();

    $.searchSuggestions = function (element, suggestUrl, searchUrl) {
        var $container;
        var containerFocus = false;
        var visible = false;
        var timeout = null;

        createContainer();
        cache[''] = '[]';

        element.keyup(function (e) {
            if (e.which == 27) {
                e.preventDefault();
                return;
            }

            show();

            clearTimeout(timeout);
            timeout = setTimeout(function () {
                getSuggestions(element.val())
            }, 250);
        });

        function getSuggestions(q) {
            if (q.length < 2) {
                return;
            }

            var result = cache[q];
            if (result == null) {
                var requestUrl = encodeURI(suggestUrl + '?query=' + q);
                $.ajax({
                    type: 'GET',
                    dataType: 'html',
                    url: requestUrl,
                    success: function (result) {
                        cache[q] = result;
                        handleResult(result);
                    }
                });
            } else {
                handleResult(result);
            }
        }

        function handleResult(result) {
            var $list = $('ul:eq(0)', $container);
            $list.html('');
            var suggestions = $.parseJSON(result);
            if (suggestions.length == 0) {
                hide();
            } else {
                for (var i = 0; i < suggestions.length; i++) {
                    var suggestion = suggestions[i];
                    $list.append(getSearchItem(suggestion.Title));
                    $('li:eq(' + i + ') a', $list).text(suggestion.Title);
                }
                show();
            }
        }

        function show() {
            if (!visible) {
                if (element.val().length < 2 || $('ul:eq(0) li').length == 0) {
                    return;
                }

                selectedIndex = -1;
                $(document).bind('click', clickHandler);
                $(document).bind('keydown', keyHandler);
                visible = true;
                containerFocus = false;
                $container.show();
            }
        }

        function hide() {
            $(document).unbind('click', clickHandler);
            $(document).unbind('keydown', keyHandler);
            visible = false;
            $container.hide();
        }

        function clickHandler(e) {
            $target = $(e.target);
            if ($target.parents('.searchSuggestions').length == 0) {
                hide();
            }
        }

        function keyHandler(e) {
            var update = false;
            switch (e.which) {
                case 9: //tab
                    //Is this the search input
                    if (element.context == e.target) {
                        hide();
                    } else if (containerFocus) {
                        e.preventDefault();
                        hide();
                        element.next().focus();
                    }
                    return;
                case 27: //escape
                    e.preventDefault();
                    hide();
                    element.focus();
                    return;
                case 38: // up arrow
                    selectedIndex--;
                    update = true;
                    break;
                case 40: // down arrow
                    selectedIndex++;
                    update = true;
                    break;
            }

            if (update) {
                e.preventDefault();
                containerFocus = true;
                if (selectedIndex < 0) {
                    selectedIndex = -1;
                    element.focus();
                } else {
                    var $items = $('li a', $container);
                    if (selectedIndex >= $items.length) {
                        selectedIndex = $items.length - 1;
                    }

                    $('li a', $container).eq(selectedIndex).focus();
                }
            }
        }

        function createContainer() {
            var left = element.position().left + 'px';
            var top = (element.position().top + element.outerHeight()) + 'px';
            var id = 'searchSuggestions' + currentId++;

            var container = '<div id="' + id + '" class="searchSuggestions" style="left: ' + left + '; top: ' + top + ';"><ul></ul></div>'
            element.parents('form').append(container);
            $container = $('#' + id);
            var width = element.outerWidth() - parseInt($container.css('border-left-width')) - parseInt($container.css('border-right-width'));
            $container.width(width);

            $container.focusin(function () {
                containerFocus = true;
            });

            $container.focusout(function (e) {
                if (containerFocus && $(e.target).parents('.searchSuggestions').length == 0) {
                    hide();
                }
            });

        }

        function getSearchItem(title) {
            return '<li><a href="' + searchUrl + '?q=' + encodeURI(title) + '"></a></li>'
        }
    }

    $.fn.searchSuggestions = function (suggestUrl, searchUrl) {
        return this.each(function () {
            new $.searchSuggestions($(this), suggestUrl, searchUrl);
        });
    };

})(jQuery);

// modal dialog
(function ($) {
    $.modalDialog = function (element) {
        var scrollLeft;
        var scrollTop;
        element.css('position', 'fixed');
        element.css('z-index', '203');
        element.bind('show', show);
        element.bind('hide', hide);
        element.wrap('<div class="modal"/>');

        function show() {
            scrollLeft = $(document).scrollLeft();
            scrollTop = $(document).scrollTop();
            $(window).bind('resize', positionElement);
            $(document).bind('focusin', focusHandler);
            $(document).bind('keydown', keyHandler);
            createBackground();
            positionElement();    
            $(element).parent().show();
            element.show();
            $(':input:enabled[type!="hidden"]', element).first().focus();
        }

        function hide() {
            removeBackground();
            $(element).parent().hide();
            element.hide();
            $(document).unbind('keydown', keyHandler);
            $(document).unbind('focusin', focusHandler);
            $(window).unbind('resize', positionElement);
            $(document).scrollLeft(scrollLeft);
            $(document).scrollTop(scrollTop);
        }

        function positionElement() {
            var top = ($(window).height() - element.height()) / 2
            var left = ($(window).width() - element.width()) / 2
            element.css('top', top + 'px');
            element.css('left', left + 'px');
        }

        function createBackground() {
            element.after('<div class="background"/>');
            $(".background").click(function (e) {
                e.preventDefault();
                $('.popup').trigger('hide');
            });
        }

        function removeBackground() {
            element.next('.background').remove();
        }

        function keyHandler(e) {
            // escape
            if(e.which == 27) { 
                    hide();
            }
        }
        
        function focusHandler(e) {
            if (!$(e.target).parents('.modal').length) {
                e.preventDefault();
                if (e.shiftKey) {
                    $(':input:enabled[type!="hidden"], a', element).last().focus();
                } else {
                    $(':input:enabled[type!="hidden"], a', element).first().focus();
                }
            }
        }
    }

    $.fn.modalDialog = function () {
        return this.each(function () {
            new $.modalDialog($(this));
        });
    };
})(jQuery);

$(function () {
    $(".popup .closePopup").click(function (e) {
        e.preventDefault();
        $(e.target).parents('.popup').trigger('hide');
    });
});

//User menu
$(function () {
    var $container = $('#signout');
    var $link = $('#signout > a');
    var $menu = $('#signout > ul');
    var $menuItems = $('#signout a');
    var visible = false;
    var originalContainerPos = null;
    var linkPaddingRight = parseInt($link.css('padding-right'));
    var selectedIndex = 0;

    if ($container.length == 0) {
        return;
    }

    originalContainerPos = $container.position().left;
    $container.css('left', originalContainerPos);

    $menu.bind('show', function () {
        visible = true;
        selectedIndex = 0;
        $container.addClass('visible');
        $(document).bind('click', clickHandler);
        $(document).bind('keydown', keyHandler);

        // Right Align name and left align menu items
        var $widest = $link;
        var leftPos = $container.position().left;
        $menuItems.each(function () {
            if ($(this).outerWidth() > $widest.outerWidth())
                $widest = $(this);
        });
        var containerOffset = $widest.outerWidth() - $link.outerWidth();
        leftPos = originalContainerPos - containerOffset;
        $link.css('padding-left', containerOffset);
        $container.css('left', leftPos);

        // Add 1 pixel to the right-side padding on the sign out link to fix wiggling effect of triangle graphic in IE 9
        if ($.browser.msie && $.browser.version == 9.0) {
            $link.css('padding-right', (linkPaddingRight + 1) + 'px');
        }
    });

    $menu.bind('hide', function () {
        visible = false;
        $container.removeClass('visible');
        $(document).unbind('click', clickHandler);
        $(document).unbind('keydown', keyHandler);

        // Reset name positioning after left aligned menu items are hidden
        $link.css('padding-left', 0);
        $container.css('left', originalContainerPos);

        // Reset padding on sign out link
        if ($.browser.msie && $.browser.version == 9.0) {
            $link.css('padding-right', linkPaddingRight + 'px');
        }
    });

    $container.bind('focusin mouseenter', function (e) {
        e.preventDefault();
        if (!visible) {
            $menu.trigger('show');
        }
    });

    $container.bind('blur mouseleave', function (e) {
        e.preventDefault();
        if (visible) {
            $menu.trigger('hide');
        }
    });

    function clickHandler(e) {
        if (!$(e.target).parents('#signout').length) {
            $menu.trigger('hide');
        }
    }

    function keyHandler(e) {
        var update = false;
        var tabbed = false;
        switch (e.which) {
            case 27: //escape
                e.preventDefault();
                $menu.trigger('hide');
                return;
            case 9: // tab
                if (e.shiftKey) {
                    selectedIndex--;
                }
                else {
                    selectedIndex++;
                }

                update = true;
                tabbed = true;
                break;
            case 38: // up arrow
                selectedIndex--;
                update = true;
                break;
            case 40: // down arrow
                selectedIndex++;
                update = true;
                break;
        }

        if (update) {
            if (selectedIndex < 0) {
                if (tabbed) {
                    $menu.trigger('hide');
                    return;
                }
                else {
                    selectedIndex = 0;
                }
            } else {
                if (selectedIndex >= $menuItems.length) {
                    if (tabbed) {
                        $menu.trigger('hide');
                        return;
                    }
                    else {
                        selectedIndex = $menuItems.length - 1;
                    }
                }
            }

            e.preventDefault();
            $menuItems[selectedIndex].focus();
        }
    }
});

/**
* jQuery.placeholder - Placeholder plugin for input fields
* Usage: 
*   Create a text input with an attr placeholderText that contains the text to put in the textbox
*   Call $("#id").placeholder() 
*   This adds the on focus and on blur functions that triggers the value to be in the textbox. 
*   It will also add a class "placeholder" to the input for styling purposes
*
* Override:
*   If you prefer to pass the text in via the creation call, you just need a default text input and call
*   $("#id").placeholder("overriding text");
*   That will replace or add the attr for the input text
*
**/
(function ($) {
    $.fn.placeholder = function (overrideText) {
        var text;
        // Check for override
        if (overrideText) {
            text = overrideText;

            // Make sure to set the attribute too
            $(this).attr("placeholderText", text);
        }
        else if ($(this).attr("placeholderText")) {
            text = $(this).attr("placeholderText");
        }
        else {
            // If no overriding text or attribute not defined, do nothing to the element
            return $(this);
        }

        // Initialize value and styling
        $(this).val(text);
        $(this).addClass("placeholder");

        return this.focus(function () {
            if ($.trim($(this).val()) === $(this).attr("placeholderText"))
                $(this).removeClass("placeholder").val("");
        }).blur(function () {
            if ($.trim($(this).val()) === "")
                $(this).addClass("placeholder").val($(this).attr("placeholderText"));
        });
    };
})(jQuery);
