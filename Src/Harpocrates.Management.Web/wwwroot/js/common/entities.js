window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.viewModels = (function (enums, common, undefined) {

    var _utilities = {

    };

    var _common = {
        collection: function (name) {
            var self = this;

            self.name = name;
            self.items = ko.observableArray();
            self.selected = ko.observable();
            self.loading = ko.observable(false);
        },
        paginatedCollection: function (pageSize) {
            var self = this;

            self.items = ko.observableArray();
            self.selected = ko.observable();
            self.loading = ko.observable(false);
            self.pageSize = ko.observable(pageSize);

            function selectedItemIndex() {
                //if (!self.selected()) return -1;

                return self.items.indexOf(self.selected());

                //for (var i = 0; i < self.items().length; i++) {
                //    if (self.items()[i] === self.selected()) {
                //        return i;
                //    }
                //}

                //return -1;
            }

            self.selectedPageIndex = ko.pureComputed(function () {
                return selectedItemIndex();
            });

            self.isPageVisible = function (currentIndex) {

                var pageCount = self.items().length;
                if (pageCount <= 5) return true;

                if (currentIndex === 0) return false;
                if (currentIndex === pageCount - 1) return false;

                //desired ux for 11 pages
                //  <-  [1] ... [3] [4] [5] ... [11]  ->

                //current ux
                //   << < ... [3] [4] [5] ... > >>



                //should always have at least 5 page indicators...

                var currentPageIdx = selectedItemIndex();
                if (currentPageIdx < 0) return true;

                if (Math.abs(currentIndex - currentPageIdx) < 3) return true;

                return false;
            };

            self.totalRecords = ko.computed(function () {
                var count = 0;
                for (var i = 0; i < self.items().length; i++) {
                    count += self.items()[i].items().length;
                }
                return count;
            });

            self.actions = {
                first: {
                    can: function () {
                        return self.items().length !== 0;
                    },
                    go: function () {
                        if (!self.actions.first.can()) return;
                        self.actions.goto(0);
                    }
                },
                last: {
                    can: function () { return self.items().length !== 0; },
                    go: function () {
                        if (!self.actions.last.can()) return;
                        self.actions.goto(self.items().length - 1);
                    }
                },
                next: {
                    can: function () {
                        var idx = selectedItemIndex();
                        return idx < self.items().length - 1;
                    },
                    go: function () {
                        if (!self.actions.next.can()) return;

                        var idx = selectedItemIndex();
                        if (idx < 0) return;
                        self.actions.goto(idx + 1);
                    }
                },
                previous: {
                    can: function () {
                        var idx = selectedItemIndex();
                        return idx > 0;
                    },
                    go: function () {
                        if (!self.actions.previous.can()) return;

                        var idx = selectedItemIndex();
                        if (idx < 0) return;
                        self.actions.goto(idx - 1);
                    }
                },
                goto: function (idx) {
                    if (idx < 0) idx = 0;
                    if (idx > self.items().length - 1) idx = self.items().length - 1;

                    self.selected(self.items()[idx]);
                }
            };

            self.helper = {
                clear: function () {
                    self.selected(null);
                    self.items.removeAll();
                },
                paginate: function (collection) {
                    if (!collection) return;
                    self.helper.clear();

                    var page = null;

                    for (var i = 0; i < collection.items().length; i++) {

                        if (null === page) {
                            page = new _common.collection("page " + i);
                            self.items.push(page);
                        }

                        if (null !== page) {
                            page.items.push(collection.items()[i]);
                        }

                        if ((i + 1) % pageSize === 0) {
                            page = null;
                        }
                    }

                    if (self.items().length > 0) {
                        self.selected(self.items()[0]);
                    }
                }
            };
        },
        searchResult: function () {
            var self = this;

            self.items = ko.observableArray();

            self.pages = new _common.paginatedCollection(0);

            self.selected = ko.observable();
            self.recordsFound = ko.observable();
            self.loading = ko.observable(false);
        },
        action: function (caption, handler, validationCallback) {
            var self = this;

            self.caption = ko.observable(caption);
            self.handler = handler;
            self.vcb = validationCallback;

            self.isvalid = ko.computed(function () {
                if (!self.vcb) return true;
                return self.vcb();
            });
        },
        alert: function () {

            var self = this;

            self.warning = ko.observable();
            self.error = ko.observable();
            self.success = ko.observable();

            self.clear = function () {
                self.warning("");
                self.error("");
                self.success("");
            };

            self.hasalerts = ko.computed(function () {
                if (self.warning() || self.error() || self.success()) return true;
                return false;
            });
        },
        confirmation: function (caption, question, onyes, onno) {
            var self = this;

            self.caption = ko.observable(caption);
            self.question = ko.observable(question);
            self.actions = {
                yes: new _common.action("Yes", function () { if (onyes) onyes(); }, function () { return true; }),
                no: new _common.action("No", function () { if (onno) onno(); }, function () { return true; })
            };

        },
        wait: function (caption, message) {
            var self = this;
            self.caption = ko.observable(caption);
            self.message = ko.observable(message);
        }
    };


    var _internalUtilities = {
        confirmationModalWarapper: function () {
            var self = this;

            var state = {
                root: null,
                modal: null,
                clear: function () {
                    state.root = null;
                    state.modal = null;
                }
            };

            self.onyes = null;
            self.onno = null;
            self.onshow = null;

            self.caption = null;
            self.question = null;

            self.show = function () {

                var confirmationVm = new _common.confirmation(self.caption, self.question, self.onyes, self.onno);

                state.root = $('div.template-dialog-confirm');
                ko.applyBindings(confirmationVm, state.root[0]);

                state.modal = state.root.find('div.modal');

                state.modal.on('shown.bs.modal', function () {
                    if (self.onshow) self.onshow();
                });

                state.modal.on('hidden.bs.modal', function () {
                    state.modal.modal('dispose');
                    ko.cleanNode(state.root[0]);
                    state.clear();
                });

                state.modal.modal('show');
            };
            self.hide = function () {
                if (state.modal) state.modal.modal('hide');
            };

        },
        modalState: function () {
            var self = this;

            self.root = null;
            self.modal = null;
            self.clear = function () {
                self.root = null;
                self.modal = null;
            }
        }
    };

    var _dataTypes = {
        timeSpan: function (d, h, m, s) {
            var self = this;

            self.mode = ko.observable(enums.controlMode.read);

            self.days = ko.observable(d);
            self.hours = ko.observable(h);
            self.minutes = ko.observable(m);
            self.seconds = ko.observable(s);

            self.totalSeconds = ko.pureComputed(function () {
                var d = parseInt(self.days()); if (isNaN(d)) d = 0;
                var h = parseInt(self.hours()); if (isNaN(h)) h = 0;
                var m = parseInt(self.minutes()); if (isNaN(m)) m = 0;
                var s = parseInt(self.seconds()); if (isNaN(s)) s = 0;

                h += d * 24;
                m += h * 60;
                s += m * 60;

                return s;
            });

            self.formattedString = ko.pureComputed(function () {
                var d = parseInt(self.days()); if (isNaN(d)) d = 0;
                var h = parseInt(self.hours()); if (isNaN(h)) h = 0;
                var m = parseInt(self.minutes()); if (isNaN(m)) m = 0;
                var s = parseInt(self.seconds()); if (isNaN(s)) s = 0;

                return d + "." + h.pad(2) + ":" + m.pad(2) + ":" + s.pad(2);
            });

            self.inEditMode = ko.pureComputed(function () {
                return parseInt(self.mode()) === enums.controlMode.edit;
            });

            function updateValues() {

                var d = 0, h = 0, m = 0, s = self.totalSeconds();

                if (s > 59) {
                    m = Math.floor(s / 60);
                    s -= m * 60;
                    if (m > 59) {
                        h = Math.floor(m / 60);
                        m -= h * 60;
                        if (h > 23) {
                            d = Math.floor(h / 24);
                            h -= d * 24;
                        }
                    }
                }

                self.days(d);
                self.hours(h);
                self.minutes(m);
                self.seconds(s);
            }

            updateValues();

            //self.days.subscribe(function (value) { });
            self.hours.subscribe(function (value) {
                if (parseInt(value) > 23) updateValues();
            });
            self.minutes.subscribe(function (value) {
                if (parseInt(value) > 59) updateValues();
            });
            self.seconds.subscribe(function (value) {
                if (parseInt(value) > 59) updateValues();
            });

            self.actions = {
                edit: {
                    begin: function () {
                        self.mode(enums.controlMode.edit);
                    },
                    end: function () {
                        self.mode(enums.controlMode.read);
                    }
                }
            };
        }
    };


    var _metaData = {
        policy: function (id, name, description, seconds) {
            var self = this;

            self.id = ko.observable(id);
            self.name = ko.observable(name);
            self.description = ko.observable(description);
            //self.intervalInSeconds = ko.observable(seconds);

            self.interval = ko.observable(new _dataTypes.timeSpan(0, 0, 0, seconds));

            var modalState = new _internalUtilities.modalState();

            self.actions = {
                edit: {
                    begin: function () {
                        modalState.root = $('div.template-body-config-editor-policy');

                        ko.applyBindings(self, modalState.root[0]);

                        modalState.modal = modalState.root.find('div.modal');

                        modalState.modal.on('shown.bs.modal', function () {

                        });

                        modalState.modal.on('hidden.bs.modal', function () {
                            modalState.modal.modal('dispose');
                            ko.cleanNode(modalState.root[0]);
                            modalState.clear();
                        });

                        modalState.modal.modal('show');
                    },
                    end: function () {
                        if (modalState.modal) modalState.modal.modal('hide');
                    }
                },
                save: function () {
                },
                refresh: function () {
                    if (!self.id()) return;

                    if (!window.Harpocrates.api.policy) throw "Policy api is required to perform this operation";
                    if (!window.Harpocrates.loader.policy) throw "Policy loader is required to perform this operation";

                    window.Harpocrates.loader.policy.get(self.id(), function (policy) {

                        self.id(policy.id());
                        self.name(policy.name());
                        self.description(policy.description());
                        self.interval(policy.interval());

                    }, function (err) { });
                }
            };
        },
        service: function (id, name, description, type, subId, srcConnString, policy) {
            var self = this;

            self.id = ko.observable(id);
            self.name = ko.observable(name);
            self.description = ko.observable(description);
            self.type = ko.observable(type);
            self.policy = ko.observable(policy);
            self.subscriptionId = ko.observable(subId);
            self.sourceConnectionString = ko.observable(srcConnString);

            var modalState = new _internalUtilities.modalState();

            self.actions = {
                edit: {
                    begin: function () {
                        modalState.root = $('div.template-body-config-editor-service');

                        ko.applyBindings(self, modalState.root[0]);

                        modalState.modal = modalState.root.find('div.modal');

                        modalState.modal.on('shown.bs.modal', function () {

                        });

                        modalState.modal.on('hidden.bs.modal', function () {
                            modalState.modal.modal('dispose');
                            ko.cleanNode(modalState.root[0]);
                            modalState.clear();
                        });

                        modalState.modal.modal('show');
                    },
                    end: function () {
                        if (modalState.modal) modalState.modal.modal('hide');
                    }
                },
                save: function () {
                },
                refresh: function () {
                    if (!self.id()) return;

                    if (!window.Harpocrates.api.service) throw "Service api is required to perform this operation";
                    if (!window.Harpocrates.loader.service) throw "Service loader is required to perform this operation";

                    window.Harpocrates.loader.service.get(self.id(), function (service) {

                        self.id(service.id());
                        self.name(service.name());
                        self.description(service.description());
                        self.type(service.type());
                        self.subscriptionId(service.subscriptionId());
                        self.sourceConnectionString(service.sourceConnectionString());

                        //should we lazy load this instead?

                        self.policy(service.policy);

                    }, function (err) { });
                }
            };
        },
        secret: function (key, uri, vault, objectType, objectName, type, name, description, subscriptionId, formatExpression, currentKeyName, lastRotated, service) {
            var self = this;

            self.id = ko.observable(key);
            self.uri = ko.observable(uri);
            self.vaultName = ko.observable(vault);
            self.objectType = ko.observable(objectType);
            self.objectName = ko.observable(objectName);
            self.subscriptionId = ko.observable(subscriptionId);
            self.formatExpression = ko.observable(formatExpression);
            self.currentKeyName = ko.observable(currentKeyName);
            self.lastRotatedOn = ko.observable(lastRotated);

            self.type = ko.observable(type);
            self.service = ko.observable(service);

            self.name = ko.observable(name);
            self.description = ko.observable(description);

            self.serviceNameText = ko.pureComputed(function () {
                if (!self.service()) return "N/A";

                return self.service().name();
            });

            self.serviceTypeText = ko.pureComputed(function () {
                if (!self.service()) return "N/A";

                return self.service().type().name();
            });

            self.lastRotatedOnText = ko.pureComputed(function () {
                if (!self.lastRotatedOn()) return "Never";

                //todo: format date
                return self.lastRotatedOn();
            });
            self.rotationOverdue = ko.pureComputed(function () {
                if (self.type() && self.type().type() === enums.secretType.dependency) return false;

                if (!self.lastRotatedOn()) return false;
                if (!self.service()) return false;

                //todo: parse out lastRotatedOn to determine if we're exceeding scheduled rotation

                return true;
            });

            var modalState = new _internalUtilities.modalState();

            self.actions = {
                edit: {
                    begin: function () {
                        modalState.root = $('div.template-body-config-editor-secret');

                        ko.applyBindings(self, modalState.root[0]);

                        modalState.modal = modalState.root.find('div.modal');

                        modalState.modal.on('shown.bs.modal', function () {

                        });

                        modalState.modal.on('hidden.bs.modal', function () {
                            modalState.modal.modal('dispose');
                            ko.cleanNode(modalState.root[0]);
                            modalState.clear();
                        });

                        modalState.modal.modal('show');
                    },
                    end: function () {
                        if (modalState.modal) modalState.modal.modal('hide');
                    }
                },
                save: function () {
                },
                refresh: function () {
                    if (!self.id()) return;

                    if (!window.Harpocrates.api.secret) throw "Secret api is required to perform this operation";
                    if (!window.Harpocrates.loader.secret) throw "Secret loader is required to perform this operation";

                    window.Harpocrates.loader.secret.get(self.id(), function (secret) {

                        self.id(secret.id());
                        self.name(secret.name());
                        self.description(secret.description());
                        self.uri(secret.uri());
                        self.vaultName(secret.vaultName());
                        self.objectType(secret.objectType());
                        self.objectName(secret.objectName());
                        self.subscriptionId(secret.subscriptionId());
                        self.formatExpression(secret.formatExpression());
                        self.currentKeyName(secret.currentKeyName());
                        self.lastRotatedOn(secret.lastRotatedOn());
                        self.type(secret.type());

                        //should we lazy load this instead?
                        self.service(secret.service());

                    }, function (err) { });
                }
            };
        },
        serviceType: function (type) {
            var self = this;

            self.type = ko.observable(type);
            self.name = ko.pureComputed(function () {
                switch (self.type()) {
                    case enums.serviceType.unspecified:
                        return "Unspecified";
                    case enums.serviceType.storageAccountKey:
                        return "Storage Account Key";
                    case enums.serviceType.cosmosDbAccountKey:
                        return "CosmosDb Master Key";
                    case enums.serviceType.cosmosDbAccountReadOnlyKey:
                        return "CosmosDb Read-Only Key";
                    case enums.serviceType.sqlServerPassword:
                        return "SQL Server Password";
                    case enums.serviceType.eventGrid:
                        return "Event Grid";
                    case enums.serviceType.apimManagement:
                        return "APIM Management Key";
                    case enums.serviceType.appRegistrationPassword:
                        return "Service Principal";
                    case enums.serviceType.redisCache:
                        return "REDIS Cache";
                }

                return "Unknown";
            });

        },
        secretType: function (type) {
            var self = this;

            self.type = ko.observable(type);
            self.name = ko.pureComputed(function () {
                switch (self.type()) {
                    case enums.secretType.attached:
                        return "attached";
                    case enums.secretType.dependency:
                        return "dependency";
                }

                return "Unknown";
            });
        }
    };

    var _metaDataConverter = {
        policyContractToVm: function (contract) {
            if (!contract) return null;

            return new _metaData.policy(contract.policyId, contract.name, contract.description, contract.rotationIntervalInSec);
        },
        policyVmToContract: function (vm) { },

        serviceContractToVm: function (contract) {
            if (!contract) return null;

            return new _metaData.service(contract.configurationId
                , contract.name
                , contract.description
                , _masterData.metaData.serviceTypes.find(contract.serviceType)
                , contract.subscriptionId
                , contract.sourceConnectionString
                , _metaDataConverter.policyContractToVm(contract.policy));
        },
        serviceVmToContract: function (vm) { },

        secretContractToVm: function (contract) {
            if (!contract) return null;
            return new _metaData.secret(contract.key
                , contract.uri
                , contract.vaultName
                , contract.objectType
                , contract.objectName
                , _masterData.metaData.secretTypes.find(contract.secretType)
                , contract.name
                , contract.description
                , contract.subscriptionId
                , contract.formatExpression
                , contract.currentKeyName
                , contract.lastRotated
                , _metaDataConverter.serviceContractToVm(contract.configuration));

        },
        secretVmToContract: function (vm) { }
    };

    var _masterData = {
        metaData: {
            serviceTypes: new _common.collection("service types"),
            secretTypes: new _common.collection("secret types"),
            policies: new _common.collection("policies"),
            services: new _common.collection("services"),
            secrets: new _common.collection("secrets")
        }
    };

    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.unspecified));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.storageAccountKey));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.cosmosDbAccountKey));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.cosmosDbAccountReadOnlyKey));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.sqlServerPassword));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.eventGrid));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.apimManagement));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.appRegistrationPassword));
    _masterData.metaData.serviceTypes.items.push(new _metaData.serviceType(enums.serviceType.redisCache));

    _masterData.metaData.serviceTypes.selected(_masterData.metaData.serviceTypes.items()[0]);

    _masterData.metaData.secretTypes.items.push(new _metaData.secretType(enums.secretType.attached));
    _masterData.metaData.secretTypes.items.push(new _metaData.secretType(enums.secretType.dependency));

    _masterData.metaData.secretTypes.selected(_masterData.metaData.secretTypes.items()[0]);

    _masterData.metaData.serviceTypes.find = function (type) {
        for (var i = 0; i < _masterData.metaData.serviceTypes.items().length; i++) {
            if (_masterData.metaData.serviceTypes.items()[i].type() === type) return _masterData.metaData.serviceTypes.items()[i];
        }
        return null;
    }

    _masterData.metaData.secretTypes.find = function (type) {
        for (var i = 0; i < _masterData.metaData.secretTypes.items().length; i++) {
            if (_masterData.metaData.secretTypes.items()[i].type() === type) return _masterData.metaData.secretTypes.items()[i];
        }
        return null;
    }


    function vmItem(entities, converter) {
        var self = this;
        self.entities = entities;
        self.converter = converter;
    }

    return {
        //financial: new vmItem(_financial, _financialConverter)
        common: new vmItem(_common, null),
        metadata: new vmItem(_metaData, _metaDataConverter),
        masterData: _masterData
    };

})(window.Harpocrates.enums, window.Harpocrates.common);