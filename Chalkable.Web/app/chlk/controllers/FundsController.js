REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.activities.funds.FundsListPage');
REQUIRE('chlk.activities.funds.SchoolPersonFundsPage');
REQUIRE('chlk.models.funds.AddCreditCardModel');

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
            var res =
                ria.async.wait([
                    this.fundsService.getPersonFunds(),
                    this.fundsService.getCreditCardInfo()
                ])
                .then(function(res){
                    var addCreditCardData = new chlk.models.funds.AddCreditCardModel(res[1]);
                    res[0].setAddCreditCardData(addCreditCardData);
                    return res[0];
                }, this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.funds.SchoolPersonFundsPage, res);
        },

        [[chlk.models.funds.AddCreditCardModel]],
        function addCreditAction(model){
            var res = this.fundsService.addCredit(
                model.getAmount(),
                model.getCardNumber(),
                model.getMonth(),
                model.getYear(),
                model.getCvcNumber(),
                model.getCardType()
            )
            .attach(this.validateResponse_())
            .then(function(result1){
                return this.fundsService.getPersonFunds()
                    .attach(this.validateResponse_())
                    .then(function(result2){
                         result2.setTransactionSuccess(result1);
                         return result2;
                    });
            }, this);
            return this.UpdateView(chlk.activities.funds.SchoolPersonFundsPage, res);
        },

        function removeCreditCardAction(){
            var res = this.fundsService.deleteCreditCardInfo()
                .attach(this.validateResponse_())
                .then(function(res){
                    return new chlk.models.funds.AddCreditCardModel();
                });
            return this.UpdateView(chlk.activities.funds.SchoolPersonFundsPage, res, chlk.activities.lib.DontShowLoader());
        }

    ])
});
