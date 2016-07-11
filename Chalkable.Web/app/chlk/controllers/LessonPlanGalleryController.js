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
                var categoryType = categoryType_ || this.getContext().getSession().get(ChlkSessionConstants.LESSON_PLAN_CATEGORY_FOR_SEARCH, null);
                return this.getLessonPlanTemplates_(classId, categoryType, filter_, sortType_, start_, count_);
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

                var state = chlk.models.announcement.StateEnum.SUBMITTED;
                var result = this.lessonPlanService
                    .getLessonPlanTemplatesList(categoryType_, filter_, sortType_, state, start_, count_)
                    .attach(this.validateResponse_())
                    .then(function(lessonPlans){
                        return new chlk.models.announcement.LessonPlanGalleryViewData(
                            lessonPlans,
                            lessonPlanCategories,
                            sortType_ || chlk.models.attachment.SortAttachmentType.NEWEST_UPLOADED,
                            classId,
                            categoryType_,
                            filter_,
                            this.getCurrentPerson().hasPermission(chlk.models.people.UserPermissionEnum.CHALKABLE_ADMIN)
                        );
                    }, this);
                return this.ShadeOrUpdateView(chlk.activities.announcement.LessonPlanGalleryDialog, result);
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
            function tryDeleteLessonPlanFromGalleryAction(lessonPlanId, classId){
                this.ShowConfirmBox('This will PERMANENTLY delete this lesson plan from the gallery for everyone. Are you sure you want to delete this?',
                    "whoa.", null, 'negative-button')
                    .thenCall(this.lessonPlanService.removeLessonPlanFromGallery, [lessonPlanId])
                    .attach(this.validateResponse_())
                    .then(function (data) {
                        return this.BackgroundNavigate('lessonplangallery', 'lessonPlanTemplatesList', [classId]);
                    }, this);
                return null;
            },
        ]);
});