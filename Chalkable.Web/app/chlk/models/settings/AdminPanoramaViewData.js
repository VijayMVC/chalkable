REQUIRE('chlk.models.panorama.AdminPanoramaSettingsViewData');
REQUIRE('chlk.models.profile.StandardizedTestViewData');
REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.settings', function () {
    "use strict";

    /** @class chlk.models.settings.AdminPanoramaViewData*/
    CLASS('AdminPanoramaViewData', [

        chlk.models.panorama.AdminPanoramaSettingsViewData, 'panoramaSettings',
        ArrayOf(chlk.models.apps.Application), 'applications',
        ArrayOf(chlk.models.profile.StandardizedTestViewData), 'standardizedTests',

        [[chlk.models.panorama.AdminPanoramaSettingsViewData, ArrayOf(chlk.models.apps.Application, ArrayOf(chlk.models.profile.StandardizedTestViewData))]],
        function $(panoramaSettings_, applications_, standardizedTests_){
            BASE();
            if(panoramaSettings_)
                this.setPanoramaSettings(panoramaSettings_);
            if(applications_)
                this.setApplications(applications_);
            if(standardizedTests_)
                this.setStandardizedTests(standardizedTests_);
        }
    ]);
});
