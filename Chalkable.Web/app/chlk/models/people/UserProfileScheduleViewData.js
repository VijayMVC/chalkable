REQUIRE('chlk.models.people.Schedule');
REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.calendar.BaseCalendar');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UserProfileScheduleViewData*/
    CLASS(
        GENERIC('TCalendar', ClassOf(chlk.models.calendar.BaseCalendar)),
        'UserProfileScheduleViewData', EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.people.Schedule)), [

            //todo : rename this property
            chlk.models.people.Schedule, 'schedule',

            chlk.models.people.Schedule, function getSchedule(){return this.getUser();},
            [[chlk.models.people.Schedule]],
            VOID, function setSchedule(userSchedule){return this.setUser_(userSchedule);},

            TCalendar, 'calendar',
            String, 'currentAction',

            ClassOf(chlk.templates.calendar.announcement.BaseCalendarBodyTpl), 'bodyTpl',

            [[
                chlk.models.common.Role,
                chlk.models.people.Schedule,
                TCalendar,
                ArrayOf(chlk.models.people.Claim),
                String,
                ClassOf(chlk.templates.calendar.announcement.BaseCalendarBodyTpl)
            ]],
            function $(role, schedule_, calendar_, claims_, currentAction, bodyTpl){
                BASE(role, schedule_, claims_);
                if(calendar_)
                    this.setCalendar(calendar_);
                this.setCurrentAction(currentAction);
                this.setBodyTpl(bodyTpl);
            }
        ]);
});
