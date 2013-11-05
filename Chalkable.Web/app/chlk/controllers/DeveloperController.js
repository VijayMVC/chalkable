REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.ApiService');


REQUIRE('chlk.templates.common.footers.DeveloperFooter');

REQUIRE('chlk.activities.developer.DeveloperDocsPage');
REQUIRE('chlk.activities.developer.ApiExplorerPage');

REQUIRE('chlk.models.common.footers.DeveloperFooter');
REQUIRE('chlk.models.api.ApiExplorerViewData');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DeveloperController*/
    CLASS(
        'DeveloperController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [ria.mvc.Inject],
            chlk.services.ApiService, 'apiService',

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
                var model = new chlk.models.common.footers.DeveloperFooter(this.appsService.getCurrentAppId(), apps);
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
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],

            [[String]],
            function apiAction(role_){
                var showForRole = role_ ? role_ : "teacher";
                var result = this.apiService
                    .listApiForRole(showForRole)
                    .then(function(data){
                        var currentApp = this.appsService.getCurrentApp();
                        var secretKey = currentApp.getSecretKey() || 'no-key';
                        var apiRoles = this.apiService.getApiRoles();
                        return chlk.models.api.ApiExplorerViewData.$create(data, secretKey, apiRoles);
                    }, this)
                return this.PushView(chlk.activities.developer.ApiExplorerPage, result);
            },


            [[Object]],
            function makeApiCallAction(data){
                var controllerName = data.controllerName;
                var methodName = data.methodName;
                var apiFormId = data.apiFormId;
                var apiCallData = chlk.models.api.ApiCallRequestData.$create(controllerName, methodName, data);
                var result = this.apiService
                    .callApi(apiCallData)
                    .then(function(data){
                        return chlk.models.api.ApiResponse.$create(apiFormId, data);
                    });
                return this.UpdateView(chlk.activities.developer.ApiExplorerPage, result);
            },


            [[String, Boolean, String]],
            function getRequiredApiCallsAction(query, isMethod, role){
                 var result = this.apiService
                     .getRequiredApiCalls(query, isMethod, role)
                     .then(function(data){
                         var seq= chlk.models.api.ApiCallSequence.$create(data);
                         return seq;
                     });
                 return this.UpdateView(chlk.activities.developer.ApiExplorerPage, result, 'update-api-calls-list');
            }
        ])
});
