REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.course.Course');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.MarkingPeriodId');


NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.Class*/
    CLASS(
        'Class', [
            chlk.models.course.Course, 'course',

            String, 'description',

            [ria.serialize.SerializeProperty('gradelevel')],
            chlk.models.grading.GradeLevel, 'gradeLevel',

            chlk.models.id.ClassId, 'id',

            [ria.serialize.SerializeProperty('markingperiodsid')],
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',

            [ria.serialize.SerializeProperty('classnumber')],
            String, 'classNumber',

            String, 'name',

            READONLY, String, 'fullClassName',
            String, function getFullClassName(){
                var classNumber = this.getClassNumber();
                if(classNumber) return classNumber + " " + this.getName();
                return this.getName();
            },

            chlk.models.people.User, 'teacher',

            Number, 'defaultAnnouncementTypeId'
        ]);
});
