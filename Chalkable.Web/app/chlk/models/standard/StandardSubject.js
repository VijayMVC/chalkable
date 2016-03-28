REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.standard', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.StandardSubject*/
    CLASS(
        'StandardSubject', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw) {
                this.name = SJX.fromValue(raw.displayname, String);
                this.shortName = SJX.fromValue(raw.name, String);
                this.description = SJX.fromValue(raw.description, String);
                this.id = SJX.fromValue(raw.id, chlk.models.id.StandardSubjectId);
            },

           String, 'name',
           String, 'shortName',
           String, 'description',
           chlk.models.id.StandardSubjectId, 'id'
        ]);
});
