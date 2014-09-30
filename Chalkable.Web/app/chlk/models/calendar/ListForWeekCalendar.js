REQUIRE('chlk.models.calendar.ListForWeekCalendarItem');

NAMESPACE('chlk.models.calendar', function () {
    "use strict";

    /** @class chlk.models.calendar.ListForWeekCalendar*/
    CLASS(
        'ListForWeekCalendar', [
            ArrayOf(chlk.models.calendar.ListForWeekCalendarItem), 'items',

            [[ArrayOf(chlk.models.calendar.ListForWeekCalendarItem)]],
            function $(items_){
                BASE();
                if(items_)
                    this.setItems(items_);
            }
        ]);
});
