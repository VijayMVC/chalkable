REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ChlkDate*/
    CLASS(
        'ChlkDate', IMPLEMENTS(ria.serialize.IDeserializable), [

            Date, 'date',
            String, function toString(format){
                return this.getDate().toString(format);
            },

            VOID, function deserialize(raw) {
                this.setDate(new Date(raw));
            }
        ]);
});
