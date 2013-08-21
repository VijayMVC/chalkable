REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.models.common.footers.DeveloperFooter');
REQUIRE('chlk.templates.common.footers.DeveloperFooter');
REQUIRE('chlk.activities.developer.DeveloperDocsPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DeveloperController*/
    CLASS(
        'DeveloperController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            OVERRIDE, ria.async.Future, function onAppInit() {
                this.appsService.getDevApplicationListChange()
                    .on(this.refresh_);

                return BASE()
                    .then(function(){
                        this.appsService
                            .getDevApps()
                            .then(this.refresh_);
                    }, this);
            },


            [[ArrayOf(chlk.models.apps.Application)]],
            VOID, function refresh_(apps) {
                var footerTpl = new chlk.templates.common.footers.DeveloperFooter();
                var model = new chlk.models.common.footers.DeveloperFooter(this.appsService.getCurrentApp(), apps);
                footerTpl.assign(model);

                new ria.dom.Dom()
                    .fromHTML(footerTpl.render())
                    .appendTo(new ria.dom.Dom('#demo-footer').empty());
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            function docsAction(){
               var devDocsModel = new chlk.models.developer.DeveloperDocs(this.getContext().getSession().get('webSiteRoot') + '/Developer/DeveloperDocs?InFrame=true');
               return this.PushView(chlk.activities.developer.DeveloperDocsPage, new ria.async.DeferredData(devDocsModel));
            }


        ])
});
