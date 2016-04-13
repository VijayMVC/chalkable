REQUIRE('chlk.models.apps.ExternalAttachAppViewData');

NAMESPACE('chlk.models.settings', function () {

    "use strict";
    /** @class chlk.models.settings.AppSettingsViewData*/
    CLASS(
        'AppSettingsViewData', EXTENDS(chlk.models.apps.ExternalAttachAppViewData), [

            ArrayOf(chlk.models.apps.Application), 'installedApps',

            [[chlk.models.common.AttachOptionsViewData, chlk.models.apps.Application, String, String, ArrayOf(chlk.models.apps.Application)]],
            function $(options, app, url, title, installedApps){
                BASE(options, app, url, title);

                this.setInstalledApps(installedApps);
            }
        ]);
});
