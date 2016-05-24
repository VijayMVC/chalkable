REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.profile.ClassDistributionStatsViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.ClassDistributionSectionViewData*/
    CLASS(
        UNSAFE, 'ClassDistributionSectionViewData', [
            chlk.models.profile.ClassDistributionStatsViewData, 'gradeAverageDistribution',
            chlk.models.profile.ClassDistributionStatsViewData, 'absencesDistribution',
            chlk.models.profile.ClassDistributionStatsViewData, 'disciplineDistribution',

            VOID, function deserialize(raw) {
                this.gradeAverageDistribution = SJX.fromDeserializable(raw.gradeaveragedistribution, chlk.models.profile.ClassDistributionStatsViewData);
                this.absencesDistribution = SJX.fromDeserializable(raw.absencesdistribution, chlk.models.profile.ClassDistributionStatsViewData);
                this.disciplineDistribution = SJX.fromDeserializable(raw.disciplinedistribution, chlk.models.profile.ClassDistributionStatsViewData);
            }
        ]);
});
