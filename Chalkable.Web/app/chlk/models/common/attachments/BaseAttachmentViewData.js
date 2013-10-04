REQUIRE('chlk.models.common.attachments.ToolbarButton');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.common.attachments', function () {
    "use strict";

    /** @class chlk.models.common.attachments.BaseAttachmentViewData*/
    CLASS(
        'BaseAttachmentViewData', [
            ArrayOf(chlk.models.common.attachments.ToolbarButton), 'toolbarButtons',
            String, 'url',

            [[String, ArrayOf(chlk.models.common.attachments.ToolbarButton)]],
            function $(url, buttons){
                BASE();
                this.setUrl(url);
                this.setToolbarButtons(buttons);
            }
        ]);
});
