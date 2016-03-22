REQUIRE('chlk.models.id.AlphaGradeId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.AlphaGrade*/
    CLASS('AlphaGrade', IMPLEMENTS(ria.serialize.IDeserializable), [
        VOID, function deserialize(raw){
            this.id = SJX.fromValue(raw.id, chlk.models.id.AlphaGradeId);
            this.name = SJX.fromValue(raw.name, String);
            this.description = SJX.fromValue(raw.description, String);
        },

        chlk.models.id.AlphaGradeId, 'id',
        String, 'name',
        String, 'description'
    ]);
});
