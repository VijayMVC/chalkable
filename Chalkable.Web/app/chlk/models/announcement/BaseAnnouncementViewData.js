REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementTypeEnum*/
    ENUM('AnnouncementTypeEnum', {
        CLASS_ANNOUNCEMENT: 1,
        ADMIN: 2,
        LESSON_PLAN: 3,
        SUPPLEMENTAL_ANNOUNCEMENT: 4
    });

    /** @class chlk.models.announcement.StateEnum*/
    ENUM('StateEnum', {
        CREATED: 0,
        SUBMITTED: 1
    });

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.BaseAnnouncementViewData*/
    CLASS(
        UNSAFE, 'BaseAnnouncementViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.annOwner = SJX.fromValue(raw.isowner, Boolean);
                this.id = SJX.fromValue(Number(raw.id), chlk.models.id.AnnouncementId);
                this.title = SJX.fromValue(raw.title, String);
                this.announcementTypeName = SJX.fromValue(raw.announcementtypename, String);
                this.type = SJX.fromValue(raw.type, chlk.models.announcement.AnnouncementTypeEnum);
                this.content = SJX.fromValue(raw.content, String);
                this.created = SJX.fromDeserializable(raw.created, chlk.models.common.ChlkDate);
                this.state = SJX.fromValue(raw.state, chlk.models.announcement.StateEnum);
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
                this.personName = SJX.fromValue(raw.personname, String);
                this.personGender = SJX.fromValue(raw.schoolpersongender, String);
            },

            chlk.models.id.AnnouncementId, 'id',
            String, 'title',
            String, 'announcementTypeName',
            chlk.models.announcement.AnnouncementTypeEnum, 'type',
            String, 'content',
            chlk.models.common.ChlkDate, 'created',
            chlk.models.announcement.StateEnum, 'state',
            chlk.models.id.SchoolPersonId, 'personId',
            String, 'personName',
            String, 'personGender',
            Boolean, 'annOwner'
        ]);
});
