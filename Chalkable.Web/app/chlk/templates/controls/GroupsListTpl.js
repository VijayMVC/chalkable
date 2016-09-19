REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.recipients.GroupsListViewData');

NAMESPACE('chlk.templates.controls', function () {

    /** @class chlk.templates.controls.GroupsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/groups-list.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupsListViewData)],
        'GroupsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups'
        ])
});