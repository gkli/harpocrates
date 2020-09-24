/// <references path="enums.js"/>
/// <references path="common.js"/>

window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.security = (function ($, common, enums, undefined) {

    var _state = {
        currentUser: {
            id: null,
            email: null,
            profile: null,
            token: null
        }
    };

    function _getCurrentUserId() {
        return _state.currentUser.id;
    }

    function _setCurrentUserId(id) {
        if (id && id !== enums.guid.empty) {
            _state.currentUser.id = id;
        }
    }

    function _getCurrentUserToken() {
        return _state.currentUser.token;
    }

    function _setCurrentUserToken(token) {
        if (token)
            _state.currentUser.token = token;
    }

    function _getCurrentUserEmail() {
        return _state.currentUser.email;
    }

    function _setCurrentUserEmail(email) {
        if (email) {
            _state.currentUser.email = email;
        }
    }

    function _getCurrentUserProfile() {
        return _state.currentUser.profile;
    }

    function _setCurrentUserProfile(profile) {
        _state.currentUser.profile = profile;
    }

    function _isUserLoggedIn() {
        return (_getCurrentUserId()) ? true : false;
    }

    return {
        user: {
            current: {
                getId: _getCurrentUserId,
                setId: _setCurrentUserId,
                setEmail: _setCurrentUserEmail,
                getEmail: _getCurrentUserEmail,
                getProfile: _getCurrentUserProfile,
                setProfile: _setCurrentUserProfile,
                getToken: _getCurrentUserToken,
                setToken: _setCurrentUserToken
            },
            isAuthenticated: _isUserLoggedIn
        }
    };

})(window.jQuery, window.Harpocrates.common, window.Harpocrates.enums);