REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.people.UserProfileAppsViewData');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.SchoolPersonProfileAppsTpl*/

    ASSET('~/assets/jade/activities/profile/ProfileApps.jade')();
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/SchoolPersonProfileAppsView.jade')],
        [ria.templates.ModelBind(chlk.models.people.UserProfileAppsViewData)],
        'SchoolPersonProfileAppsTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl), [

        ]);
});