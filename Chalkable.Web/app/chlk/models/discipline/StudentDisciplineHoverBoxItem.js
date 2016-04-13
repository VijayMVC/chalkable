NAMESPACE('chlk.models.discipline', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.discipline.StudentDisciplineHoverBoxItem*/
    CLASS(
        UNSAFE, FINAL, 'StudentDisciplineHoverBoxItem', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.value = SJX.fromValue(raw.value, Number);
                this.className = SJX.fromValue(raw.classname, String);
            },

            Number, 'value',
            String, 'className'
        ]);
});
