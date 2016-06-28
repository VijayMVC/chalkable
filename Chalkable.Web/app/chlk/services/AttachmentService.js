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

            [[Object]],
            ria.async.Future, function uploadAttachment(files) {
                return this.uploadFiles('Attachment/Upload', files, chlk.models.attachment.Attachment, {});
            },

            [[String, chlk.models.attachment.SortAttachmentType, Number, Number]],
            ria.async.Future, function getAttachments(filter_, sortType_, start_, count_){
                return this.getPaginatedList('Attachment/AttachmentsList.json', chlk.models.attachment.Attachment,{
                    filter: filter_,
                    sortType: sortType_ && sortType_.valueOf(),
                    start: start_,
                    count: count_
                });
            },

            [[ArrayOf(chlk.models.id.AttachmentId)]],
            ria.async.Future, function getByIds(ids){
                return this.getPaginatedList('Attachment/List.json', chlk.models.attachment.Attachment, {
                    ids: this.arrayToIds(ids)
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
