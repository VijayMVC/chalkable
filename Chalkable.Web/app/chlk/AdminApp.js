REQUIRE('chlk.BaseApp');

REQUIRE('chlk.controllers.AnnouncementController');
REQUIRE('chlk.controllers.AppMarketController');
REQUIRE('chlk.controllers.FeedController');
REQUIRE('chlk.controllers.AttendanceController');
REQUIRE('chlk.controllers.AccountController');
REQUIRE('chlk.controllers.CalendarController');
REQUIRE('chlk.controllers.ClassController');
REQUIRE('chlk.controllers.GradingController');
REQUIRE('chlk.controllers.SettingsController');
REQUIRE('chlk.controllers.SetupController');
REQUIRE('chlk.controllers.StudentsController');
REQUIRE('chlk.controllers.TeachersController');
REQUIRE('chlk.controllers.MessageController');
REQUIRE('chlk.controllers.DisciplineController');
REQUIRE('chlk.services.SearchService');
REQUIRE('chlk.templates.search.SiteSearch');

NAMESPACE('chlk', function (){

    /** @class chlk.AdminApp */
    CLASS(
        'AdminApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('feed');
                dispatcher.setDefaultControllerAction('admin');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
               // var currentPerson = this.getCurrentPerson();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.ADMINGRADE, 'Admin'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/AdminSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
