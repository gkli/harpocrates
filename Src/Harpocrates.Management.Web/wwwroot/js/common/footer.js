
window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.ui = window.Harpocrates.ui || {};
window.Harpocrates.ui.footer = (function ($, enums, common, undefined) {
    if (!common) throw "common is required";

    common.templates.load().done(function () {
        var vm = {
            year: new Date().getFullYear()
        };

        ko.applyBindings(vm, $('div.template-footer')[0]);
    });

})(window.jQuery, window.Harpocrates.enums, window.Harpocrates.common);