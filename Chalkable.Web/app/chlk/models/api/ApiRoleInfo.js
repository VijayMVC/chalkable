REQUIRE('chlk.models.api.ApiControllerInfo');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiRoleInfo*/
    CLASS(
        'ApiRoleInfo', [
            ArrayOf(chlk.models.api.ApiControllerInfo), 'controllers',
            String, 'token',
            String, 'role',

            [[ArrayOf(chlk.models.api.ApiControllerInfo), String, String]],
            function $create(controllers, token, role){
                BASE();
                this.setControllers(controllers);
                this.setToken(token);
                this.setRole(role);
            }
        ]);
});
