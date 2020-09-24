window.Harpocrates = window.Harpocrates || {};

window.Harpocrates.app = (function ($, data, enums, common, security, undefined) {

    function _createViewModel() {

        function featuredItem(item, template) {
            var self = this;

            self.template = ko.observable(template);
            self.item = ko.observable(item);
        }

        var navVm = window.Harpocrates.ui.menu.createNavViewModel();

        var vm = {
            nav: {
                menu: navVm.menu,
                sidebar: navVm.sidebar,
                footer: window.Harpocrates.ui.footer.createFooterViewModel()
            },
            body: {
                pages: new data.common.entities.collection()
            }
        };

        return vm;
    }

    return {
        createViewModel: _createViewModel
    };
})(window.jQuery, window.Harpocrates.viewModels, window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.security);