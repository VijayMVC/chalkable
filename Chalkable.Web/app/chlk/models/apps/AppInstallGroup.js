REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AppInstallGroupId');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppInstallGroupTypeEnum*/

    ENUM('AppInstallGroupTypeEnum', {
        ALL: 0,
        CLAZZ: 1,
        GRADELEVEL: 2,
        DEPARTMENT: 3,
        ROLE: 4,
        GROUP: 5,
        CURRENT_USER: 6
    });

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppInstallGroup*/
    CLASS(
        UNSAFE, FINAL, 'AppInstallGroup', IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.description = SJX.fromValue(raw.description, String);
                this.tooltipHint = SJX.fromValue(raw.tooltiphint, String);
                this.groupType = SJX.fromValue(raw.grouptype, chlk.models.apps.AppInstallGroupTypeEnum);
                this.id = SJX.fromValue(raw.id, chlk.models.id.AppInstallGroupId);
                this.installed = SJX.fromValue(raw.isinstalled, Boolean);
            },

            String, 'description',
            String, 'tooltipHint',
            chlk.models.apps.AppInstallGroupTypeEnum, 'groupType',
            chlk.models.id.AppInstallGroupId, 'id',
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
            }
    ])
});
