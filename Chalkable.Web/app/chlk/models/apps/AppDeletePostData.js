NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppDeletePostData*/
    CLASS(
        'AppDeletePostData', [
            String, 'appName',
            String, 'applicationInstallIds',
            Boolean, 'selfInstalled',
            Boolean, 'personal',
            Boolean, 'uninstallable'
        ]);
});
