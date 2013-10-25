REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppPermissionId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.model.apps.AppPermissionTypeEnum*/
    ENUM('AppPermissionTypeEnum',{
        USER: 0,
        MESSAGE: 1,
        GRADE: 2,
        ATTENDANCE: 3,
        ANNOUNCEMENT: 4,
        CLAZZ: 5,
        SCHEDULE: 6,
        DISCIPLINE: 7,
        UNKNOWN: 999
    });

    /** @class chlk.model.apps.AppPermission*/
    CLASS(
        'AppPermission', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.apps.AppPermissionTypeEnum, 'id',
            String, 'name',
            [[chlk.models.apps.AppPermissionTypeEnum, String]],
            function $(id_, name_){
                BASE();
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            },

            VOID, function deserialize(raw){
                this.setId(new chlk.models.apps.AppPermissionTypeEnum(raw.type));
                this.setName(raw.name);
            }
        ]);
});