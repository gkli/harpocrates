window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.api = window.Harpocrates.api || {};

window.Harpocrates.api.policy = (function (enums, common, undefined) {

    function _getPolicies() {

        var loader = new common.loader(function () {

            //perform load here
            var ajax = common.ajax.request({
                url: '/api/policies',
                type: "GET"
            });

            ajax.done(function (policies) {
                loader.completed(policies);
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });


        });

        return loader;
    }

    function _getPolicy(policyId) {

        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/policies/' + policyId,
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
                url: '/api/policies',
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
                url: '/api/policies/' + policyId,
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