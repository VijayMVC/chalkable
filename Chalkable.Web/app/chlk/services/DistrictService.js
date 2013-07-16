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
                return this.getPaginatedList('app/data/blobs.json', chlk.models.district.District, {
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

            [[chlk.models.district.DistrictId, String, String, String, String, String, Number]],
            ria.async.Future, function updateDistrict(id, name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType) {
                return this.post('District/Update.json', chlk.models.district.District, {
                    districtId: id.valueOf(),
                    name: name,
                    dbName: dbName,
                    sisUrl: sisUrl,
                    sisUserName: sisUserName,
                    sisPassword: sisPassword,
                    sisSystemType: sisSystemType
                });
            },

            [[chlk.models.district.DistrictId, String, String, String, String, String, Number]],
            ria.async.Future, function saveDistrict(id_, name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType) {
                if (id_) return this.updateDistrict(id_, name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType);
                return this.addDistrict(name, dbName, sisUrl, sisUserName, sisPassword, sisSystemType);
            },

            [[chlk.models.district.DistrictId]],
            ria.async.Future, function removeDistrict(id) {
                return this.post('District/Delete.json', chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            },
            [[chlk.models.district.DistrictId]],
            ria.async.Future, function getDistrict(id) {
                return this.post('District/Info.json', chlk.models.district.District, {
                    districtId: id.valueOf()
                });
            }
        ])
});