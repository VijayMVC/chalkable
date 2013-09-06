REQUIRE('chlk.models.people.Schedule');
REQUIRE('chlk.models.calendar.announcement.Day');
NAMESPACE('chlk.models.people', function () {
    "use strict";

    /** @class chlk.models.people.SchedulePage*/
    CLASS(
        'SchedulePage', [
            chlk.models.people.Schedule, 'schedule',

            chlk.models.calendar.announcement.Day, 'calendar',

            [[chlk.models.people.Schedule, chlk.models.calendar.announcement.Day]],
            function $(schedule_, calendar_){
                BASE();
                if(schedule_)
                    this.setSchedule(schedule_);
                if(calendar_)
                    this.setCalendar(calendar_);
            }
        ]);
});
