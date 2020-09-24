window.Harpocrates = window.Harpocrates || {};

window.Harpocrates.home = (function ($, data, enums, undefined) {

    function _createViewModel() {

        function featuredItem(item, template) {
            var self = this;

            self.template = ko.observable(template);
            self.item = ko.observable(item);
        }

        var vm = {
        };

        return vm;
    }

    return {
        createViewModel: _createViewModel
    };
})(window.jQuery, window.Harpocrates.viewModels, window.Harpocrates.enums);