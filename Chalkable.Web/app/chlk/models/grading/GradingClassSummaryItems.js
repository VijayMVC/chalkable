REQUIRE('chlk.models.grading.GradingClassSummaryItem');
REQUIRE('chlk.models.common.NameId');

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
                return (this.getAvg() != null ? Msg.Avg + " " + this.getAvg() : 'No grades yet');
            }
        ]);
});
