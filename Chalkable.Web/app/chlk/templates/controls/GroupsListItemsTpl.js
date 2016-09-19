REQUIRE('chlk.templates.controls.GroupsListTpl');

NAMESPACE('chlk.templates.controls', function () {

    /** @class chlk.templates.controls.GroupsListItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/groups-list-items.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupsListViewData)],
        'GroupsListItemsTpl', EXTENDS(chlk.templates.controls.GroupsListTpl), [])
});