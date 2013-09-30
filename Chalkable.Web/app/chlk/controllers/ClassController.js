REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.AttendanceCalendarService');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassScheduleViewData');
REQUIRE('chlk.models.classes.ClassProfileAttendanceViewData');

REQUIRE('chlk.activities.classes.SummaryPage');
REQUIRE('chlk.activities.classes.ClassInfoPage');
REQUIRE('chlk.activities.classes.ClassSchedulePage');
REQUIRE('chlk.activities.classes.ClassProfileAttendancePage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ClassController */
    CLASS(
        'ClassController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.AttendanceCalendarService, 'attendanceCalendarService',

            [ria.mvc.Inject],
            chlk.services.AttendanceService, 'attendanceService',


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
            },
            [[chlk.models.id.ClassId]],
            ria.async.Future, function attendanceAction(classId){
                var res =
                    ria.async.wait(
                        this.classService.getAttendance(classId),
                        this.attendanceCalendarService.getClassAttendancePerMonth(classId, null)
                    )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var mp = this.getCurrentMarkingPeriod();
                        var attCalendar = new chlk.models.calendar.attendance.ClassAttendanceMonthCalendar(null
                            , mp.getStartDate(), mp.getEndDate(), result[1], classId);
                        return new chlk.models.classes.ClassProfileAttendanceViewData(result[0], attCalendar);
                    }, this);
                return this.PushView(chlk.activities.classes.ClassProfileAttendancePage, res);
            },
            [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
            ria.async.Future, function attendanceMonthAction(date, classId){
                var res =  this.attendanceCalendarService.getClassAttendancePerMonth(classId, date)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var mp = this.getCurrentMarkingPeriod();
                        return new chlk.models.calendar.attendance.ClassAttendanceMonthCalendar(date
                            , mp.getStartDate(), mp.getEndDate(), data, classId);
                    }, this);
                return this.UpdateView(chlk.activities.classes.ClassProfileAttendancePage, res);
            }
//            ,
//            [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
//            ria.async.Future, function attendanceListAction(date, classId){
//                var res = this.attendanceService.getClassList(classId, date)
//                    .attach(this.validateResponse_())
//                    .then(function(data){
//
//                    });
//                return this.PushView(chlk.activities.classes.ClassProfileAttendanceListPage, res);
//            }
        ]);
});
