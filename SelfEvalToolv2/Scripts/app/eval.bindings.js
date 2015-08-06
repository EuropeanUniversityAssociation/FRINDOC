// Hooks up a form to jQuery Validation
ko.bindingHandlers.validate = {
    init: function (elem, valueAccessor) {
        $(elem).validate();
    }
};

// Controls whether or not the text in a textbox is selected based on a model property
ko.bindingHandlers.selected = {
    init: function (elem, valueAccessor) {
        $(elem).blur(function () {
            var boundProperty = valueAccessor();
            if (ko.isWriteableObservable(boundProperty)) {
                boundProperty(false);
            }
        });
    },
    update: function (elem, valueAccessor) {
        var shouldBeSelected = ko.utils.unwrapObservable(valueAccessor());
        if (shouldBeSelected) {
            $(elem).select();
        }
    }
};

// Makes a textbox lose focus if you press "enter"
ko.bindingHandlers.blurOnEnter = {
    init: function (elem, valueAccessor) {
        $(elem).keypress(function (evt) {
            if (evt.keyCode === 13 /* enter */) {
                evt.preventDefault();
                $(elem).triggerHandler("change");
                $(elem).blur();
            }
        });
    }
};

// Simulates HTML5-style placeholders on older browsers
ko.bindingHandlers.placeholder = {
    init: function (elem, valueAccessor) {
        var placeholderText = ko.utils.unwrapObservable(valueAccessor()),
            input = $(elem);

        input.attr('placeholder', placeholderText);

        // For older browsers, manually implement placeholder behaviors
        if (!Modernizr.input.placeholder) {
            input.focus(function () {
                if (input.val() === placeholderText) {
                    input.val('');
                    input.removeClass('placeholder');
                }
            }).blur(function () {
                setTimeout(function () {
                    if (input.val() === '' || input.val() === placeholderText) {
                        input.addClass('placeholder');
                        input.val(placeholderText);
                    }
                }, 0);
            }).blur();

            input.parents('form').submit(function () {
                if (input.val() === placeholderText) {
                    input.val('');
                }
            });
        }
    }
};

ko.bindingHandlers.hidden = {
    update: function (element, valueAccessor) {
        ko.bindingHandlers.visible.update(element, function () { return !ko.utils.unwrapObservable(valueAccessor()); });
    }
};

ko.bindingHandlers.toggleVisible = {
    init: function (element, valueAccessor) {
        $(element)
            .css('cursor', 'pointer')
            .click(function () {
                $(element).siblings('ul').slideToggle('slow', function () {
                    $(this).parent().toggleClass('collapsed', !$(this).is(':visible'));
                });
            });
    }
};

ko.bindingHandlers.textAreaAutoScroll = {
    init: function (element, valueAccessor) {
        $(element).keyup(function (e) {
            //  the following will help the text expand as typing takes place
            while ($(this).outerHeight() < this.scrollHeight + parseFloat($(this).css("borderTopWidth")) + parseFloat($(this).css("borderBottomWidth"))) {
                $(this).height($(this).height() + 1);
            };
        });
    }
};

ko.bindingHandlers.clickToEdit = {
    init: function (element, valueAccessor) {
        var observable = valueAccessor(),
            link = document.createElement("span"),
            input = document.createElement("input")

        // set a css classname to allow further styling
        input.className = "clicktoedit";

        element.appendChild(link);
        element.appendChild(input);

        observable.editing = ko.observable(false);

        ko.applyBindingsToNode(link, {
            text: observable,
            hidden: observable.editing,
            click: observable.editing.bind(null, true)
        });

        ko.applyBindingsToNode(input, {
            value: observable,
            visible: observable.editing,
            hasfocus: observable.editing,
            event: {
                keyup: function (data, event) {
                    //if user hits enter, set editing to false, which makes field lose focus
                    if (event.keyCode === 13) {
                        observable.editing(false);
                        return false;
                    }
                        //if user hits escape, push the current observable value back to the field, then set editing to false
                    else if (event.keyCode === 27) {
                        observable.valueHasMutated();
                        observable.editing(false);
                        return false;
                    }
                }
            }
        });
    }
};

// Here's a custom Knockout binding that makes elements shown/hidden via jQuery's fadeIn()/fadeOut() methods
// Could be stored in a separate utility library
ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        // Initially set the element to be instantly visible/hidden depending on the value
        var value = valueAccessor();
        $(element).toggle(ko.unwrap(value)); // Use "unwrapObservable" so we can handle values that may or may not be observable
    },
    update: function (element, valueAccessor) {
        // Whenever the value subsequently changes, slowly fade the element in or out
        var value = valueAccessor();
        ko.unwrap(value) ? $(element).fadeIn() : $(element).fadeOut();
    }
};

ko.bindingHandlers.starRating = {
    init: function (element, valueAccessor, x, y, z) {
        $(element).addClass("starRating");
        for (var i = 0; i < 6; i++)
            $("<span>").addClass('star').appendTo(element);

        var legend = $("<div>").addClass('star-legend');
        for (var i = 0; i < 6; i++)
            $("<span>").html(i).appendTo(legend);
        legend.insertAfter($(element));
    },
    update: function (element, valueAccessor) {
        // Give the first x stars the "chosen" class, where x <= rating
        var observable = valueAccessor();
        $("span", element).each(function (index) {
            $(this).toggleClass("chosen", index <= observable());
        });
    }
};

ko.bindingHandlers.editStarRating = {
    init: function (element, valueAccessor, x, y, z) {
        $(element).addClass("starRating");
        for (var i = 0; i < 6; i++)
            $("<span>").addClass('star').appendTo(element);

        var legend = $("<div>").addClass('star-legend');
        for (var i = 0; i < 6; i++)
            $("<span>").html(i).appendTo(legend);
        legend.insertAfter($(element));

        // Handle mouse events on the stars
        $("span", element).each(function (index) {
            $(this).hover(
                function () { $(this).prevAll().add(this).addClass("hoverChosen") },
                function () { $(this).prevAll().add(this).removeClass("hoverChosen") }
            ).click(function () {
                var observable = valueAccessor();  // Get the associated observable
                observable(index);               // Write the new rating to it
            });
        });
    },
    update: function (element, valueAccessor) {
        // Give the first x stars the "chosen" class, where x <= rating
        var observable = valueAccessor();
        $("span", element).each(function (index) {
            $(this).toggleClass("chosen", index <= observable());
        });
    }
};


ko.bindingHandlers.editSuperStarRating = {
    init: function (element, valueAccessor, x, y, z) {
        $(element).addClass("superstarRating");
        for (var i = 1; i < 5; i++)
            $("<span>").addClass('superstar').appendTo(element);

        var legend = $("<div>").addClass('superstar-legend');
        for (var i = 1; i < 5; i++)
            $("<span>").html(i).appendTo(legend);
        legend.insertAfter($(element));

        // Handle mouse events on the stars
        $("span", element).each(function (index) {
            $(this).hover(
                function () { $(this).prevAll().add(this).addClass("hoverChosen") },
                function () { $(this).prevAll().add(this).removeClass("hoverChosen") }
            ).click(function () {
                var observable = valueAccessor();  // Get the associated observable
                observable(index);               // Write the new rating to it
            });
        });
    },
    update: function (element, valueAccessor) {
        // Give the first x stars the "chosen" class, where x <= rating
        var observable = valueAccessor();
        $("span", element).each(function (index) {
            $(this).toggleClass("chosen", index <= observable());
        });
    }
};