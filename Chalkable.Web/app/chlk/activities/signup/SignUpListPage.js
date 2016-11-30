REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.signup.SignUpList');

NAMESPACE('chlk.activities.signup', function () {

    /** @class chlk.activities.signup.SignUpListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.signup.SignUpList)],
        'SignUpListPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});