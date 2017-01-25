REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DeveloperService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.activities.profile.DeveloperPage');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.account.ChangePassword');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AccountController*/
    CLASS(
        'AccountController', EXTENDS(chlk.controllers.BaseController), [


        [ria.mvc.Inject],
        chlk.services.DeveloperService, 'developerService',

        [ria.mvc.Inject],
        chlk.services.AccountService, 'accountService',

        [ria.mvc.ServiceEvent(chlk.services.DeveloperService)],
        [[String]],
        VOID, function onUserInfoChange(userName) {
            this.getContext().getDefaultView().updateUserName(userName);
        },

        [[chlk.models.account.ChangePassword]],
        function changePasswordAction(model){
            this.accountService
                .changePassword(model.getOldPassword(), model.getNewPassword(), model.getNewPasswordConfirmation(), model.isWithOldPassword())
                .catchException(chlk.lib.exception.ChalkableException, function(ex){
                    return this.ShowMsgBox(ex.getMessage(), 'oops',[{ text: Msg.GOT_IT.toUpperCase() }])
                        .then(function(){
                            this.BackgroundNavigate('settings', 'dashboard', []);
                        }, this)
                        .thenBreak();
                }, this)
                .attach(this.validateResponse_())
                .then(function(success){
                    return success
                        ? this.ShowAlertBox('Password was changed.')
                        : this.ShowAlertBox('Change password failed.');
                }, this)
                .then(function () {
                    return this.BackgroundNavigate('settings', 'dashboard', []);
                }, this);

            return null;
        },

         function logoutAction(){
             return this.accountService
                 .logOut()
                 .attach(this.validateResponse_())
                 .then(function(res){
                    if (res.isSuccess()){
                        location.href = this.getContext().getSession().get('webSiteRoot');
                        return ria.async.BREAK;
                    }
                 }, this);
         },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [chlk.controllers.SidebarButton('settings')],
         function profileDeveloperAction(){
             var withOldPassword =  this.getContext().getSession().get(ChlkSessionConstants.REDIRECT_URL) == '' ? true : false;
             var result = this.developerService
                 .getInfo(this.developerService.getCurrentDeveloperSync().getId())
                 .then(function (res) {
                    res.setWithOldPassword(withOldPassword);
                    return res;
                 })
                 .attach(this.validateResponse_());
             return this.PushView(chlk.activities.profile.DeveloperPage, result);
         },

         [chlk.controllers.AccessForRoles([
             chlk.models.common.RoleEnum.DEVELOPER
         ])],
        [chlk.controllers.SidebarButton('settings')],
        [[chlk.models.developer.DeveloperInfo]],
        function profileSaveDeveloperAction(model){
            this.developerService
                .saveInfo(
                    model.getId(),
                    model.getName(),
                    model.getWebSite(),
                    model.getEmail()
                )
                .attach(this.validateResponse_())
                .then(function(res){
                    return res != null && res.isValidInfo()
                        ? this.ShowAlertBox('Info saved.')
                        : this.ShowAlertBox(!res.isValidEmail() ? 'Email already exists' : 'Info save failed.')
                }, this)
                .then(function () {
                    return this.BackgroundNavigate('settings', 'dashboard', []);
                }, this);
            return null;
        },

        [chlk.controllers.SidebarButton('settings')],
        function resetPasswordAction(){
            var withOldPassword =  this.getContext().getSession().get(ChlkSessionConstants.REDIRECT_URL) == '' ? true : false;
            var model = new chlk.models.account.ChangePassword("old", "new", "new", withOldPassword);
            return this.PushView(chlk.activities.settings.ChangePasswordPage, ria.async.DeferredData(model));
        }
    ])
});
