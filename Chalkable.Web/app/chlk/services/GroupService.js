REQUIRE('chlk.models.id.GroupId');
REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.SchoolPersonId');

REQUIRE('chlk.models.group.StudentForGroup');
REQUIRE('chlk.models.group.Group');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GroupService */
    CLASS(
        'GroupService', EXTENDS(chlk.services.BaseService), [

            ria.async.Future, function list() {
                return this.get('Group/GroupsList.json', ArrayOf(chlk.models.group.Group), {});
            },

            [[chlk.models.id.GroupId]],
            ria.async.Future, function groupExplorer(groupId) {
                return this.get('Group/GroupExplorer.json', chlk.models.group.GroupExplorer, {
                    groupId: groupId && groupId.valueOf()
                });
            },

            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId, ArrayOf(chlk.models.id.ClassId), ArrayOf(chlk.models.id.CourseId)]],
            ria.async.Future, function studentForGroup(groupId, schoolYearId, gradeLevelId, classesIds_, coursesIds_) {
                return this.post('Group/GetStudentsForGroup.json', ArrayOf(chlk.models.group.StudentForGroup), {
                    groupId: groupId && groupId.valueOf(),
                    schoolYearId: schoolYearId && schoolYearId.valueOf(),
                    gradeLevelId: gradeLevelId && gradeLevelId.valueOf(),
                    classesIds: classesIds_ && this.arrayToCsv(classesIds_),
                    coursesIds: coursesIds_ && this.arrayToCsv(coursesIds_)
                });
            },

            [[String]],
            ria.async.Future, function create(name){
                return this.post('Group/CreateGroup.json', ArrayOf(chlk.models.group.Group),{
                    name: name
                });
            },

            [[chlk.models.id.GroupId, String]],
            ria.async.Future, function editName(groupId, name){
                return this.post('Group/EditGroupName.json', ArrayOf(chlk.models.group.Group),{
                    groupId: groupId && groupId.valueOf(),
                    name: name
                });
            },

            [[chlk.models.id.GroupId]],
            ria.async.Future, function deleteGroup(groupId){
                return this.post('Group/DeleteGroup.json', ArrayOf(chlk.models.group.Group),{
                    groupId: groupId && groupId.valueOf()
                });
            },

            [[chlk.models.id.GroupId, ArrayOf(chlk.models.id.SchoolPersonId)]],
            ria.async.Future, function assignStudents(groupId, studentIds){
                return this.post('Group/AssignStudents.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    studentIds: this.arrayToCsv(studentIds)
                });
            },

            [[chlk.models.id.GroupId, ArrayOf(chlk.models.id.SchoolPersonId)]],
            ria.async.Future, function unassignStudents(groupId, studentIds){
                return this.post('Group/UnassignStudents.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    studentIds: this.arrayToCsv(studentIds)
                });
            },
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
            ria.async.Future, function assignGradeLevel(groupId, schoolYearId, gradeLevelId){
                return this.post('Group/AssignSchoolGradeLevel.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    schoolYearId: schoolYearId && schoolYearId.valueOf(),
                    gradeLevelId: gradeLevelId && gradeLevelId.valueOf()
                });
            },

            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
            ria.async.Future, function unassignGradeLevel(groupId,  schoolYearId, gradeLevelId){
                return this.post('Group/UnassignSchoolGradeLevel.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    schoolYearId: schoolYearId && schoolYearId.valueOf(),
                    gradeLevelId: gradeLevelId && gradeLevelId.valueOf()
                });
            },

            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId]],
            ria.async.Future, function assignSchool(groupId, schoolYearId){
                return this.post('Group/AssignSchool.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    schoolYearId: schoolYearId && schoolYearId.valueOf()
                });
            },
            [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId]],
            ria.async.Future, function unassignSchool(groupId, schoolYearId){
                return this.post('Group/UnassignSchool.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    schoolYearId: schoolYearId && schoolYearId.valueOf()
                });
            },

            [[chlk.models.id.GroupId]],
            ria.async.Future, function assignAllSchools(groupId){
                return this.post('Group/AssignAllShools.json', Boolean,{
                    groupId: groupId && groupId.valueOf()
                });
            },
            [[chlk.models.id.GroupId]],
            ria.async.Future, function unassignAllSchools(groupId) {
                return this.post('Group/UnassignAllShools.json', Boolean, {
                    groupId: groupId && groupId.valueOf()
                });
            }
        ])
});