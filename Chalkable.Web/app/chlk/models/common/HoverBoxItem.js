REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.common.HoverBoxItem*/
    CLASS(
        UNSAFE, 'HoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable), [
            String, 'summary',
            Number, 'total',

            VOID, function deserialize(raw){
                this.summary = SJX.fromValue(raw.summary, String);
                this.total = SJX.fromValue(raw.total, Number);
            }
        ]);
});
