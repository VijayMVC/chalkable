REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.templates.announcement', function(){

    /**@class chlk.templates.announcement.AnnouncementAttachmentTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementAttachment.jade')],
        [ria.templates.ModelBind(chlk.models.attachment.AnnouncementAttachment)],
        'AnnouncementAttachmentTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementAttachmentId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttachmentId, 'attachmentId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'owner',

            [ria.templates.ModelPropertyBind],
            Boolean, 'openOnStart',

            [ria.templates.ModelPropertyBind],
            Boolean, 'teachersAttachment',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            Number, 'order',

            [ria.templates.ModelPropertyBind],
            String, 'thumbnailUrl',

            [ria.templates.ModelPropertyBind],
            String, 'bigUrl',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.AttachmentTypeEnum, 'type',

            [ria.templates.ModelPropertyBind],
            String, 'url',

            [ria.templates.ModelPropertyBind],
            Number, 'fileIndex',

            [ria.templates.ModelPropertyBind],
            Number, 'total',

            [ria.templates.ModelPropertyBind],
            Number, 'loaded',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'attributeAttachment',

            function getSize(){
                var texts = ['b', 'kb', 'mb', 'gb'], step = 0;
                var total = this.getTotal();
                while(total > 1023){
                    step++;
                    total = parseFloat((total / 1024).toFixed(2));
                }
                return total + texts[step];
            }
    ]);
});