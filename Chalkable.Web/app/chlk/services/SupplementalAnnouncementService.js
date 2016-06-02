REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.AnnouncementView');

REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.LpGalleryCategoryId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.announcement.AnnouncementTitleViewData');
REQUIRE('chlk.models.announcement.SupplementalAnnouncementForm');


NAMESPACE('chlk.services', function () {
    "use strict";

    var Serializer = new chlk.lib.serialize.ChlkJsonSerializer;

    /** @class chlk.services.SupplementalAnnouncementService */
    CLASS(
        'SupplementalAnnouncementService', EXTENDS(chlk.services.BaseService), [

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function create(classId_, expiresDate_) {
                return this.get('SupplementalAnnouncement/CreateSupplemental.json', chlk.models.announcement.SupplementalAnnouncementForm, {
                    classId: classId_ ? classId_.valueOf() : null,
                    expiresDate: expiresDate_ ? expiresDate_.valueOf() : null
                });
            },

            /*[[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function create(classId_, expiresDate_) {
                return this.get('ClassAnnouncement/CreateClassAnnouncement.json', null, {
                    classId: classId_ ? classId_.valueOf() : null,
                    expiresDate: expiresDate_ ? expiresDate_.valueOf() : null
                })
                    .then(function(data){
                        data.announcement.supplementalsnnouncementdata = data.announcement.classannouncementdata;
                        data.announcement.supplementalsnnouncementdata.type = 4;
                        data.announcement.type = 4;
                        data.announcement.classannouncementdata = null;
                        return Serializer.deserialize(data, chlk.models.announcement.SupplementalAnnouncementForm);
                    });
            },*/

            /*[[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function editTitle(announcementId, title) {
                return this.get('SupplementalAnnouncement/EditTitle.json', Boolean, {
                    supplementalAnnouncementPlanId: announcementId.valueOf(),
                    title: title
                });
            },

            [[String, chlk.models.id.AnnouncementId]],
            ria.async.Future, function existsTitle(title, announcementId) {
                return this.get('SupplementalAnnouncement/Exists.json', Boolean, {
                    title: title,
                    excludeSupplementalAnnouncementPlanId: announcementId.valueOf()
                });
            },*/

            [[chlk.models.id.AnnouncementId, String]],
            ria.async.Future, function editTitle(announcementId, title) {
                return this.get('ClassAnnouncement/EditTitle.json', Boolean, {
                    announcementId: announcementId.valueOf(),
                    title: title
                });
            },

            [[String, chlk.models.id.AnnouncementId]],
            ria.async.Future, function existsTitle(title, announcementId) {
                return this.get('ClassAnnouncement/Exists.json', Boolean, {
                    title: title,
                    classId: 13772,
                    expiresDate: new chlk.models.common.ChlkDate(getDate()).toStandardFormat(),
                    excludeAnnouncementId: announcementId.valueOf()
                });
            },

            [[String, chlk.models.id.AnnouncementId]],
            ria.async.Future, function existsInGallery(title, excludedLessonPlanId) {
                return this.get('SupplementalAnnouncement/ExistsInGallery.json', Boolean, {
                    title: title,
                    excludeSupplementalAnnouncementPlanId: excludedLessonPlanId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function makeVisible(announcementId) {
                return this.post('SupplementalAnnouncement/MakeVisible', chlk.models.announcement.FeedAnnouncementViewData, {
                    supplementalAnnouncementPlanId: announcementId.valueOf()
                });
            },

            /*[[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String
                , chlk.models.common.ChlkDate, Boolean, Array, Array]],
            ria.async.Future, function saveSupplementalAnnouncement(id, classId_, title_, content_
                , expiresDate_, hideFromStudent_, attributesListData, recipientsIds) {
                return this.post('SupplementalAnnouncement/Save.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    supplementalAnnouncementPlanId:id.valueOf(),
                    title: title_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    expiresDate: expiresDate_ && expiresDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false,
                    attributes: attributesListData,
                    recipientsIds: recipientsIds
                });
            },*/

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String
                , chlk.models.common.ChlkDate, Boolean, Array, String]],
            ria.async.Future, function saveSupplementalAnnouncement(id, classId_, title_, content_
                , expiresDate_, hideFromStudent_, attributesListData, recipientsIds) {
                return this.post('ClassAnnouncement/SaveAnnouncement.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    announcementId:id.valueOf(),
                    classAnnouncementTypeId: 8993,
                    title: title_,
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    expiresDate: expiresDate_ && expiresDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false,
                    attributes: attributesListData,
                    recipientsIds: recipientsIds
                });
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, String
                , chlk.models.common.ChlkDate, Boolean, Array, String]],
            ria.async.Future, function submitSupplementalAnnouncement(id, classId_, title_, content_
                , expiresDate_, hideFromStudent_, attributesListData, recipientsIds) {
                return this.post('SupplementalAnnouncement/Submit.json', chlk.models.announcement.FeedAnnouncementViewData, {
                    supplementalAnnouncementPlanId:id.valueOf(),
                    title: title_ || ('test ' + id.valueOf()),
                    classId: classId_ ? classId_.valueOf() : null,
                    content: content_,
                    expiresDate: expiresDate_ && expiresDate_.toStandardFormat(),
                    hidefromstudents: hideFromStudent_ || false,
                    attributes: attributesListData,
                    recipientsIds: recipientsIds
                });
            }
        ])
});