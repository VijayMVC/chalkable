REQUIRE('chlk.models.calendar.announcement.MonthItem');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.calendar.BaseMonthCalendar');
REQUIRE('chlk.models.grading.GradeLevelsForTopBar');

NAMESPACE('chlk.models.calendar.announcement', function () {
    "use strict";

    /** @class chlk.models.calendar.announcement.Month*/
    CLASS(
        'Month', EXTENDS(chlk.models.calendar.BaseMonthCalendar), [
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items',

            chlk.models.classes.ClassesForTopBar, 'topData', //todo: rename

            chlk.models.grading.GradeLevelsForTopBar, 'gradeLevelsForToolBar',

            Boolean, 'admin',

            [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate
                , ArrayOf(chlk.models.calendar.announcement.MonthItem)
                , chlk.models.classes.ClassesForTopBar
                , chlk.models.grading.GradeLevelsForTopBar
            ]],
            function $(date_, minDate_, maxDate_, monthItems_, classes_, gradeLevels_){
                BASE(date_, minDate_, maxDate_);
                if(monthItems_)
                    this.setItems(monthItems_);
                if(classes_)
                    this.setTopData(classes_);
                if(gradeLevels_)
                    this.setGradeLevelsForToolBar(gradeLevels_);
            }
        ]);
});
