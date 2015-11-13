REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.activities.district.DistrictListPage');
REQUIRE('chlk.activities.district.DistrictDialog');
REQUIRE('chlk.activities.district.DistrictSummaryPage');

REQUIRE('chlk.models.district.District');
REQUIRE('chlk.models.id.DistrictId');

REQUIRE('chlk.services.DistrictService');
REQUIRE('chlk.services.BgTaskService');
REQUIRE('chlk.services.AdminDistrictService');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DistrictController */
    CLASS(
        'DistrictController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.DistrictService, 'districtService',
        [ria.mvc.Inject],
        chlk.services.BgTaskService, 'bgTaskService',
        [ria.mvc.Inject],
        chlk.services.AdminDistrictService, 'adminDistrictService',

        [chlk.controllers.SidebarButton('districts')],

        [[Number]],
        function listAction(pageIndex_) {
            var result = this.districtService
                .getDistricts(pageIndex_|0, 100)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.district.DistrictListPage, result);
        },

        [[Number]],
        function listSysAdminAction(pageIndex_) {
            var result = this.districtService
                .getDistricts(pageIndex_|0, 100)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.district.DistrictListPage, result);
        },

        //TODO: join with list
        [[Number]],
        function pageAction(pageIndex) {
            var result = this.districtService
                .getDistricts(pageIndex, 100)
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
                        .getDistricts(0, 100)
                        .attach(this.validateResponse_());
                }, this);
            return this.UpdateView(chlk.activities.district.DistrictListPage, result);
        },

        [[chlk.models.id.BgTaskId]],
        function cancelBgTaskAction(id){
            return this.bgTaskService
                .cancelTask(id)
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.pageAction(0);
                }, this);
        },

        [chlk.controllers.SidebarButton('classes')],
        function summaryAction(){
            var result = ria.async.wait([
                    this.adminDistrictService.getDistrictSummary(),
                    this.adminDistrictService.getSchoolStatistic()
                ])
                .then(function(result){
                    return new chlk.models.district.DistrictFullSummaryViewData(result[0], result[1]);
                })
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.district.DistrictSummaryPage, result);
        }

    ])
});
