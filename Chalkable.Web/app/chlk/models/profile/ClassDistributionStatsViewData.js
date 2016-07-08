REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.profile.DistributionItemViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.ClassDistributionStatsViewData*/
    CLASS(
        UNSAFE, 'ClassDistributionStatsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            ArrayOf(chlk.models.profile.DistributionItemViewData), 'distributionStats',
            Number, 'classAvg',

            VOID, function deserialize(raw) {
                this.distributionStats = SJX.fromArrayOfDeserializables(raw.distributionstats, chlk.models.profile.DistributionItemViewData);
                this.classAvg = SJX.fromValue(raw.classavg, Number);
            }
        ]);
});
