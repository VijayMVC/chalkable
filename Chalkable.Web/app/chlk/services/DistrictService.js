REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DistrictService */
    CLASS(
        'DistrictService', EXTENDS(chlk.services.BaseService), [
            [[Number, Number]],
            ria.async.Future, function getDistricts(pageIndex_, count_) {
                return this.getPaginatedList('District/List.json', chlk.models.district.District, {
                    start: pageIndex_|0,
                    count: count_
                });
            },

            [[chlk.models.id.DistrictId]],
            ria.async.Future, function removeDistrict(id) {
                return this.post('District/Delete.json', chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            },

            [[chlk.models.id.DistrictId]],
            ria.async.Future, function syncDistrict(id) {
                return this.post('District/Sync.json', chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            },

            [[chlk.models.id.DistrictId]],
            ria.async.Future, function getDistrict(id) {
                return this.post('District/Info.json', chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            },

            [[chlk.models.id.DistrictId, Boolean]],
            ria.async.Future, function UpdateReportCardsEnabled(districtId, enabled){
                return this.post('District/UpdateReportCardsEnabled.json', Boolean,{
                    districtId: districtId && districtId.valueOf(),
                    enabled: enabled
                });
            }
        ])
});