REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.Mapping*/
    CLASS(
        UNSAFE, FINAL, 'Mapping', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.gradingAbcf = SJX.fromArrayOfValues(raw.gradingabcf, Number);
                this.gradingComplete = SJX.fromArrayOfValues(raw.gradingcomplete, Number);
                this.gradingCheck = SJX.fromArrayOfValues(raw.gradingcheck, Number);
            },

            ArrayOf(Number), 'gradingAbcf',
            ArrayOf(Number), 'gradingComplete',
            ArrayOf(Number), 'gradingCheck'
        ]);
});
