REQUIRE('chlk.models.id.EthnicityId');

NAMESPACE('chlk.models.people', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.people.EthnicityViewData*/
    CLASS(FINAL, UNSAFE, 'EthnicityViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        VOID, function deserialize(raw){
           this.id = SJX.fromValue(raw.id, chlk.models.id.EthnicityId);
           this.code = SJX.fromValue(raw.code, String);
           this.name = SJX.fromValue(raw.name, String);
           this.description = SJX.fromValue(raw.description, String);
        },

        chlk.models.id.EthnicityId, 'id',
        String, 'code',
        String, 'name',
        String, "description"
    ]);
});
