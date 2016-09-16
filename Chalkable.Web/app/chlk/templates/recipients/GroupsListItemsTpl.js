REQUIRE('chlk.templates.recipients.GroupsListTpl');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.GroupsListItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/GroupsListItems.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupsListViewData)],
        'GroupsListItemsTpl', EXTENDS(chlk.templates.recipients.GroupsListTpl), [])
});