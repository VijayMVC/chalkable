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
            [[Number, Number]],
            ria.async.Future, function getSchools(districtId, pageIndex_) {
                return this.getPaginatedList('data/schools.json', chlk.models.school.School, {
                    start: pageIndex_,
                    districtI: districtId
                });
            },

            [[Number]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('/app/data/schoolDetails.json', chlk.models.school.SchoolDetails, {
                    start: pageIndex_
                });
            },

            [[Number]],
            ria.async.Future, function getSisInfo(schoolId) {
                return this.get('/app/data/schoolSisInfo.json', chlk.models.school.SchoolSisInfo, {
                    start: pageIndex_
                });
            },

            [[Number]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('/app/data/peopleSummary.json', chlk.models.school.SchoolPeopleSummary, {
                    start: pageIndex_
                });
            },

            ria.async.Future, function getTimezones() {
                return this.getPaginatedList('School/ListTimezones.json', chlk.models.school.Timezone, {});
            }
        ])
});