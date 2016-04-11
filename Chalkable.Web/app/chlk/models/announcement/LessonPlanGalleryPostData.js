REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.attachment.Attachment');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.LessonPlanGalleryPostData*/
    CLASS(
        'LessonPlanGalleryPostData', [

            chlk.models.id.ClassId, 'classId',

            [ria.serialize.SerializeProperty('lpGallerySortType')],
            chlk.models.attachment.SortAttachmentType, 'sortType',

            [ria.serialize.SerializeProperty('lpGalleryCategoryType')],
            chlk.models.id.LpGalleryCategoryId, 'categoryType',

            Number, 'start',
            Number, 'count',
            String, 'filter'

    ]);
});
