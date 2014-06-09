REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.developer', function () {
    "use strict";
    /** @class chlk.models.developer.DeveloperBalance*/
    CLASS(
        'DeveloperBalance', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.SchoolPersonId, 'id',
            Number, 'daysToPayout',
            Number, 'daysInMonth',
            Number, 'balance',

            function $(){
                BASE();
                this.prepare_();
            },

            function prepare_(){

                var now = new chlk.models.common.ChlkDate(getDate());
                var diff = now.getDateDiffInDays(now, now.getLastDay());
                var daysInMonth = now.getDaysInMonth();
                this.setDaysToPayout(diff);
                this.setDaysInMonth(daysInMonth);
            },

            [[Object]],
            VOID, function deserialize(raw) {
                this.setBalance(Number(raw.balance));
                this.prepare_();

            }
        ]);
});
