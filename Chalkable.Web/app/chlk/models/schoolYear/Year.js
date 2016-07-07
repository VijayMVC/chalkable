REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.schoolYear', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.schoolYear.Year*/
    CLASS(
        'Year', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.SchoolYearId);
                this.name = SJX.fromValue(raw.name, String);
                this.description = SJX.fromValue(raw.description, String);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.isCurrent = SJX.fromValue(raw.iscurrent, Boolean);
                this.numberOfMarkingPeriod = SJX.fromValue(raw.numberofmarkingperiod, Number);
            },

            chlk.models.id.SchoolYearId, 'id',
            String, 'description',
            String, 'name',
            chlk.models.common.ChlkDate, 'startDate',
            chlk.models.common.ChlkDate, 'endDate',
            Boolean, 'isCurrent',
            Number, 'numberOfMarkingPeriod'
        ]);
});
