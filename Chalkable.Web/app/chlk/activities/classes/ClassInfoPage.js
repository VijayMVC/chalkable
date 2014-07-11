REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassInfoTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassInfoPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassInfoTpl)],
        'ClassInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});