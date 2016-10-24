REQUIRE('chlk.models.lunchCount.MealType');
REQUIRE('chlk.models.lunchCount.MealCountItem');

NAMESPACE('chlk.models.lunchCount', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.lunchCount.MealItem*/
    CLASS(UNSAFE,
        'MealItem', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.mealType = SJX.fromDeserializable(raw.mealtype, chlk.models.lunchCount.MealType);
                this.mealCountItems = SJX.fromArrayOfDeserializables(raw.mealcountitems, chlk.models.lunchCount.MealCountItem);
                this.total = SJX.fromValue(raw.total, Number);
            },

            chlk.models.lunchCount.MealType, 'mealType',
            ArrayOf(chlk.models.lunchCount.MealCountItem), 'mealCountItems',
            Number, 'total'
        ]);
});