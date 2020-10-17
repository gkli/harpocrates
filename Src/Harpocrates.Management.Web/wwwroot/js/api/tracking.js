window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.api = window.Harpocrates.api || {};

window.Harpocrates.api.tracking = (function (enums, common, undefined) {

    function _getTransactions(start, end) {

        var loader = new common.loader(function () {

            var url = '/api/tracking';
            if (start || end) url += '?';

            if (start) url += 'from=' + start;
            if (end) {
                if (start) url += '&';
                url += 'to=' + end;
            }

            //perform load here
            var ajax = common.ajax.request({
                url: url,
                type: "GET"
            });

            ajax.done(function (transactions) {
                loader.completed(transactions);
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });


        });

        return loader;
    }

    function _getTransaction(id) {
        shallow = (shallow) ? true : false;
        var loader = new common.loader(function () {
            //perform load here
            var ajax = common.ajax.request({
                url: '/api/tracking/' + id,
                type: "GET"
            });

            ajax.done(function (transaction) {
                loader.completed(transaction);
            });

            ajax.fail(function (err) {
                loader.fail(err);
            });

        });

        return loader;
    }

    return {
        getAll: _getTransactions,
        get: _getTransaction
    };
})(window.Harpocrates.enums, window.Harpocrates.common);