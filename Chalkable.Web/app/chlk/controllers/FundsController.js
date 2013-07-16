REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.activities.funds.FundsListPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.FundsController */
    CLASS(
        'FundsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.FundsService, 'fundsService',

        [chlk.controllers.SidebarButton('statistic')],
        [[Number]],
        function listAction(pageIndex_) {
            var result = this.fundsService
                .getFunds(pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.funds.FundsListPage, result);
        },
        [[Number]],
        function acceptAction(id) {

        },
        [[Number]],
        function rejectAction(id) {

        },
        [[Number]],
        function exportAction(id) {

        }

        ])
});
