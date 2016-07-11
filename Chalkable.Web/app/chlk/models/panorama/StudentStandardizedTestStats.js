REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.panorama.StudentDetailsViewData');
REQUIRE('chlk.models.profile.StandardizedTestStatsViewData');

NAMESPACE('chlk.models.panorama', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.panorama.StudentStandardizedTestStats*/
    CLASS(
        UNSAFE, 'StudentStandardizedTestStats', IMPLEMENTS(ria.serialize.IDeserializable),  [

        VOID, function deserialize(raw){
            this.student = SJX.fromDeserializable(raw.student, chlk.models.panorama.StudentDetailsViewData);
            this.standardizedTestsStats = SJX.fromArrayOfDeserializables(raw.standardizedtestsstats, chlk.models.profile.StandardizedTestStatsViewData);
        },

        chlk.models.panorama.StudentDetailsViewData, 'student',
        ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStats'
    ]);
});
