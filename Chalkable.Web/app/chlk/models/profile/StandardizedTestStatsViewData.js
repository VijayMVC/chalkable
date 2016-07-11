REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.profile.StandardizedTestViewData');
REQUIRE('chlk.models.profile.StandardizedTestItemViewData');
REQUIRE('chlk.models.common.ChartDateItem');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.StandardizedTestStatsViewData*/
    CLASS(
        UNSAFE, 'StandardizedTestStatsViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.profile.StandardizedTestViewData, 'standardizedTest',
            chlk.models.profile.StandardizedTestItemViewData, 'component',
            chlk.models.profile.StandardizedTestItemViewData, 'scoreType',
            ArrayOf(chlk.models.common.ChartDateItem), 'dailyStats',

            VOID, function deserialize(raw) {
                this.standardizedTest = SJX.fromDeserializable(raw.standardizedtest, chlk.models.profile.StandardizedTestViewData);
                this.component = SJX.fromDeserializable(raw.component, chlk.models.profile.StandardizedTestItemViewData);
                this.scoreType = SJX.fromDeserializable(raw.scoretype, chlk.models.profile.StandardizedTestItemViewData);
                this.dailyStats = SJX.fromArrayOfDeserializables(raw.dailystats, chlk.models.common.ChartDateItem)
            },

            function getFullName(){
                return this.getStandardizedTest().getDisplayName() + ' | ' + this.getComponent().getName() + ' | ' + this.getScoreType().getName()
            }
        ]);
});
