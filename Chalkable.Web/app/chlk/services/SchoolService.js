REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.School');
REQUIRE('chlk.models.school.SchoolDetails');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SchoolService */
    CLASS(
        'SchoolService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getSchools(pageIndex_) {
                return this.getPaginatedList('/app/data/schools.json', chlk.models.School, pageIndex_);
            },

            [[Number]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('/app/data/schoolDetails.json', chlk.models.school.SchoolDetails);
            },

            [[Number]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('/app/data/peopleSummary.json', chlk.models.school.SchoolPeopleSummary);
            }
        ])
});