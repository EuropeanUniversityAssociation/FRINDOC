/*
Usage:  <textarea data-bind="inlineCkeditor: observable"></textarea>
*/

(function (ko, CKEDITOR) {
    ko.bindingHandlers.inlineCkeditor = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {

            element.editor = CKEDITOR.inline(element, {
                extraPlugins: 'autogrow',
                autoGrow_maxHeight: 800,
                // Remove the Resize plugin as it does not make sense to use it in conjunction with the AutoGrow plugin.
                removePlugins: 'resize'
            });
            element.editor.on('blur', function () {
                // retrieve current data from editor and process it with its dataprocessor
                var formattedDataForWysiWyg = element.editor.dataProcessor.toHtml(element.editor.getData());
                // "decode" content processed by ckeditor
                var sourceData = element.editor.dataProcessor.toDataFormat(formattedDataForWysiWyg);
                // set formatted data back
                element.editor.setData(sourceData);
                valueAccessor()(sourceData);
            });

            // Custom autogrow script
            element.editor.on('change', function () {
                var displayedHeight = $(".cke_textarea_inline", $(element).siblings()).height();
                if (displayedHeight < element.height)
                    element.editor.resize(element.width, height);
            });

            //handle destroying
            ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
                if (element.editor)
                    element.editor.destroy(true);
            });

            // Add Word html cleanup after each paste
            CKEDITOR.on('instanceReady', function (ev) {
                ev.editor.on('paste', function (evt) {
                    evt.data['html'] = '<!--class="Mso"-->' + evt.data['html'];
                }, null, null, 9);
            });
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
            //handle programmatic updates to the observable
            var value = ko.utils.unwrapObservable(valueAccessor()),
                existingEditor = element.editor;

            if (existingEditor) {
                if (value !== existingEditor.getData()) {
                    existingEditor.setData(value);
                }
            }
        }
    };

})(ko, CKEDITOR);