REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.common.ChartDateItem*/
    CLASS(
        'ChartDateItem', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.number = SJX.fromValue(raw.number, Number);
                this.summary = SJX.fromValue(raw.summary, String);
                this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
            },

            String, 'summary',

            chlk.models.common.ChlkDate, 'date',

            Number, 'number'
        ]);
});
