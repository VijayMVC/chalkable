REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.DistributionItemViewData*/
    CLASS(
        UNSAFE, 'DistributionItemViewData', [
            Number, 'count',
            String, 'summary',
            Number, 'startInterval',
            Number, 'endInterval',

            VOID, function deserialize(raw) {
                this.count = SJX.fromValue(raw.count, Number);
                this.summary = SJX.fromValue(raw.summary, String);
                this.startInterval = SJX.fromValue(raw.startinterval, Number);
                this.endInterval = SJX.fromValue(raw.endinterval, Number);
            }
        ]);
});
