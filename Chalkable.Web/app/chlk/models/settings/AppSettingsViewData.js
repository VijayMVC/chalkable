REQUIRE('chlk.models.apps.ExternalAttachAppViewData');

NAMESPACE('chlk.models.settings', function () {

    "use strict";
    /** @class chlk.models.settings.AppSettingsViewData*/
    CLASS(
        'AppSettingsViewData', EXTENDS(chlk.models.apps.ExternalAttachAppViewData), [

            ArrayOf(chlk.models.apps.Application), 'applications',

            [[chlk.models.common.AttachOptionsViewData, chlk.models.apps.Application, String, String, ArrayOf(chlk.models.apps.Application)]],
            function $(options, app, url, title, applications){
                BASE(options, app, url, title);

                this.setApplications(applications);
            }
        ]);
});
