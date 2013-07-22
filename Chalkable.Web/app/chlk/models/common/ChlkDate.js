REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ChlkDate*/
    CLASS(
        'ChlkDate', IMPLEMENTS(ria.serialize.IDeserializable), [

            Date, 'date',
            String, function toString(format){
                var dateVal = this.getDate() || new Date();
                return dateVal.toString(format);
            },

            VOID, function deserialize(raw) {
                var date = raw ? new Date(raw) : new Date();
                this.setDate(date);
            }
        ]);
});
