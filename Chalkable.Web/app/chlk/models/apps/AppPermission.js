REQUIRE('chlk.models.id.AppPermissionId');
NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.model.apps.AppPermission*/
    CLASS(
        'AppPermission', [
            chlk.models.id.AppPermissionId, 'id',
            Number, 'type',
            String, 'name',
            [[chlk.models.id.AppPermissionId, String, Number]],
            function $(id, name, type){
                this.setId(id);
                this.setName(name);
                this.setType(type);
            }
        ]);
});