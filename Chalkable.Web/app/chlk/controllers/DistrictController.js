REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DistrictService');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.activities.district.DistrictListPage');
REQUIRE('chlk.activities.district.DistrictDialog');
REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DistrictController */
    CLASS(
        'DistrictController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.DistrictService, 'districtService',

        [chlk.controllers.SidebarButton('districts')],

        [[Number]],
        function listAction(pageIndex_) {
            var result = this.districtService
                .getDistricts(pageIndex_|0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.district.DistrictListPage, result);
        },

        [[Number]],
        function listSysAdminAction(pageIndex_) {
            var result = this.districtService
                .getDistricts(pageIndex_|0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.district.DistrictListPage, result);
        },

        //TODO: join with list
        [[Number]],
        function pageAction(pageIndex) {
            var result = this.districtService
                .getDistricts(pageIndex)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.district.DistrictListPage, result);
        },

        [[chlk.models.id.DistrictId]],
        function syncAction(id) {
            var result = this.districtService
                .syncDistrict(id)
                .attach(this.validateResponse_());
            this.ShowMsgBox('Sync task is created', 'fyi.', [{
                text: Msg.GOT_IT.toUpperCase(),
                controller: "district",
                action: "list"
            }]);
            return null;
        },

        [[chlk.models.id.DistrictId]],
        function deleteAction(id) {
            var result= this.districtService
                .removeDistrict(id)
                .attach(this.validateResponse_())
                .then(function (data) {
                    return this.districtService
                        .getDistricts(0)
                        .attach(this.validateResponse_());
                }, this);
            return this.UpdateView(chlk.activities.district.DistrictListPage, result);
        }

    ])
});
