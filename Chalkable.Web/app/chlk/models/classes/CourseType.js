REQUIRE('chlk.models.id.CourseTypeId');
REQUIRE('chlk.models.classes.Course');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.CourseType*/
    CLASS(
        UNSAFE, 'CourseType', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.courseTypeId = SJX.fromValue(raw.couretypeid, chlk.models.id.CourseTypeId);
                this.courseTypeCode = SJX.fromValue(raw.couretypecode, String);
                this.courseTypeName = SJX.fromValue(raw.couretypename, String);
                this.courses = SJX.fromArrayOfDeserializables(raw.courses, chlk.models.classes.Course);
            },

            chlk.models.id.CourseTypeId, 'courseTypeId',
            String, 'courseTypeCode',
            String, 'courseTypeName',
            ArrayOf(chlk.models.classes.Course), 'courses'
        ]);
});
