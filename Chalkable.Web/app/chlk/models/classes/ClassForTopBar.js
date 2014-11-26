REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.classes.ClassForTopBar*/
    CLASS(
        UNSAFE, FINAL, 'ClassForTopBar', EXTENDS(chlk.models.classes.Class),IMPLEMENTS(ria.serialize.IDeserializable),  [
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.controller = SJX.fromValue(raw.controller, String);
                this.action = SJX.fromValue(raw.action, String);
                this.params = SJX.fromArrayOfValues(raw.params, null);//this was intended
                this.index = SJX.fromValue(raw.index, Number);
                this.pressed = SJX.fromValue(raw.pressed, Boolean);
                this.disabled = SJX.fromValue(raw.disabled, Boolean);
            },
            String, 'controller',
            String, 'action',
            Array, 'params',
            Boolean, 'pressed',
            Number, 'index',
            Boolean, 'disabled'
        ]);
});
