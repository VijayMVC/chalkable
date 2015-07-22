REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MarkingPeriodId');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.schoolYear.MarkingPeriod*/
    CLASS(
        UNSAFE, 'MarkingPeriod', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.MarkingPeriodId, 'id',
            String, 'description',
            [ria.serialize.SerializeProperty('startdate')],
            chlk.models.common.ChlkDate, 'startDate',
            [ria.serialize.SerializeProperty('enddate')],
            chlk.models.common.ChlkDate, 'endDate',
            Number, 'weekdays',
            String, 'name',

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.MarkingPeriodId);
                this.description = SJX.fromValue(raw.description, String);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.weekdays = SJX.fromValue(raw.weekdays, Number);
                this.name = SJX.fromValue(raw.name, String);
            }
        ]);
});
