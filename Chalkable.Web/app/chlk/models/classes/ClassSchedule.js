REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.calendar.announcement.DayItem');

NAMESPACE('chlk.models.classes', function(){
    "use strict";

    /**@class chlk.models.classes.ClassSchedule*/
    CLASS('ClassSchedule', [

        [ria.serialize.SerializeProperty('class')],
        chlk.models.classes.Class, 'clazz',

        chlk.models.calendar.announcement.DayItem, 'schedule'
    ]);
});