REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.services', function () {
    "use strict";
    /** @class chlk.services.TeacherService*/
    CLASS(
        'TeacherService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getInfo(personId) {
                return this.get('Teacher/Info.json', chlk.models.people.User, {
                    personId: personId.valueOf()
                });
            }
        ])
});