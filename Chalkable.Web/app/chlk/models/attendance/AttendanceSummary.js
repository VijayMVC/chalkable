REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.attendance.AbsentLateSummaryItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.attendance.AttendanceSummary*/
    CLASS(
        UNSAFE, FINAL, 'AttendanceSummary', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.late = SJX.fromDeserializable(raw.late, chlk.models.attendance.AbsentLateSummaryItem);
                this.absent = SJX.fromDeserializable(raw.absent, chlk.models.attendance.AbsentLateSummaryItem);
            },

            chlk.models.attendance.AbsentLateSummaryItem, 'late',
            chlk.models.attendance.AbsentLateSummaryItem, 'absent'
        ]);
});
