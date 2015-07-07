REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');
REQUIRE('chlk.models.announcement.AnnouncementAttributeListViewData');
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
                this.announcementAttributes = SJX.fromArrayOfDeserializables(raw.announcementattributes, chlk.models.announcement.AnnouncementAttributeViewData);
                this.announcementQnAs = SJX.fromArrayOfDeserializables(raw.announcementqnas, chlk.models.announcement.AnnouncementQnA);
                this.applications = SJX.fromArrayOfDeserializables(raw.applications, chlk.models.apps.AppAttachment);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.Standard);
                this.studentAnnouncements = SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.StudentAnnouncements);
                this.autoGradeApps = SJX.fromArrayOfValues(raw.autogradeapps, Object);
                this.owner = SJX.fromDeserializable(raw.owner, chlk.models.people.User);
                this.exempt = SJX.fromValue(raw.exempt, Boolean);
                this.ableToRemoveStandard = SJX.fromValue(raw.canremovestandard, Boolean);
                this.suggestedApps = SJX.fromArrayOfDeserializables(raw.suggestedapps, chlk.models.apps.ApplicationForAttach);
                this.recipients = SJX.fromArrayOfDeserializables(raw.recipients, chlk.models.announcement.AdminAnnouncementRecipient);
                this.grade = SJX.fromValue(raw.grade, Number);
                this.comment = SJX.fromValue(raw.comment, String);

                this.groupIds = SJX.fromValue(raw.groupIds, String);
                this.attachments = SJX.fromValue(raw.attachments, String);
                this.gradeViewApps = SJX.fromArrayOfDeserializables(raw.gradeviewapps, chlk.models.apps.AppAttachment);
                this.applicationsIds = SJX.fromValue(raw.applicationsids, String);
                this.markingPeriodId = SJX.fromValue(raw.markingperiodid, chlk.models.id.MarkingPeriodId);
                this.categories = SJX.fromArrayOfDeserializables(raw.categories, chlk.models.common.NameId);

                this.submitType = SJX.fromValue(raw.submitType, String);
                this.galleryCategoryId = SJX.fromValue(raw.galleryCategoryId, Number);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.announcementTypeId = SJX.fromValue(raw.announcementTypeId, Number);
                this.maxScore = SJX.fromValue(raw.maxscore, Number);
                this.weightMultiplier = SJX.fromValue(raw.weightmultiplier, Number);
                this.weightAddition = SJX.fromValue(raw.weightaddition, Number);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.classId = SJX.fromValue(raw.classId, chlk.models.id.ClassId);
                this.ableDropStudentScore = SJX.fromValue(raw.candropstudentscore, Boolean);
                this.galleryCategoryForSearch = SJX.fromValue(raw.galleryCategoryForSearch, Number);
                this.filter = SJX.fromValue(raw.filter, String);

                this.ableEdit = SJX.fromValue(raw.ableedit, Boolean);

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


            ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData), 'announcementAttributes',
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

            Number, 'announcementTypeId',
            ArrayOf(chlk.models.common.NameId), 'categories',
            String, 'groupIds',
            String, 'attachments',
            String, 'applicationsIds',
            String, 'submitType',
            Boolean, 'needButtons',
            Boolean, 'needDeleteButton',
            Boolean, 'ableEdit',
            chlk.models.id.MarkingPeriodId, 'markingPeriodId',
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',
            Number, 'galleryCategoryId',
            Number, 'galleryCategoryForSearch',
            String, 'filter',
            Boolean, 'hiddenFromStudents',
            chlk.models.common.ChlkDate, 'expiresDate',
            chlk.models.common.ChlkDate, 'startDate',
            chlk.models.common.ChlkDate, 'endDate',
            Number, 'maxScore',
            Number, 'weightMultiplier',
            Number, 'weightAddition',
            Boolean, 'ableDropStudentScore',
            chlk.models.id.ClassId, 'classId',

            function getTitleModel(){
                var title = this.getClassAnnouncementData().getTitle();
                return new chlk.models.announcement.AnnouncementTitleViewData(title);
            },

            String, function calculateGradesAvg(count_) {
                var studentAnnouncements = this.getStudentAnnouncements();
                if (!studentAnnouncements)
                    return null;

                var classAvg = studentAnnouncements.getGradesAvg(count_);
                studentAnnouncements.setClassAvg(classAvg);
                return classAvg;
            },

            chlk.models.announcement.AnnouncementAttributeListViewData, function getAttributesListViewData(){
                var attrViewData = chlk.models.announcement.AnnouncementAttributeListViewData();
                attrViewData.setAnnouncementId(this.getId());
                attrViewData.setAnnouncementType(this.getType());
                attrViewData.setAnnouncementAttributes(this.getAnnouncementAttributes());
                return attrViewData;
            }
        ]);
});
