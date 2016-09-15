REQUIRE('chlk.models.group.StudentForGroup');
REQUIRE('chlk.models.id.GroupId');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.StudentsForGroupViewData*/

    CLASS('StudentsForGroupViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.group.StudentForGroup), 'students',

        chlk.models.id.GroupId, 'groupId',

        chlk.models.id.GradeLevelId, 'gradeLevelId',

        chlk.models.id.SchoolYearId, 'schoolYearId',

        VOID, function deserialize(raw){
            this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.group.StudentForGroup);
            this.groupId = SJX.fromValue(raw.groupId, chlk.models.id.GroupId);
            this.gradeLevelId = SJX.fromValue(raw.gradeLevelId, chlk.models.id.GradeLevelId);
            this.schoolYearId = SJX.fromValue(raw.schoolYearId, chlk.models.id.SchoolYearId);
        },

        [[chlk.models.id.GroupId, chlk.models.id.GradeLevelId, chlk.models.id.SchoolYearId, ArrayOf(chlk.models.group.StudentForGroup)]],
        function $(groupId_, gradeLevelId_, schoolYearId_, students_){
            BASE();
            if(groupId_)
                this.setGroupId(groupId_);
            if(gradeLevelId_)
                this.setGradeLevelId(gradeLevelId_);
            if(schoolYearId_)
                this.setSchoolYearId(schoolYearId_);
            if(students_)
                this.setStudents(students_);
        }
    ]);
});