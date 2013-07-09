REQUIRE('chlk.activities.lib.TemplateActivity');
REQUIRE('chlk.templates.signup.SignUpList');

NAMESPACE('chlk.activities.signup', function () {

    /** @class chlk.activities.signup.SignUpListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BindTemplate(chlk.templates.signup.SignUpList)],
        'SignUpListPage', EXTENDS(chlk.activities.lib.TemplateActivity), [ ]);
});