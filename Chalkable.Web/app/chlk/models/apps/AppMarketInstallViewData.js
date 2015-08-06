REQUIRE('chlk.models.apps.AppMarketApplication');
REQUIRE('chlk.models.apps.AppInstallGroup');

REQUIRE('chlk.models.classes.AllSchoolsActiveClasses');

NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.AppMarketInstallViewData*/
    CLASS(
        'AppMarketInstallViewData',EXTENDS(chlk.models.apps.AppMarketDetailsViewData), [
            Boolean, 'alreadyInstalled',

            chlk.models.classes.AllSchoolsActiveClasses, 'allClasses',

            [[chlk.models.apps.AppMarketApplication, chlk.models.classes.AllSchoolsActiveClasses]],
            function $(app, allClasses_, fromNewItem_){
                BASE(app, "", [], [], 0, app.isAlreadyInstalled(), fromNewItem_);
                this.setAlreadyInstalled(app.isAlreadyInstalled());
                this.setAllClasses(allClasses_ || null);
            }
        ]);


});
