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

        [[chlk.models.account.ChangePassword]],
        function teacherChangePasswordAction(model){
            return this.accountService
                .changePassword(model.getOldPassword(), model.getNewPassword(), model.getNewPasswordConfirmation())
                .attach(this.validateResponse_())
                .then(function(success){
                    if (success)
                        return this.BackgroundNavigate('settings', 'dashboardTeacher', []);
                }, this);
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

         function goToInowAction(){
             return this.accountService.redirectToINOW();
         },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [chlk.controllers.SidebarButton('settings')],
         [[chlk.models.id.SchoolPersonId]],
         function profileDeveloperAction(id){
             var result = this.developerService
                 .getInfo(id)
                 .attach(this.validateResponse_());
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
