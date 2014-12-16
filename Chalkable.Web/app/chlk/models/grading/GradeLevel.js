REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.GradeLevel*/
    CLASS(
        UNSAFE, 'GradeLevel', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.id, chlk.models.id.GradeLevelId);
                this.name = SJX.fromValue(raw.name, String);
                this.number = SJX.fromValue(raw.number, Number);
                this.serialPart = SJX.fromValue(raw.serialPart, String);
                this.fullText = SJX.fromValue(raw.fullText, String);
            },
            chlk.models.id.GradeLevelId, 'id',
            String, 'name',
            Number, 'number',
            String, 'serialPart',
            String, 'fullText',

            String, function getFullText(){
                return getSerial(this.getName());
            }
        ]);
});
