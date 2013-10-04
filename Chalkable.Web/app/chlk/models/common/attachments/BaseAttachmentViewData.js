REQUIRE('chlk.models.common.attachments.ToolbarButton');

NAMESPACE('chlk.models.common.attachments', function () {
    "use strict";

    /** @class chlk.models.common.attachments.BaseAttachmentViewData*/
    CLASS(
        'BaseAttachmentViewData', [
            ArrayOf(chlk.models.common.attachments.ToolbarButton), 'toolbarButtons',
            String, 'url'
        ]);
});
