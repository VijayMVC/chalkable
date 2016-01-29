REQUIRE('chlk.models.grading.GradingClassSummaryItem');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItems*/
    CLASS(
        'GradingClassSummaryItems', [
            [ria.serialize.SerializeProperty('byannouncementtypes')],
            ArrayOf(chlk.models.grading.GradingClassSummaryItem), 'byAnnouncementTypes',

            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            Number, 'avg',

            Boolean, 'autoUpdate',

            function getTooltipText(){
                return (this.getAvg() ? Msg.Avg + " " + this.getAvg().toFixed(2) : 'No grades yet');
            }
        ]);
});
