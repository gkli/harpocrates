window.Harpocrates = window.Harpocrates || {};

window.Harpocrates.app = (function ($, data, enums, common, security, loader, undefined) {

    function _createViewModel() {

        var structures = {
            editor: function (name, template) {
                var self = this;

                self.name = ko.observable(name);
                self.template = ko.observable(template);
            },
            section: function (name, template, editorTemplate, parent, load, dataCollection, extend) {
                var self = this;

                self.template = ko.observable(template);
                self.name = ko.observable(name);
                self.editor = ko.observable(new structures.editor("editor: " + name, editorTemplate));

                self.data = dataCollection; //new data.common.entities.collection();

                self.paginated = new data.common.entities.paginatedCollection(10);

                self.actions = {
                    select: function () {
                        if (!parent) return;
                        if (!parent.selected);

                        parent.selected(self);
                    },
                    refresh: function () {
                        if (load) {

                            //if (!masterDataCollection) masterDataCollection = new data.common.entities.collection();

                            self.data.loading(true);
                            self.data.items.removeAll();

                            //load shallow only...
                            load(function (data) {
                                if (!data) return;

                                for (var i = 0; i < data.length; i++) {
                                    if (extend) extend(data[i]);
                                    self.data.items.push(data[i]);
                                }

                                self.paginated.helper.paginate(self.data);

                                self.data.loading(false);

                            }, function (err) {
                                console.log("Error: " + err.statusText);
                                self.data.loading(false);
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

                var mainSection = new structures.section("main", "", "", page.sections,
                    function (oncomplete, onerror) {
                        var start = '01/01/1999';
                        var end = '';
                        loader.tracking.getAll(start, end, oncomplete, onerror)
                    }, new data.common.entities.collection(), function (transaction) {
                        var foo = '';
                    });
                page.sections.items.push(mainSection);
                page.sections.selected(mainSection);

                mainSection.actions.refresh();





                // building bar chart

                /*
                 
                  var data = {
            labels: ['Jan', 'Feb', 'Mar', 'Apr', 'Mai', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
            series: [
                [542, 443, 320, 780, 553, 453, 326, 434, 568, 610, 756, 895],
                [412, 243, 280, 580, 453, 353, 300, 364, 368, 410, 636, 695]
            ]
        };

        var options = {
            seriesBarDistance: 10,
            axisX: {
                showGrid: false
            },
            height: "245px"
        };

        var responsiveOptions = [
            ['screen and (max-width: 640px)', {
                seriesBarDistance: 5,
                axisX: {
                    labelInterpolationFnc: function(value) {
                        return value[0];
                    }
                }
            }]
        ];

        var chartActivity = Chartist.Bar('#chartActivity', data, options, responsiveOptions);                 
                 */

                /*
                 
               //  building pie chart

 var dataPreferences = {
            series: [
                [25, 30, 20, 25]
            ]
        };

        var optionsPreferences = {
            donut: true,
            donutWidth: 40,
            startAngle: 0,
            total: 100,
            showLabel: false,
            axisX: {
                showGrid: false
            }
        };

        Chartist.Pie('#chartPreferences', dataPreferences, optionsPreferences);

        Chartist.Pie('#chartPreferences', {
            labels: ['53%', '36%', '11%'],
            series: [53, 36, 11]
        });


                 
                 */


                return page;
            },
            config: function () {
                var page = new structures.page("config", "template-body-config");

                //resolve the following issue:
                // if services are refreshed after secrets are fetched, secrets that depend on the service(s) need to be updated to use refreshed service
                // same applies to service/policy relationship
                // currently, you can refresh higher level object, but not lower (secret, service)

                var secretSection = new structures.section("secrets", "template-body-config-secrets", "template-body-config-editor-secret", page.sections, function (oncomplete, onerror) {
                    loader.secret.getAll(true, oncomplete, onerror)
                }, data.masterData.metaData.secrets, function (secret) {
                    //extend secret by looking for referenced service
                    var serviceId = secret.service() ? secret.service().id() : null;
                    if (serviceId) {
                        for (var i = 0; i < data.masterData.metaData.services.items().length; i++) {
                            if (data.masterData.metaData.services.items()[i].id() === serviceId) {
                                secret.service(data.masterData.metaData.services.items()[i]);
                                break;
                            }
                        }
                    }
                });
                page.sections.items.push(secretSection);
                page.sections.selected(secretSection);

                var serviceSection = new structures.section("services", "template-body-config-services", "template-body-config-editor-service", page.sections, function (oncomplete, onerror) {
                    loader.service.getAll(true, oncomplete, onerror)
                }, data.masterData.metaData.services, function (service) {
                    //extend secret by looking for referenced policy
                    var policyId = service.policy() ? service.policy().id() : null;
                    if (policyId) {
                        for (var i = 0; i < data.masterData.metaData.policies.items().length; i++) {
                            if (data.masterData.metaData.policies.items()[i].id() === policyId) {
                                service.policy(data.masterData.metaData.policies.items()[i]);
                                break;
                            }
                        }
                    }
                });

                page.sections.items.push(serviceSection);

                var policySection = new structures.section("policies", "template-body-config-policies", "template-body-config-editor-policy", page.sections,
                    function (oncomplete, onerror) {
                        loader.policy.getAll(true, oncomplete, onerror)
                    }, data.masterData.metaData.policies, null);
                page.sections.items.push(policySection);

                //load in bottom -> up order, load policies, when happy, load services, when happy load secrets -- could use promises here...
                policySection.actions.refresh();
                serviceSection.actions.refresh();
                secretSection.actions.refresh();

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