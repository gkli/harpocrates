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

        var dataManager = {
            metadata: {
                secrets: data.masterData.metaData.secrets,
                services: data.masterData.metaData.services,
                policies: data.masterData.metaData.policies,
                handlers: {
                    onPoliciesChanged: function (changes) {
                        //when policies change, update services
                        changes.forEach(function (change) {
                            if (change.status === 'added') {
                                //re-evaluate services for matching policies
                                var policy = change.value;
                                if (policy) {
                                    dataManager.metadata.services.items().forEach(function (service) {
                                        if (service && service.policy() && service.policy().id() === policy.id()) service.policy(policy);
                                    });
                                }
                            }
                        });
                    },
                    onServicesChanged: function (changes) {
                        changes.forEach(function (change) {
                            if (change.status === 'added') {
                                //re-evaluate secrets for matching policies
                                var service = change.value;
                                if (service) {
                                    dataManager.metadata.secrets.items().forEach(function (secret) {
                                        if (secret && secret.service() && secret.service().id() === service.id()) secret.service(service);
                                    });
                                }
                            }
                        });
                    },
                    onSecretsChanged: function (changes) {
                        changes.forEach(function (change) {
                            if (change.status === 'added') {
                                var secret = change.value;
                                if (secret) {
                                    dataManager.tracking.transactions.items().forEach(function (tx) {
                                        if (tx && tx.secret.key() && tx.secret.key() === secret.id()) tx.secret.instance(secret);
                                    });
                                }
                            }
                        });
                    }

                }
            },
            tracking: {
                transactions: new data.common.entities.collection("datamanager.tracking.transactions"),
                handlers: {
                    onTransactionsChanged: function (changes) { }
                }
            },
            startListening: function () {
                //wire up listeners... -- secrets listens to services, services listens to policies, transactions listens to secrets

                dataManager.metadata.policies.items.subscribe(dataManager.metadata.handlers.onPoliciesChanged, null, "arrayChange");
                dataManager.metadata.services.items.subscribe(dataManager.metadata.handlers.onServicesChanged, null, "arrayChange");
                dataManager.metadata.secrets.items.subscribe(dataManager.metadata.handlers.onSecretsChanged, null, "arrayChange");

                dataManager.tracking.transactions.items.subscribe(dataManager.tracking.handlers.onTransactionsChanged, null, "arrayChange");
            }
        };

        dataManager.startListening();

        var builder = {
            dashboard: function () {
                var page = new structures.page("dashboard", "template-body-dashboard");

                //extend page to have dateRange dropdown

                function duration(name, minutes, parent) {
                    var self = this;
                    self.name = ko.observable(name);
                    self.minutes = ko.observable(minutes);
                    self.parent = ko.observable(parent);

                    //self.startOn = ko.computed(function () { });
                    //self.endOn = ko.computed(function () { });

                    //todo: duration can be fixed and variable, if fixed, we allow to specify start/end
                    //if variable we base computation on minutes
                    //variable can be determined if minutes are < 0 ?

                    self.actions = {
                        select: function () {
                            if (!self.parent()) return;
                            self.parent().selected(self);
                        }
                    };
                }

                var durations = new data.common.entities.collection("durations");
                durations.items.push(new duration("Last 30 min", 30, durations));
                durations.items.push(new duration("Last hour", 60, durations));
                durations.items.push(new duration("Last 24 hours", 24 * 60, durations));
                durations.items.push(new duration("Last 7 days", 7 * 24 * 60, durations));
                durations.items.push(new duration("Last 30 days", 30 * 24 * 60, durations));
                durations.items.push(new duration("Last 6 months", 6 * 30 * 24 * 60, durations));
                durations.items.push(new duration("Last 1 year", 12 * 30 * 24 * 60, durations));
               // durations.items.push(new duration("Custom", -1, durations));

                durations.selected(durations.items()[3]);

                page.durations = durations;

                var mainSection = new structures.section("main", "", "", page.sections,
                    function (oncomplete, onerror) {

                        var start = '01/01/1999';
                        var end = '';

                        if (page && page.durations && page.durations.selected()) {
                            var minutes = parseInt(page.durations.selected().minutes());
                            if (minutes > 0) {
                                var now = new Date();
                                var startOn = new Date(now.getTime() - minutes * 60000);

                                end = (now.getUTCMonth() + 1) + "/" + now.getUTCDate() + "/" + now.getUTCFullYear() + " " + now.getUTCHours() + ":" + now.getUTCMinutes(); // format to look like date
                                start = (startOn.getUTCMonth() + 1) + "/" + startOn.getUTCDate() + "/" + startOn.getUTCFullYear() + " " + startOn.getUTCHours() + ":" + startOn.getUTCMinutes(); // format to look like date
                            }
                        }

                        loader.tracking.getAll(start, end, oncomplete, onerror)
                    }, dataManager.tracking.transactions, function (tx) {
                        var secretId = tx ? tx.secret.key() : null;
                        if (secretId) {
                            for (var i = 0; i < dataManager.metadata.secrets.items().length; i++) {
                                if (dataManager.metadata.secrets.items()[i].id() === secretId) {
                                    tx.secret.instance(dataManager.metadata.secrets.items()[i]);
                                    break;
                                }
                            }
                        }
                    });
                page.sections.items.push(mainSection);
                page.sections.selected(mainSection);

                mainSection.actions.refresh();

                page.durations.selected.subscribe(function () {
                    mainSection.actions.refresh();
                });



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
                }, dataManager.metadata.secrets, function (secret) {
                    //extend secret by looking for referenced service
                    var serviceId = secret.service() ? secret.service().id() : null;
                    if (serviceId) {
                        for (var i = 0; i < dataManager.metadata.services.items().length; i++) {
                            if (dataManager.metadata.services.items()[i].id() === serviceId) {
                                secret.service(dataManager.metadata.services.items()[i]);
                                break;
                            }
                        }
                    }
                });
                page.sections.items.push(secretSection);
                page.sections.selected(secretSection);

                var serviceSection = new structures.section("services", "template-body-config-services", "template-body-config-editor-service", page.sections, function (oncomplete, onerror) {
                    loader.service.getAll(true, oncomplete, onerror)
                }, dataManager.metadata.services, function (service) {
                    //extend secret by looking for referenced policy
                    var policyId = service.policy() ? service.policy().id() : null;
                    if (policyId) {
                        for (var i = 0; i < dataManager.metadata.policies.items().length; i++) {
                            if (dataManager.metadata.policies.items()[i].id() === policyId) {
                                service.policy(dataManager.metadata.policies.items()[i]);
                                break;
                            }
                        }
                    }
                });

                page.sections.items.push(serviceSection);

                var policySection = new structures.section("policies", "template-body-config-policies", "template-body-config-editor-policy", page.sections,
                    function (oncomplete, onerror) {
                        loader.policy.getAll(true, oncomplete, onerror)
                    }, dataManager.metadata.policies, null);
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