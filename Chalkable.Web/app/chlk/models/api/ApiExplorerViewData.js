REQUIRE('chlk.models.api.ApiRoleInfo');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiExplorerViewData*/
    CLASS(
        'ApiExplorerViewData', [
            chlk.models.api.ApiRoleInfo, 'apiInfo',
            String, 'appSecretKey',
            ArrayOf(String), 'apiRoles',

            [[chlk.models.api.ApiRoleInfo, String, ArrayOf(String)]],
            function $create(apiInfo, appSecretKey, apiRoles){
                BASE();
                this.setApiInfo(apiInfo);
                this.setAppSecretKey(appSecretKey);
                this.setApiRoles(apiRoles);
            }
        ]);
});
