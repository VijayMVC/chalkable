REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.people.UserProfileInfoViewData');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.SchoolPersonInfoPageTpl*/
    ASSET('~/assets/jade/activities/profile/profile-info-blocks.jade')();

    CLASS(
         [ria.templates.TemplateBind('~/assets/jade/activities/profile/school-person-info-page.jade')],
         [ria.templates.ModelBind(chlk.models.people.UserProfileInfoViewData)],
        'SchoolPersonInfoPageTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.people.User)), [

        ]);
});