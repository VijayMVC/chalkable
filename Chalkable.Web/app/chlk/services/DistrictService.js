REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DistrictService */
    CLASS(
        'DistrictService', EXTENDS(chlk.services.BaseService), [
            [[Object]],
            ria.async.Future, function getDistricts(districtId, pageIndex_) {
                return this.getPaginatedList('app/data/districts.json', chlk.models.district.District, {
                    start: pageIndex_,
                    districtId: districtId
                });
            }
        ])
});