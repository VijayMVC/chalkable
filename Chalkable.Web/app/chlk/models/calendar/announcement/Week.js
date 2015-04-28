REQUIRE('chlk.models.calendar.announcement.WeekItem');
REQUIRE('chlk.models.calendar.BaseDayWeekCalendar');
REQUIRE('chlk.models.grading.GradeLevelsForTopBar');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Week*/
    CLASS(
        'Week', EXTENDS(chlk.models.calendar.BaseDayWeekCalendar), [
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items',

            chlk.models.classes.ClassesForTopBar, 'topData',  //todo: rename

            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsForToolBar',

            Boolean, 'admin',

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
                , ArrayOf(chlk.models.calendar.announcement.WeekItem)
                , chlk.models.classes.ClassesForTopBar
                , chlk.models.grading.GradeLevelsForTopBar
            ]],
            function $(date_, minDate_, maxDate_, weekItems_, classes_, gradeLevels_){
                BASE(date_, minDate_, maxDate_);
                if(weekItems_)
                    this.setItems(weekItems_);
                if(classes_)
                    this.setTopData(classes_);
                if(gradeLevels_)
                    this.setGradeLevelsForToolBar(gradeLevels_);
            }
        ]);
});
