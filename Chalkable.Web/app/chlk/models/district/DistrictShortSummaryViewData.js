REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.district', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.district.DistrictShortSummaryViewData*/
    CLASS(
        'DistrictShortSummaryViewData',
        IMPLEMENTS(ria.serialize.IDeserializable), [
            String, 'name',
            Number, 'studentsCount',
            Number, 'schoolsCount',

            VOID, function deserialize(raw) {
                this.name = SJX.fromValue(raw.name, String);
                this.studentsCount = SJX.fromValue(raw.studentscount, Number);
                this.schoolsCount = SJX.fromValue(raw.schoolscounts, Number);
            }
        ]);
});
