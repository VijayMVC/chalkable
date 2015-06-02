REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.group.AnnouncementGroupsViewData');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.AnnouncementGroupsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGroups.jade')],
        [ria.templates.ModelBind(chlk.models.group.AnnouncementGroupsViewData)],
        'AnnouncementGroupsTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.id.GroupId), 'groupIds',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [[chlk.models.group.Group]],
            Boolean, function isGroupChecked(group){
                return this.getGroupIds().indexOf(group.getId()) > -1;
            }
        ])
});
