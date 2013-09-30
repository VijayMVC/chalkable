REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.calendar.announcement.Day');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassScheduleViewData*/
    CLASS('ClassScheduleViewData', [

        chlk.models.classes.Class, 'clazz',

        chlk.models.calendar.announcement.Day, 'scheduleCalendar',

        [[chlk.models.classes.Class, chlk.models.calendar.announcement.Day]],
        function $(clazz_, scheduleCalendar_){
            BASE();
            if(clazz_)
                this.setClazz(clazz_);
            if(scheduleCalendar_)
                this.setScheduleCalendar(scheduleCalendar_);
        }
    ]);
});