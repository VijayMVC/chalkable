REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.attachment.Attachment');

REQUIRE('chlk.models.announcement.AnnouncementType');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.apps.ApplicationForAttach');

REQUIRE('chlk.models.announcement.AdminAnnouncementRecipient');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.StateEnum*/
    ENUM('StateEnum', {
        CREATED: 0,
        SUBMITTED: 1
    });

    /** @class chlk.models.announcement.Announcement*/
    CLASS(
        UNSAFE, 'Announcement',
                EXTENDS(chlk.models.announcement.BaseAnnouncementViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.canAddStandard = SJX.fromValue(raw.canaddstandard, Boolean);
                this.ableToRemoveStandard = SJX.fromValue(raw.canremovestandard, Boolean);
                this.setAnnouncementTypeId(SJX.fromValue(raw.announcementtypeid, Number));
                this.chalkableAnnouncementType = SJX.fromValue(raw.chalkableannouncementtypeid, Number);
                this.classId = SJX.fromValue(Number(raw.classid), chlk.models.id.ClassId);
                this.applicationName = SJX.fromValue(raw.applicationname, String);
                this.applicationsCount = SJX.fromValue(raw.applicationscount, Number);
                this.autoGradeApps = SJX.fromArrayOfValues(raw.autogradeapps, Object);
                this.avgNumeric = SJX.fromValue(raw.avgnumeric, Number);
                this.clazz = raw['class'] || null;
                this.className = SJX.fromValue(raw.fullclassname, String);
                this.announcementAttachments = SJX.fromArrayOfDeserializables(raw.announcementattachments, chlk.models.attachment.Attachment);
                this.departmentId = SJX.fromValue(raw.departmentid, chlk.models.id.DepartmentId);
                this.gradingStudentsCount = SJX.fromValue(raw.gradingstudentscount, Number);
                this.gradingStyle = SJX.fromValue(raw.gradingstyle, Number);
                this.ownerAttachmentsCount = SJX.fromValue(raw.ownerattachmentscount, Number);
                this.recipientId = SJX.fromValue(raw.recipientid, chlk.models.id.ClassId);
                this.schoolPersonGender = SJX.fromValue(raw.schoolpersongender, String);
                this.schoolPersonName = SJX.fromValue(raw.schoolpersonname, String);
                this.personId = SJX.fromValue(raw.personid, chlk.models.id.SchoolPersonId);
                this.personName = SJX.fromValue(raw.personname, String);
                this.shortContent = SJX.fromValue(raw.shortcontent, String);
                this.showGradingIcon = SJX.fromValue(raw.showgradingicon, Boolean);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.Standard);
                this.stateTyped = SJX.fromValue(raw.statetyped, Number);
                this.studentAnnouncementId = SJX.fromValue(raw.studentannouncementid, chlk.models.id.StudentAnnouncementId);
                this.studentAnnouncements = SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.StudentAnnouncements);
                this.announcementQnAs = SJX.fromArrayOfDeserializables(raw.announcementqnas, chlk.models.announcement.AnnouncementQnA);
                this.weightMultiplier = SJX.fromValue(raw.weightmultiplier, Number);
                this.weightAddition = SJX.fromValue(raw.weightaddition, Number);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.ableToExempt = SJX.fromValue(raw.maybeexempt, Boolean);
                this.systemType = SJX.fromValue(raw.systemtype, Number);
                this.wasSubmittedToAdmin = SJX.fromValue(raw.wassubmittedtoadmin, Boolean);
                this.classId = SJX.fromValue(raw.classid, chlk.models.id.ClassId);
                this.chalkableAnnouncementType = SJX.fromValue(raw.chalkableannouncementtypeid, Number);
                this.setChalkableAnnouncementType(this.chalkableAnnouncementType);
                this.applications = SJX.fromArrayOfDeserializables(raw.applications, chlk.models.apps.AppAttachment);
                this.gradeViewApps = SJX.fromArrayOfDeserializables(raw.gradeviewapps, chlk.models.apps.AppAttachment);
                this.attachments = SJX.fromValue(raw.attachments, String);
                this.applicationsIds = SJX.fromValue(raw.applicationsids, String);
                this.comment = SJX.fromValue(raw.comment, String);
                this.content = SJX.fromValue(raw.content, String);
                this.created = SJX.fromDeserializable(raw.created, chlk.models.common.ChlkDate);
                this.expiresDateText = SJX.fromValue(raw.expiresdatetext, String);
                this.owner = SJX.fromDeserializable(raw.owner, chlk.models.people.User);
                this.currentUser = SJX.fromDeserializable(raw.currentuser, chlk.models.people.User);
                this.complete = SJX.fromValue(raw.complete, Boolean);
                this.state = SJX.fromValue(raw.state, Number);
                this.grade = SJX.fromValue(raw.grade, Number);
                this.subject = SJX.fromValue(raw.subject, String);
                this.markingPeriodId = SJX.fromValue(raw.markingperiodid, chlk.models.id.MarkingPeriodId);
                this.annRecipients = SJX.fromValue(raw.annrecipients, String);
                this.ableEdit = SJX.fromValue(raw.ableedit, Boolean);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.groupIds = SJX.fromValue(raw.groupIds, String);
                this.suggestedApps = SJX.fromArrayOfDeserializables(raw.suggestedapps, chlk.models.apps.ApplicationForAttach);
                this.shortClassName = SJX.fromValue(raw.classname, String);
                this.assessmentApplicationId = SJX.fromValue(raw.assessmentapplicationid, chlk.models.id.AppId);

                this.recipients = SJX.fromArrayOfDeserializables(raw.recipients, chlk.models.announcement.AdminAnnouncementRecipient);
                this.adminAnnouncement = SJX.fromValue(raw.isadminannouncement, Boolean);

                if(this.autoGradeApps && this.autoGradeApps.length){
                    var autoGradeApps = [];
                    this.autoGradeApps.forEach(function(item){
                        var app = autoGradeApps.filter(function(app){return app.id == item.announcementapplicationid})[0];
                        if(!app){
                            autoGradeApps.push({
                                name: this.applications.filter(function(app){return app.getAnnouncementApplicationId().valueOf() == item.announcementapplicationid})[0].name,
                                id: item.announcementapplicationid,
                                students: [{id:item.studentid, grade:item.grade}]
                            })
                        }else{
                            app.students.push({id:item.studentid, grade:item.grade});
                        }
                    }, this);
                    this.autoGradeApps = autoGradeApps;
                }

            },
            function $(){
                BASE();
                this._chalkableAnnouncementType = null;
                this.announcementTypeId = null;
                this.classId = null;
                this._annTypeEnum = chlk.models.announcement.AnnouncementTypeEnum;
            },

            Number, 'announcementTypeId',
            [[Number]],
            VOID, function setAnnouncementTypeId(announcementTypeId){
                this.announcementTypeId = announcementTypeId;
                if(!announcementTypeId)
                    this.setChalkableAnnouncementType(this.isAdminAnnouncement() ? this._annTypeEnum.ADMIN.valueOf() : this._annTypeEnum.ANNOUNCEMENT.valueOf())
            },

            Boolean, 'canAddStandard',
            Boolean, 'ableToRemoveStandard',
            ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',
            String, 'applicationName',
            Number, 'applicationsCount',
            Number, 'attachmentsSummary',
            Array, 'autoGradeApps',
            Number ,'avgNumeric',
            Object, 'clazz',
            String, 'className',
            String, 'shortClassName',
            String, 'groupIds',

            chlk.models.id.DepartmentId, 'departmentId',
            Number, 'gradesSummary',
            Number, 'gradingStudentsCount',
            Number, 'gradingStyle',
            chlk.models.id.ClassId, 'recipientId',
            String, 'schoolPersonGender',
            String, 'schoolPersonName',
            chlk.models.id.SchoolPersonId, 'personId',
            String, 'personName',
            String, 'shortContent',
            Boolean, 'showGradingIcon',
            ArrayOf(chlk.models.standard.Standard), 'standards',
            ArrayOf(chlk.models.apps.ApplicationForAttach), 'suggestedApps',

            Number, 'stateTyped',
            chlk.models.id.StudentAnnouncementId, 'studentAnnouncementId',
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements',
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',
            Number, 'weightMultiplier',
            Number, 'ownerAttachmentsCount',
            Number, 'weightAddition',
            Boolean, 'hiddenFromStudents',
            Boolean, 'ableToExempt',
            Number, 'systemType',
            Boolean, 'wasSubmittedToAdmin',
            chlk.models.id.ClassId, 'classId',

            VOID, function setClassId(classId){
                this.classId = classId;
            },
            Number, 'chalkableAnnouncementType',

            [[Number]],
            VOID, function setChalkableAnnouncementType(chalkableAnnouncementType){
                this._chalkableAnnouncementType = chalkableAnnouncementType
                    || this._annTypeEnum.ANNOUNCEMENT.valueOf();
            },
            Number, function getChalkableAnnouncementType(){ return this._chalkableAnnouncementType;},


            READONLY, Boolean, 'standartAnnouncement',
            Boolean, function isStandartAnnouncement(){
                return !this.getAnnouncementTypeId();
            },
            ArrayOf(chlk.models.apps.AppAttachment), 'applications',
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',
            String, 'attachments',
            String, 'applicationsIds',
            String, 'comment',
            String, 'content',
            chlk.models.common.ChlkDate, 'created',
            String, 'expiresDateColor',
            String, 'expiresDateText',
            Number, 'grade',
            chlk.models.people.User, 'owner',
            chlk.models.people.User, 'currentUser',
            Boolean, 'complete',
            Number, 'state',
            String, 'subject',
            chlk.models.id.MarkingPeriodId, 'markingPeriodId',
            String, 'submitType',
            Boolean, 'needButtons',
            Boolean, 'needDeleteButton',
            String, 'annRecipients',
            Boolean, 'ableEdit',
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients',

            Boolean, 'adminAnnouncement',

            chlk.models.id.AppId, 'assessmentApplicationId',

            function prepareExpiresDateText(){
                var now = getSchoolYearServerDate();
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
            },
            function getTitleModel(){
                var title = this.getTitle();
                return new chlk.models.announcement.AnnouncementTitleViewData(title);
            },

            String, function calculateGradesAvg(count_) {
                var studentAnnouncements = this.getStudentAnnouncements();
                console.log("calculateGradesAvg", studentAnnouncements);
                if (!studentAnnouncements)
                    return null;

                var classAvg = studentAnnouncements.getGradesAvg(count_);
                studentAnnouncements.setClassAvg(classAvg);
                return classAvg;
            }

        ]);
});
