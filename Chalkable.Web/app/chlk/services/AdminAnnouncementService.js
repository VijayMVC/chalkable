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
REQUIRE('chlk.models.people.User');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AdminAnnouncementService */
    CLASS(
        'AdminAnnouncementService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.common.ChlkDate]],
            ria.async.Future, function addAdminAnnouncement(expiresDate_) {
                return this.get('AdminAnnouncement/CreateAdminAnnouncement.json', chlk.models.announcement.AnnouncementCreate, {
                    expiresDate: expiresDate_ ? expiresDate_.valueOf() : null
                });
            },

            [[chlk.models.id.AnnouncementId, Number, Number]],
            ria.async.Future, function getAdminAnnouncementRecipients(id, start_, count_) {
                return this.getPaginatedList('AdminAnnouncement/GetAdminAnnouncementRecipients.json', chlk.models.people.User, {
                    announcementId:id.valueOf(),
                    start: start_ || 0,
                    count: count_ || 10
                });
            },

            [[chlk.models.id.AnnouncementId, String, String, chlk.models.common.ChlkDate,
                Array
            ]],
            ria.async.Future, function saveAdminAnnouncement(id, title_, content_, expiresdate_, attributesListData) {
                return this.post('AdminAnnouncement/Save.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    adminAnnouncementId:id.valueOf(),
                    title: title_,
                    content: content_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    attributes: attributesListData
                });
            },

            [[chlk.models.id.AnnouncementId, String, String, chlk.models.common.ChlkDate,
                Array
            ]],
            ria.async.Future, function submitAdminAnnouncement(id, content_, title_, expiresdate_, attributesListData) {
                return this.post('AdminAnnouncement/Submit.json', Boolean, {
                    adminAnnouncementId:id.valueOf(),
                    title: title_,
                    content: content_,
                    expiresdate: expiresdate_ && expiresdate_.toStandardFormat(),
                    attributes: attributesListData
                });
            },

            [[chlk.models.id.AnnouncementId, String, String]],
            ria.async.Future, function addGroupsToAnnouncement(id, groupIds_, studentsIds_) {
                return this.post('AdminAnnouncement/SubmitGroupsToAnnouncement.json', Boolean, {
                    adminAnnouncementId:id.valueOf(),
                    groupsIds: groupIds_,
                    studentsIds: studentsIds_
                });
            },

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function editTitle(announcementId, title) {
                return this.get('AdminAnnouncement/EditTitle.json', Boolean, {
                    announcementId: announcementId.valueOf(),
                    title: title
                });
            },

            [[String, chlk.models.id.AnnouncementId]],
            ria.async.Future, function existsTitle(title, announcementId) {
                return this.get('AdminAnnouncement/Exists.json', Boolean, {
                    title: title,
                    excludeAdminAnnouncementId: announcementId.valueOf()
                });
            }
        ])
});