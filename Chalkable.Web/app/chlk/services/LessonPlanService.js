REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.LessonPlanForm');
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
                return this.get('LessonPlan/Create.json', chlk.models.announcement.LessonPlanForm, {
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
            ria.async.Future, function searchLessonPlansTemplates(galleryCategoryId, filter) {
                return this.get('LessonPlan/SearchLessonPlansTemplates.json', chlk.models.announcement.LessonPlanViewData, {
                    galleryCategoryId: galleryCategoryId.valueOf(),
                    filter: filter
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

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String, chlk.models.id.GalleryCategoryId
                , chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean]],
            ria.async.Future, function saveLessonPlan(id, classId_, title_, content_, galleryCategoryId_
                , startDate_, endDate_, hideFromStudent_) {
                return this.post('LessonPlan/Save.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    lessonPlanId:id.valueOf(),
                    title: title_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    galleryCategoryId: galleryCategoryId_ ? galleryCategoryId_.valueOf() : null,
                    startDate: startDate_ && startDate_.toStandardFormat(),
                    endDate: endDate_ && endDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String, chlk.models.id.GalleryCategoryId
                , chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean]],
            ria.async.Future, function submitLessonPlan(id, classId_, title_, content_, galleryCategoryId_
                , startDate_, endDate_, hideFromStudent_) {
                return this.post('LessonPlan/Submit.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    lessonPlanId:id.valueOf(),
                    title: title_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    galleryCategoryId: galleryCategoryId_ ? galleryCategoryId_.valueOf() : null,
                    startDate: startDate_ && startDate_.toStandardFormat(),
                    endDate: endDate_ && endDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false
                });
            }
        ])
});