REQUIRE('chlk.BaseApp');

REQUIRE('chlk.controllers.AnnouncementController');
REQUIRE('chlk.controllers.AppsController');
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
REQUIRE('chlk.controllers.AdminsController');
REQUIRE('chlk.controllers.NotificationController');

REQUIRE('chlk.services.SearchService');
REQUIRE('chlk.templates.search.SiteSearch');

NAMESPACE('chlk', function (){

    /** @class chlk.StudentApp */
    CLASS(
        'StudentApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('feed');
                dispatcher.setDefaultControllerAction('list');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.STUDENT, 'Student'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/StudentSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
