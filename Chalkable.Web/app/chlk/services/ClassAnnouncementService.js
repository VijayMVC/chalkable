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

    /** @class chlk.services.ClassAnnouncementService */
    CLASS(
        'ClassAnnouncementService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate]],
            ria.async.Future, function addClassAnnouncement(classId_, classAnnouncementTypeId_, expiresDate_) {
                return this.get('ClassAnnouncement/CreateClassAnnouncement.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    classAnnouncementTypeId: classAnnouncementTypeId_,
                    expiresDate: expiresDate_ ? expiresDate_.valueOf() : null
                });
            },

            [[chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function listLast(classId, classAnnouncementTypeId, schoolPersonId) {
                return this.get('ClassAnnouncement/ListLast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    classAnnouncementTypeId: classAnnouncementTypeId,
                    personId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String
                , chlk.models.common.ChlkDate
                , Number, Number, Number, Boolean, Boolean,
                Array, Boolean, Boolean, Boolean
            ]],
            ria.async.Future, function saveClassAnnouncement(id, classId_, classAnnouncementTypeId_, title_, content_
                , expiresdate_, maxScore_, weightAddition_, weighMultiplier_
                , hideFromStudent_, canDropStudentScore_, attributesListData
                , discussionEnabled, previewCommentsEnabled, requireCommentsEnabled) {
                return this.post('ClassAnnouncement/SaveAnnouncement.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId:id.valueOf(),
                    classAnnouncementTypeId:classAnnouncementTypeId_,
                    title: title_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    maxscore: maxScore_,
                    weightaddition: weightAddition_,
                    weightmultiplier: weighMultiplier_,
                    hidefromstudents: hideFromStudent_ || false,
                    candropstudentscore: canDropStudentScore_ || false,
                    attributes: attributesListData,
                    discussionEnabled: discussionEnabled,
                    previewCommentsEnabled: previewCommentsEnabled,
                    requireCommentsEnabled: requireCommentsEnabled
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, Number, String, String
                , chlk.models.common.ChlkDate, Number, Number, Number, Boolean, Boolean,
                    Boolean, Array, Boolean, Boolean, Boolean
            ]],
            ria.async.Future, function submitClassAnnouncement(id, classId_, announcementTypeId_, title_, content_
                , expiresdate_, maxScore_, weightAddition_, weighMultiplier_
                , hideFromStudent_, canDropStudentScore_, gradable_, attributesListData
                , discussionEnabled, previewCommentsEnabled, requireCommentsEnabled) {
                return this.post('ClassAnnouncement/SubmitAnnouncement.json', Boolean, {
                    announcementid:id.valueOf(),
                    classannouncementtypeid:announcementTypeId_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    maxscore: maxScore_,
                    weightaddition: weightAddition_,
                    weightmultiplier: weighMultiplier_,
                    hidefromstudents: hideFromStudent_ || false,
                    candropstudentscore: canDropStudentScore_ || false,
                    gradable: gradable_ || false,
                    attributes: attributesListData,
                    discussionEnabled: discussionEnabled,
                    previewCommentsEnabled: previewCommentsEnabled,
                    requireCommentsEnabled: requireCommentsEnabled
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function duplicateAnnouncement(id, classIds) {
                return this.get('ClassAnnouncement/DuplicateAnnouncement.json', Boolean, {
                    announcementId: id.valueOf(),
                    classIds: classIds
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function dropAnnouncement(id) {
                return this.post('ClassAnnouncement/Drop.json', Boolean, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function unDropAnnouncement(id) {
                return this.post('ClassAnnouncement/Undrop.json', Boolean, {
                    announcementId: id.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function makeVisible(announcementId) {
                return this.post('ClassAnnouncement/MakeVisible', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function editTitle(announcementId, title) {
                return this.get('ClassAnnouncement/EditTitle.json', Boolean, {
                    announcementId: announcementId.valueOf(),
                    title: title
                });
            },

            [[String, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.id.AnnouncementId]],
            ria.async.Future, function existsTitle(title, classId, expiresDate, announcementId) {
                return this.get('ClassAnnouncement/Exists.json', Boolean, {
                    title: title,
                    classId: classId.valueOf(),
                    expiresDate: expiresDate && expiresDate.toStandardFormat(),
                    excludeAnnouncementId: announcementId.valueOf()
                });
            }
        ])
});