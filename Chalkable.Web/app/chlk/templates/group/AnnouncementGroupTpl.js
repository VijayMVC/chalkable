REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.AnnouncementGroupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGroup.jade')],
        [ria.templates.ModelBind(chlk.models.group.Group)],
        'AnnouncementGroupTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GroupId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            chlk.models.id.AnnouncementId, 'announcementId'
        ])
});
