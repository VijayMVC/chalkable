REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.calendar.announcement.WeekPage');
REQUIRE('chlk.activities.calendar.announcement.MonthPage');
REQUIRE('chlk.activities.calendar.announcement.DayPage');
REQUIRE('chlk.activities.calendar.announcement.MonthDayPopUp');
REQUIRE('chlk.activities.calendar.announcement.WeekBarPopUp');
REQUIRE('chlk.activities.calendar.announcement.WeekDayPopUp');
REQUIRE('chlk.activities.calendar.announcement.DayPeriodPopUp');
REQUIRE('chlk.activities.calendar.announcement.AdminDayCalendarPage');

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
        function showMonthDayPopUpAction(date) {
            var result = this.calendarService
                .getMonthDayInfo(date)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    return model;
                });
            return this.ShadeView(chlk.activities.calendar.announcement.MonthDayPopUp, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, Number]],
        function showDayPopUpAction(date, periodNumber) {
            var result = this.calendarService
                .getDayPopupInfo(date, periodNumber)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    model.setDate(date);
                    return model;
                });
            return this.ShadeView(chlk.activities.calendar.announcement.DayPeriodPopUp, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, Number]],
        function showWeekBarPopUpAction(date, periodNumber_) {
            var result = this.calendarService
                .getWeekDayInfo(date, periodNumber_)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    if(periodNumber_ >= 0)
                        model.setDate(date);
                    return model;
                });
            if(periodNumber_ >= 0)
                return this.ShadeView(chlk.activities.calendar.announcement.WeekDayPopUp, result);
            return this.ShadeView(chlk.activities.calendar.announcement.WeekBarPopUp, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, String]],
        function dayAction(date_, gradeLevels_){
            var result = this.calendarService
                .getDayInfo(date_)
                .attach(this.validateResponse_());
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.DayPage, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, String]],
        function dayAdminAction(date_, gradeLevels_){
            var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
            var result = this.calendarService
                .getAdminDay(date_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(data){
                    var glsInputData = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevels_);
                    data.setGradeLevelsInputData(glsInputData);
                    return data;
                }, this);
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.AdminDayCalendarPage, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, String]],
        function weekAdminAction(date_, gradeLevels_){
            return this.week(date_, null, gradeLevels_);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function weekAction(date_, classId_){
            return this.week(date_, classId_);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, String]],
        function week(date_, classId_, gradeLevels_){
            var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
            var glsInputData = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevels_);

            var result = this.calendarService
                .getWeekInfo(classId_, date_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                    model.setTopData(topModel);
                    model.setGradeLevelsForToolBar(glsInputData);
                    model.setAdmin(this.userIsAdmin());
                    return new ria.async.DeferredData(model);
                }, this);
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.WeekPage, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, String]],
        function monthAdminAction(date_, gradeLevels_){
            return this.month(date_, null, gradeLevels_);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function monthAction(date_, classId_){
            return this.month(date_, classId_);
        },

        //TODO: refactor
        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, String]],
        function month(date_, classId_, gradeLevels_){
            var markingPeriod = this.getCurrentMarkingPeriod();
            var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
            var glsInputData = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevels_);
            var result = this.calendarService
                .listForMonth(classId_, date_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                    model.setTopData(topModel);
                    model.setGradeLevelsForToolBar(glsInputData);
                    model.setAdmin(this.userIsAdmin());
                    return new ria.async.DeferredData(model);
                }, this);
            return this.PushOrUpdateView(chlk.activities.calendar.announcement.MonthPage, result);
        }
    ])
});
