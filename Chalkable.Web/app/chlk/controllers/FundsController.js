REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.FundsService');
REQUIRE('chlk.services.AdminService');
REQUIRE('chlk.activities.funds.FundsListPage');
REQUIRE('chlk.activities.funds.SchoolPersonFundsPage');
REQUIRE('chlk.models.funds.AddCreditCardModel');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.FundsController */
    CLASS(
        'FundsController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.FundsService, 'fundsService',

        [ria.mvc.Inject],
        chlk.services.AdminService, 'adminService',

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
           return this.Forward('funds','schoolPersonFunds',[]);
//            return this.PushView(chlk.activities.funds.SchoolPersonFundsPage, this.getPersonFunds_(chlk.models.common.RoleEnum.TEACHER));
        },
        function viewStudentAction(){
           return this.Forward('funds', 'schoolPersonFunds', []);
//            return this.PushView(chlk.activities.funds.SchoolPersonFundsPage, this.getPersonFunds_(chlk.models.common.RoleEnum.STUDENT));
        },
        function viewParentAction(){
           return this.Forward('funds','schoolPersonFunds',[]);
//            return this.PushView(chlk.activities.funds.SchoolPersonFundsPage, this.getPersonFunds_());
        },
//
        [[Boolean]],
        function schoolPersonFundsAction(isSuccessfulTransaction_){
            var res = this.getPersonFunds_(this.getCurrentRole());
            if(isSuccessfulTransaction_ != null && isSuccessfulTransaction_ != undefined){
                res = res.then(function(data){
                    data.setTransactionSuccess(isSuccessfulTransaction_);
                    return data;
                });
            }
            return this.PushView(chlk.activities.funds.SchoolPersonFundsPage, res);
        },

        [[chlk.models.common.Role]],
        ria.async.Future, function getPersonFunds_(role_){
            var res = ria.async.wait([
                    this.fundsService.getPersonFunds(),
                    this.fundsService.getCreditCardInfo()
                ])
                .then(function(res){
                    var addCreditCardData = new chlk.models.funds.AddCreditCardModel.$create(res[1]);
                    res[0].setAddCreditCardData(addCreditCardData);
                    return res[0];
                }, this)
                .attach(this.validateResponse_());
            if(role_ && role_.isTeacher()){
                return res.then(function (data){
                    return this.adminService.getAdmins(true)
                        .attach(this.validateResponse_())
                        .then(function (admins){
                            data.setSendToPersons(admins);
                            return data;
                        });
                }, this);
            }
            return res;
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
                return this.getPersonFunds_(this.getCurrentRole())
                    .then(function(result2){
                         result2.setTransactionSuccess(result1);
                         return result2;
                    });
            }, this);
            return this.UpdateView(chlk.activities.funds.SchoolPersonFundsPage, res);
        },

        [[chlk.models.funds.AddCreditCardModel]],
        function payPalAction(model){
           var res = this.fundsService.addCreditViaPayPal(model.getAmount())
               .attach(this.validateResponse_())
               .then(function (data1){
//                    return this.getPersonFunds_(this.getCurrentRole())
//                        .then(function(result2){
//                            result2.setTransactionSuccess(data1);
//                            return result2;
//                        });
                   window.location = data1.getRedirectUrl();
               },this);
//           return this.UpdateView(chlk.activities.funds.SchoolPersonFundsPage, res);
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
