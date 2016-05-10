REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.apps', function(){
   "use strict";

    /**@class chlk.models.apps.InstalledApp*/
    CLASS('InstalledApp', EXTENDS(chlk.models.apps.Application),[

        [ria.serialize.SerializeProperty('hasmyapps')],
        Boolean, 'myapps'
    ]);
});