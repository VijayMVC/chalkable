REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.activities.funds.FundsListPage');
REQUIRE('chlk.activities.funds.SchoolPersonFundsPage');

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

        },

        function viewTeacherAction(){
           return this.Redirect('funds','schoolPersonFunds',[]);
        },
        function viewStudentAction(){
           return this.Redirect('funds', 'schoolPersonFunds', []);
        },
        function viewParentAction(){
           return this.Redirect('funds','schoolPersonFunds',[]);
        },

        function schoolPersonFundsAction(){
            var res = this.fundsService.getPersonFunds().attach(this.validateResponse_());
            return this.PushView(chlk.activities.funds.SchoolPersonFundsPage, res);
        },

        function addCreditAction(){

        }

    ])
});
