ko.bindingHandlers.slideVisible = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        var options = valueAccessor() || {};
        var value = options.value,
            allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);
        var duration = ko.utils.unwrapObservable(options.duration) || 400;
        var direction = ko.utils.unwrapObservable(options.direction) || 'right';
        if (valueUnwrapped == true) {
            $(element).css({ position: 'relative', overflow: 'auto' }).show('slide', { direction: direction }, duration); // Make the element visible
        }
        else {
            //direction = (direction == 'left' ? 'right' : 'left');
            $(element).css({ position: 'absolute', overflow: 'hidden' }).hide();   // Make the element invisible
            //$(element).hide();
        }
    }
};