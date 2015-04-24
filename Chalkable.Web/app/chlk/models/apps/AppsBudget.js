REQUIRE('chlk.models.apps.InstalledApp');

NAMESPACE('chlk.models.apps', function(){
   "use strict";

    /**@class chlk.models.apps.AppsBudget*/
    CLASS('AppsBudget', [

        Number, 'balance',
        Number, 'reserve',

        [ria.serialize.SerializeProperty('installedapplications')],
        ArrayOf(chlk.models.apps.InstalledApp), 'installedApps'
    ]);
});