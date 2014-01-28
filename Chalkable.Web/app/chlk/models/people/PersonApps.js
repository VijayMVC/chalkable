REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.apps.AppsBudget');

NAMESPACE('chlk.models.people', function(){
   "use strict";

    /**@class chlk.models.people.PersonApps*/
    CLASS('PersonApps', EXTENDS(chlk.models.people.ShortUserInfo),[
        [ria.serialize.SerializeProperty('appsbudget')],
        chlk.models.apps.AppsBudget, 'appsBudget'
    ]);
});