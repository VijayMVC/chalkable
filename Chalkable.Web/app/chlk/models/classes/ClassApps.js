REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.apps.AppsBudget');

NAMESPACE('chlk.models.classes', function(){
   "use strict";
    /**@class chlk.models.classes.ClassApps*/

    CLASS('ClassApps', EXTENDS(chlk.models.classes.Class),[

        [ria.serialize.SerializeProperty('appsbudget')],
        chlk.models.apps.AppsBudget, 'appsBudget'
    ]);
});