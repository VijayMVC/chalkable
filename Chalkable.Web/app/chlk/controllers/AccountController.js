REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DeveloperService');
REQUIRE('chlk.services.AccountService');
REQUIRE('chlk.activities.profile.DeveloperPage');
REQUIRE('chlk.activities.profile.ChangePasswordPage');
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

         function changePasswordAction(){
             var model = new ria.async.DeferredData(new Class());
             return this.PushView(chlk.activities.profile.ChangePasswordPage, model);
         },

        [[chlk.models.account.ChangePassword]],
        function doChangePasswordAction(model){
            this.accountService
                .changePassword(model.getOldPassword(), model.getNewPassword(), model.getNewPasswordConfirmation())
                .then(
                    function(success){
                        if (success)
                            return this.redirect_('settings', 'dashboardTeacher', []);
                    }.bind(this)
                );
        },

         function logoutAction(){
             this.accountService
                 .logOut()
                 .then(function(res){
                    if (res.isSuccess()){
                        location.href = this.getContext().getSession().get('webSiteRoot');
                    }
                 }, this);
         },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [chlk.controllers.SidebarButton('settings')],
         [[chlk.models.id.SchoolPersonId]],
         function profileDeveloperAction(id){
             var result = this.developerService.getInfo(id);
             return this.PushView(chlk.activities.profile.DeveloperPage, result);
         },

         [chlk.controllers.AccessForRoles([
             chlk.models.common.RoleEnum.DEVELOPER
         ])],
        [chlk.controllers.SidebarButton('settings')],
        [[chlk.models.developer.DeveloperInfo]],
        function profileSaveDeveloperAction(model){
            var result = this.developerService
                .saveInfo(
                    model.getId(),
                    model.getName(),
                    model.getWebSite(),
                    model.getEmail()
                )
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.profile.DeveloperPage, result);

        }
    ])
});
