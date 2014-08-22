REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.ApplicationOrAttachment');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.ApplicationsAndAttachments*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/StudentAnnouncement.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.ApplicationOrAttachment)],
        'ApplicationsAndAttachments', EXTENDS(chlk.templates.ChlkTemplate), [
            [[ArrayOf(chlk.models.apps.Application), ArrayOf(chlk.models.attachment.Attachment)]],
            ArrayOf(chlk.models.announcement.ApplicationOrAttachment), function getSortedAppsAndAttachments(applications_, attachments_){
                var attachments = attachments_ || this.getAnnouncementAttachments() || [],
                    applications = applications_ || this.getApplications() || [],
                    res = [], that = this;
                attachments.forEach(function(item){
                    res.push(new chlk.models.announcement.ApplicationOrAttachment(
                        item.getId(),
                        item.isOwner(),
                        item.getOrder(),
                        chlk.models.announcement.ApplicationOrAttachmentEnum(item.getType()),
                        item.getName(),
                        item.getThumbnailUrl(),
                        item.getUrl(),
                        item.isTeachersAttachment()
                    ));
                });
                applications.forEach(function(item){
                    res.push(new chlk.models.announcement.ApplicationOrAttachment(
                        item.getAnnouncementApplicationId(),
                        null,
                        item.getOrder(),
                        chlk.models.announcement.ApplicationOrAttachmentEnum.APPLICATION,
                        item.getName(),
                        that.getPictureURL(item.getBigPictureId(), 74, 74, true),
                        item.getUrl(),
                        null,
                        item.getEditUrl(),
                        item.getGradingViewUrl(),
                        item.getViewUrl(),
                        item.getBanInfo() || new chlk.models.apps.BannedAppData()
                    ));
                });
                res.sort(function(a,b){
                    return a.getOrder() > b.getOrder()
                });
                return res;
            }
        ])
});