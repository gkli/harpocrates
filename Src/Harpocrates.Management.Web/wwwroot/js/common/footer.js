
window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.ui = window.Harpocrates.ui || {};
window.Harpocrates.ui.footer = (function ($, enums, common, undefined) {
    if (!common) throw "common is required";

    function _createViewModel() {
        var vm = {
            year: new Date().getFullYear()
        };

        return vm;
    }

    //common.templates.load().done(function () {
    //    var vm = {
    //        year: new Date().getFullYear()
    //    };

    //    ko.applyBindings(vm, $('div.template-footer')[0]);
    //});

    return {
        createFooterViewModel: _createViewModel
    };

})(window.jQuery, window.Harpocrates.enums, window.Harpocrates.common);