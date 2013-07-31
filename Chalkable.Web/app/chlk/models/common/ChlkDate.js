REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ChlkDate*/
    CLASS(
        'ChlkDate', IMPLEMENTS(ria.serialize.IDeserializable), [

            Date, 'date',

            [[String]],
            String, function toString(joinBy_){
                var dateVal = this.getDate() || new Date();
                return this.toChalkableDate(dateVal, joinBy_);
            },

            [[Date, String]],
            String, function toChalkableDate(date, joinBy_) {
                var m = date.getMonth() + 1;
                var d = date.getDate();
                var joinBy = joinBy_ || '-';
                return [date.getFullYear(), (m < 10 ? ('0' + m) : m), (d < 10 ? ('0' + d) : d)].join(joinBy);
            },

            VOID, function deserialize(raw) {
                var date = raw ? new Date(raw.replace(/-/g, '/')) : new Date();
                this.setDate(date);
            }
        ]);
});
