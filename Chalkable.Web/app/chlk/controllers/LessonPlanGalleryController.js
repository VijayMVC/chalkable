REQUIRE('chlk.models.id.ClassId');

REQUIRE('chlk.controllers.BaseController');


REQUIRE('chlk.services.LessonPlanService');
REQUIRE('chlk.services.LpGalleryCategoryService');

REQUIRE('chlk.activities.announcement.LessonPlanGalleryDialog');

REQUIRE('chlk.models.announcement.LessonPlanGalleryViewData');
REQUIRE('chlk.models.announcement.LessonPlanGalleryPostData');


NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.LessonPlanGalleryController */
    CLASS(
        'LessonPlanGalleryController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.LessonPlanService, 'lessonPlanService',

            [ria.mvc.Inject],
            chlk.services.LpGalleryCategoryService, 'lpGalleryCategoryService',

            [[
                chlk.models.id.ClassId,
                chlk.models.id.LpGalleryCategoryId,
                String,
                chlk.models.attachment.SortAttachmentType,
                Number,
                Number
            ]],
            function lessonPlanTemplatesListAction(classId, categoryType_, filter_, sortType_, start_, count_){
                return this.getLessonPlanTemplates_(classId, categoryType_, filter_, sortType_, start_, count_);
            },

            [chlk.controllers.SidebarButton('add-new')],
            [[chlk.models.announcement.LessonPlanGalleryPostData]],
            function lessonPlanListFilterAction(postData){
                return this.getLessonPlanTemplates_(
                    postData.getClassId(),
                    postData.getCategoryType(),
                    postData.getFilter(),
                    postData.getSortType(),
                    postData.getStart(),
                    postData.getCount()
                );
            },

            [[
                chlk.models.id.ClassId,
                chlk.models.id.LpGalleryCategoryId,
                String,
                chlk.models.attachment.SortAttachmentType,
                Number,
                Number
            ]],
            function getLessonPlanTemplates_(classId, categoryType_, filter_, sortType_, start_, count_){
                var lessonPlanCategories = this.lpGalleryCategoryService.getLessonPlanCategoriesSync();

                var result = this.lessonPlanService
                    .getLessonPlanTemplatesList(categoryType_, filter_, sortType_, start_, count_)
                    .attach(this.validateResponse_())
                    .then(function(lessonPlans){
                        return new chlk.models.announcement.LessonPlanGalleryViewData(
                            lessonPlans,
                            lessonPlanCategories,
                            sortType_ || chlk.models.attachment.SortAttachmentType.NEWEST_UPLOADED,
                            classId,
                            categoryType_,
                            filter_
                        );
                    }, this);
                return this.ShadeOrUpdateView(chlk.activities.announcement.LessonPlanGalleryDialog, result);
            }

        ]);
});