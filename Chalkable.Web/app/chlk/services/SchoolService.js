REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.school.SchoolDetails');
REQUIRE('chlk.models.school.Timezone');
REQUIRE('chlk.models.school.SchoolSisInfo');
REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SchoolService */
    CLASS(
        'SchoolService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.district.DistrictId, Number, Boolean, Boolean]],
            ria.async.Future, function getSchools(districtId, start_, demoOnly_, unimportedOnly_) {
                return this.getPaginatedList(start_ > 0 ? 'data/schools2.json' : 'data/schools.json', chlk.models.school.School, {
                    start: start_,
                    districtId: districtId.valueOf(),
                    demoOnly: demoOnly_,
                    unimportedOnly: unimportedOnly_
                });
            },

            [[Number]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('data/schoolDetails.json', chlk.models.school.SchoolDetails, {
                    schoolId: schoolId
                });
            },

            [[Number]],
            ria.async.Future, function getSisInfo(schoolId) {
                return this.get('data/schoolSisInfo.json', chlk.models.school.SchoolSisInfo, {
                    schoolId: schoolId
                });
            },

            [[Number]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('data/peopleSummary.json', chlk.models.school.SchoolPeopleSummary, {
                    schoolId: schoolId
                });
            },

            ria.async.Future, function getTimezones() {
                return this.getPaginatedList('School/ListTimezones.json', chlk.models.school.Timezone, {});
            }
        ])
});