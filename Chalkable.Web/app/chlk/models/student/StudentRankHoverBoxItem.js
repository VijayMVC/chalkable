REQUIRE('chlk.models.id.MarkingPeriodId');

NAMESPACE('chlk.models.student', function(){
    "use strict";
    /**@class chlk.models.student.StudentRankHoverBoxItem*/
    CLASS('StudentRankHoverBoxItem', [

        [ria.serialize.SerializeProperty('markingperiodid')],
        chlk.models.id.MarkingPeriodId, 'markingPeriodId',

        [ria.serialize.SerializeProperty('markingpeiordname')],
        String, 'markingPeriodName',

        Number, 'rank'
    ]);
});