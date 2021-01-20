
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

    var _controlMode = {
        read: 1,
        edit: 2
    };

    var _trackingStatus = {
        pending: 1,
        success: 2,
        failed: 4,
        aborted: 8,
        retryRequested: 16,
        deadLetter: 32,
        deleteMessage: 64,
        skipped: 128
    };
    var _trackingAction = {
        unknown: -1,
        doNothing: 1,
        rotate: 2,
        scheduleDependencyUpdates: 3,
        performDependencyUpdate: 4,
        cleanup: 5
    };
    var _trackingEvent = {
        unknown: 0,
        created: 1,
        expiring: 2,
        expired: 3
    };
    var _trackingTxPurpose = {
        unknown: -1,
        processKVEvent: 1,
        execProcess: 2
    };

    return {
        nullableBool: _threeStateBool,
        guid: _guid,
        serviceType: _serviceType,
        secretType: _secretType,
        controlMode: _controlMode,
        tracking: {
            status: _trackingStatus,
            action: _trackingAction,
            event: _trackingEvent,
            purpose: _trackingTxPurpose
        }
    };

})();