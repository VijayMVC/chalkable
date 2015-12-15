REQUIRE('chlk.models.calendar.announcement.DayItem');
REQUIRE('chlk.models.calendar.announcement.BaseAnnouncementDayCalendar');
REQUIRE('chlk.models.classes.ClassesForTopBar');


NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Day*/
    CLASS(
        'Day', EXTENDS(chlk.models.calendar.announcement.BaseAnnouncementDayCalendar),  [
            chlk.models.classes.ClassesForTopBar, 'topData', //todo rename

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
                , chlk.models.common.ChlkDate, ArrayOf(chlk.models.calendar.announcement.DayItem)
                , chlk.models.classes.ClassesForTopBar
            ]],
            function $(date_, minDate_, maxDate_, dayItems_, classes_){
                BASE(date_, minDate_, maxDate_, dayItems_);
                if(classes_)
                    this.setTopData(classes_);
            }
        ]);
});
