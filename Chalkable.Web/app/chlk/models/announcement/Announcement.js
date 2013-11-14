REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.attachment.Attachment');

REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.announcement.Reminder');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.apps.AppAttachment');



NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.StateEnum*/
    ENUM('StateEnum', {
        CREATED: 0,
        SUBMITTED: 1
    });

    /** @class chlk.models.announcement.Announcement*/
    CLASS(
        'Announcement', [

            function $(){
                BASE();
                this._chalkableAnnouncementType = null;
                this._announcementTypeId = null;
                this._annTypeEnum = chlk.models.announcement.AnnouncementTypeEnum;
            },

            chlk.models.id.AnnouncementId, 'id',

            [ria.serialize.SerializeProperty('announcementattachments')],
            ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',

            [ria.serialize.SerializeProperty('announcementreminders')],
            ArrayOf(chlk.models.announcement.Reminder), 'announcementReminders',

            [ria.serialize.SerializeProperty('announcementtypeid')],
            Number, 'announcementTypeId',
            [[Number]],
            VOID, function setAnnouncementTypeId(announcementTypeId){
                this._announcementTypeId = announcementTypeId;
                if(!announcementTypeId)
                    this.setChalkableAnnouncementType(this._annTypeEnum.ADMIN.valueOf())
            },
            Number, function getAnnouncementTypeId(){return this._announcementTypeId; },

            [ria.serialize.SerializeProperty('chalkableannouncementtypeid')], // make enum
            Number, 'chalkableAnnouncementType',
            [[Number]],
            VOID, function setChalkableAnnouncementType(chalkableAnnouncementType){
                this._chalkableAnnouncementType = chalkableAnnouncementType
                    || this._annTypeEnum.ANNOUNCEMENT.valueOf();
            },
            Number, function getChalkableAnnouncementType(){ return this._chalkableAnnouncementType;},


            READONLY, Boolean, 'adminAnnouncement',
            Boolean, function isAdminAnnouncement(){
                return  this.getChalkableAnnouncementType() == this._annTypeEnum.ADMIN.valueOf()
                    && !this.getAnnouncementTypeId()
            },

            READONLY, Boolean, 'standartAnnouncement',
            Boolean, function isStandartAnnouncement(){
                return this.getChalkableAnnouncementType() == this._annTypeEnum.ANNOUNCEMENT.valueOf()
            },

            [ria.serialize.SerializeProperty('announcementtypename')],
            String, 'announcementTypeName',

            ArrayOf(chlk.models.apps.AppAttachment), 'applications',
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',

            [ria.serialize.SerializeProperty('applicationname')],
            String, 'applicationName',

            [ria.serialize.SerializeProperty('applicationscount')],
            Number, 'applicationsCount',

            String, 'attachments',

            [ria.serialize.SerializeProperty('attachmentscount')],
            Number, 'attachmentsCount',

            [ria.serialize.SerializeProperty('attachmentsummary')],
            Number, 'attachmentsSummary',

            [ria.serialize.SerializeProperty('autogradeapps')],
            ArrayOf(String), 'autoGradeApps',

            Number, 'avg',

            [ria.serialize.SerializeProperty('avgnumeric')],
            Number ,'avgNumeric',

            [ria.serialize.SerializeProperty('class')],
            Object, 'clazz',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className',

            String, 'comment',
            String, 'content',
            chlk.models.common.ChlkDate, 'created',
            Boolean, 'dropped',

            [ria.serialize.SerializeProperty('expiresdate')],
            chlk.models.common.ChlkDate, 'expiresDate',

            String, 'expiresDateColor',

            String, 'expiresDateText',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

            Boolean, 'gradable',
            Number, 'grade',

            [ria.serialize.SerializeProperty('gradesummary')],
            Number, 'gradesSummary',

            [ria.serialize.SerializeProperty('gradingstudentscount')],
            Number, 'gradingStudentsCount',

            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'annOwner',

            [ria.serialize.SerializeProperty('nongradingstudentscount')],
            Number, 'nonGradingStudentsCount',

            Number, 'order',

            [ria.serialize.SerializeProperty('ownerattachmentscount')],
            Number, 'ownerAttachmentsCount',

            chlk.models.people.User, 'owner',

            chlk.models.people.User, 'currentUser',

            [ria.serialize.SerializeProperty('qnacount')],
            Number, 'qnaCount',

            [ria.serialize.SerializeProperty('recipientid')],
            chlk.models.id.ClassId, 'recipientId',

            [ria.serialize.SerializeProperty('schoolpersongender')],
            String, 'schoolPersonGender',

            [ria.serialize.SerializeProperty('schoolpersonname')],
            String, 'schoolPersonName',

            [ria.serialize.SerializeProperty('personid')],
            chlk.models.id.SchoolPersonId, 'personId',

            [ria.serialize.SerializeProperty('personname')],
            String, 'personName',

            [ria.serialize.SerializeProperty('shortcontent')],
            String, 'shortContent',

            [ria.serialize.SerializeProperty('showgradingicon')],
            Boolean, 'showGradingIcon',

            Boolean, 'starred',
            Number, 'state',

            [ria.serialize.SerializeProperty('statetyped')],
            Number, 'stateTyped',

            [ria.serialize.SerializeProperty('studentannouncementid')],
            chlk.models.id.StudentAnnouncementId, 'studentAnnouncementId',

            [ria.serialize.SerializeProperty('studentannouncements')],
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements',

            [ria.serialize.SerializeProperty('studentscount')],
            Number, 'studentsCount',

            [ria.serialize.SerializeProperty('studentscountwithattachments')],
            Number, 'studentsWithAttachmentsCount',

            [ria.serialize.SerializeProperty('studentscountwithoutattachments')],
            Number, 'studentsWithoutAttachmentsCount',

            String, 'subject',
            [ria.serialize.SerializeProperty('systemtype')],

            Number, 'systemType',

            String, 'title',

            [ria.serialize.SerializeProperty('wasannouncementtypegraded')],
            Boolean, 'wasAnnouncementTypeGraded',

            [ria.serialize.SerializeProperty('wassubmittedtoadmin')],
            Boolean, 'wasSubmittedToAdmin',

            chlk.models.id.MarkingPeriodId, 'markingPeriodId',

            String, 'submitType',

            Boolean, 'needButtons',

            Boolean, 'needDeleteButton',

            [ria.serialize.SerializeProperty('announcementqnas')],
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',

            String, 'annRecipients',

            function prepareExpiresDateText(){
                var now = getDate();
                var days = 0;
                var expTxt = "";
                var expires = this.getExpiresDate();
                var expiresDate = expires.getDate();
                var date = expires.format('(D m/d)');
                this.setExpiresDateColor('blue');

                if(formatDate(now, 'dd-mm-yy') == expires.format('dd-mm-yy')){
                    this.setExpiresDateColor('blue');
                    this.setExpiresDateText(Msg.Due_today);
                }else{
                    if(now > expires.getDate()){
                        this.setExpiresDateColor('red');
                        days = getDateDiffInDays(expiresDate, now);
                        expTxt = days == 1 ? Msg.Due_yesterday + " " + date : Msg.Due_days_ago(days) + " " + date;

                    }else{
                        days = getDateDiffInDays(now, expiresDate);
                        expTxt = days == 1 ? Msg.Due_tomorrow + " " + date : Msg.Due_in_days(days) + " " + date;
                    }
                    this.setExpiresDateText(expTxt);
                }
            }


        ]);
});
