NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppErrorViewData*/
    CLASS(
        'AppErrorViewData', [
            String, 'developerEmail',

            [[String]],
            function $(devMail){
                BASE();
                this.setDeveloperEmail(devMail);
            }
        ]);
});
