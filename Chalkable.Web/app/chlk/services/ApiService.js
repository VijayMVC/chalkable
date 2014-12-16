REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.SimpleResult');
REQUIRE('chlk.models.api.ApiRoleInfo');
REQUIRE('chlk.models.api.ApiCallRequestData');
REQUIRE('chlk.models.api.ApiListItem');




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
                            controllers = controllers
                                .filter(function(contr){
                                   var methods = contr.getMethods() || [];
                                    return methods.length > 0;
                                })
                                .map(function(contr){
                                    var controllerName = contr.getName().toLowerCase();

                                    if (controllerName.toLowerCase() != 'person'){
                                        var filteredMethods = (contr.getMethods() || []).filter(function(method){
                                            return method.getName().toLowerCase() != 'me';
                                        });
                                        contr.setMethods(filteredMethods);
                                    }
                                    return contr;
                                });
                            dataItem.setControllers(controllers);
                            apiCache[dataItem.getRole()] = dataItem;
                            apiRoles.push(dataItem.getRole());
                        });
                        this.getContext().getSession().set(ChlkSessionConstants.API_CACHED, apiCache);
                        this.getContext().getSession().set(ChlkSessionConstants.API_ROLES, apiRoles);
                        return apiCache;
                    }, this);
            },

            [[String]],
            ria.async.Future, function listApiForRole(role){
                var apiCache = this.getContext().getSession().get(ChlkSessionConstants.API_CACHED);
                return apiCache && apiCache.hasOwnProperty(role) ? ria.async.DeferredData(apiCache[role]) :
                    this
                        .listApi()
                        .then(function(data){
                            return data[role];
                    });
            },

            ArrayOf(String), function getApiRoles(){
                return this.getContext().getSession().get(ChlkSessionConstants.API_ROLES) || [];
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
            },


            [[String, Boolean, String]],
            ria.async.Future, function getRequiredApiCalls(query, isMethod, role){
                return this
                    .post('Developer/GetRequiredMethodCallsFor.json', ArrayOf(chlk.models.api.ApiListItem), {
                        query: query,
                        role: role,
                        isMethod: isMethod
                    })
                    .then(function(data){
                        return data;
                    });
            },

            [[String, String]],
            ria.async.Future, function getList(role, query){
                return this
                    .post('Developer/MethodParamList.json', ArrayOf(chlk.models.api.ApiListItem), {
                        query: query,
                        role: role
                    })
                    .then(function(data){
                        return data.map(function(item){
                           item.setRole(role);
                           item.setCategory(item.isMethod() ? "Methods" : "Params");
                           return item;
                        })
                    });
            }
        ])
});