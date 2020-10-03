window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.api = window.Harpocrates.api || {};

window.Harpocrates.api.policy = (function (enums, common, undefined) {

    function _getPolicies() {

        var loader = new common.loader(function () {

            var dummies = [];
            dummies.push({ "policyId": "6cada23d-76b1-4b50-a773-e4d0822d6821", "name": "24-Hour Rotation", "description": "Rotates secrets every 24 hours", "rotationInterval": "1.00:00:00" });
            dummies.push({ "policyId": "520fd7e3-04ef-48f6-b163-99b7dc74b216", "name": "1-Hour Rotation", "description": "Rotates secrets every 1 hour", "rotationInterval": "01:00:00" });
            dummies.push({ "policyId": "86705a80-4f30-466d-900a-25e80c0e15e4", "name": "15-Day Rotation", "description": "Rotates secrets every 15 days", "rotationInterval": "15.00:00:00" });
            dummies.push({ "policyId": "d5bfb8a3-bc76-4a1e-bb46-50904ebb9273", "name": "30-Day Rotation", "description": "Rotates secrets every 30 days", "rotationInterval": "30.00:00:00" });

            loader.completed(dummies);

            if (false) {
                //perform load here
                var ajax = common.ajax.request({
                    url: '/api/policy',
                    type: "GET"
                });

                ajax.done(function (policies) {
                    loader.completed(policies);
                });

                ajax.fail(function (err) {
                    loader.fail(err);
                });
            }

        });

        return loader;
    }

    function _getPolicy(policyId) {

        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/policy/' + policyId,
                type: "GET"
            });

            ajax.done(function (policy) {
                loader.completed(policy);
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });

        return loader;
    }

    function _savePolicy(policy) {
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/policy',
                type: "POST"
            });

            ajax.done(function (policy) {
                loader.completed(policy);
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });
    }

    function _deletePolicy(policyId) {
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/policy/' + policyId,
                type: "DELETE"
            });

            ajax.done(function (status) {
                loader.completed(status);
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });
    }

    return {
        getAll: _getPolicies,
        get: _getPolicy,
        save: _savePolicy,
        delete: _deletePolicy
    };
})(window.Harpocrates.enums, window.Harpocrates.common);