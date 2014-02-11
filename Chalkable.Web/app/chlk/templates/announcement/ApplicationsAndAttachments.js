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
                    applications = applications_ || this.getApplications() || [], res=[], that = this;
                attachments.forEach(function(item){
                    res.push(new chlk.models.announcement.ApplicationOrAttachment(
                        item.getId(),
                        item.isOwner(),
                        item.getOrder(),
                        item.getType() == chlk.models.announcement.ApplicationOrAttachmentEnum.DOCUMENT ?
                            chlk.models.announcement.ApplicationOrAttachmentEnum.DOCUMENT :
                            chlk.models.announcement.ApplicationOrAttachmentEnum.PICTURE,
                        item.getName(),
                        item.getThumbnailUrl()
                    ));
                });
                applications.forEach(function(item){
                    res.push(new chlk.models.announcement.ApplicationOrAttachment(
                        item.getAnnouncementApplicationId(),
                        null,
                        item.getOrder(),
                        chlk.models.announcement.ApplicationOrAttachmentEnum.APPLICATION,
                        item.getName(),
                        that.getPictureURL(item.getSmallPictureId(), 74, 74),
                        item.getUrl(),
                        item.getEditUrl(),
                        item.getGradingViewUrl(),
                        item.getViewUrl()
                    ));
                });
                res.sort(function(a,b){
                    return a.getOrder() > b.getOrder()
                });
                return res;
            }
        ])
});