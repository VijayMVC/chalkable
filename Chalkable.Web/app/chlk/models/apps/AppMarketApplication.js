REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.developer.DeveloperInfo');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.apps.AppInstallInfo');
REQUIRE('chlk.models.apps.AppRating');


NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppMarketApplication*/
    CLASS(
        UNSAFE,  'AppMarketApplication', EXTENDS(chlk.models.apps.Application), IMPLEMENTS(ria.serialize.IDeserializable), [
            
            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.installedForGroups = SJX.fromArrayOfDeserializables(raw.installedforpersonsgroup, chlk.models.apps.AppInstallGroup);
                this.installedOnlyForCurrentUser = SJX.fromValue(raw.isinstalledonlyforme, Boolean);
                this.applicationInstalls = SJX.fromArrayOfDeserializables(raw.applicationinstalls, chlk.models.apps.AppInstallInfo);
                this.uninstallable = SJX.fromValue(raw.uninstallable, Boolean);
                this.selfInstalled = SJX.fromValue(raw.selfinstalled, Boolean);
                this.personal = SJX.fromValue(raw.personal, Boolean);
                this.applicationInstallIds = SJX.fromValue(raw.applicationinstallids, String);
                this.applicationRating = SJX.fromDeserializable(raw.applicationrating, chlk.models.apps.AppRating);

            },
            ArrayOf(chlk.models.apps.AppInstallGroup), 'installedForGroups',
            Boolean, 'installedOnlyForCurrentUser',
            ArrayOf(chlk.models.apps.AppInstallInfo), 'applicationInstalls',
            Boolean, 'uninstallable',
            Boolean, 'selfInstalled',
            Boolean, 'personal',
            String,  'applicationInstallIds',
            chlk.models.apps.AppRating, 'applicationRating'

        ]);


});
