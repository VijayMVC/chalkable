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
            ria.async.Future, function getSchools(districtId, start_) {
                return this.getPaginatedList(start_ > 0 ? 'app/data/schools2.json' : 'app/data/schools.json', chlk.models.school.School, {
                    start: start_,
                    districtId: districtId
                });
            },

            [[Number]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('app/data/schoolDetails.json', chlk.models.school.SchoolDetails, {
                    schoolId: schoolId
                });
            },

            [[Number]],
            ria.async.Future, function getSisInfo(schoolId) {
                return this.get('app/data/schoolSisInfo.json', chlk.models.school.SchoolSisInfo, {
                    schoolId: schoolId
                });
            },

            [[Number]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('app/data/peopleSummary.json', chlk.models.school.SchoolPeopleSummary, {
                    schoolId: schoolId
                });
            },

            ria.async.Future, function getTimezones() {
                return this.getPaginatedList('School/ListTimezones.json', chlk.models.school.Timezone, {});
            }
        ])
});