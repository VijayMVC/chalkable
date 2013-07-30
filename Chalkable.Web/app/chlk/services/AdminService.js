REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolId');

REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminService */
    CLASS(
        'AdminService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolId, Number, Number, Boolean, Number]],
            ria.async.Future, function getUsers(schoolId, roleId, gradeLevelId, byLastName, start) {
                return this.getPaginatedList('app/data/people.json', chlk.models.people.User, {
                    schoolId: schoolId.valueOf(),
                    start: start,
                    roleId: roleId,
                    gradeLevelId: gradeLevelId,
                    byLastName: byLastName
                });
            }
        ])
});