REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.models.people.UserProfileModel');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.SchoolPersonInfoPageTpl*/
    ASSET('~/assets/jade/activities/profile/profile-info-blocks.jade')();

    CLASS(
         [ria.templates.TemplateBind('~/assets/jade/activities/profile/school-person-info-page.jade')],
         [ria.templates.ModelBind(chlk.models.people.UserProfileModel)],
        'SchoolPersonInfoPageTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'user',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ActionLinkModel), 'actionLinksData'
        ]);
});