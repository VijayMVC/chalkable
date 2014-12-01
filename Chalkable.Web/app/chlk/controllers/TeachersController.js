REQUIRE('chlk.controllers.UserController');
REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.activities.profile.SchoolPersonInfoPage');
REQUIRE('chlk.activities.profile.PersonProfileSummaryPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.UserProfileInfoViewData');

NAMESPACE('chlk.controllers', function (){
    "use strict";
    /** @class chlk.controllers.TeachersController */
    CLASS(
        'TeachersController', EXTENDS(chlk.controllers.UserController), [

            function getInfoPageClass(){
                return chlk.activities.profile.SchoolPersonInfoPage;
            },

            [[chlk.models.id.SchoolPersonId]],
            function detailsAction(personId){
                var res = this.teacherService
                    .getSummary(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return new chlk.models.people.UserProfileSummaryViewData(this.getCurrentRole(), model, this.getUserClaims_());
                    }, this);
                return this.PushView(chlk.activities.profile.PersonProfileSummaryPage, res);
            },

            [[chlk.models.id.SchoolPersonId]],
            function infoAction(personId){
                var result = this.teacherService
                    .getInfo(personId)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var userData = this.prepareProfileData(model);
                        var res = new chlk.models.people.UserProfileInfoViewData(
                            this.getCurrentRole(), userData, this.getUserClaims_()
                        );
                        this.setUserToSession(res);
                        return res;
                    }, this);
                return this.PushView(chlk.activities.profile.SchoolPersonInfoPage, result);
            },
            [[chlk.models.people.User]],
            function infoEditAction(model){
                var res = this.infoEdit_(model, chlk.models.people.UserProfileInfoViewData);
                return this.UpdateView(chlk.activities.profile.SchoolPersonInfoPage, res);
            },

            [[chlk.models.id.SchoolPersonId]],
            function scheduleAction(personId){
                return this.Redirect('teachers', 'daySchedule', [null, personId]);
            },


            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function dayScheduleAction(date_, personId){
                return this.schedule_(
                    chlk.models.common.RoleNamesEnum.TEACHER.valueOf(),
                    personId,
                    date_,
                    this.calendarService.getDayWeekInfo(date_, personId),
                    chlk.models.calendar.announcement.Week,
                    chlk.activities.profile.SchedulePage,
                    'daySchedule',
                    chlk.templates.calendar.announcement.DayCalendarBodyTpl
                );
            },

            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function weekScheduleAction(date_, personId){
                return this.schedule_(
                    chlk.models.common.RoleNamesEnum.TEACHER.valueOf(),
                    personId,
                    date_,
                    this.calendarService.getDayWeekInfo(date_, personId),
                    chlk.models.calendar.announcement.Week,
                    chlk.activities.profile.ScheduleWeekPage,
                    'weekSchedule',
                    chlk.templates.calendar.announcement.WeekCalendarBodyTpl
                );
            },

            [[chlk.models.common.ChlkDate, chlk.models.id.SchoolPersonId]],
            function monthScheduleAction(date_, personId){
                return this.schedule_(
                    chlk.models.common.RoleNamesEnum.TEACHER.valueOf(),
                    personId,
                    date_,
                    this.calendarService.listForMonth(null, date_, null, personId),
                    chlk.models.calendar.announcement.Month,
                    chlk.activities.profile.ScheduleMonthPage,
                    'monthSchedule',
                    chlk.templates.calendar.announcement.MonthCalendarBodyTpl
                );
            }
        ])
});
