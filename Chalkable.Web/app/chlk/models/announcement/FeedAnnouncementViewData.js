REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.apps.ApplicationForAttach');
REQUIRE('chlk.models.announcement.AdminAnnouncementRecipient');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.FeedAnnouncementViewData*/
    CLASS(
        UNSAFE, 'FeedAnnouncementViewData',
                EXTENDS(chlk.models.announcement.Announcement),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.announcementAttachments = SJX.fromArrayOfDeserializables(raw.announcementattachments, chlk.models.attachment.Attachment);
                this.announcementQnAs = SJX.fromArrayOfDeserializables(raw.announcementqnas, chlk.models.announcement.AnnouncementQnA);
                this.applications = SJX.fromArrayOfDeserializables(raw.applications, chlk.models.apps.AppAttachment);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.Standard);
                this.studentAnnouncements = SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.StudentAnnouncements);
                this.autoGradeApps = SJX.fromArrayOfValues(raw.autogradeapps, Object);
                this.owner = SJX.fromDeserializable(raw.owner, chlk.models.people.User);
                this.exempt = SJX.fromDeserializable(raw.exempt, Boolean);
                this.ableToRemoveStandard = SJX.fromValue(raw.canremovestandard, Boolean);
                this.suggestedApps = SJX.fromArrayOfDeserializables(raw.suggestedapps, chlk.models.apps.ApplicationForAttach);
                this.recipients = SJX.fromArrayOfDeserializables(raw.recipients, chlk.models.announcement.AdminAnnouncementRecipient);
                this.grade = SJX.fromValue(raw.grade, Number);
                this.comment = SJX.fromValue(raw.comment, String);

                this.groupIds = SJX.fromValue(raw.groupIds, String);

                /*
                this.recipientId = SJX.fromValue(raw.recipientid, chlk.models.id.ClassId);
                this.stateTyped = SJX.fromValue(raw.statetyped, Number);
                this.systemType = SJX.fromValue(raw.systemtype, Number);
                this.wasSubmittedToAdmin = SJX.fromValue(raw.wassubmittedtoadmin, Boolean);
                this.gradeViewApps = SJX.fromArrayOfDeserializables(raw.gradeviewapps, chlk.models.apps.AppAttachment);
                this.attachments = SJX.fromValue(raw.attachments, String);
                this.applicationsIds = SJX.fromValue(raw.applicationsids, String);
                this.expiresDateText = SJX.fromValue(raw.expiresdatetext, String);
                this.currentUser = SJX.fromDeserializable(raw.currentuser, chlk.models.people.User);
                this.subject = SJX.fromValue(raw.subject, String);
                this.markingPeriodId = SJX.fromValue(raw.markingperiodid, chlk.models.id.MarkingPeriodId);
                this.annRecipients = SJX.fromValue(raw.annrecipients, String);
                this.ableEdit = SJX.fromValue(raw.ableedit, Boolean);
                this.submitType = SJX.fromValue(raw.submitType, String);
                this.adminAnnouncement = SJX.fromValue(raw.isadminannouncement, Boolean);*/

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

            ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',
            ArrayOf(chlk.models.apps.AppAttachment), 'applications',
            ArrayOf(chlk.models.standard.Standard), 'standards',
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements',
            Array, 'autoGradeApps',
            chlk.models.people.User, 'owner',
            Boolean, 'exempt',
            Boolean, 'ableToRemoveStandard',
            ArrayOf(chlk.models.apps.ApplicationForAttach), 'suggestedApps',
            Number, 'grade',
            String, 'comment',
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients',

            String, 'groupIds',
            /*
            chlk.models.id.ClassId, 'recipientId',
            Number, 'stateTyped',
            Number, 'systemType',
            Boolean, 'wasSubmittedToAdmin',
            READONLY, Boolean, 'standartAnnouncement',
            Boolean, function isStandartAnnouncement(){
                return !this.getAnnouncementTypeId();
            },
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',
            String, 'attachments',
            String, 'applicationsIds',
            String, 'expiresDateColor',
            String, 'expiresDateText',
            chlk.models.people.User, 'currentUser',
            String, 'subject',
            chlk.models.id.MarkingPeriodId, 'markingPeriodId',
            String, 'submitType',
            Boolean, 'needButtons',
            Boolean, 'needDeleteButton',
            String, 'annRecipients',
            Boolean, 'adminAnnouncement',*/

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
                if (!studentAnnouncements)
                    return null;

                var classAvg = studentAnnouncements.getGradesAvg(count_);
                studentAnnouncements.setClassAvg(classAvg);
                return classAvg;
            }
        ]);
});
