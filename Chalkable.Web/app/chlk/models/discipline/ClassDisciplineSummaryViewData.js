REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.discipline.DisciplineDailySummaryViewData');

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
    /** @class chlk.models.discipline.ClassDisciplineSummaryViewData*/
    CLASS('ClassDisciplineSummaryViewData', [

        VOID, function deserialize(raw){
            this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
            this.dateType = SJX.fromValue(raw.dateType, chlk.models.announcement.AnnouncementTypeEnum);
            this.dailySummaries = SJX.fromArrayOfDeserializables(raw.dailysummaries, chlk.models.discipline.DisciplineDailySummaryViewData);
        },

        chlk.models.id.ClassId, 'classId',

        Number, 'dateType',

        ArrayOf(chlk.models.discipline.DisciplineDailySummaryViewData), 'dailySummaries'
    ]);
});