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
                            page = new _common.collection("page");
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

        }
    };


    function vmItem(entities, converter) {
        var self = this;
        self.entities = entities;
        self.converter = converter;
    }

    return {
        //financial: new vmItem(_financial, _financialConverter)
        common: new vmItem(_common, null)
    };

})(window.Harpocrates.enums, window.Harpocrates.common);