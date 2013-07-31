REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.DeveloperService');
REQUIRE('chlk.activities.profile.DeveloperPage');
REQUIRE('chlk.models.id.DeveloperId');
REQUIRE('chlk.models.developer.DeveloperInfo');



NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AccountController*/
    CLASS(
        'AccountController', EXTENDS(chlk.controllers.BaseController), [


         [ria.mvc.Inject],
         chlk.services.DeveloperService, 'developerService',

         function resetPasswordAction(){

         },


        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DEVELOPER
        ])],
        [chlk.controllers.SidebarButton('settings')],
         //[[chlk.models.id.DeveloperId]]
         function profileDeveloperAction(){
             //var result = this.developerService.getInfo(id);
             var developerInfo = new chlk.models.developer.DeveloperInfo();
             return this.PushView(chlk.activities.profile.DeveloperPage, ria.async.DeferredData(developerInfo));
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
            return this.PushView(chlk.activities.profile.DeveloperPage, result);

        }
    ])
});
