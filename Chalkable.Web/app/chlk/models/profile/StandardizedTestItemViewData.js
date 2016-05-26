REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.StandardizedTestId');
REQUIRE('chlk.models.id.StandardizedTestItemId');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.StandardizedTestItemViewData*/
    CLASS(
        UNSAFE, 'StandardizedTestItemViewData', [
            chlk.models.id.StandardizedTestItemId, 'id',
            String, 'name',
            chlk.models.id.StandardizedTestId, 'standardizedTestId',

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.StandardizedTestItemId);
                this.name = SJX.fromValue(raw.name, String);
                this.standardizedTestId = SJX.fromValue(raw.standardizedtestid, chlk.models.id.StandardizedTestId);
            }
        ]);
});
