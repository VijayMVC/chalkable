REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.activities.calendar.announcement.MonthPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.CalendarController*/
    CLASS(
        'CalendarController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('calendar')],
        function monthTeacherAction(){
            var result = this.announcementService
                .listForMonth()
                .attach(this.validateResponse_())
                .then(function(data){
                    return new ria.async.DeferredData(new chlk.models.calendar.announcement.Month(data));
                });

            return this.PushView(chlk.activities.calendar.announcement.MonthPage, result);
        }
    ])
});
