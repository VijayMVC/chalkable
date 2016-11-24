REQUIRE('chlk.models.id.MealTypeId');

NAMESPACE('chlk.models.lunchCount', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.lunchCount.MealType*/
    CLASS(UNSAFE,
        'MealType', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, chlk.models.id.MealTypeId);
                this.name = SJX.fromValue(raw.name, String);
            },

            chlk.models.id.MealTypeId, 'id',
            String, 'name'
        ]);
});