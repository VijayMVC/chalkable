REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.MonthItem');
REQUIRE('chlk.models.calendar.announcement.Week');
REQUIRE('chlk.models.calendar.announcement.WeekItem');
REQUIRE('chlk.models.calendar.announcement.CalendarDayItem');
REQUIRE('chlk.models.calendar.TeacherSettingsCalendarDay');
REQUIRE('chlk.models.calendar.announcement.DayItem');
REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.id.SchoolPersonId');

REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    var Serializer = new ria.serialize.JsonSerializer;

    /** @class chlk.services.CalendarService */
    CLASS(
        'CalendarService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function listForMonth(classId_, date_) {
                return this.get('AnnouncementCalendar/List.json', ArrayOf(chlk.models.calendar.announcement.MonthItem), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy')
                }).then(function(model){
                    this.getContext().getSession().set('monthCalendarData', model);
                    return model;
                }, this);
            },

            [[chlk.models.common.ChlkDate]],
            ria.async.Future, function getMonthDayInfo(date){
                var monthCalendarData = this.getContext().getSession().get('monthCalendarData', []), res = null;
                monthCalendarData.forEach(function(day){
                    if(day.getDate().isSameDay(date))
                        res = day;
                });
                return new ria.async.DeferredData(res);
            },

            [[chlk.models.common.ChlkDate, Number]],
            ria.async.Future, function getDayPopupInfo(date, periodNumber){
                var dayCalendarData = this.getContext().getSession().get('dayCalendarData', []), res = null;
                dayCalendarData.forEach(function(day){
                    if(day.getDate().isSameDay(date))
                        res = day;
                });
                res = res.getCalendarDayItems()[periodNumber];
                return new ria.async.DeferredData(res);
            },

            [[chlk.models.common.ChlkDate, Number]],
            ria.async.Future, function getWeekDayInfo(date, periodNumber_){
                var weekCalendarData = this.getContext().getSession().get('weekCalendarData', []), res = null;
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
                    this.getContext().getSession().set('dayCalendarData', model);
                    return this.prepareDayData(model, date_);
                }.bind(this));
            },

            [[chlk.models.common.ChlkDate, String]],
            ria.async.Future, function getAdminDay(date_, gradeLevelsIds_) {
                return this.get('AnnouncementCalendar/AdminDay.json', chlk.models.calendar.announcement.AdminDayCalendar, {
                    date: date_ && date_.toString('mm-dd-yy'),
                    gradeLevelsIds: gradeLevelsIds_
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getWeekInfo(classId_, date_) {
                return this.get('AnnouncementCalendar/Week.json', ArrayOf(chlk.models.calendar.announcement.WeekItem), {
                    classId: classId_ && classId_.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy')
                }).then(function(data){
                    return this.prepareWeekData(data, date_);
                }.bind(this));
            },

            //PREPARING INFO

            [[ArrayOf(chlk.models.calendar.announcement.WeekItem), chlk.models.common.ChlkDate]],
            ArrayOf(chlk.models.calendar.announcement.WeekItem), function prepareWeekData(data, date_){
                var max = 0, index = 0, kil=0, empty= 0, empty2=0, sun, date, startArray = [], endArray = [];
                var len = data.length;
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
                var dt = new chlk.models.common.ChlkDate(getDate());
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
                this.getContext().getSession().set('weekCalendarData', res);
                return res;
            },

            [[ArrayOf(chlk.models.calendar.announcement.DayItem), chlk.models.common.ChlkDate]],
            chlk.models.calendar.announcement.Day, function prepareDayData(days, date_){
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
                var max = 0, index = 0, len, item;
                if(days.length == 6){
                    sunday = new chlk.models.calendar.announcement.DayItem();
                    var sunDate = days[0].getDate().add(chlk.models.common.ChlkDateEnum.DAY, -1);
                    sunday.setDate(sunDate);
                    sunday.setDay(sunDate.getDate().getDate());
                    sunday.setCalendarDayItems([]);
                    days.unshift(sunday);
                }
                days.forEach(function(day, i){
                    day.setTodayClassName(today.isSameDay(day.getDate()) ? 'today' : '');
                    if(!day.getCalendarDayItems())
                        day.setCalendarDayItems([]);
                    len = day.getCalendarDayItems().length;
                    if(max < len){
                        max = len;
                        index = i;
                    }
                });
                days.forEach(function(day){
                    len = day.getCalendarDayItems().length;
                    if(max > len){
                        for(var i = len; i < max; i++){
                            item = new chlk.models.calendar.announcement.CalendarDayItem();
                            item.setPeriod(days[index].getCalendarDayItems()[i].getPeriod());
                            item.setAnnouncementClassPeriods([]);
                            day.getCalendarDayItems().push(item);
                        }
                    }
                });
                var model = new chlk.models.calendar.announcement.Day();
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
                return model;
            }
        ])
});