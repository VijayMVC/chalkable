REQUIRE('chlk.templates.group.GroupsListTpl');
REQUIRE('chlk.models.group.AnnouncementGroupsViewData');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.AnnouncementGroupsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGroups.jade')],
        [ria.templates.ModelBind(chlk.models.group.AnnouncementGroupsViewData)],
        'AnnouncementGroupsTpl', EXTENDS(chlk.templates.group.GroupsListTpl), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.id.GroupId), 'selected',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            Object, 'hiddenParams',

            [[chlk.models.group.Group]],
            Boolean, function isGroupChecked(group){
                return this.getSelected().indexOf(group.getId()) > -1;
            }
        ])
});
