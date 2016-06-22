REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.StandardizedTestId');
REQUIRE('chlk.models.id.StandardizedTestItemId');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.StandardizedTestFilterViewData*/
    CLASS(
        UNSAFE, 'StandardizedTestFilterViewData', [
            chlk.models.id.StandardizedTestItemId, 'scoreTypeId',
            chlk.models.id.StandardizedTestItemId, 'componentId',
            chlk.models.id.StandardizedTestId, 'id',

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.standardizedtestid, chlk.models.id.StandardizedTestId);
                this.componentId = SJX.fromValue(raw.componentid, chlk.models.id.StandardizedTestItemId);
                this.scoreTypeId = SJX.fromValue(raw.scoretypeid, chlk.models.id.StandardizedTestItemId);
            }
        ]);
});
