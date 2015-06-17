REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.calendar.announcement.WeekPage');
REQUIRE('chlk.activities.calendar.announcement.MonthPage');
REQUIRE('chlk.activities.calendar.announcement.DayPage');
REQUIRE('chlk.activities.calendar.announcement.MonthDayPopUp');
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
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function showMonthDayPopUpAction(date, classId_) {
            var result = this.calendarService
                .getMonthDayInfo(date)
                .attach(this.validateResponse_())
                .then(function(model){
                    var dateRangeObj = this.calendarService.getDefaultCalendarDateRange();
                    var startDate = dateRangeObj.startDate;
                    var endDate = dateRangeObj.endDate;
                    if(date.getDate() < startDate.getDate() || date.getDate() > endDate.getDate())
                        if(model.getAnnouncements().length || model.getItems().length)
                            model.setNoPlusButton(true);
                        else
                            return ria.async.BREAK;
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    if(classId_)
                        model.setSelectedClassId(classId_);
                    return model;
                }, this);
            return this.ShadeView(chlk.activities.calendar.announcement.MonthDayPopUp, result);
        },

        /*[chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, Number, chlk.models.id.ClassId]],
        function showDayPopUpAction(date, periodNumber, classId_) {
            var result = this.calendarService
                .getDayPopupInfo(date, periodNumber)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    model.setDate(date);
                    if(classId_)
                        model.setSelectedClassId(classId_);
                    return model;
                });
            return this.ShadeView(chlk.activities.calendar.announcement.DayPeriodPopUp, result);
        },*/

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, chlk.models.id.ClassId, Number]],
        function showWeekBarPopUpAction(date, periodClassId_, classId_, periodOrder_) {
            var model = this.calendarService
                .getWeekDayInfo(date, periodClassId_, periodOrder_);

            var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD);
            var mpStartDate = markingPeriod.getStartDate();
            var mpEndDate = markingPeriod.getEndDate();
            if(date.getDate() < mpStartDate.getDate() || date.getDate() > mpEndDate.getDate())
                if(model.getAnnouncements().length)
                    model.setNoPlusButton(true);
                else
                    return null;

            Assert(model);

            model.setTarget(chlk.controls.getActionLinkControlLastNode());
            if(periodClassId_ != undefined)
                model.setDate(date);

            if(classId_)
                model.setSelectedClassId(classId_);

            var result = new ria.async.DeferredData(model);
            if(periodClassId_ != undefined)
                return this.ShadeView(chlk.activities.calendar.announcement.WeekDayPopUp, result);

            return this.ShadeView(chlk.activities.calendar.announcement.WeekBarPopUp, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, String]],
        function dayAction(classId_, date_, gradeLevels_){
            var result = this.calendarService
                .getDayWeekInfo(date_)
                .attach(this.validateResponse_());
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
