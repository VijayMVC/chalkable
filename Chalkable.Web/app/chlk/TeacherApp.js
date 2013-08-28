REQUIRE('chlk.BaseApp');

REQUIRE('chlk.controllers.AnnouncementController');
REQUIRE('chlk.controllers.FeedController');
REQUIRE('chlk.controllers.AccountController');
REQUIRE('chlk.controllers.CalendarController');
REQUIRE('chlk.controllers.SettingsController');
REQUIRE('chlk.controllers.SetupController');
REQUIRE('chlk.controllers.StudentsController');
REQUIRE('chlk.controllers.TeachersController');
REQUIRE('chlk.controllers.MessageController');
REQUIRE('chlk.services.SearchService');
REQUIRE('chlk.templates.search.SiteSearch');

NAMESPACE('chlk', function (){

    /** @class chlk.TeacherApp */
    CLASS(
        'TeacherApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('feed');
                dispatcher.setDefaultControllerAction('list');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.TEACHER, 'Teacher'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/TeacherSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
