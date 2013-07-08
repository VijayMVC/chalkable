REQUIRE('chlk.models.school.ActionButtons');
REQUIRE('chlk.templates.Popup');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.ActionButtons*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/ActionButtons.jade')],
        [ria.templates.ModelBind(chlk.models.school.ActionButtons)],
        'ActionButtons', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelBind],
            ArrayOf(Number), 'buttons',
            [ria.templates.ModelBind],
            ArrayOf(String), 'emails'
        ])
});
