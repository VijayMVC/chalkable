REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminService */
    CLASS(
        'AdminService', EXTENDS(chlk.services.BaseService), [
            [[Number, Number, Number, Boolean, Number]],
            ria.async.Future, function getUsers(schoolId, roleId, gradeLevelId, byLastName, start) {
                return this.getPaginatedList('app/data/people.json', chlk.models.people.User, {
                    schoolId: schoolId,
                    start: start,
                    roleId: roleId,
                    gradeLevelId: gradeLevelId,
                    byLastName: byLastName
                });
            }
        ])
});