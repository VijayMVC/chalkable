REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.people.User');
REQUIRE('chlk.templates.people.User');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.Hello*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/Hello.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'Hello', EXTENDS(chlk.templates.people.User), [])
});