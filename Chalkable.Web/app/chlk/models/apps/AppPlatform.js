NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppPlatformTypeEnum*/
    ENUM('AppPlatformTypeEnum',{
        WEB: 0,
        IOS: 1,
        ANDROID: 2
    });

    /** @class chlk.models.apps.AppPlatform*/
    CLASS(
        'AppPlatform', IMPLEMENTS(ria.serialize.IDeserializable),  [
            chlk.models.apps.AppPlatformTypeEnum, 'id',
            String, 'name',
            [[chlk.models.apps.AppPlatformTypeEnum, String]],
            function $(id_, name_){
                BASE();
                if (id_)
                    this.setId(id_);
                if (name_)
                    this.setName(name_);
            },

            VOID, function deserialize(raw){
                this.setId(new chlk.models.apps.AppPlatformTypeEnum(raw.type));
                this.setName(raw.name);
            }
        ]);
});