REQUIRE('chlk.models.common.attachments.ToolbarButton');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.models.common.attachments', function () {
    "use strict";

    /** @class chlk.models.common.attachments.BaseAttachmentViewData*/
    CLASS(
        'BaseAttachmentViewData', [
            ArrayOf(chlk.models.common.attachments.ToolbarButton), 'toolbarButtons',

            String, 'url',

            chlk.models.attachment.AttachmentTypeEnum, 'type',

            [[String, ArrayOf(chlk.models.common.attachments.ToolbarButton), chlk.models.attachment.AttachmentTypeEnum]],
            function $(url, buttons, type_){
                BASE();
                this.setUrl(url);
                this.setToolbarButtons(buttons);
                if(type_)
                    this.setType(type_);
            }
        ]);
});
