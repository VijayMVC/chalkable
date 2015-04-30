REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.apps.AppsBudget');

NAMESPACE('chlk.models.people', function(){
   "use strict";

    var SJX = ria.serialize.SJX;
    /**@class chlk.models.people.PersonApps*/
    CLASS(
        UNSAFE, 'PersonApps', EXTENDS(chlk.models.people.ShortUserInfo), [

        //Number, 'balance',
        //Number, 'reserve',
        //
       // ArrayOf(chlk.models.apps.InstalledApp), 'installedApps',

        chlk.models.apps.AppsBudget, 'appsBudget',

        OVERRIDE, VOID, function deserialize(raw){
            BASE(raw.person);
            this.appsBudget = new chlk.models.apps.AppsBudget();
            this.appsBudget.setBalance(SJX.fromValue(raw.balance, Number));
            this.appsBudget.setReserve(SJX.fromValue(raw.reserve, Number));
            this.appsBudget.setInstalledAppsCount(SJX.fromValue(raw.installedappscount, Number));
            if(raw.installedapps)
                this.appsBudget.setInstalledApps(SJX.fromArrayOfDeserializables(raw.installedapps, chlk.models.apps.InstalledApp));
        }

    ]);
});