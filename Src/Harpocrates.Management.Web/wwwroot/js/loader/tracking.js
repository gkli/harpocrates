window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.loader = window.Harpocrates.loader || {};

window.Harpocrates.loader.tracking = (function (enums, common, data, api, undefined) {

    function _getTransactions(start, end, oncomplete, onerror) {

        var loader = api.tracking.getAll(start, end);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (transactions) {
            if (oncomplete) {
                var vms = [];
                if (transactions) {
                    for (var i = 0; i < transactions.length; i++) {
                        var tx = data.tracking.converter.txContractToVm(transactions[i]);
                        if (tx) vms.push(tx);
                    }
                }
                oncomplete(vms);
            }
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    function _getTransaction(id, oncomplete, onerror) {

        var loader = api.tracking.get(id);
        if (!loader) {
            if (onerror) onerror({ status: 401, statusText: "User is not authenticated" });
            return;
        }

        loader = loader.load();

        loader.done(function (transaction) {
            if (oncomplete) oncomplete(data.tracking.converter.txContractToVm(transaction));
        });

        loader.fail(function (err) {
            if (onerror) onerror(err);
        });
    }

    return {
        getAll: _getTransactions,
        get: _getTransaction
    };
})(window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.viewModels, window.Harpocrates.api);