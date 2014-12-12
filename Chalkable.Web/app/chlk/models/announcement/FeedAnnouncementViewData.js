REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var annTypeEnum = chlk.models.announcement.AnnouncementTypeEnum;

    /** @class chlk.models.announcement.FeedAnnouncementViewData*/
    CLASS(
        'FeedAnnouncementViewData', [
            function $(){
                BASE();
                this._chalkableAnnouncementType = null;
                this.announcementTypeId = null;
            },

            chlk.models.id.AnnouncementId, 'id',

            [ria.serialize.SerializeProperty('fullclassname')],
            String, 'className',

            [ria.serialize.SerializeProperty('ownerattachmentscount')],
            Number, 'ownerAttachmentsCount',

            [ria.serialize.SerializeProperty('applicationscount')],
            Number, 'applicationsCount',

            [ria.serialize.SerializeProperty('showgradingicon')],
            Boolean, 'showGradingIcon',

            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId',

            Boolean, 'complete',

            [ria.serialize.SerializeProperty('gradingstudentscount')],
            Number, 'gradingStudentsCount',

            [ria.serialize.SerializeProperty('studentscount')],
            Number, 'studentsCount',

            [ria.serialize.SerializeProperty('studentscountwithattachments')],
            Number, 'studentsWithAttachmentsCount',

            VOID, function setAnnouncementTypeId(announcementTypeId){
                this.announcementTypeId = announcementTypeId;
                if(!announcementTypeId)
                    this.setChalkableAnnouncementType(this.isAdminAnnouncement() ? annTypeEnum.ADMIN.valueOf() : annTypeEnum.ANNOUNCEMENT.valueOf())
            },
            //Number, function getAnnouncementTypeId(){return this._announcementTypeId; },

            [ria.serialize.SerializeProperty('chalkableannouncementtypeid')], // make enum
            Number, 'chalkableAnnouncementType',
            [[Number]],
            VOID, function setChalkableAnnouncementType(chalkableAnnouncementType){
                this._chalkableAnnouncementType = chalkableAnnouncementType
                    || annTypeEnum.ANNOUNCEMENT.valueOf();
            },
            Number, function getChalkableAnnouncementType(){ return this._chalkableAnnouncementType;},
            Boolean, 'adminAnnouncement',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

            chlk.models.id.ClassId, function setClassId(classId){
                this.classId = classId;
                this.setAdminAnnouncement(!classId);
            },

            Boolean, function setAdminAnnouncement(isAdminAnnouncement){
                if(isAdminAnnouncement === null)
                    this.adminAnnouncement = !this.classId;
            },

            [ria.serialize.SerializeProperty('announcementtypename')],
            String, 'announcementTypeName',

            String, 'title',

            [ria.serialize.SerializeProperty('shortcontent')],
            String, 'shortContent',

            [ria.serialize.SerializeProperty('expiresdate')],
            chlk.models.common.ChlkDate, 'expiresDate',

            [ria.serialize.SerializeProperty('applicationname')],
            String, 'applicationName',

            [ria.serialize.SerializeProperty('hidefromstudents')],
            Boolean, 'hiddenFromStudents'
        ]);
});
