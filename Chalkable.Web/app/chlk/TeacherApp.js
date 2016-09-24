REQUIRE('chlk.BaseApp');

REQUIRE('chlk.controllers.LpGalleryCategoryController');
REQUIRE('chlk.controllers.LessonPlanGalleryController');
REQUIRE('chlk.controllers.AnnouncementController');
REQUIRE('chlk.controllers.AppsController');
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
REQUIRE('chlk.controllers.NotificationController');
REQUIRE('chlk.controllers.StandardController');
REQUIRE('chlk.controllers.SchoolsController');
REQUIRE('chlk.controllers.GroupController');
REQUIRE('chlk.services.SearchService');
REQUIRE('chlk.templates.search.SiteSearch');
REQUIRE('chlk.controllers.AttachController');
REQUIRE('chlk.controllers.AnnouncementCommentController');
REQUIRE('chlk.controllers.ReportingController');


NAMESPACE('chlk', function (){

    /** @class chlk.TeacherApp */
    CLASS(
        'TeacherApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('feed');
                dispatcher.setDefaultControllerAction('doToList');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.TEACHER, 'Teacher'));
                return session;
            },

            OVERRIDE, ria.async.Future, function onStart_() {
                var sidebarOptions = this.prepareSideBarOptions_();
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/TeacherSidebar.jade')(sidebarOptions))
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
