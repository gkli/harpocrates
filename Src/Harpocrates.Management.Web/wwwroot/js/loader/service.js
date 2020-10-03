window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.loader = window.Harpocrates.loader || {};

window.Harpocrates.loader.service = (function (enums, common, data, api, undefined) {

    function _getServices() {

        var loader = api.service.getAll();
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (services) {
            if (oncomplete) oncomplete(services);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _getService(serviceId) {

        var loader = api.service.get(serviceId);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (service) {
            if (oncomplete) oncomplete(service);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _saveService(service) {
        var loader = api.service.save(service);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (service) {
            if (oncomplete) oncomplete(service);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _deleteService(serviceId) {
        var loader = api.service.delete(serviceId);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (result) {
            if (oncomplete) oncomplete(result);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    return {
        getAll: _getServices,
        get: _getService,
        save: _saveService,
        delete: _deleteService
    };
})(window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.viewModels, window.Harpocrates.api);