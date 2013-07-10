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
                .getDistricts(0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.district.DistrictListPage, result);
        },

        [[chlk.models.district.District]],
        VOID, function addDistrictAction(model_) {
                model_ = model_ || new chlk.models.district.District;
                return this.ShadeView(chlk.activities.district.AddDistrictDialog, new ria.async.DeferredData(model_));
        },

        [[Number]],
        function detailsAction(id) {
            return this.redirect_('schools', 'list', [id, 1]);
        },

        [[Number]],
        function deleteAction(id) {
        }
    ])
});
