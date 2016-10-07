REQUIRE('chlk.BaseApp');

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
REQUIRE('chlk.controllers.StandardController');
REQUIRE('chlk.controllers.TeachersController');
REQUIRE('chlk.controllers.DisciplineController');
REQUIRE('chlk.controllers.GroupController');
REQUIRE('chlk.controllers.DistrictController');
REQUIRE('chlk.controllers.SchoolsController');
REQUIRE('chlk.controllers.AttachController');
REQUIRE('chlk.controllers.LessonPlanGalleryController');
REQUIRE('chlk.controllers.LpGalleryCategoryController');
REQUIRE('chlk.controllers.AnnouncementCommentController');
REQUIRE('chlk.controllers.ReportingController');

REQUIRE('chlk.services.SearchService');
REQUIRE('chlk.templates.search.SiteSearch');

NAMESPACE('chlk', function (){

    /** @class chlk.DistrictAdminApp */
    CLASS(
        'DistrictAdminApp', EXTENDS(chlk.BaseApp), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('feed');
                dispatcher.setDefaultControllerAction('doToList');
                return dispatcher;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                // var currentPerson = this.getCurrentPerson();
                session.set('role', new chlk.models.common.Role(chlk.models.common.RoleEnum.DISTRICTADMIN, 'DistrictAdmin'));
                return session;
            },

            //TODO: make sideBarOption model and template
            OVERRIDE, ria.async.Future, function onStart_() {
                var isStudyCenterEnabled = this.getContext().getSession().get(ChlkSessionConstants.STUDY_CENTER_ENABLED, false);
                var isAssessmentEnabled = this.getContext().getSession().get(ChlkSessionConstants.ASSESSMENT_ENABLED, false);
                var isClassesEnabled = this.getContext().getSession().get(ChlkSessionConstants.USER_CLAIMS, []).filter(function(item){
                    return item.hasPermission(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN)
                        || item.hasPermission(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN);
                }).length > 0;
                var isPeopleEnabled = this.getContext().getSession().get(ChlkSessionConstants.USER_CLAIMS, []).filter(function(item){
                        return item.hasPermission(chlk.models.people.UserPermissionEnum.VIEW_STUDENT)
                            || item.hasPermission(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_STUDENTS);
                    }).length > 0;

                var sidebarOptions = {
                    isAppStoreEnabled: isStudyCenterEnabled,
                    isAssessmentEnabled: isAssessmentEnabled || isStudyCenterEnabled,
                    isClassesEnabled: isClassesEnabled,
                    isPeopleEnabled: isPeopleEnabled
                };
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/DistrictAdminSideBar.jade')(sidebarOptions))
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
