REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.signup.SignUpInfo');

NAMESPACE('chlk.templates.signup', function () {

    /** @class chlk.templates.signp.SignUpList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/signup/SignUps.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'SignUpList', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.signup.SignUpInfo), 'items'
        ])
});