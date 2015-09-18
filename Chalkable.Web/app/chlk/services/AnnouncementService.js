REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.AnnouncementView');

REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');

REQUIRE('chlk.models.announcement.AnnouncementTitleViewData');
REQUIRE('chlk.models.announcement.AnnouncementAttributeType');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AnnouncementService */
    CLASS(
        'AnnouncementService', EXTENDS(chlk.services.BaseService), [

            function $()
            {
                BASE();
                this.setCache({});
            },

            Object, 'cache',
            Number, 'importantCount',

            [[Number, chlk.models.id.ClassId]],
            ria.async.Future, function markDone(option, classId_) {
                return this.get('Announcement/Done.json', Boolean, {
                    option: option.valueOf(),
                    classId: classId_ && classId_.valueOf()
                });
            },

            [[Number, chlk.models.id.ClassId, Boolean, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean, Boolean]],
            ria.async.Future, function getAnnouncements(start_, classId_, importantOnly_, startDate_, endDate_, lessonPlansOnly_, latest_) {
                return this.get('Feed/List.json', chlk.models.feed.Feed, {
                    start: start_ || 0,
                    classId: classId_ ? classId_.valueOf() : null,
                    complete: importantOnly_ ? false : null,
                    startDate: startDate_ && startDate_.toStandardFormat(),
                    endDate: endDate_ && endDate_.toStandardFormat(),
                    lessonPlansOnly: lessonPlansOnly_,
                    sortType: latest_,
                    count: 10
                });

            },

            [[Number, String, Boolean, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean, Boolean]],
            ria.async.Future, function getAnnouncementsForAdmin(start_, gradeLevels_, importantOnly_, startDate_, endDate_, lessonPlansOnly_, latest_) {
                return this.get('Feed/DistrictAdminFeed.json', chlk.models.feed.FeedAdmin, {
                    gradeLevelIds : gradeLevels_,
                    start: start_ || 0,
                    complete: importantOnly_ ? false : null,
                    startDate: startDate_ && startDate_.toStandardFormat(),
                    endDate: endDate_ && endDate_.toStandardFormat(),
                    lessonPlansOnly: lessonPlansOnly_,
                    sortType: latest_,
                    count: 10
                });

            },

            [[chlk.models.id.AnnouncementId, Boolean]],
            ria.async.Future, function setShowGradesToStudents(announcementId, value) {
                return ria.async.DeferredData(true);
            },

            [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteApp(announcementAppId, announcementType) {
                return this.get('Application/RemoveFromAnnouncement.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementApplicationId: announcementAppId.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate]],
            ria.async.Future, function addAnnouncement(classId_, classAnnouncementTypeId_, expiresDate_) {
                return this.get('Announcement/Create.json', chlk.models.announcement.AnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    classAnnouncementTypeId: classAnnouncementTypeId_,
                    expiresDate: expiresDate_ ? expiresDate_.valueOf() : null
                });
            },
            
            [[chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function listLast(classId, classAnnouncementTypeId, schoolPersonId) {
                return this.get('Announcement/ListLast.json', ArrayOf(String), {
                    classId: classId.valueOf(),
                    classAnnouncementTypeId: classAnnouncementTypeId,
                    personId: schoolPersonId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function editAnnouncement(id, announcementType) {
                return this.get('Announcement/Edit.json', chlk.models.announcement.AnnouncementForm, {
                    announcementId: id.valueOf(),
                    announcementType: announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function deleteAnnouncement(id, announcementType) {
                return this.post('Announcement/Delete.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: id.valueOf(),
                    announcementType: announcementType && announcementType.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function deleteDrafts(id, announcementType) {
                return this.post('Announcement/DeleteDrafts.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    personId: id.valueOf(),
                    announcementType: announcementType && announcementType.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function getAnnouncement(id, announcementType_) {
                return this.get('Announcement/Read.json', chlk.models.announcement.AnnouncementView, {
                    announcementId: id.valueOf(),
                    announcementType: announcementType_ && announcementType_.valueOf()
                })
                .then(function(announcement){
                  this.cache[announcement.getId().valueOf()] = announcement;
                  return announcement;
                }, this);
            },

            [[chlk.models.id.AnnouncementId]],
            chlk.models.announcement.AnnouncementView, function getAnnouncementSync(id){
                return this.cache[id.valueOf()];
            },

            [[chlk.models.id.AnnouncementId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function checkItem(announcementId, complete_, type_) {
                this.setImportantCount(this.getImportantCount() + (complete_ ? 1 : -1));
                return this.post('Announcement/Complete', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    complete: complete_,
                    announcementType: type_ && type_.valueOf(),
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
            ria.async.Future, function addStandard(announcementId, standardId) {
                return this.get('Announcement/AddStandard.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    standardId: standardId.valueOf(),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
            ria.async.Future, function removeStandard(announcementId, standardId) {
                return this.get('Announcement/RemoveStandard.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId: announcementId.valueOf(),
                    standardId: standardId.valueOf(),
                    announcementType: this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf()
                });
            }

        ]);
});