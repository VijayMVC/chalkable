REQUIRE('chlk.models.school.ActionButtons');
REQUIRE('chlk.templates.Popup');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.ActionButtons*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/ActionButtons.jade')],
        [ria.templates.ModelBind(chlk.models.school.ActionButtons)],
        'ActionButtons', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(Number), 'buttons',
            [ria.templates.ModelPropertyBind],
            ArrayOf(String), 'emails'
        ])
});
