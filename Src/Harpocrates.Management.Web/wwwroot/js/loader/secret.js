window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.loader = window.Harpocrates.loader || {};

window.Harpocrates.loader.secret = (function (enums, common, data, api, undefined) {

    function _getSecrets() {

        var loader = api.secret.getAll();
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (secrets) {
            if (oncomplete) oncomplete(secrets);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _getSecret(secretId) {

        var loader = api.secret.get(secretId);
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

    function _saveSecret(secret) {

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

    function _deleteSecret(secretId) {
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