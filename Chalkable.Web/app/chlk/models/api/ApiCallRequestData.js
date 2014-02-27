NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiCallRequestData*/
    CLASS(
        'ApiCallRequestData', [
            String, 'controllerName',
            String, 'actionName',
            Object, 'params',

            [[String, String, Object]],
            function $create(controller, action, params){
                BASE();
                this.setControllerName(controller);
                this.setActionName(action);
                this.setParams(params);
            },

            String, function getApiRole(){
                var data = this.getParams() || {apiCallRole: "none"};
                return data['apiCallRole'];
            },

            Object, function getApiCallParams(){
                var data = this.getParams();
                if (data.apiCallRole)
                    delete data.apiCallRole;
                if (data.controllerName)
                    delete data.controllerName;
                if (data.methodName)
                    delete data.methodName;
                 if (data.apiFormId)
                    delete data.apiFormId;
                return data;
            }
        ]);
});
