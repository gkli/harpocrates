window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.loader = window.Harpocrates.loader || {};

window.Harpocrates.loader.secret = (function (enums, common, data, api, undefined) {

    function _getSecrets(oncomplete, onerror) {

        var loader = api.secret.getAll();
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (secrets) {
            if (oncomplete) {
                var vms = [];
                if (secrets) {
                    for (var i = 0; i < secrets.length; i++) {
                        var secret = data.metadata.converter.secretContractToVm(secrets[i]);
                        if (secret) vms.push(secret);
                    }
                }
                oncomplete(vms);
            }
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _getSecret(secretId, oncomplete, onerror) {

        var loader = api.secret.get(secretId);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (secret) {
            if (oncomplete) oncomplete(data.metadata.converter.secretContractToVm(secret));
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _saveSecret(secret, oncomplete, onerror) {

        var loader = api.secret.save(secret);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (secret) {
            if (oncomplete) oncomplete(secret);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _deleteSecret(secretId, oncomplete, onerror) {
        var loader = api.secret.delete(secretId);
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
        getAll: _getSecrets,
        get: _getSecret,
        save: _saveSecret,
        delete: _deleteSecret
    };
})(window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.viewModels, window.Harpocrates.api);