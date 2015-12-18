REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChartDateItem');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.DateTypeEnum*/
    ENUM('DateTypeEnum', {
        YEAR: 0,
        GRADING_PERIOD: 1,
        LAST_MONTH: 2,
        LAST_WEEK: 3
    });

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.discipline.ClassDisciplineStatsViewData*/
    CLASS('ClassDisciplineStatsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        VOID, function deserialize(raw){
            this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
            this.dateType = SJX.fromValue(raw.datetype, chlk.models.discipline.DateTypeEnum);
            this.dailySummaries = SJX.fromArrayOfDeserializables(raw.dailysummaries, chlk.models.common.ChartDateItem);
        },

        chlk.models.id.ClassId, 'classId',

        chlk.models.discipline.DateTypeEnum, 'dateType',

        ArrayOf(chlk.models.common.ChartDateItem), 'dailySummaries'
    ]);
});