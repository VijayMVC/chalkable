REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.ProgramId');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.recipients.Program*/
    CLASS(UNSAFE,
        'Program', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.name = SJX.fromValue(raw.name, String);
                this.code = SJX.fromValue(raw.code, String);
                this.description = SJX.fromValue(raw.description, String);
                this.id = SJX.fromValue(raw.id, chlk.models.id.ProgramId);
            },

            String, 'code',
            String, 'name',
            String, 'description',
            chlk.models.id.ProgramId, 'id'
        ]);
});
