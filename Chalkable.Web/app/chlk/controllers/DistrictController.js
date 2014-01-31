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
        function updateAction(id) {
            var result = this.districtService
                .getDistrict(id)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.district.DistrictDialog, result);
        },


        function addAction() {
            var result = new ria.async.DeferredData(new chlk.models.district.District);
            return this.ShadeView(chlk.activities.district.DistrictDialog, result);
        },

        [[chlk.models.district.District]],
        function saveAction(model){
            var result = this.districtService
                .saveDistrict(
                    model.getId(),
                    model.getName(),
                    model.getDbName(),
                    model.getSisUrl(),
                    model.getSisUserName(),
                    model.getSisPassword(),
                    model.getSisSystemType()
                )
                .attach(this.validateResponse_())
                .then(function (data) {
                    this.view.getCurrent().close();
                    return this.districtService
                        .getDistricts(0)
                        .attach(this.validateResponse_());
                }, this);

            return this.UpdateView(chlk.activities.district.DistrictListPage, result);
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
