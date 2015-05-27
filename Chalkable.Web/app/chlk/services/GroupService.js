REQUIRE('chlk.models.id.GroupId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GroupService */
    CLASS(
        'GroupService', EXTENDS(chlk.services.BaseService), [

            ria.async.Future, function list() {
                return this.get('Group/GroupsList.json', ArrayOf(chlk.models.group.Group), {});
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
                    groupId: groupId && groupId.valueOf(),
                });
            },

            [[chlk.models.id.GroupId, ArrayOf(chlk.models.id.SchoolPersonId)]],
            ria.async.Future, function assignStudentToGroup(groupId, studentIds){
                return this.post('Group/DeleteGroup.json', Boolean,{
                    groupId: groupId && groupId.valueOf(),
                    studentIds: this.arrayToCsv(studentIds)
                });
            }
    ]);
});