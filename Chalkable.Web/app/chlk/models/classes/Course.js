REQUIRE('chlk.models.id.CourseId');
REQUIRE('chlk.models.classes.Class');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.Course*/
    CLASS(
        UNSAFE, 'Course', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.description = SJX.fromValue(raw.description, String);
                this.id = SJX.fromValue(raw.id, chlk.models.id.CourseId);
                this.classNumber = SJX.fromValue(raw.classnumber, String);
                this.name = SJX.fromValue(raw.name, String);
                this.classes = SJX.fromArrayOfDeserializables(raw.classes, chlk.models.classes.Class);
            },

            String, 'description',
            chlk.models.id.CourseId, 'id',
            String, 'classNumber',
            String, 'name',
            ArrayOf(chlk.models.classes.Class), 'classes'
        ]);
});
