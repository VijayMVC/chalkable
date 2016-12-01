REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.id.DepartmentId');

NAMESPACE('chlk.models.search', function () {

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.search.SearchTypeEnum*/
    ENUM('SearchTypeEnum', {
        PERSONS: 0,
        APPLICATIONS: 1,
        ANNOUNCEMENTS: 2,
        ATTACHMENTS: 3,
        CLASSES: 4,
        STUDENT: 5,
        STAFF: 6,
        GROUP: 7
    });

    "use strict";
    /** @class chlk.models.search.SearchItem*/
    CLASS(
        UNSAFE, 'SearchItem', IMPLEMENTS(ria.serialize.IDeserializable),  [
            String, 'id',
            String, 'description',
            chlk.models.people.ShortUserInfo, 'personInfo',
            chlk.models.search.SearchTypeEnum, 'searchType',
            String, 'announcementId',
            Number, 'announcementType',
            Boolean, 'adminAnnouncement',
            String, 'smallPictureId',
            chlk.models.id.DepartmentId, 'departmentId',
            String, 'documentThumbnailUrl',

            VOID, function deserialize(raw) {
                this.id = SJX.fromValue(raw.id, String);
                this.description = SJX.fromValue(raw.description, String);
                if(raw.shortpersoninfo)
                    this.personInfo = SJX.fromDeserializable(raw.shortpersoninfo, chlk.models.people.ShortUserInfo);
                this.searchType = SJX.fromValue(raw.searchtype, chlk.models.search.SearchTypeEnum);
                this.announcementId = SJX.fromValue(raw.announcementid, String);
                this.announcementType = SJX.fromValue(raw.announcementtype, Number);
                this.adminAnnouncement = SJX.fromValue(raw.isadminannouncement, Boolean);
                this.smallPictureId = SJX.fromValue(raw.smallpictureid, String);
                this.departmentId = SJX.fromValue(raw.departmentid, chlk.models.id.DepartmentId);
                this.documentThumbnailUrl = SJX.fromValue(raw.attachmentthumbnailurl, String);
            }

        ]);
});

