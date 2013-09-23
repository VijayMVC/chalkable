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

    var Serializer = new ria.serialize.JsonSerializer;

    /** @class chlk.controllers.CalendarController*/
    CLASS(
        'CalendarController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.CalendarService, 'calendarService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],

        [[chlk.models.common.ChlkDate]],
        VOID, function showMonthDayPopUpAction(date) {
            var result = this.calendarService.getMonthDayInfo(date)
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    return model;
                });
            return this.ShadeView(chlk.activities.calendar.announcement.MonthDayPopUp, result);
        },

        [[chlk.models.common.ChlkDate, Number]],
        VOID, function showDayPopUpAction(date, periodNumber) {
            var result = this.calendarService.getDayPopupInfo(date, periodNumber)
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    model.setDate(date);
                    return model;
                });
            return this.ShadeView(chlk.activities.calendar.announcement.DayPeriodPopUp, result);
        },

        [[chlk.models.common.ChlkDate, Number]],
        VOID, function showWeekBarPopUpAction(date, periodNumber_) {
            var result = this.calendarService.getWeekDayInfo(date, periodNumber_)
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

        [[chlk.models.common.ChlkDate]],
        function dayAction(date_){
            var result = this.calendarService
                .getDayInfo(date_)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.calendar.announcement.DayPage, result);
        },

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
            return this.PushView(chlk.activities.calendar.announcement.AdminDayCalendarPage, result);
        },

        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, String]],
        function weekAction(date_, classId_, gradeLevels_){
            var markingPeriod = this.getContext().getSession().get('markingPeriod');
            var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
            var glsInputData = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevels_);

            var result = this.calendarService
                .getWeekInfo(classId_, date_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(days){
                    var startDate = markingPeriod.getStartDate();
                    var endDate = markingPeriod.getEndDate();
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                    var model = new chlk.models.calendar.announcement.Week(date_, startDate, endDate, days, topModel, glsInputData);
                    model.setAdmin(this.userIsAdmin());
                    return new ria.async.DeferredData(model);
                }.bind(this));

            return this.PushView(chlk.activities.calendar.announcement.WeekPage, result);
        },

        //TODO: refactor
        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId, String]],
        function monthAction(date_, classId_, gradeLevels_){
            var markingPeriod = this.getContext().getSession().get('markingPeriod');
            var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
            var glsInputData = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevels_);
            var result = this.calendarService
                .listForMonth(classId_, date_, gradeLevels_)
                .attach(this.validateResponse_())
                .then(function(days){
                    var mpStartDate = markingPeriod.getStartDate();
                    var mpEndDate = markingPeriod.getEndDate();
                    days.forEach(function(day){
                        var itemsArray = [], itemsObject = {};
                        var items = day.getItems();
                        for (var i = 0; i < items.length; i++){
                            var typeName = items[i].getAnnouncementTypeName();
                            var title = items[i].getTitle();
                            var typeId = items[i].getAnnouncementTypeId();
                            var typesEnum = chlk.models.announcement.AnnouncementTypeEnum;
                            if (itemsObject[typeName]){
                                if(typeof itemsObject[typeName] == 'number'){
                                    itemsObject[typeName] = itemsObject[typeName] + 1;
                                }
                                else{
                                    itemsObject[typeName] = 2;
                                }
                            }
                            else{
                                var showSubject = title !== null && typeId == typesEnum.ADMIN || typeId == typesEnum.ANNOUNCEMENT;
                                itemsObject[typeName] = showSubject ? title + ' ' + typeName : typeName;
                            }
                        }

                        for (var a in itemsObject){
                            if (typeof itemsObject[a] == "number"){
                                var count = itemsObject[a];
                                itemsArray.push({ count: count, title: count + ' ' + a + 's'});
                            }
                            else{
                                itemsArray.push({title: itemsObject[a], count: 1});
                            }
                        }
                        day.setItemsArray(itemsArray);


                        var date = day.getDate().getDate();
                        var today = new chlk.models.common.ChlkDate(new Date());
                        day.setTodayClassName((today.format('mm-dd-yy') == day.getDate().format('mm-dd-yy')) ? 'today' : '');
                        day.setRole(this.userIsAdmin() ? 'admin' : 'no-admin');
                        day.setAnnLimit(this.userIsAdmin() ? 7 : 3);
                        day.setClassName((day.isCurrentMonth() && date >= mpStartDate.getDate() &&
                            date <= mpEndDate.getDate()) ? '' : 'not-current-month');
                    }.bind(this));
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                    var model = new chlk.models.calendar.announcement.Month(date_, mpStartDate, mpEndDate, days, topModel, glsInputData);
                    model.setAdmin(this.userIsAdmin());
                    return new ria.async.DeferredData(model);
                }.bind(this));

            return this.PushView(chlk.activities.calendar.announcement.MonthPage, result);
        }
    ])
});
