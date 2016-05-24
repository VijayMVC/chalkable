REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.StandardizedTestId');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.StandardizedTestViewData*/
    CLASS(
        UNSAFE, 'StandardizedTestViewData', [
            String, 'name',
            String, 'displayName',
            chlk.models.id.StandardizedTestId, 'id',
            ArrayOf(chlk.models.profile.StandardizedTestItemViewData), 'components',
            ArrayOf(chlk.models.profile.StandardizedTestItemViewData), 'scoreTypes',

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.StandardizedTestId);
                this.name = SJX.fromValue(raw.name, String);
                this.standardizedTestId = SJX.fromValue(raw.standardizedtestid, chlk.models.id.StandardizedTestId);
                this.components = SJX.fromArrayOfDeserializables(raw.components, chlk.models.profile.StandardizedTestItemViewData);
                this.scoreTypes = SJX.fromArrayOfDeserializables(raw.scoretypes, chlk.models.profile.StandardizedTestItemViewData);
            }
        ]);
});
