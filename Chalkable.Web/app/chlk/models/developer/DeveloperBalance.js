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
                var date = new Date(),
                    y = date.getFullYear(),
                    m = date.getMonth();
                var lastDay = new Date(y, m + 1, 0);
                var diff = this.dateDiffInDays_(date, lastDay);
                var daysInMonth = this.daysInMonth_(m, y);

                this.setDaysToPayout(diff);
                this.setDaysInMonth(daysInMonth);
            },

            function dateDiffInDays_(a, b) {
                var _MS_PER_DAY = 1000 * 60 * 60 * 24;
                // Discard the time and time-zone information.
                var utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
                var utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());
                return Math.floor((utc2 - utc1) / _MS_PER_DAY);
            },

            function daysInMonth_(month, year) {
                return new Date(year, month, 0).getDate();
            },



            [[Object]],
            VOID, function deserialize(raw) {
                this.setBalance(Number(raw.balance));
                this.prepare_();

            }
        ]);
});
