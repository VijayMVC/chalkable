NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiCallRequestData*/
    CLASS(
        'ApiCallRequestData', [
            String, 'controllerName',
            String, 'actionName',
            String, 'callType',
            Object, 'params',

            [[String, String, Object, String]],
            function $create(controller, action, params, callType){
                BASE();
                this.setControllerName(controller);
                this.setActionName(action);
                this.setParams(params);
                this.setCallType(callType);
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
