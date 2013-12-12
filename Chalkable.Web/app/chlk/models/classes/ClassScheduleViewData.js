REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.calendar.announcement.Day');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassScheduleViewData*/
    CLASS('ClassScheduleViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        chlk.models.calendar.announcement.Day, 'scheduleCalendar',

        [[chlk.models.common.Role, chlk.models.classes.Class
            , chlk.models.calendar.announcement.Day
            , ArrayOf(chlk.models.people.Claim)
        ]],
        function $(role_, clazz_, scheduleCalendar_, claims_){
            BASE(role_, clazz_, claims_);
            if(scheduleCalendar_)
                this.setScheduleCalendar(scheduleCalendar_);
        }
    ]);
});