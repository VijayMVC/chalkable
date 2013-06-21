REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.School');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SchoolService */
    CLASS(
        'SchoolService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getSchools() {
                return this.getPaginatedList('/app/data/schools.json', chlk.models.School, 0);
            }
        ])
});