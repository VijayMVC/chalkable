REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.attachment.Attachment');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AttachmentService */
    CLASS(
        'AttachmentService', EXTENDS(chlk.services.BaseService), [

            [[String, chlk.models.attachment.SortAttachmentType, Number, Number]],
            ria.async.Future, function getAttachments(filter_, sortType_, start_, count_){
                return this.getPaginatedList('Attachment/AttachmentsList.json', chlk.models.attachment.Attachment,{
                    filter: filter_,
                    sortType: sortType_ && sortType_.valueOf(),
                    start: start_,
                    count: count_
                });
            },

            [[chlk.models.id.AttachmentId, Boolean, Number, Number]],
            String, function getDownloadUri(attachmentId, needsDownload, width, height) {
                return this.getUrl('Attachment/DownloadAttachment', {
                    attachmentId: attachmentId.valueOf(),
                    needsDownload: needsDownload,
                    width: width,
                    height: height
                });
            },

            [[String]],
            String, function getViewSessionUrl(session){
                return 'https://crocodoc.com/view/' + session;
            }
        ]);
});
