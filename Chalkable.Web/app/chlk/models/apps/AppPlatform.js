REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppPlatformTypeEnum*/
    ENUM('AppPlatformTypeEnum',{
        WEB: 0,
        IOS: 1,
        ANDROID: 2
    });

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppPlatform*/
    CLASS(
        UNSAFE, FINAL ,'AppPlatform', IMPLEMENTS(ria.serialize.IDeserializable),  [
            VOID, function deserialize(raw){
                this.id = SJX.fromValue(raw.type, chlk.models.apps.AppPlatformTypeEnum);
                this.name = SJX.fromValue(raw.name, String);
            },

            chlk.models.apps.AppPlatformTypeEnum, 'id',
            String, 'name',

            [[chlk.models.apps.AppPlatformTypeEnum, String]],
            function $(id_, name_){
                BASE();
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            }
        ]);
});