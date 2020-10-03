Number.prototype.pad = function (size) {
    var s = String(this);
    while (s.length < (size || 2)) { s = "0" + s; }
    return s;
}

window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.common = (function ($, undefined) {

    function _loader(onload) {
        var self = this;

        var deferred = new $.Deferred();

        self.getPromise = function () { return deferred.promise(); };

        self.completed = function (data) {
            deferred.resolve(data);
        };

        self.fail = function (err) {
            deferred.reject(err);
        };

        self.load = function () {
            if (onload) onload();

            return self.getPromise();
        };
    }

    var _templates = new _loader(function () {
        var expecting = 0, loaded = 0;
        $('script[type="text/html"]').each(function () {
            var src = $(this).attr('src');

            if (!src) return;
            expecting++;

            $(this).load(src, function () {
                var id = $(this).attr('id');
                if (id) {
                    var html = $('<div data-bind="template:\'' + id + '\'"></div>');
                    html.addClass(id);
                    $('placeholder.' + id).replaceWith(html);
                }
                $(this).removeAttr('src'); //clear out src attr...

                loaded++;
                if (expecting === loaded) {
                    _templates.completed();
                }
            });

        });
    });

    //utilities - holds common utility methods
    var _utilities = {
        paths: {
            home: "home",
            privatehome: "private",
            game: "game",
            demo: "demo",
            login: "login",
            signup: "signup",
            about: "about",
            profile: "profile",
            tou: "tou",
            privacy: "privacy"
        },
        getCurrentPath: function () {
            var path = window.location.pathname.toLowerCase();

            if (path.indexOf("/private/game", 0) > -1)
                return _utilities.paths.game;

            if (path.indexOf("/private") > -1)
                return _utilities.paths.privatehome;

            if (path.indexOf("/home/demo") > -1)
                return _utilities.paths.demo;

            if (path.indexOf("/home/about") > -1)
                return _utilities.paths.about;

            if (path.indexOf("/home/tou", 0) > -1)
                return _utilities.paths.tou;

            if (path.indexOf("/home/privacy", 0) > -1)
                return _utilities.paths.privacy;

            //if (path.indexOf("/private/account/account.html", 0) > -1)
            //    return _utilities.paths.profile;

            //if (path.indexOf("/home/browse", 0) > -1)
            //    return _utilities.paths.browse;

            //if (path.indexOf("/home/review", 0) > -1)
            //    return _utilities.paths.review;

            //if (path.indexOf("/account/login.html", 0) > -1)
            //    return _utilities.paths.login;

            //if (path.indexOf("/home/about") > -1)
            //    return _utilities.paths.about;

            //if (path.indexOf("/index.html") > -1)
            //    return _utilities.paths.home;

            //if (path.indexOf("/signup.html", 0) > -1)
            //    return _utilities.paths.signup;

            //if (path.indexOf("/index.html") > -1)
            return _utilities.paths.home;
        },
        isCurrentPathPrivate: function () {
            var path = window.location.pathname;

            return path.indexOf("/private/") > -1;
        },
        base64: {

            // private property
            _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",

            // public method for encoding
            encode: function (input) {
                var output = "";
                var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
                var i = 0;

                input = _utilities.base64._utf8_encode(input);

                while (i < input.length) {

                    chr1 = input.charCodeAt(i++);
                    chr2 = input.charCodeAt(i++);
                    chr3 = input.charCodeAt(i++);

                    enc1 = chr1 >> 2;
                    enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                    enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                    enc4 = chr3 & 63;

                    if (isNaN(chr2)) {
                        enc3 = enc4 = 64;
                    } else if (isNaN(chr3)) {
                        enc4 = 64;
                    }

                    output = output +
                        this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
                        this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

                }

                return output;
            },

            // public method for decoding
            decode: function (input) {
                var output = "";
                var chr1, chr2, chr3;
                var enc1, enc2, enc3, enc4;
                var i = 0;

                input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

                while (i < input.length) {

                    enc1 = this._keyStr.indexOf(input.charAt(i++));
                    enc2 = this._keyStr.indexOf(input.charAt(i++));
                    enc3 = this._keyStr.indexOf(input.charAt(i++));
                    enc4 = this._keyStr.indexOf(input.charAt(i++));

                    chr1 = (enc1 << 2) | (enc2 >> 4);
                    chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                    chr3 = ((enc3 & 3) << 6) | enc4;

                    output = output + String.fromCharCode(chr1);

                    if (enc3 !== 64) {
                        output = output + String.fromCharCode(chr2);
                    }
                    if (enc4 !== 64) {
                        output = output + String.fromCharCode(chr3);
                    }

                }

                output = _utilities.base64._utf8_decode(output);

                return output;

            },

            // private method for UTF-8 encoding
            _utf8_encode: function (string) {
                string = string.replace(/\r\n/g, "\n");
                var utftext = "";

                for (var n = 0; n < string.length; n++) {

                    var c = string.charCodeAt(n);

                    if (c < 128) {
                        utftext += String.fromCharCode(c);
                    }
                    else if ((c > 127) && (c < 2048)) {
                        utftext += String.fromCharCode((c >> 6) | 192);
                        utftext += String.fromCharCode((c & 63) | 128);
                    }
                    else {
                        utftext += String.fromCharCode((c >> 12) | 224);
                        utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                        utftext += String.fromCharCode((c & 63) | 128);
                    }

                }

                return utftext;
            },

            // private method for UTF-8 decoding
            _utf8_decode: function (utftext) {
                var string = "";
                var i = 0;
                var c = c1 = c2 = 0;

                while (i < utftext.length) {

                    c = utftext.charCodeAt(i);

                    if (c < 128) {
                        string += String.fromCharCode(c);
                        i++;
                    }
                    else if ((c > 191) && (c < 224)) {
                        c2 = utftext.charCodeAt(i + 1);
                        string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                        i += 2;
                    }
                    else {
                        c2 = utftext.charCodeAt(i + 1);
                        c3 = utftext.charCodeAt(i + 2);
                        string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                        i += 3;
                    }

                }

                return string;
            }
        },
        formatters: {
            phone: function (p) {
                if (!p) return "";
                if (p.length === 10)
                    return p.replace(/(\d{3})(\d{3})(\d{4})/, '$1-$2-$3');

                return p;
            }
        },
        url: {
            queryString: function (key) {
                if (!key) return null;
                key = key.toLowerCase();
                var query = window.location.search.substring(1);
                var vars = query.split("&");
                for (var i = 0; i < vars.length; i++) {
                    var pair = vars[i].split("=");
                    if (pair[0] && pair[0].toLowerCase() === key) { return pair[1]; }
                }
                return null;
            }
        },
        date: {
            toLocalTime: function (date) {
                var offset = new Date().getTimezoneOffset() * -1;
                date = new Date(date);
                var localDate = new Date();
                localDate.setDate(date.getDate());
                localDate.setTime(date.getTime() + offset * 60000);

                return localDate;
            }
        }
    };

    var _config = {
        imageBaseUrl: "/assets/images",
        currentTouVersion: "Oct 01, 2020"
    };

    var _guids = {
        manager: null
    };



    var ajaxDefaultSettings = {
        type: "GET",
        dataType: "JSON",
        contentType: "application/json; charset=utf-8",
        cache: false,
        beforeSend: function (xhr) {
        },
        error: function (err) {
            console.log("Ajax error occured.", err.status);
            console.log("Ajax error detail.", err.statusText);
        }
    };

    var _ajax = {
        request: function (settings) {
            var options = $.extend({}, ajaxDefaultSettings);
            $.extend(options, settings);
            return $.ajax(options);
        }
    };



    //manage any page-level initialization
    $(function () {

        //_guids.manager = $.fn.createGuidManager();

        _templates.load();
    });


    return {
        templates: {
            load: _templates.getPromise
        },
        utilities: _utilities,
        ajax: _ajax,
        loader: _loader,
        config: _config
        //guids: _guids
    };

})(window.jQuery);