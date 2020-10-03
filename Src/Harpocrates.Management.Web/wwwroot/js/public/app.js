window.Harpocrates = window.Harpocrates || {};

window.Harpocrates.app = (function ($, data, enums, common, security, loader, undefined) {

    function _createViewModel() {

        var structures = {
            section: function (name, template, parent, load) {
                var self = this;

                self.template = ko.observable(template);
                self.name = ko.observable(name);

                self.data = new data.common.entities.collection();;

                self.actions = {
                    select: function () {
                        if (!parent) return;
                        if (!parent.selected);

                        parent.selected(self);
                    },
                    refresh: function () {
                        if (load) {

                            self.data.items.removeAll();

                            load(function (data) {
                                if (!data) return;

                                for (var i = 0; i < data.length; i++) {
                                    self.data.items.push(data[i]);
                                }


                            }, function (err) {
                                console.log("Error: " + err.statusText);
                            });
                        }
                    }
                };

            },
            page: function (name, template) {
                var self = this;

                self.template = ko.observable(template);
                self.name = ko.observable(name);

                self.sections = new data.common.entities.collection();
            }
        };

        var builder = {
            dashboard: function () {
                var page = new structures.page("dashboard", "template-body-dashboard");

                return page;
            },
            config: function () {
                var page = new structures.page("config", "template-body-config");

                var section = new structures.section("secrets", "template-body-config-secrets", page.sections, loader.secret.getAll);
                section.actions.refresh();
                page.sections.items.push(section);
                page.sections.selected(section);

                section = new structures.section("services", "template-body-config-services", page.sections, loader.service.getAll);
                section.actions.refresh();
                page.sections.items.push(section);

                section = new structures.section("policies", "template-body-config-policies", page.sections, loader.policy.getAll);
                section.actions.refresh();
                page.sections.items.push(section);

                return page;
            },
            settings: function () {
                var page = new structures.page("settings", "template-body-settings");

                return page;
            }
        };


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

        var knownPages = {
            dashboard: builder.dashboard(),
            config: builder.config(),
            settings: builder.settings()
        };

        vm.body.pages.items.push(knownPages.dashboard);
        vm.body.pages.selected(knownPages.dashboard);

        vm.body.pages.items.push(knownPages.config);

        vm.body.pages.items.push(knownPages.settings);


        function onMenuItemChanged(active) {
            if (!active) return;

            var page = null;

            switch (active.tag().toLowerCase()) {
                case "dashboard":
                    page = knownPages.dashboard;
                    break;
                case "config":
                    page = knownPages.config;
                    break;
                case "settings":
                    page = knownPages.settings;
                    break;
            }

            if (null == page) return;

            vm.body.pages.selected(page);
        }

        for (var i = 0; i < vm.nav.menu.items.right().length; i++) {
            //vm.nav.menu.items.right()[i].isActive.subscribe(onMenuItemChanged);
            vm.nav.menu.items.right()[i].events.selected = onMenuItemChanged;
        }

        for (var i = 0; i < vm.nav.menu.items.left().length; i++) {
            //vm.nav.menu.items.left()[i].isActive.subscribe(onMenuItemChanged);
            vm.nav.menu.items.left()[i].events.selected = onMenuItemChanged;
        }

        return vm;
    }

    return {
        createViewModel: _createViewModel
    };
})(window.jQuery, window.Harpocrates.viewModels, window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.security, window.Harpocrates.loader);