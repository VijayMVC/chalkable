REQUIRE('chlk.models.grading.StudentWithAvg');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.grading.StudentAverageInfo');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.ShortGradingClassSummaryGridItems*/
    CLASS(
        'ShortGradingClassSummaryGridItems', [
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.serialize.SerializeProperty('gradingitems')],
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',

            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.common.NameId, 'gradingPeriod',

            Number, 'avg',

            Number, 'rowIndex',

            Boolean, 'autoUpdate',

            [ria.serialize.SerializeProperty('displayalphagrades')],
            Boolean , 'ableDisplayAlphaGrades',

            [ria.serialize.SerializeProperty('displaystudentaverage')],
            Boolean , 'ableDisplayStudentAverage',

            [ria.serialize.SerializeProperty('displaytotalpoints')],
            Boolean , 'ableDisplayTotalPoints',

            [ria.serialize.SerializeProperty('totalavarages')],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',

            [ria.serialize.SerializeProperty('totalpoints')],
            ArrayOf(Number), 'totalPoints',

            function getTooltipText(){
                return (this.getAvg() != null ? Msg.Avg + " " + this.getAvg() : 'No grades yet');
            }
        ]);
});
