NAMESPACE('chlk.models.lunchCount', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.lunchCount.MealCountItem*/
    CLASS(UNSAFE,
        'MealCountItem', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.count = SJX.fromValue(raw.count, Number);
                this.personId = SJX.fromValue(raw.personid, Number);
                this.guest = SJX.fromValue(raw.guest, Boolean);
            },

            Number, 'count',
            Number, 'personId',
            Boolean, 'guest'
        ]);
});