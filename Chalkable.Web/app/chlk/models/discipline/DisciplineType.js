REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.DisciplineTypeId');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.discipline.DisciplineType*/
    CLASS(
        UNSAFE, 'DisciplineType', IMPLEMENTS(ria.serialize.IDeserializable), [
         chlk.models.id.DisciplineTypeId, 'id',
         String, 'name',

        VOID, function deserialize(raw) {
            this.id = SJX.fromValue(raw.id, chlk.models.id.DisciplineTypeId);
            this.name = SJX.fromValue(raw.name, String);

        }
    ]);
});