REQUIRE('chlk.models.apps.Application');

NAMESPACE('chlk.models.common.footers', function () {
    "use strict";

    /** @class chlk.models.common.footers.DeveloperFooter*/
    CLASS(
        'DeveloperFooter', [
            chlk.models.apps.Application, 'currentApp',
            ArrayOf(chlk.models.apps.Application), 'developerApps',

            [[chlk.models.apps.Application,  ArrayOf(chlk.models.apps.Application)]],
            function $(app, devApps){
                this.setCurrentApp(app);
                this.setDeveloperApps(devApps);
            }
        ]);
});
