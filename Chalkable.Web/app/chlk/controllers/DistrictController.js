REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DistrictService');
REQUIRE('chlk.models.district.District');
REQUIRE('chlk.activities.district.DistrictListPage');
REQUIRE('chlk.activities.district.AddDistrictDialog');

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
        function pageAction(pageIndex) {
            var result = this.districtService
                .getDistricts(pageIndex)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.district.DistrictListPage, result);
        },

        [[chlk.models.district.District]],
        function addDistrictAction(model_) {
            var result;
            if (model_){
                result = this.districtService.addDistrict(
                    model_.getName(),
                    model_.getDbName(),
                    model_.getSisUrl(),
                    model_.getSisUserName(),
                    model_.getSisPassword(),
                    model_.getSisSystemType()
                )
                    .attach(this.validateResponse_())
                    .then(function (data) {
                        this.view.getCurrent().close();
                        return this.districtService.getDistricts(0)
                    }.bind(this));

                return this.UpdateView(chlk.activities.district.DistrictListPage, result);
            }
            else{
                model_ = new chlk.models.district.District;
                result = new ria.async.DeferredData(model_);
            }
            return this.ShadeView(chlk.activities.district.AddDistrictDialog, result);
        },

        [[chlk.models.district.DistrictId]],
        function deleteAction(id) {
            var result= this.districtService
                .removeDistrict(id)
                .attach(this.validateResponse_())
                .then(function (data) {
                    return this.districtService.getDistricts(0)
                },this);
            return this.UpdateView(chlk.activities.district.DistrictListPage, result);
        },
        [[chlk.models.district.DistrictId]],
        function updateAction(id) {

        }

    ])
});
