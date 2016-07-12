REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChartDateItem');

NAMESPACE('chlk.models.attendance', function(){
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.attendance.ClassAttendanceStatsViewData*/
    CLASS('ClassAttendanceStatsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        VOID, function deserialize(raw){
            this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            this.dateType = SJX.fromValue(raw.dateperiodtype, chlk.models.classes.DateTypeEnum);
            this.absences = SJX.fromArrayOfDeserializables(raw.absences, chlk.models.common.ChartDateItem);
            this.lates = SJX.fromArrayOfDeserializables(raw.lates, chlk.models.common.ChartDateItem);
            this.presents = SJX.fromArrayOfDeserializables(raw.presents, chlk.models.common.ChartDateItem);
        },

        chlk.models.id.ClassId, 'classId',

        chlk.models.classes.DateTypeEnum, 'dateType',

        ArrayOf(chlk.models.common.ChartDateItem), 'absences',

        ArrayOf(chlk.models.common.ChartDateItem), 'lates',

        ArrayOf(chlk.models.common.ChartDateItem), 'presents'
    ]);
});