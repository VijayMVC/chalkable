REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassScheduleViewData');

REQUIRE('chlk.activities.classes.SummaryPage');
REQUIRE('chlk.activities.classes.ClassInfoPage');
REQUIRE('chlk.activities.classes.ClassSchedulePage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ClassController */
    CLASS(
        'ClassController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [[chlk.models.id.ClassId]],
            function detailsAction(classId){
                var result = this.classService
                    .getSummary(classId)
                    .attach(this.validateResponse_());
                return this.PushView(chlk.activities.classes.SummaryPage, result);
            },

            [[chlk.models.id.ClassId]],
            function infoAction(classId){
                var res = this.classService
                    .getInfo(classId).
                    attach(this.validateResponse_());
                return this.PushView(chlk.activities.classes.ClassInfoPage, res);
            },

            [[chlk.models.id.ClassId]],
            function scheduleAction(classId){
                var res = this.schedule_(classId, null);
                return this.PushView(chlk.activities.classes.ClassSchedulePage, res);
            },
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            function scheduleUpdateAction(classId, date){
                var res = this.schedule_(classId, date)
                    .then(function(data){ return data.getScheduleCalendar(); });
                return this.UpdateView(chlk.activities.classes.ClassSchedulePage, res);
            },
            ria.async.Future, function schedule_(classId, date_){
                var mp = this.getCurrentMarkingPeriod();
                var res = this.classService
                    .getSchedule(classId, date_)
                    .attach(this.validateResponse_())
                    .then(function (data){
                        var scheduleCalendar = new chlk.models.calendar.announcement.Day(date_, mp.getStartDate()
                            , mp.getEndDate(), data.getCalendarDayItems());
                        return new chlk.models.classes.ClassScheduleViewData(data.getClazz(), scheduleCalendar);
                    });
                return res;
            }
        ]);
});
