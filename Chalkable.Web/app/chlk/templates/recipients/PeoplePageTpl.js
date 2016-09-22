REQUIRE('chlk.templates.controls.group_people_selector.SelectorBaseTpl');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.PeoplePageTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/PeoplePage.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupSelectorViewData)],
        'PeoplePageTpl', EXTENDS(chlk.templates.controls.group_people_selector.SelectorBaseTpl), [])
});