REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.calendar.announcement.MonthPage');
REQUIRE('chlk.models.calendar.announcement.Month');
REQUIRE('chlk.models.class.ClassesForTopBar');

NAMESPACE('chlk.controllers', function (){

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
        [chlk.controllers.SidebarButton('calendar')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function monthTeacherAction(date_, classId_){
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
                    model.setCurrentMonth(date.format('MM'));
                    model.setCurrentDate(date);
                    var startDate = markingPeriod.getStartDate().getDate();
                    var endDate = markingPeriod.getEndDate().getDate();
                    if(prevDate >= startDate){
                        model.setPrevMonthDate(new chlk.models.common.ChlkDate(prevDate));
                    }else{
                        if(startDate.getMonth() != date.getDate().getMonth())
                            model.setPrevMonthDate(markingPeriod.getStartDate());
                    }
                    if(nextDate <= endDate){
                        model.setNextMonthDate(new chlk.models.common.ChlkDate(nextDate));
                    }else{
                        if(nextDate.getMonth() != date.getDate().getMonth())
                            model.setNextMonthDate(markingPeriod.getEndDate());
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
                    model.setDays(days);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.class.ClassesForTopBar();
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
