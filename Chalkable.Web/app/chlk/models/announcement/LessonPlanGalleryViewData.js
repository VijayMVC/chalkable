REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.LpGalleryCategoryId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.LessonPlanGalleryViewData*/
    CLASS(
        UNSAFE, 'LessonPlanGalleryViewData', [

            chlk.models.common.PaginatedList, 'lessonPlans',

            chlk.models.attachment.SortAttachmentType, 'sortType',

            String, 'filter',

            ArrayOf(chlk.models.announcement.CategoryViewData), 'categories',

            chlk.models.id.ClassId, 'classId',

            Boolean, 'empty',

            chlk.models.id.LpGalleryCategoryId, 'categoryType',

            [[
                chlk.models.common.PaginatedList,
                ArrayOf(chlk.models.announcement.CategoryViewData),
                chlk.models.attachment.SortAttachmentType,
                chlk.models.id.ClassId,
                chlk.models.id.LpGalleryCategoryId,
                String
            ]],
            function $(lessonPlans, categories, sortType, classId, categoryType_, filter_){
                BASE();
                this.setLessonPlans(lessonPlans);
                if (filter_)
                    this.setFilter(filter_);

                if (categoryType_)
                    this.setCategoryType(categoryType_);

                this.setCategories(categories);

                this.setSortType(sortType);
                this.setClassId(classId);

                this.setEmpty((this.getLessonPlans().getItems() || []).length == 0);
            }

        ]);
});
