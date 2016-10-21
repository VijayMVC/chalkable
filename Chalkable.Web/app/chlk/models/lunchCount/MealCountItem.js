NAMESPACE('chlk.models.lunchCount', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.lunchCount.MealCountItem*/
    CLASS(UNSAFE,
        'MealCountItem', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.name = SJX.fromValue(raw.name, Number);
                this.personId = SJX.fromValue(raw.personid, Number);
                this.guest = SJX.fromValue(raw.guest, Boolean);
                this.override = SJX.fromValue(raw.override, Boolean);
                this.enabled = SJX.fromValue(raw.enabled, Boolean);
            },

            Number, 'name',
            Number, 'personId',
            Boolean, 'guest',
            Boolean, 'override',
            Boolean, 'enabled'
        ]);
});