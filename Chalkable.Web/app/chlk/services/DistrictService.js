REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.district.District');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.DistrictService */
    CLASS(
        'DistrictService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getDistricts(pageIndex_) {
                return this.getPaginatedList('District/List.json', chlk.models.district.District, {
                    start: pageIndex_|0
                });
            },

            [[String, String, String, String, String, Number]],
            ria.async.Future, function addDistrict(name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType) {
                return this.post('District/Create.json', chlk.models.district.District, {
                    name: name,
                    dbName: dbName,
                    sisUrl: sisUrl,
                    sisUserName: sisUserName,
                    sisPassword: sisPassword,
                    sisSystemType: sisSystemType
                });
            },
            [[chlk.models.district.DistrictId]],
            ria.async.Future, function removeDistrict(id) {
                return this.post('District/Delete.json', chlk.models.district.District, {
                    id: id.valueOf()
                });
            },
            [[chlk.models.district.DistrictId]],
            ria.async.Future, function getDistrict(id) {
                return this.post('District/GetById.json', chlk.models.district.District, {
                    id: id.valueOf()
                });
            }
        ])
});