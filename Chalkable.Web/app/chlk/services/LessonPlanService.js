REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.AnnouncementView');

REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GalleryCategoryId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.announcement.AnnouncementTitleViewData');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.LessonPlanService */
    CLASS(
        'LessonPlanService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.ClassId]],
            ria.async.Future, function addLessonPlan(classId_) {
                return this.get('LessonPlan/CreateLessonPlan.json', chlk.models.announcement.LessonPlanForm, {
                    classId: classId_ ? classId_.valueOf() : null
                });
            },

            ria.async.Future, function listCategories() {
                return this.get('LPGalleryCategory/ListCategories.json', ArrayOf(chlk.models.common.NameId));
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
            ria.async.Future, function createFromTemplate(lessonPlanTplId, classId) {
                return this.get('LessonPlan/CreateFromTemplate.json', chlk.models.announcement.LessonPlanForm, {
                    lessonPlanTplId: lessonPlanTplId.valueOf(),
                    classId: classId.valueOf()
                });
            },

            [[Number, String]],
            ria.async.Future, function searchLessonPlansTemplates(galleryCategoryId_, filter_) {
                return this.get('LessonPlan/SearchLessonPlansTemplates.json', ArrayOf(chlk.models.announcement.LessonPlanViewData), {
                    galleryCategoryId: this.getContext().getSession().get(ChlkSessionConstants.LESSON_PLAN_CATEGORY_FOR_SEARCH, galleryCategoryId_),
                    filter: filter_
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function editTitle(announcementId, title) {
                return this.get('LessonPlan/EditTitle.json', Boolean, {
                    lessonPlanId: announcementId.valueOf(),
                    title: title
                });
            },

            [[String, chlk.models.id.AnnouncementId]],
            ria.async.Future, function existsTitle(title, announcementId) {
                return this.get('LessonPlan/Exists.json', Boolean, {
                    title: title,
                    excludeLessonPlanId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function listLast(classId) {
                return this.get('LessonPlan/ListLast.json', ArrayOf(String), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function makeVisible(announcementId) {
                return this.post('LessonPlan/MakeVisible', chlk.models.announcement.FeedAnnouncementViewData, {
                    lessonPlanId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String, Number
                , chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean,
                chlk.models.announcement.AnnouncementAttributeListViewData
            ]],
            ria.async.Future, function saveLessonPlan(id, classId_, title_, content_, galleryCategoryId_
                , startDate_, endDate_, hideFromStudent_, attributesListData) {
                return this.post('LessonPlan/Save.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    lessonPlanId:id.valueOf(),
                    title: title_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    galleryCategoryId: galleryCategoryId_ ? galleryCategoryId_.valueOf() : null,
                    startDate: startDate_ && startDate_.toStandardFormat(),
                    endDate: endDate_ && endDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false,
                    attributes: attributesListData.getPostData()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String, Number
                , chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean,
                chlk.models.announcement.AnnouncementAttributeListViewData]],
            ria.async.Future, function submitLessonPlan(id, classId_, title_, content_, galleryCategoryId_
                , startDate_, endDate_, hideFromStudent_, attributesListData) {
                return this.post('LessonPlan/Submit.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    lessonPlanId:id.valueOf(),
                    title: title_ || ('test ' + id.valueOf()),
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    galleryCategoryId: galleryCategoryId_ ? galleryCategoryId_.valueOf() : null,
                    startDate: startDate_ && startDate_.toStandardFormat(),
                    endDate: endDate_ && endDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false,
                    attributes: attributesListData.getPostData()
                });
            }
        ])
});