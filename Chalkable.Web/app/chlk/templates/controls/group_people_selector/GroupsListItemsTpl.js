REQUIRE('chlk.templates.controls.group_people_selector.GroupsListTpl');

NAMESPACE('chlk.templates.controls.group_people_selector', function () {

    /** @class chlk.templates.controls.group_people_selector.GroupsListItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/groups-list-items.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupsListViewData)],
        'GroupsListItemsTpl', EXTENDS(chlk.templates.controls.group_people_selector.GroupsListTpl), [])
});