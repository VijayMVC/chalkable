REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.district.DistrictShortSummaryViewData');
REQUIRE('chlk.models.admin.BaseStatistic');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminDistrictService */
    CLASS(
        'AdminDistrictService', EXTENDS(chlk.services.BaseService), [

            ria.async.Future, function getDistrictSummary() {
                return this.get('AdminDistrict/DistrictSummary.json', chlk.models.district.DistrictShortSummaryViewData, {});
            },

            [[Number, String, Number, Number]],
            ria.async.Future, function getSchoolStatistic(start_, filter_, count_, sortType_) {
                return this.get('AdminDistrict/Schools.json', ArrayOf(chlk.models.admin.BaseStatistic.OF(chlk.models.id.SchoolId)), {
                    start:start_ || 0,
                    count: count_ || 10,
                    filter: filter_,
                    sortType: sortType_
                });
            }
        ])
});