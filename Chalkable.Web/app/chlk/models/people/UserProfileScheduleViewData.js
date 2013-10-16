REQUIRE('chlk.models.people.Schedule');
REQUIRE('chlk.models.calendar.announcement.Day');
REQUIRE('chlk.models.people.UserProfileViewData');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.UserProfileScheduleViewData*/
    CLASS(
        'UserProfileScheduleViewData', EXTENDS(chlk.models.people.UserProfileViewData), [

            //todo : rename this property
            chlk.models.people.Schedule, 'schedule',

            chlk.models.people.Schedule, function getSchedule(){return this.getUser();},
            [[chlk.models.people.Schedule]],
            VOID, function setSchedule(userSchedule){return this.setUser_(userSchedule);},

            chlk.models.calendar.announcement.Day, 'calendar',

            [[chlk.models.common.Role, chlk.models.people.Schedule, chlk.models.calendar.announcement.Day]],
            function $(role, schedule_, calendar_){
                BASE(role, schedule_);
                if(calendar_)
                    this.setCalendar(calendar_);
            }
        ]);
});
