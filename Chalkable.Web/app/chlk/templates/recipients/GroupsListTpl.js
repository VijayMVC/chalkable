REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.recipients.GroupsListViewData');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.GroupsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/GroupsList.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupsListViewData)],
        'GroupsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups'
        ])
});