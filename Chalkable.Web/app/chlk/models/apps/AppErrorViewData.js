REQUIRE('chlk.models.apps.AppWrapperViewData');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppErrorViewData*/
    CLASS(
        'AppErrorViewData', EXTENDS(chlk.models.apps.AppWrapperViewData), [
            String, 'developerEmail',

            function $(){
                BASE('', []);
            }
        ]);
});
