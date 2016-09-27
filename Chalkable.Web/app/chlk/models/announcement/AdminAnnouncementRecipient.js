REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.GroupId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.AdminAnnouncementRecipient*/
    CLASS(
        'AdminAnnouncementRecipient', IMPLEMENTS(ria.serialize.IDeserializable), [

            chlk.models.id.AnnouncementId, 'announcementId',

            chlk.models.id.GroupId, 'groupId',

            String, 'groupName',

            Number, 'studentCount',

            ArrayOf(String), 'studentsDisplayName',

            VOID, function deserialize(raw){
                this.announcementId = SJX.fromValue(raw.announcementid, chlk.models.id.AnnouncementId);
                this.groupId = SJX.fromValue(raw.groupid, chlk.models.id.GroupId);
                this.groupName = SJX.fromValue(raw.groupname, String);
                this.studentCount = SJX.fromValue(raw.studentcount, Number);
                this.studentsDisplayName = SJX.fromArrayOfValues(raw.studentsdisplayname, String);
            },

            [[chlk.models.id.AnnouncementId, chlk.models.id.GroupId, String]],
            function $(announcementId_, groupId_, groupName_){
                BASE();
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
                if(groupId_)
                    this.setGroupId(groupId_);
                if(groupName_)
                    this.setGroupName(groupName_);
            }

        ]);
});
