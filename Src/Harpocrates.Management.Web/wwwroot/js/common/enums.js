
window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.enums = (function (undefined) {

    var _threeStateBool = {
        Null: 0,
        True: 1,
        False: 2
    };

    var _guid = {
        empty: "00000000-0000-0000-0000-000000000000"
    };

    var _serviceType = {
        unspecified: 0,
        storageAccountKey: 1,
        cosmosDbAccountKey: 2,
        cosmosDbAccountReadOnlyKey: 3,
        sqlServerPassword: 4,
        eventGrid: 5,
        apimManagement: 6,
        appRegistrationPassword: 7,
        redisCache: 8
    };

    var _secretType = {
        attached: 0,
        dependency: 1
    };


    return {
        nullableBool: _threeStateBool,
        guid: _guid,
        serviceType: _serviceType,
        secretType: _secretType
    };

})();