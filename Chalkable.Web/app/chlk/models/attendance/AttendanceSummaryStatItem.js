REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.attendance.AttendanceStatItem');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.attendance.AttendanceSummaryStatItem*/
    CLASS(
        UNSAFE, FINAL, 'AttendanceSummaryStatItem', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.dayStats = raw.daystats || [];
                this.clazz = SJX.fromDeserializable(raw.class, chlk.models.classes.Class);
            },

            Array, 'dayStats',
            chlk.models.classes.Class, 'clazz'
        ]);
});
