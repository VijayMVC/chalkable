REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.calendar.announcement.WeekPage');
REQUIRE('chlk.activities.calendar.announcement.MonthPage');
REQUIRE('chlk.activities.calendar.announcement.DayPage');
REQUIRE('chlk.activities.calendar.announcement.MonthDayPopUp');
REQUIRE('chlk.activities.calendar.announcement.MonthDayLessonPlansPopUp');
REQUIRE('chlk.activities.calendar.announcement.WeekBarPopUp');
REQUIRE('chlk.activities.calendar.announcement.WeekDayPopUp');

REQUIRE('chlk.models.calendar.announcement.Month');
REQUIRE('chlk.models.classes.ClassesForTopBar');

NAMESPACE('chlk.controllers', function (){

    var Serializer = new chlk.lib.serialize.ChlkJsonSerializer;

    /** @class chlk.controllers.CalendarController*/
    CLASS(
        'CalendarController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.CalendarService, 'calendarService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',


        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate]],
        function indexAction(date_) {
            var classId = this.getCurrentClassId();
            return this.Redirect('calendar', 'month', [classId, date_]);
        },


        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function showMonthDayPopUpAction(date, classId_) {
            var result = this.calendarService
                .getMonthDayInfo(date)
                .attach(this.validateResponse_())
                .then(function(model){

                    model.setNoPlusButton(this.userIsStudent() || (this.userIsTeacher() && !this.getCurrentGradingPeriod().isDateInPeriod(date)));

                    var annCount = model.getAnnouncements().length + model.getSupplementalAnnouncements().length;
                    var adminAnnCount = model.getAdminAnnouncements().length;
                    if(!annCount && !adminAnnCount && model.isNoPlusButton())
                        return ria.async.BREAK;

                    model.setTarget(chlk.controls.getActionLinkControlLastNode());

                    if(classId_)
                        model.setSelectedClassId(classId_);
                    return model;
                }, this);
            return this.ShadeView(chlk.activities.calendar.announcement.MonthDayPopUp, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function showMonthDayLessonPlansAction(date, classId_) {
            var result = this.calendarService
                .getMonthDayInfo(date)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    return model;
                }, this);
            return this.ShadeView(chlk.activities.calendar.announcement.MonthDayLessonPlansPopUp, result);
        },


        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, Number]],
        function showDayPopUpAction(date, classId, periodOrder_){

            var model = this.calendarService.getAnnouncementPeriod(date, classId, periodOrder_);
            model.setDate(date);
            model.setSelectedClassId(classId);
            model.setTarget(chlk.controls.getActionLinkControlLastNode());
            return this.ShadeView(chlk.activities.calendar.announcement.WeekDayPopUp, new ria.async.DeferredData(model));
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, String]],
        function dayAction(classId_, date_, gradeLevels_){
            var result = this.calendarService
                .getDayWeekInfo(date_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var topModel = new chlk.models.classes.ClassesForTopBar(null, classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }, this);
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.DayPage, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function weekAction(classId_, date_){
            return this.week(date_, classId_);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, String]],
        function week(date_, classId_, gradeLevels_){
            var result = this.calendarService
                .getDayWeekInfo(date_, null, classId_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var topModel = new chlk.models.classes.ClassesForTopBar(null, classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }, this);
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.WeekPage, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function monthAction(classId_, date_){
            return this.month(date_, classId_);
        },

        //TODO: refactor
        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, String]],
        function month(date_, classId_, gradeLevels_){
            var result = this.calendarService
                .listForMonth(classId_, date_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var topModel = new chlk.models.classes.ClassesForTopBar(null, classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }, this);
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.MonthPage, result);
        }
    ])
});
