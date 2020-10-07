window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.loader = window.Harpocrates.loader || {};

window.Harpocrates.loader.policy = (function (enums, common, data, api, undefined) {

    function _getPolicies(shallow, oncomplete, onerror) {

        var loader = api.policy.getAll(shallow);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (policies) {
            if (oncomplete) {
                var vms = [];
                if (policies) {
                    for (var i = 0; i < policies.length; i++) {
                        var policy = data.metadata.converter.policyContractToVm(policies[i]);
                        if (policy) vms.push(policy);
                    }
                }
                oncomplete(vms);
            }
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _getPolicy(policyId, shallow, oncomplete, onerror) {

        var loader = api.policy.get(policyId, shallow);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (policy) {
            if (oncomplete) oncomplete(data.metadata.converter.policyContractToVm(policy));
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _savePolicy(policy, oncomplete, onerror) {

        var loader = api.policy.save(policy);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (policy) {
            if (oncomplete) oncomplete(policy);
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _deletePolicy(policyId, oncomplete, onerror) {

        var loader = api.policy.delete(policyId);
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
        getAll: _getPolicies,
        get: _getPolicy,
        save: _savePolicy,
        delete: _deletePolicy
    };
})(window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.viewModels, window.Harpocrates.api);