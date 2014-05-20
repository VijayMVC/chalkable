NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiResponse*/
    CLASS(
        'ApiResponse', [
            String, 'responseData',
            String, 'apiFormId',

            [[String, Object]],
            function $create(apiFormId, response){
                BASE();
                this.setApiFormId(apiFormId);
                var responseText = JSON.stringify(response, undefined, 2);
                this.setResponseData(responseText);
            }
        ]);
});
