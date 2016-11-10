REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.profile', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.profile.DistributionItemViewData*/
    CLASS(
        UNSAFE, 'DistributionItemViewData', IMPLEMENTS(ria.serialize.IDeserializable), [
            Number, 'count',
            String, 'summary',
            Number, 'startInterval',
            Number, 'endInterval',
            ArrayOf(Number), 'studentIds',

            VOID, function deserialize(raw) {
                this.count = SJX.fromValue(raw.count, Number);
                this.summary = SJX.fromValue(raw.summary, String);
                this.startInterval = SJX.fromValue(raw.startinterval, Number);
                this.endInterval = SJX.fromValue(raw.endinterval, Number);
                this.studentIds = SJX.fromArrayOfValues(raw.studentids || [3326,3366,3391,3708,3827,3828], Number);
            }
        ]);
});
