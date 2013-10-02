REQUIRE('chlk.models.classes.BaseClassProfileViewData');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.calendar.announcement.Day');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassScheduleViewData*/
    CLASS('ClassScheduleViewData', EXTENDS(chlk.models.classes.BaseClassProfileViewData),[

        chlk.models.calendar.announcement.Day, 'scheduleCalendar',

        [[chlk.models.common.Role, chlk.models.classes.Class, chlk.models.calendar.announcement.Day]],
        function $(role_, clazz_, scheduleCalendar_){
            BASE(role_, clazz_);
            if(scheduleCalendar_)
                this.setScheduleCalendar(scheduleCalendar_);
        }
    ]);
});