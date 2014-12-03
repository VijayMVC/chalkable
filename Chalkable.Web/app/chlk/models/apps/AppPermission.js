REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppPermissionId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppPermissionTypeEnum*/
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

    /** @class chlk.models.apps.AppPermission*/
    CLASS(
        UNSAFE, FINAL, 'AppPermission', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.type, chlk.models.apps.AppPermissionTypeEnum);
                this.name = SJX.fromValue(raw.name, String);
            },

            chlk.models.apps.AppPermissionTypeEnum, 'id',
            String, 'name',
            [[chlk.models.apps.AppPermissionTypeEnum, String]],
            function $(id_, name_){
                BASE();
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            }
        ]);
});