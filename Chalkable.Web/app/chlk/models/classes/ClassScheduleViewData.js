REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.calendar.announcement.Week');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassScheduleViewData*/
    CLASS('ClassScheduleViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        chlk.models.calendar.announcement.Week, 'scheduleCalendar',

        [[chlk.models.common.Role, chlk.models.classes.Class
            , chlk.models.calendar.announcement.Week
            , ArrayOf(chlk.models.people.Claim)
            , Boolean
        ]],
        function $(role_, clazz_, scheduleCalendar_, claims_, isAssignedToClass_){
            BASE(role_, clazz_, claims_, isAssignedToClass_);
            if(scheduleCalendar_)
                this.setScheduleCalendar(scheduleCalendar_);
        }
    ]);
});