REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ChlkTime*/
    CLASS(
        'ChlkTime', IMPLEMENTS(ria.serialize.IDeserializable), [

            String, 'time',

            [[String]],
            String, function toString(){
                return this.getTime();
            },

            VOID, function deserialize(raw) {
                var h = Math.floor(raw / 60);

                h %= 24;

                raw %= 60;
                if (raw < 10)
                    raw = '0' + raw;
                this.setTime(h + ':' + raw);
            }

        ]);
});
