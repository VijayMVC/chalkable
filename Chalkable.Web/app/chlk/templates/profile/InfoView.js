REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.people.User');
REQUIRE('chlk.templates.people.User');

NAMESPACE('chlk.templates.profile', function () {

    /** @class chlk.templates.profile.InfoView*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/InfoView.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'InfoView', EXTENDS(chlk.templates.people.User), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit'
        ])
});