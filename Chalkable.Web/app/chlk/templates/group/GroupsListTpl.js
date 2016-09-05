REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.group.GroupsListViewData');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.GroupsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/EditGroups.jade')],
        [ria.templates.ModelBind(chlk.models.group.GroupsListViewData)],
        'GroupsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            String, 'listRequestId'
        ])
});
