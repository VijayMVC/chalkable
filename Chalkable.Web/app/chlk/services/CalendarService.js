REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');
REQUIRE('chlk.models.calendar.announcement.MonthItem');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.models.calendar.announcement.WeekItem');
REQUIRE('chlk.models.calendar.announcement.CalendarDayItem');
REQUIRE('chlk.models.calendar.TeacherSettingsCalendarDay');
REQUIRE('chlk.models.calendar.announcement.DayItem');
REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.calendar.ListForWeekCalendarItem');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    var Serializer = new chlk.lib.serialize.ChlkJsonSerializer;

    /** @class chlk.services.CalendarService */
    CLASS(
        'CalendarService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.common.ChlkDate]],
            ria.async.Future, function getListForWeek(date_) {
                return this.get('AnnouncementCalendar/ListForWeek.json', ArrayOf(chlk.models.calendar.ListForWeekCalendarItem) , {
                    date: date_ && date_.toString('mm-dd-yy')
                });
            },

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
            ria.async.Future, function listByDateRange(startDate_, endDate_, classId_) {
                return this.get('AnnouncementCalendar/ListByDateRange.json', ArrayOf(chlk.models.announcement.BaseAnnouncementViewData) , {
                    startDate: startDate_ && startDate_.toString('mm-dd-yy'),
                    endDate: endDate_ && endDate_.toString('mm-dd-yy'),
                    classId: classId_ && classId_.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, String, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function listForMonth(classId_, date_, gradeLevels_, schoolPersonId_) {
                return this.get('AnnouncementCalendar/List.json', ArrayOf(chlk.models.calendar.announcement.MonthItem), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy'),
                    gradeLevelIds : gradeLevels_ && gradeLevels_.toString(),
                    schoolPersonId : schoolPersonId_ && schoolPersonId_.valueOf()
                }).then(function(model){
                    this.getContext().getSession().set(ChlkSessionConstants.MONTH_CALENDAR_DATA, model);
                    return this.prepareMonthData(model, date_);
                }, this);
            },

            [[chlk.models.common.ChlkDate]],
            ria.async.Future, function getMonthDayInfo(date){
                var monthCalendarData = this.getContext().getSession().get(ChlkSessionConstants.MONTH_CALENDAR_DATA, []), res = null;
                monthCalendarData.forEach(function(day){
                    if(day.getDate().isSameDay(date))
                        res = day;
                });
                return new ria.async.DeferredData(res);
            },

            [[chlk.models.common.ChlkDate, Number]],
            ria.async.Future, function getDayPopupInfo(date, periodNumber){
                var dayCalendarData = this.getContext().getSession().get(ChlkSessionConstants.DAY_CALENDAR_DATA, []), res = null;
                dayCalendarData.forEach(function(day){
                    if(day.getDate().isSameDay(date))
                        res = day;
                });
                res = res.getCalendarDayItems()[periodNumber];
                return new ria.async.DeferredData(res);
            },

            [[chlk.models.common.ChlkDate, Number]],
            ria.async.Future, function getWeekDayInfo(date, periodNumber_){
                var weekCalendarData = this.getContext().getSession().get(ChlkSessionConstants.WEEK_CALENDAR_DATA, []), res = null;
                weekCalendarData.forEach(function(day){
                    if(day.getDate().isSameDay(date))
                        res = day;
                });
                if(periodNumber_ >= 0)
                    res = res.getAnnouncementPeriods()[periodNumber_];
                return new ria.async.DeferredData(res);
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getTeacherClassWeek(classId_, date_) {
                return this.get('AnnouncementCalendar/TeacherClassWeek.json', ArrayOf(chlk.models.calendar.TeacherSettingsCalendarDay), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy')
                });
            },

            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getDayInfo(date_, schoolPersonId_) {
                return this.get('AnnouncementCalendar/Day.json', ArrayOf(chlk.models.calendar.announcement.DayItem), {
                    date: date_ && date_.toString('mm-dd-yy'),
                    schoolPersonId: schoolPersonId_ && schoolPersonId_.valueOf()
                }).then(function(model){
                    this.saveDayCalendarDataInSession(model);
                    return this.prepareDayData(model, date_);
                }.bind(this));
            },
            [[ArrayOf(chlk.models.calendar.announcement.DayItem)]],
            function saveDayCalendarDataInSession(model){
                this.getContext().getSession().set(ChlkSessionConstants.DAY_CALENDAR_DATA, model);
            },

            [[chlk.models.common.ChlkDate, String]],
            ria.async.Future, function getAdminDay(date_, gradeLevelsIds_) {
                return this.get('AnnouncementCalendar/AdminDay.json', chlk.models.calendar.announcement.AdminDayCalendar, {
                    day: date_ && date_.toString('mm-dd-yy'),
                    gradeLevelsIds: gradeLevelsIds_
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, String, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getWeekInfo(classId_, date_, gradeLevels_, schoolPersonId_) {
                return this.get('AnnouncementCalendar/Week.json', ArrayOf(chlk.models.calendar.announcement.WeekItem), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy'),
                    gradeLevelIds : gradeLevels_,
                    schoolPersonId : schoolPersonId_ && schoolPersonId_.valueOf()
                }).then(function(data){
                    return this.prepareWeekData(data, date_);
                }.bind(this));
            },

            //PREPARING INFO

            chlk.models.common.Role, function getCurrentRole(){
                return this.getContext().getSession().get(ChlkSessionConstants.USER_ROLE);
            },

            [[chlk.models.common.RoleEnum]],
            Boolean, function userInRole(roleId){
                return this.getCurrentRole().getRoleId() == roleId;
            },

            Boolean, function userIsAdmin(){
                return this.userInRole(chlk.models.common.RoleEnum.ADMINEDIT) ||
                    this.userInRole(chlk.models.common.RoleEnum.ADMINGRADE) ||
                    this.userInRole(chlk.models.common.RoleEnum.ADMINVIEW);
            },

            [[ArrayOf(chlk.models.calendar.announcement.MonthItem), chlk.models.common.ChlkDate]],
            chlk.models.calendar.announcement.Month, function prepareMonthData(days, date_){
                var isAdmin = this.userIsAdmin();
                var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD);
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
                            itemsObject[typeName] = showSubject ? title + ' ' + typeName : title || typeName;
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
                    var today = new chlk.models.common.ChlkSchoolYearDate();
                    day.setTodayClassName((today.format('mm-dd-yy') == day.getDate().format('mm-dd-yy')) ? 'today' : '');
                    day.setRole(isAdmin ? 'admin' : 'no-admin');
                    day.setAnnLimit(isAdmin ? 7 : 3);
                    day.setClassName((day.isCurrentMonth() && date >= mpStartDate.getDate() &&
                        date <= mpEndDate.getDate()) ? '' : 'not-current-month');
                }.bind(this));
                return new chlk.models.calendar.announcement.Month(date_, mpStartDate, mpEndDate, days);
                //return res;
            },

            [[ArrayOf(chlk.models.calendar.announcement.WeekItem), chlk.models.common.ChlkDate]],
            chlk.models.calendar.announcement.Week, function prepareWeekData(data, date_){
                var max = 0, index = 0, kil=0, empty= 0, empty2=0, sun, date, startArray = [], endArray = [];
                var len = data.length;
                date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
                if(len < 7){
                    var dt = len ? data[0].getDate() : date_;
                    kil = dt.getDate().getDay();
                    sun = dt.add(chlk.models.common.ChlkDateEnum.DAY, -kil);
                    empty = 7 - len;
                    empty2 = dt.getDate().getDay();
                }
                len = 7;

                function pushEmptyItem(array, i){
                    date = sun.add(chlk.models.common.ChlkDateEnum.DAY, i);
                    array.push(Serializer.deserialize({
                        announcementperiods: [],
                        announcements: [],
                        date: date.getDate(),
                        day: date.getDate().getDate(),
                        //index: i,
                        sunday: date.format('DD') == Msg.Sunday
                    }, chlk.models.calendar.announcement.WeekItem));
                }
                for(var i = 0; i < len; i++){
                    if(kil){
                        kil--;
                        pushEmptyItem(startArray, i);
                    }else{
                        if(empty && (!data[i-empty2])){
                            empty--;
                            pushEmptyItem(endArray, i);
                        }else{
                            if(data[i-empty2]){
                                //data[i-empty2].setIndex(i-empty2);
                                //data.normal = true;
                                if(max < data[i-empty2].getAnnouncementPeriods().length){
                                    max = data[i-empty2].getAnnouncementPeriods().length;
                                    index = i-empty2;
                                }
                            }
                        }
                    }
                }

                function pushEmptyPeriods(item, periodBeginIndex, periods, biggestItem){
                    for(var j = periodBeginIndex; j < max; j++){
                        var period = Object.extend({}, biggestItem.getAnnouncementPeriods()[j].getPeriod());
                        var newPeriod = Object.extend({}, period);
                        //newPeriod.setDate(new chlk.models.common.ChlkDate(getDate(item.date)));
                        //newPeriod.setDay(item.day);
                        periods.push(Serializer.deserialize({
                            announcements: [],
                            period: newPeriod
                        }, chlk.models.announcement.AnnouncementPeriod));
                    };
                    return periods;
                }
                var dt = new chlk.models.common.ChlkSchoolYearDate();
                for(i = 0; i < data.length; i++){
                    var announcementperiods = data[i].getAnnouncementPeriods();
                    //data[i].setSunday(data[i].getDate().format('DD') == Msg.Sunday);
                    data[i].setTodayClassName(dt.format('dd-mm-yy') == data[i].getDate().format('dd-mm-yy') ? 'today' : '');
                    if(!(announcementperiods instanceof Array)){
                        data[i].setAnnouncementPeriods([]);
                    }
                    announcementperiods.forEach(function(item, index){
                        if(item.getPeriod())
                            item.setIndex(index);
                    });
                    if((announcementperiods.length < max)){
                        var begin = announcementperiods.length;
                        pushEmptyPeriods(data[i], begin, announcementperiods, data[index]);
                    }
                }
                startArray.forEach(function (item){pushEmptyPeriods(item,  0, item.getAnnouncementPeriods(), data[index]);});
                endArray.forEach(function (item){pushEmptyPeriods(item, 0, item.getAnnouncementPeriods(), data[index]);});
                var res = startArray.concat(data).concat(endArray);
                this.getContext().getSession().set(ChlkSessionConstants.WEEK_CALENDAR_DATA, res);
                var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD);
                var startDate = markingPeriod.getStartDate();
                var endDate = markingPeriod.getEndDate();
                return new chlk.models.calendar.announcement.Week(date_, startDate, endDate, res);
                //return res;
            },

            [[ArrayOf(chlk.models.calendar.announcement.DayItem), chlk.models.common.ChlkDate]],
            chlk.models.calendar.announcement.Day, function prepareDayData(days, date_){
                var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD);
                var startDate = markingPeriod.getStartDate();
                var endDate = markingPeriod.getEndDate();
                return new chlk.models.calendar.announcement.Day(date_, startDate, endDate, days);
            }
        ])
});