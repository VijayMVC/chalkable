REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.school.SchoolDetails');
REQUIRE('chlk.models.school.Timezone');
REQUIRE('chlk.models.school.SchoolSisInfo');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.SchoolId');



NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.SchoolService */
    CLASS(
        'SchoolService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.DistrictId, Number, Boolean, Boolean]],
            ria.async.Future, function getSchools(districtId, start_, demoOnly_, unimportedOnly_) {
                return this.getPaginatedList('School/List.json', chlk.models.school.School, {
                    start: start_,
                    districtId: districtId.valueOf(),
                    demoOnly: demoOnly_,
                    unimportedOnly: unimportedOnly_
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function getDetails(schoolId) {
                return this.get('chalkable2/data/schoolDetails.json', chlk.models.school.SchoolDetails, {
                    schoolId: schoolId.valueOf()
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function getSisInfo(schoolId) {
                return this.get('chalkable2/data/schoolSisInfo.json', chlk.models.school.SchoolSisInfo, {
                    schoolId: schoolId.valueOf()
                });
            },

            [[chlk.models.id.SchoolId]],
            ria.async.Future, function getPeopleSummary(schoolId) {
                return this.get('chalkable2/data/peopleSummary.json', chlk.models.school.SchoolPeopleSummary, {
                    schoolId: schoolId.valueOf()
                });
            },

            ria.async.Future, function getTimezones() {
                return this.getPaginatedList('School/ListTimezones.json', chlk.models.school.Timezone, {});
            }
        ])
});