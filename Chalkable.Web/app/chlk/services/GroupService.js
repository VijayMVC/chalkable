REQUIRE('chlk.models.id.GroupId');
REQUIRE('chlk.models.id.GradeLevelId');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.SchoolPersonId');

REQUIRE('chlk.models.group.Group');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GroupService */
    CLASS(
        'GroupService', EXTENDS(chlk.services.BaseService), [

            [[String]],
            ria.async.Future, function list(filter_) {
                return this.get('Group/GroupsList.json', ArrayOf(chlk.models.group.Group), {
                    filter: filter_
                });
            },

            [[String, ArrayOf(chlk.models.id.SchoolPersonId)]],
            ria.async.Future, function create(name, studentIds_){
                return this.post('Group/CreateGroup.json', chlk.models.group.Group,{
                    name: name,
                    studentIds: this.arrayToCsv(studentIds_ || [])
                });
            },

            [[chlk.models.id.GroupId, String, ArrayOf(chlk.models.id.SchoolPersonId)]],
            ria.async.Future, function edit(groupId, name, studentIds_){
                return this.post('Group/EditGroup.json', chlk.models.group.Group,{
                    groupId: groupId && groupId.valueOf(),
                    name: name,
                    studentIds: this.arrayToCsv(studentIds_ || [])
                });
            },

            [[chlk.models.id.GroupId]],
            ria.async.Future, function info(groupId){
                return this.post('Group/Info.json', chlk.models.group.Group,{
                    groupId: groupId && groupId.valueOf()
                });
            },

            [[chlk.models.id.GroupId]],
            ria.async.Future, function deleteGroup(groupId){
                return this.post('Group/DeleteGroup.json', ArrayOf(chlk.models.group.Group),{
                    groupId: groupId && groupId.valueOf()
                });
            }
        ])
});