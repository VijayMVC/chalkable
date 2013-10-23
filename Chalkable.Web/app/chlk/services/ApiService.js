REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.SimpleResult');
REQUIRE('chlk.models.api.ApiRoleInfo');
REQUIRE('chlk.models.api.ApiCallRequestData');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ApiService*/
    CLASS(
        'ApiService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function listApi(){
                return this
                    .get('Developer/ListApi.json', ArrayOf(chlk.models.api.ApiRoleInfo), {})
                    .then(function(data){
                        var apiCache = {};
                        var items = data || [];
                        var apiRoles = [];
                        items.forEach(function(dataItem){
                            var controllers = dataItem.getControllers() || [];
                            controllers = controllers.filter(function(contr){
                               var methods = contr.getMethods() || [];
                                return methods.length > 0;
                            });
                            dataItem.setControllers(controllers);
                            apiCache[dataItem.getRole()] = dataItem;
                            apiRoles.push(dataItem.getRole());
                        });
                        this.getContext().getSession().set('apiCached', apiCache);
                        this.getContext().getSession().set('apiRoles', apiRoles);
                        return apiCache;
                    }, this);
            },

            [[String]],
            ria.async.Future, function listApiForRole(role){
                var apiCache = this.getContext().getSession().get('apiCached');
                return apiCache ? ria.async.DeferredData(apiCache[role]) :
                    this
                        .listApi()
                        .then(function(data){
                            return data[role];
                    });
            },

            ArrayOf(String), function getApiRoles(){
                return this.getContext().getSession().get('apiRoles') || [];
            },

            [[chlk.models.api.ApiCallRequestData]],
            ria.async.Future, function callApi(requestData){
                var role = requestData.getApiRole();
                //todo: check if there is token for role
                return this
                    .listApiForRole(role)
                    .then(function(data){
                        var token = data.getToken();
                        var url = requestData.getControllerName() + "/" + requestData.getActionName()+ ".json";
                        var params = requestData.getApiCallParams();
                        return this.makeApiCall(url, token,  params);
                    }, this)
                //todo: update response area
            }

        ])
});