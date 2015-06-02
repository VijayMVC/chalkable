REQUIRE('chlk.models.group.StudentForGroup');
REQUIRE('chlk.models.id.GroupId');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.group.StudentsForGroupViewData*/

    CLASS('StudentsForGroupViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.group.StudentForGroup), 'students',

        chlk.models.id.GroupId, 'groupId',

        VOID, function deserialize(raw){
            this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.group.StudentForGroup);
            this.groupId = SJX.fromValue(raw.groupId, chlk.models.id.GroupId);
        },

        [[chlk.models.id.GroupId, ArrayOf(chlk.models.group.StudentForGroup)]],
        function $(groupId_, students_){
            BASE();
            if(groupId_)
                this.setGroupId(groupId_);
            if(students_)
                this.setStudents(students_);
        }
    ]);
});