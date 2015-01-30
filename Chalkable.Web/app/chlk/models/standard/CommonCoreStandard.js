REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.CommonCoreStandardId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.CommonCoreStandard*/
    CLASS(
        UNSAFE, 'CommonCoreStandard', IMPLEMENTS(ria.serialize.IDeserializable), [

            chlk.models.id.CommonCoreStandardId, 'id',
            chlk.models.id.CommonCoreStandardId, 'parentStandardId',
            String, 'standardCode',
            String, 'description',

            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.CommonCoreStandardId);
                this.parentStandardId = SJX.fromValue(raw.parentstandardid, chlk.models.id.CommonCoreStandardId);
                this.standardCode = SJX.fromValue(raw.standardcode, String);
                this.description = SJX.fromValue(raw.description, String);
            }
    ]);
});
