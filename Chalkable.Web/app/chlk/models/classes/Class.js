REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.Class*/
    CLASS(
        UNSAFE, 'Class', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw){
                this.departmentId = SJX.fromValue(raw.departmentid, chlk.models.id.DepartmentId);
                this.description = SJX.fromValue(raw.description, String);
                this.gradeLevel = SJX.fromDeserializable(raw.gradelevel, chlk.models.grading.GradeLevel);
                this.id = SJX.fromValue(raw.id, chlk.models.id.ClassId);
                this.markingPeriodsId = SJX.fromArrayOfValues(raw.markingperiodsid, chlk.models.id.MarkingPeriodId);
                this.classNumber = SJX.fromValue(raw.classnumber, String);
                this.name = SJX.fromValue(raw.name, String);
                this.teacher = SJX.fromDeserializable(raw.teacher, chlk.models.people.User);
                this.defaultAnnouncementTypeId = SJX.fromValue(raw.defaultAnnouncementTypeId, Number);
            },

            chlk.models.id.DepartmentId, 'departmentId',
            String, 'description',
            chlk.models.grading.GradeLevel, 'gradeLevel',
            chlk.models.id.ClassId, 'id',
            ArrayOf(chlk.models.id.MarkingPeriodId), 'markingPeriodsId',
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
