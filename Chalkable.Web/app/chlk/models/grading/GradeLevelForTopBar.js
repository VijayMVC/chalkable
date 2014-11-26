REQUIRE('chlk.models.grading.GradeLevel');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    var SJX = ria.serialize.SJX;
    /** @class chlk.models.grading.GradeLevelForTopBar*/
    CLASS(
        UNSAFE, FINAL ,'GradeLevelForTopBar', EXTENDS(chlk.models.grading.GradeLevel), IMPLEMENTS(ria.serialize.IDeserializable),  [
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.controller = SJX.fromValue(raw.controller, String);
                this.action = SJX.fromValue(raw.action, String);
                this.params = SJX.fromArrayOfValues(raw.params, null);//this was intended
                this.index = SJX.fromValue(raw.index, Number);
                this.pressed = SJX.fromValue(raw.pressed, Boolean);
            },
            String, 'controller',
            String, 'action',
            Array, 'params',
            Number, 'index',
            Boolean, 'pressed'
        ]);
});
