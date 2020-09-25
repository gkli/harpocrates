window.Harpocrates = window.Harpocrates || {};

window.Harpocrates.app = (function ($, data, enums, common, security, undefined) {

    function _createViewModel() {

        function featuredItem(item, template) {
            var self = this;

            self.template = ko.observable(template);
            self.item = ko.observable(item);
        }

        function page(name, template) {
            var self = this;

            self.template = ko.observable(template);
            self.name = ko.observable(name);
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



        var p = new page("dashboard", "template-body-dashboard");
        vm.body.pages.items.push(p);
        vm.body.pages.selected(p);

        p = new page("config", "template-body-config");
        vm.body.pages.items.push(p);

        p = new page("manage", "template-body-management");
        vm.body.pages.items.push(p);

        function onMenuItemChanged(newValue) {
            if (!newValue) return;
            var o = [];
        }

        for (var i = 0; i < vm.nav.menu.items.right().length; i++) {
            vm.nav.menu.items.right()[i].isActive.subscribe(onMenuItemChanged);
        }

        for (var i = 0; i < vm.nav.menu.items.left().length; i++) {
            vm.nav.menu.items.left()[i].isActive.subscribe(onMenuItemChanged);
        }

        return vm;
    }

    return {
        createViewModel: _createViewModel
    };
})(window.jQuery, window.Harpocrates.viewModels, window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.security);