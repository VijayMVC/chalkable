REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.lunchCount.MealItem');

NAMESPACE('chlk.models.lunchCount', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.lunchCount.LunchCountGrid*/
    CLASS(UNSAFE,
        'LunchCountGrid', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.people.User);
                this.staffs = SJX.fromArrayOfDeserializables(raw.staffs, chlk.models.people.User);
                this.mealItems = SJX.fromArrayOfDeserializables(raw.mealitems, chlk.models.lunchCount.MealItem);
                this.classId = SJX.fromValue(raw.classid, Number);
                this.date = SJX.fromValue(raw.date, chlk.models.common.ChlkDate);
                this.override = SJX.fromValue(raw.override, Boolean);
                this.enabled = SJX.fromValue(raw.enabled, Boolean);
            },

            ArrayOf(chlk.models.people.User), 'students',
            ArrayOf(chlk.models.people.User), 'staffs',
            ArrayOf(chlk.models.lunchCount.MealItem), 'mealItems',
            Number, 'classId',
            chlk.models.common.ChlkDate, 'date',
            Boolean, 'override',
            Boolean, 'enabled'
        ]);
});