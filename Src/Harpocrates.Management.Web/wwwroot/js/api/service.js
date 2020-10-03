window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.api = window.Harpocrates.api || {};

window.Harpocrates.api.service = (function (enums, common, undefined) {

    function _getServices() {

        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/services',
                type: "GET"
            });

            ajax.done(function (session) {
                if (session) {
                    loader.completed(session);
                }
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });

        return loader;
    }

    function _getService(serviceId) {

        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/services/' + serviceId,
                type: "GET"
            });

            ajax.done(function (hand) {
                if (hand) {
                    loader.completed(hand);
                }
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });

        return loader;
    }

    function _saveService(service) {
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/services',
                type: "POST"
            });

            ajax.done(function (session) {
                if (session) {
                    loader.completed(session);
                }
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });
    }

    function _deleteService(serviceId) {
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/services/' + serviceId,
                type: "DELETE"
            });

            ajax.done(function (session) {
                if (session) {
                    loader.completed(session);
                }
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });
    }

    return {
        getAll: _getServices,
        get: _getService,
        save: _saveService,
        delete: _deleteService
    };
})(window.Harpocrates.enums, window.Harpocrates.common);