window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.loader = window.Harpocrates.loader || {};

window.Harpocrates.loader.service = (function (enums, common, data, api, undefined) {

    function _getServices(oncomplete, onerror) {

        var loader = api.service.getAll();
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (services) {
            if (oncomplete) {
                var vms = [];
                if (services) {
                    for (var i = 0; i < services.length; i++) {
                        var service = data.metadata.converter.serviceContractToVm(services[i]);
                        if (service) vms.push(service);
                    }
                }
                oncomplete(vms);
            }
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _getService(serviceId, oncomplete, onerror) {

        var loader = api.service.get(serviceId);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (service) {
            if (oncomplete) oncomplete(data.metadata.converter.serviceContractToVm(service));
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _saveService(service, oncomplete, onerror) {
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

    function _deleteService(serviceId, oncomplete, onerror) {
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