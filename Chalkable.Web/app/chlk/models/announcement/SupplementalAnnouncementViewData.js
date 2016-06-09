REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.AnnouncementWithExpiresDateViewData');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.LpGalleryCategoryId');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.SupplementalAnnouncementViewData*/
    CLASS(
        UNSAFE, 'SupplementalAnnouncementViewData',
                EXTENDS(chlk.models.announcement.AnnouncementWithExpiresDateViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.setAnnouncementTypeId(SJX.fromValue(raw.announcementtypeid, Number));
                this.classId = SJX.fromValue(Number(raw.classid), chlk.models.id.ClassId);
                this.shortClassName = SJX.fromValue(raw.classname, String);
                this.className = SJX.fromValue(raw.fullclassname, String);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.galleryCategoryId = SJX.fromValue(raw.gallerycategoryid, chlk.models.id.LpGalleryCategoryId);
                this.recipients = SJX.fromArrayOfDeserializables(raw.recipients, chlk.models.people.User);
            },

            ArrayOf(chlk.models.people.User), 'recipients',
            chlk.models.id.ClassId, 'classId',
            String, 'shortClassName',
            String, 'className',
            Boolean, 'hiddenFromStudents',
            chlk.models.id.LpGalleryCategoryId, 'galleryCategoryId',
            Number, 'announcementTypeId',

            function getRecipientsTooltip(){
                var res = this.getRecipients() ? this.getRecipients().map(function(item){return item.getFullName()}).join('\n') : '';
                return res;
            }
        ]);
});
