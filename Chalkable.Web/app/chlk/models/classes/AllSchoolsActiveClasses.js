REQUIRE('ria.serialize.SJX');

REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.classes.CourseType');
REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.models.classes', function () {

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.AllSchoolsActiveClasses */
    CLASS(
        'AllSchoolsActiveClasses', IMPLEMENTS(ria.serialize.IDeserializable), [

            ArrayOf(chlk.models.classes.Class), 'classes',
            ArrayOf(chlk.models.classes.CourseType), 'courseTypes',
            ArrayOf(chlk.models.school.School), 'schools',

            [[Object]],
            VOID, function deserialize(raw) {
                this.classes = SJX.fromArrayOfDeserializables(raw.classes, chlk.models.classes.Class);
                this.courseTypes = SJX.fromArrayOfDeserializables(raw.coursetypes, chlk.models.classes.CourseType);
                this.schools = SJX.fromArrayOfDeserializables(raw.schools, chlk.models.school.School);
            }
        ])
});