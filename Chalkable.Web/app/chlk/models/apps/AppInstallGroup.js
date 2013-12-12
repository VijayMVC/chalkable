REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppInstallGroupId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppInstallGroupTypeEnum*/

    ENUM('AppInstallGroupTypeEnum', {
        CLAZZ: 1,
        GRADELEVEL: 2,
        DEPARTMENT: 3,
        ROLE: 4,
        ALL: 5,
        CURRENT_USER: 6
    });

    /** @class chlk.models.apps.AppInstallGroup*/
    CLASS(
        'AppInstallGroup', IMPLEMENTS(ria.serialize.IDeserializable),[

            String, 'description',

            [ria.serialize.SerializeProperty('grouptype')],
            chlk.models.apps.AppInstallGroupTypeEnum, 'groupType',

            chlk.models.id.AppInstallGroupId, 'id',

            chlk.models.apps.AppInstallGroupTypeEnum, 'isinstalled',
            Boolean, 'installed',

            [[chlk.models.id.AppInstallGroupId, chlk.models.apps.AppInstallGroupTypeEnum, Boolean, String]],
            function $(installGroupId_ ,groupType_, installed_, description_){
                BASE();
                if (installGroupId_)
                    this.setId(installGroupId_);
                if (groupType_)
                    this.setGroupType(groupType_);
                if (installed_)
                    this.setInstalled(installed_);
                if (description_)
                    this.setDescription(description_);
            },

            VOID, function deserialize(raw){
                this.setGroupType(new chlk.models.apps.AppInstallGroupTypeEnum(Number(raw.grouptype)));
                this.setId(new chlk.models.id.AppInstallGroupId(raw.id));
                this.setDescription(raw.description);
                this.setInstalled(Boolean(raw.isinstalled));
            }
    ])

});
