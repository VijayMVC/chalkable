REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.discipline.DisciplineDailySummaryViewData*/
    CLASS('DisciplineDailySummaryViewData', [

        VOID, function deserialize(raw){
            this.occurrences = SJX.fromValue(raw.occurrences, Number);
            this.summary = SJX.fromValue(raw.summary, String);
            this.dateTime = SJX.fromDeserializable(raw.datetime, chlk.models.common.ChlkDate);
        },

        String, 'summary',

        chlk.models.common.ChlkDate, 'dateTime',

        Number, 'occurrences'
    ]);
});