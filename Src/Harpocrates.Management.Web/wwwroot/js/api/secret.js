window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.api = window.Harpocrates.api || {};

window.Harpocrates.api.secret = (function (enums, common, undefined) {

    function _getSecrets() {

        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/secrets',
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

    function _getSecret(secretId) {

        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/secrets/' + secretId,
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

    function _saveSecret(secret) {
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/secrets',
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

    function _deleteSecret(secretId) {
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/secrets/' + secretId,
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
        getAll: _getSecrets,
        get: _getSecret,
        save: _saveSecret,
        delete: _deleteSecret
    };
})(window.Harpocrates.enums, window.Harpocrates.common);