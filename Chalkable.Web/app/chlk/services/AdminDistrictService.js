REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.school.SchoolStatistic');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminDistrictService */
    CLASS(
        'AdminDistrictService', EXTENDS(chlk.services.BaseService), [

            ria.async.Future, function getDistrictSummary() {
                return this.get('AdminDistrict/DistrictSummary.json', chlk.models.district.DistrictShortSummaryViewData, {});
            },

            [[Number, String, Number]],
            ria.async.Future, function getSchoolStatistic(start_, filter_, count_) {
                return this.getPaginatedList('AdminDistrict/Schools.json', chlk.models.school.SchoolStatistic, {
                    start:start_ || 0,
                    count: count_ || 10,
                    filter: filter_
                });
            }
        ])
});