REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolId');

REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminService */
    CLASS(
        'AdminService', EXTENDS(chlk.services.BaseService), [
            [[String, String, String, Boolean, Number]],
            ria.async.Future, function getUsers(filter_, roleId_, gradeLevelIds_, byLastName_, start_) {
                return this.getPaginatedList('Admin/GetPersons.json', chlk.models.people.User, {
                    start: start_,
                    roleIds: roleId_ || null,
                    gradeLevelIds: gradeLevelIds_,
                    byLastName: byLastName_,
                    filter: filter_
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
            }
        ]);
});