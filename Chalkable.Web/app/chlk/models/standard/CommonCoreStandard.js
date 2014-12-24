REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.CommonCoreStandard*/
    CLASS(
        UNSAFE, 'CommonCoreStandard', IMPLEMENTS(ria.serialize.IDeserializable), [
            String, 'standardCode',
            String, 'description',

            VOID, function deserialize(raw){
                this.standardCode = SJX.fromValue(raw.standardcode, String);
                this.description = SJX.fromValue(raw.description, String);
            }
    ]);
});
