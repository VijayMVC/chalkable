REQUIRE('chlk.models.announcement.LessonPlanGalleryViewData');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    /** @class chlk.templates.announcement.LessonPlanGalleryDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/LessonPlanGalleryDialog.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.LessonPlanGalleryViewData)],
        'LessonPlanGalleryDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'lessonPlans',

            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.CategoryViewData), 'categories',

            [ria.templates.ModelPropertyBind],
            chlk.models.attachment.SortAttachmentType, 'sortType',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'empty',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.LpGalleryCategoryId, 'categoryType'

        ]);
});