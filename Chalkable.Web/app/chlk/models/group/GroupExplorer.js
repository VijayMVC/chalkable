REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.group', function(){

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.group.GroupMember*/

    CLASS('GroupMember', IMPLEMENTS(ria.serialize.IDeserializable),[
        chlk.models.id.GroupId, 'groupId',
        chlk.models.id.SchoolYearId, 'schoolYearId',
        chlk.models.id.SchoolId, 'schoolId',
        chlk.models.id.GradeLevelId, 'gradeLevelId',

        Number, 'memberState',

        VOID, function deserialize(raw){
            this.groupId =  SJX.fromValue(raw.groupid, chlk.models.id.GroupId);
            this.schoolYearId =  SJX.fromValue(raw.schoolyearid, chlk.models.id.SchoolYearId);
            this.schoolId =  SJX.fromValue(raw.schoolid, chlk.models.id.SchoolId);
            this.gradeLevelId =  SJX.fromValue(raw.gradelevelid, chlk.models.id.GradeLevelId);
            this.memberState = SJX.fromValue(raw.memberstate, Number);
        }
    ]);

    /** @class chlk.models.group.GroupExplorer*/

    CLASS('GroupExplorer', IMPLEMENTS(ria.serialize.IDeserializable), [

        chlk.models.group.Group, 'group',
        ArrayOf(chlk.models.common.NameId), 'gradeLevels',
        ArrayOf(chlk.models.school.School), 'schools',
        ArrayOf(chlk.models.group.GroupMember), 'groupMembers',

        VOID, function deserialize(raw){
            this.group =  SJX.fromDeserializable(raw.group, chlk.models.group.Group);
            this.gradeLevels = SJX.fromArrayOfDeserializables(raw.gradelevels, chlk.models.common.NameId);
            this.schools = SJX.fromArrayOfDeserializables(raw.schools, chlk.models.school.School);
            this.groupMembers = SJX.fromArrayOfDeserializables(raw.members, chlk.models.group.GroupMember);
        }
    ]);
});