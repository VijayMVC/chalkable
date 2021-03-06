REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.ApiService');
REQUIRE('chlk.services.DeveloperService');

REQUIRE('chlk.templates.common.footers.DeveloperFooter');

REQUIRE('chlk.activities.developer.DeveloperDocsPage');
REQUIRE('chlk.activities.developer.ApiExplorerPage');
REQUIRE('chlk.activities.developer.PayPalSettingsPage');

REQUIRE('chlk.models.common.footers.DeveloperFooter');
REQUIRE('chlk.models.api.ApiExplorerViewData');
REQUIRE('chlk.models.developer.PayPalInfo');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.DeveloperController*/
    CLASS(
        'DeveloperController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.ApplicationService, 'appsService',

            [ria.mvc.Inject],
            chlk.services.DeveloperService, 'developerService',

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


            [chlk.controllers.SidebarButton('docs')],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            function docsAction(){
               var devDocsModel = new chlk.models.developer.DeveloperDocs(this.getContext().getSession().get('webSiteRoot') + '/Developer/DeveloperDocs?InFrame=true');
               return this.PushView(chlk.activities.developer.DeveloperDocsPage, new ria.async.DeferredData(devDocsModel));
            },

            [chlk.controllers.AccessForRoles([chlk.models.common.RoleEnum.DEVELOPER])],
            function paypalSettingsAction(){
                var developer = this.developerService.getCurrentDeveloperSync();
                var paypalSettings = new chlk.models.developer.PayPalInfo(developer.getPayPalAddress());
                return this.PushView(chlk.activities.developer.PayPalSettingsPage, new ria.async.DeferredData(paypalSettings));
            },

            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            [[chlk.models.developer.PayPalInfo]],
            function updatePaymentInfoAction(info){
                var developer = this.developerService.getCurrentDeveloperSync();
                var result = this.developerService.updatePaymentInfo(
                    developer.getId(),
                    info.getEmail()
                );
                return this.UpdateView(chlk.activities.developer.PayPalSettingsPage, result);
            },

            [chlk.controllers.SidebarButton('api')],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            [[String]],
            function apiAction(role_){
                var showForRole = role_ ? role_ : "teacher";
                var result = this.apiService
                    .listApiForRole(showForRole)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var currentApp = this.appsService.getCurrentApp();
                        var secretKey = currentApp.getSecretKey() || 'no-key';
                        var apiRoles = this.apiService.getApiRoles();
                        return chlk.models.api.ApiExplorerViewData.$create(data, secretKey, apiRoles);
                    }, this);
                return this.PushView(chlk.activities.developer.ApiExplorerPage, result);
            },

            [chlk.controllers.SidebarButton('api')],
            [chlk.controllers.AccessForRoles([
                chlk.models.common.RoleEnum.DEVELOPER
            ])],
            [[Object]],
            function makeApiCallAction(data){
                var controllerName = data.controllerName;
                var methodName = data.methodName;
                var apiFormId = data.apiFormId;
                var methodCallType = data.callType;
                var fakeResponse = data.response;

                delete data.controllerName;
                delete data.methodName;
                delete data.apiFormId;
                delete data.callType;
                delete data.response;

                var apiCallData = chlk.models.api.ApiCallRequestData.$create(controllerName, methodName, data, methodCallType);
                var result = fakeResponse !== ""
                    ? ria.async.DeferredData(chlk.models.api.ApiResponse.$create(apiFormId, JSON.parse(fakeResponse)))
                    : this.apiService
                        .callApi(apiCallData)
                        .attach(this.validateResponse_())
                        .then(function(data){
                            return chlk.models.api.ApiResponse.$create(apiFormId, data);
                        });
                
                return this.UpdateView(chlk.activities.developer.ApiExplorerPage, result);
            },


            [[String, Boolean, String]],
            function getRequiredApiCallsAction(query, isMethod, role){
                 var result = this.apiService
                     .getRequiredApiCalls(query, isMethod, role)
                     .attach(this.validateResponse_())
                     .then(function(data){
                         return chlk.models.api.ApiCallSequence.$create(data);
                     });
                 return this.UpdateView(chlk.activities.developer.ApiExplorerPage, result, 'update-api-calls-list');
            }
        ])
});
