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




        //TODO: refactor
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function weekAction(date_, classId_){
            var markingPeriod = this.getContext().getSession().get('markingPeriod');
            var today = new chlk.models.common.ChlkDate(new Date());
            var date = date_ || today;
            var dayNumber = date.getDate().getDay(), sunday = date, saturday = date;
            if(dayNumber){
                sunday = date.add(chlk.models.common.ChlkDateEnum.DAY, -dayNumber);
            }
            if(dayNumber !=6)
                saturday = sunday.add(chlk.models.common.ChlkDateEnum.DAY, 6);
            var title = sunday.format('MM d - ');
            title = title + (sunday.format('M') == saturday.format('M') ? saturday.format('d') : saturday.format('M d'));
            var prevDate = sunday.add(chlk.models.common.ChlkDateEnum.DAY, -1);
            var nextDate = saturday.add(chlk.models.common.ChlkDateEnum.DAY, 1);
            var result = this.calendarService
                .getWeekInfo(classId_, date_)
                .attach(this.validateResponse_())
                .then(function(days){
                    var model = new chlk.models.calendar.announcement.Week();
                    model.setCurrentTitle(title);
                    model.setCurrentDate(date);
                    var startDate = markingPeriod.getStartDate();
                    var endDate = markingPeriod.getEndDate();
                    if(prevDate.format('yy-mm-dd') >= startDate.format('yy-mm-dd')){
                        model.setPrevDate(prevDate);
                    }
                    if(nextDate.format('yy-mm-dd') <= endDate.format('yy-mm-dd')){
                        model.setNextDate(nextDate);
                    }
                    model.setItems(days);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(false);
                    classId_ && topModel.setSelectedItemId(classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }.bind(this));

            return this.PushView(chlk.activities.calendar.announcement.WeekPage, result);
        },

        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function monthAction(date_, classId_){
            var markingPeriod = this.getContext().getSession().get('markingPeriod');
            var today = new chlk.models.common.ChlkDate(new Date());
            var date = date_ || today;
            var year = date.getDate().getFullYear();
            var month = date.getDate().getMonth();
            var day = date.getDate().getDate();
            var prevMonth = month ? month - 1 : 11;
            var prevYear = month ? year : year - 1;
            var prevDate = new Date(prevYear, prevMonth, day);
            var nextMonth = month == 11 ? 0 : month + 1;
            var nextYear = month == 11 ? year + 1 : year;
            var nextDate = new Date(nextYear, nextMonth, day);
            var result = this.calendarService
                .listForMonth(classId_, date_)
                .attach(this.validateResponse_())
                .then(function(days){
                    var model = new chlk.models.calendar.announcement.Month();
                    model.setCurrentTitle(date.format('MM'));
                    model.setCurrentDate(date);
                    var startDate = markingPeriod.getStartDate().getDate();
                    var endDate = markingPeriod.getEndDate().getDate();
                    if(prevDate >= startDate){
                        model.setPrevDate(new chlk.models.common.ChlkDate(prevDate));
                    }else{
                        if(startDate.getMonth() != date.getDate().getMonth())
                            model.setPrevDate(markingPeriod.getStartDate());
                    }
                    if(nextDate <= endDate){
                        model.setNextDate(new chlk.models.common.ChlkDate(nextDate));
                    }else{
                        if(nextDate.getMonth() != date.getDate().getMonth())
                            model.setNextDate(markingPeriod.getEndDate());
                    }
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

                        day.setTodayClassName((today.format('mm-dd-yy') == day.getDate().format('mm-dd-yy')) ? 'today' : '');
                        day.setRole(this.userIsAdmin() ? 'admin' : 'no-admin');
                        day.setAnnLimit(this.userIsAdmin() ? 7 : 3);
                        day.setClassName((day.isCurrentMonth() && date >= markingPeriod.getStartDate().getDate() &&
                            date <= markingPeriod.getEndDate().getDate()) ? '' : 'not-current-month');
                    }.bind(this));
                    model.setItems(days);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(false);
                    classId_ && topModel.setSelectedItemId(classId_);
                    model.setTopData(topModel);
                    return new ria.async.DeferredData(model);
                }.bind(this));

            return this.PushView(chlk.activities.calendar.announcement.MonthPage, result);
        }
    ])
});
