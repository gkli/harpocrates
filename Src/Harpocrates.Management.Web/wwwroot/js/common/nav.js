/// <references path="common.js"/>
/// <references path="security.js"/>ui.menu

window.Harpocrates = window.Harpocrates || {};
window.Harpocrates.ui = window.Harpocrates.ui || {};
window.Harpocrates.ui.menu = (function ($, enums, common, security, undefined) {
    if (!common) throw "common is required";

    function _createViewModel() {

        function img(href, errorHref) {
            var self = this;

            self.href = ko.observable(href);
            self.errorHref = ko.observable(errorHref);
            self.onerror = function (data, event) { (event.target || event.srcElement).src = self.errorHref(); };
        }

        function menuItem(text, url, isactive, isheader, highlight, icon, logo) {
            var self = this;
            self.text = ko.observable(text);
            self.url = ko.observable(url);
            self.isActive = ko.observable(isactive);
            self.visible = ko.observable(true);
            self.highlight = ko.observable(highlight);
            self.icon = ko.observable(icon);
            self.logo = ko.observable(logo);

            self.isHeader = ko.observable(isheader);
            self.isDivider = ko.observable(false);

            self.children = ko.observableArray();
            self.hasChildren = ko.computed(function () { return self.children().length > 0; });

            self.actions = {
                select: function () {
                    menuVm.current(self);
                    for (var i = 0; i < itemsMasterList.length; i++) {
                        itemsMasterList[i].isActive(false);
                    }
                    self.isActive(true);
                }
            };

            //self.events = {
            //    selected: function () { }
            //};
        }

        var isLoggedIn = false; //security.user.isAuthenticated();

        var sidebarVm = {
            items: ko.observableArray()
        };

        var itemsMasterList = [];

        var menuVm = {
            items: {
                right: ko.observableArray(),
                left: ko.observableArray()
            },
            current: ko.observable(null),
            sidebar: ko.observable(false),
            dark: ko.observable(isLoggedIn)
        };

        var menuContainers = {
            home: "home",
            privatehome: "private",
            game: "game",
            demo: "demo",
            profile: "profile",
            login: "login",
            signup: "signup",
            about: "about",
            other: "other"
        };

        function _getMenuConainer() {
            var currentPath = common.utilities.getCurrentPath();
            switch (currentPath) {
                case common.utilities.paths.home:
                    return menuContainers.home;
                case common.utilities.paths.privatehome:
                    return menuContainers.privatehome;
                case common.utilities.paths.game:
                    return menuContainers.game;
                case common.utilities.paths.demo:
                    return menuContainers.demo;
                case common.utilities.paths.profile:
                    return menuContainers.profile;
                case common.utilities.paths.login:
                    return menuContainers.login;
                case common.utilities.paths.signup:
                    return menuContainers.signup;
                case common.utilities.paths.about:
                    return menuContainers.about;
                default:
                    return menuContainers.home;
            }
        }

        var curretMenuContainer = _getMenuConainer();

        var divider = new menuItem(null, null, false, false, false);
        divider.isDivider(true);


        if (curretMenuContainer === menuContainers.game) {// || curretMenuContainer === menuContainers.privatehome) {
            menuVm.sidebar(true);
        }

        var title = "";
        var url = "";
        var item = null;
        var isHomeActive = false, isDemoActive = false, isLoginActive = false, isSignupActive = false;

        title = "dashboard";
        if (curretMenuContainer === menuContainers.home) {
            url = "#";
            isHomeActive = true;
        }
        else {
            if (isLoggedIn) url = "/private";
            else url = "/home";
        }

        item = new menuItem(title, url, isHomeActive, false, false, "nc-icon nc-chart-pie-36");
        itemsMasterList.push(item);
        menuVm.items.left.push(item);
        sidebarVm.items.push(item);

        if (isHomeActive) menuVm.current(item);

        if (!isLoggedIn) {
            title = "secrets";
            if (curretMenuContainer === menuContainers.demo) {
                url = "#";
                isDemoActive = true;
            }
            else {
                url = "/home/secrets";
            }

            item = new menuItem(title, url, isDemoActive, false, false, "nc-icon nc-settings-90");
            itemsMasterList.push(item);
            menuVm.items.left.push(item);
            sidebarVm.items.push(item);

            if (isDemoActive) menuVm.current(item);
        }

        if (!isLoggedIn) {
            menuVm.items.right.push(new menuItem("log in", "/account/signin", false, false, true, "nc-icon nc-circle-09"));
        }
        //else {
        //    //add logged in controls here
        //}

        return {
            menu: menuVm,
            sidebar: sidebarVm
        };
    }


    //common.templates.load().done(function () {

    //    function img(href, errorHref) {
    //        var self = this;

    //        self.href = ko.observable(href);
    //        self.errorHref = ko.observable(errorHref);
    //        self.onerror = function (data, event) { (event.target || event.srcElement).src = self.errorHref(); };
    //    }

    //    function menuItem(text, url, isactive, isheader, highlight, icon, logo) {
    //        var self = this;
    //        self.text = ko.observable(text);
    //        self.url = ko.observable(url);
    //        self.isActive = ko.observable(isactive);
    //        self.visible = ko.observable(true);
    //        self.highlight = ko.observable(highlight);
    //        self.icon = ko.observable(icon);
    //        self.logo = ko.observable(logo);

    //        self.isHeader = ko.observable(isheader);
    //        self.isDivider = ko.observable(false);

    //        self.children = ko.observableArray();
    //        self.hasChildren = ko.computed(function () { return self.children().length > 0; });

    //        self.actions = {
    //            select: function () {
    //                menuVm.current(self);
    //                for (var i = 0; i < itemsMasterList.length; i++) {
    //                    itemsMasterList[i].isActive(false);
    //                }
    //                self.isActive(true);
    //            }
    //        };

    //    }


    //    var isLoggedIn = false; //security.user.isAuthenticated();

    //    var sidebarVm = {
    //        items: ko.observableArray()
    //    };

    //    var itemsMasterList = [];

    //    var menuVm = {
    //        items: {
    //            right: ko.observableArray(),
    //            left: ko.observableArray()
    //        },
    //        current: ko.observable(null),
    //        sidebar: ko.observable(false),
    //        dark: ko.observable(isLoggedIn)
    //    };

    //    var menuContainers = {
    //        home: "home",
    //        privatehome: "private",
    //        game: "game",
    //        demo: "demo",
    //        profile: "profile",
    //        login: "login",
    //        signup: "signup",
    //        about: "about",
    //        other: "other"
    //    };

    //    function _getMenuConainer() {
    //        var currentPath = common.utilities.getCurrentPath();
    //        switch (currentPath) {
    //            case common.utilities.paths.home:
    //                return menuContainers.home;
    //            case common.utilities.paths.privatehome:
    //                return menuContainers.privatehome;
    //            case common.utilities.paths.game:
    //                return menuContainers.game;
    //            case common.utilities.paths.demo:
    //                return menuContainers.demo;
    //            case common.utilities.paths.profile:
    //                return menuContainers.profile;
    //            case common.utilities.paths.login:
    //                return menuContainers.login;
    //            case common.utilities.paths.signup:
    //                return menuContainers.signup;
    //            case common.utilities.paths.about:
    //                return menuContainers.about;
    //            default:
    //                return menuContainers.home;
    //        }
    //    }

    //    var curretMenuContainer = _getMenuConainer();

    //    var divider = new menuItem(null, null, false, false, false);
    //    divider.isDivider(true);


    //    if (curretMenuContainer === menuContainers.game) {// || curretMenuContainer === menuContainers.privatehome) {
    //        menuVm.sidebar(true);
    //    }

    //    var title = "";
    //    var url = "";
    //    var item = null;
    //    var isHomeActive = false, isDemoActive = false, isLoginActive = false, isSignupActive = false;

    //    title = "dashboard";
    //    if (curretMenuContainer === menuContainers.home) {
    //        url = "#";
    //        isHomeActive = true;
    //    }
    //    else {
    //        if (isLoggedIn) url = "/private";
    //        else url = "/home";
    //    }


    //    item = new menuItem(title, url, isHomeActive, false, false, "nc-icon nc-chart-pie-36");
    //    itemsMasterList.push(item);
    //    menuVm.items.left.push(item);
    //    sidebarVm.items.push(item);

    //    if (isHomeActive) menuVm.current(item);

    //    if (!isLoggedIn) {
    //        title = "secrets";
    //        if (curretMenuContainer === menuContainers.demo) {
    //            url = "#";
    //            isDemoActive = true;
    //        }
    //        else {
    //            url = "/home/secrets";
    //        }

    //        item = new menuItem(title, url, isDemoActive, false, false, "nc-icon nc-settings-90");
    //        itemsMasterList.push(item);
    //        menuVm.items.left.push(item);
    //        sidebarVm.items.push(item);

    //        if (isDemoActive) menuVm.current(item);
    //    }

    //    if (!isLoggedIn) {
    //        menuVm.items.right.push(new menuItem("log in", "/account/signin", false, false, true, "nc-icon nc-circle-09"));
    //    }
    //    else {

    //        //var accnt = new menuItem("account", "", false, true, false, "far fa-user-circle", null);

    //        //window.Harpocrates.loader.user.me(false, function (usr) {
    //        //    var user = window.Harpocrates.security.user.current.getProfile();
    //        //    if (user) {
    //        //        accnt.text(user.displayName());
    //        //    }
    //        //}, function (err) {

    //        //});


    //        //var profile = new menuItem("profile", "", false, false, false, "far fa-id-card");

    //        //profile.action = function () {
    //        //    //ensure profile is laoded
    //        //    window.Harpocrates.loader.user.me(false, function (usr) {
    //        //        var user = window.Harpocrates.security.user.current.getProfile();
    //        //        if (user) {
    //        //            user.actions.edit();
    //        //        }
    //        //    }, function (err) {

    //        //    });
    //        //};

    //        //accnt.children.push(profile);
    //        //accnt.children.push(divider);
    //        //accnt.children.push(new menuItem("sign out", "/account/signout", false, false, false, "fas fa-sign-out-alt"));

    //        //menuVm.items.right.push(accnt);
    //    }

    //    ko.applyBindings(menuVm, $('div.template-menu')[0]);
    //    ko.applyBindings(sidebarVm, $('div.template-sidebar')[0]);
    //});
    return {
        createNavViewModel: _createViewModel
    };
})(window.jQuery, window.Harpocrates.enums, window.Harpocrates.common, window.Harpocrates.security);