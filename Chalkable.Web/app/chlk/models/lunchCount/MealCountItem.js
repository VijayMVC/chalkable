REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.models.lunchCount', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.lunchCount.MealCountItem*/
    CLASS(UNSAFE,
        'MealCountItem', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.count = SJX.fromValue(raw.count, Number);
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
                this.guest = SJX.fromValue(raw.guest, Boolean);
            },

            Number, 'count',
            chlk.models.id.SchoolPersonId, 'personId',
            Boolean, 'guest',

            [[chlk.models.id.SchoolPersonId, Boolean, Number]],
            function $(personId_, guest_, count_){
                BASE();
                personId_ && this.setPersonId(personId_);
                guest_ && this.setGuest(guest_);
                (count_ || count_ === 0) && this.setCount(count_);
            }
        ]);
});