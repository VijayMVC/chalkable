REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppInstallInfo');

NAMESPACE('chlk.models.apps', function(){
   "use strict";

    /**@class chlk.models.apps.InstalledApp*/
    CLASS('InstalledApp', EXTENDS(chlk.models.apps.Application),[

        [ria.serialize.SerializeProperty('applicationinstalls')],
        ArrayOf(chlk.models.apps.AppInstallInfo), 'appInstalls',

        [ria.serialize.SerializeProperty('hasmyapps')],
        Boolean, 'myapps'
    ]);
});