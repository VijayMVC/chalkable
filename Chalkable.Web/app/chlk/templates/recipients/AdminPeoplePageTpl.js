REQUIRE('chlk.templates.controls.group_people_selector.SelectorBaseTpl');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.AdminPeoplePageTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/AdminPeoplePage.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupSelectorViewData)],
        'AdminPeoplePageTpl', EXTENDS(chlk.templates.controls.group_people_selector.SelectorBaseTpl), [])
});