REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolId');

REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.people.Schedule');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminService */
    CLASS(
        'AdminService', EXTENDS(chlk.services.BaseService), [
            [[String, String, String, Boolean, Number, Number]],
            ria.async.Future, function getUsers(filter_, roleId_, gradeLevelIds_, byLastName_, start_, count_) {
                return this.getPaginatedList('Admin/GetPersons.json', chlk.models.people.User, {
                    start: start_,
                    count: count_,
                    roleIds: roleId_ || null,
                    gradeLevelIds: gradeLevelIds_,
                    byLastName: byLastName_,
                    filter: filter_
                });
            },

            ria.async.Future, function getAdmins(byLastName_){
                var adminsRoles = chlk.models.common.Role.GET_ADMIN_ROLES_IDS().join(',');
                return this.getUsers(null, adminsRoles, null, byLastName_, 0, 1000)
                    .then(function(data){
                        return data.getItems();
                    });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Admin/Info.json', chlk.models.people.User, {
                    personId: personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getSummary(personId){
                return this.get('Admin/Info.json', chlk.models.people.PersonSummary,{
                    personId: personId && personId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getSchedule(personId) {
                return this.get('Teacher/Schedule.json', chlk.models.people.Schedule, {
                    personId: personId.valueOf()
                });
            }
        ]);
});