REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentRankHoverBoxItem*/
    CLASS(
        UNSAFE, 'StudentRankHoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.id.MarkingPeriodId, 'markingPeriodId',
        String, 'markingPeriodName',
        Number, 'rank',

        VOID, function deserialize(raw){
            this.markingPeriodId = SJX.fromValue(raw.markingperiodid, chlk.models.id.MarkingPeriodId);
            this.markingPeriodName = SJX.fromValue(raw.markingperiodname, String);
            this.rank = SJX.fromValue(raw.rank, Number);
        }
    ]);
});