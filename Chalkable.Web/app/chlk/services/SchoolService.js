REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.school.SchoolDetails');
REQUIRE('chlk.models.school.Timezone');
REQUIRE('chlk.models.school.SchoolSisInfo');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SchoolService */
    CLASS(
        'SchoolService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getSchools(pageIndex_) {
                return this.getPaginatedList('/School/List.json', chlk.models.school.School, pageIndex_);
            },

            [[Number]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('/app/data/schoolDetails.json', chlk.models.school.SchoolDetails);
            },

            [[Number]],
            ria.async.Future, function getSisInfo(schoolId) {
                return this.get('/app/data/schoolSisInfo.json', chlk.models.school.SchoolSisInfo);
            },

            [[Number]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('/app/data/peopleSummary.json', chlk.models.school.SchoolPeopleSummary);
            },

            ria.async.Future, function getTimezones() {
                return this.getPaginatedList('/app/data/timezones.json', chlk.models.school.Timezone, 0);
            }
        ])
});