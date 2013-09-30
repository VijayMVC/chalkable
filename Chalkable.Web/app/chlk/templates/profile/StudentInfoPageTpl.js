REQUIRE('chlk.templates.profile.SchoolPersonInfoPageTpl');
REQUIRE('chlk.models.people.UserProfileModel');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.StudentInfoPageTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/student-info-page.jade')],
        [ria.templates.ModelBind(chlk.models.people.UserProfileModel)],
        'StudentInfoPageTpl', EXTENDS(chlk.templates.profile.SchoolPersonInfoPageTpl), [])
});
