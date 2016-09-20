REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');
REQUIRE('chlk.models.announcement.AnnouncementAttributeListViewData');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.announcement.AnnouncementQnA');
REQUIRE('chlk.models.announcement.AnnouncementComment');
REQUIRE('chlk.models.apps.AppAttachment');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.announcement.AdminAnnouncementRecipient');
REQUIRE('chlk.models.announcement.CategoryViewData');
REQUIRE('chlk.models.announcement.StudentAnnouncementApplicationMeta');

REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.FeedSortTypeEnum*/
    ENUM('FeedSortTypeEnum', {
        DUE_DATE_ASCENDING: 0,
        DUE_DATE_DESCENDING: 1
    });

    /** @class chlk.models.announcement.FeedAnnouncementViewData*/
    CLASS(
        UNSAFE, 'FeedAnnouncementViewData',
                EXTENDS(chlk.models.announcement.Announcement),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.announcementAttachments = SJX.fromArrayOfDeserializables(raw.announcementattachments, chlk.models.attachment.AnnouncementAttachment);
                this.announcementAttributes = SJX.fromArrayOfDeserializables(raw.announcementattributes, chlk.models.announcement.AnnouncementAttributeViewData);
                this.announcementAssignedAttrs = SJX.fromValue(raw.announcementAssignedAttrs, String);
                this.announcementQnAs = SJX.fromArrayOfDeserializables(raw.announcementqnas, chlk.models.announcement.AnnouncementQnA);
                this.announcementComments = SJX.fromArrayOfDeserializables(raw.announcementcomments, chlk.models.announcement.AnnouncementComment);
                this.applications = SJX.fromArrayOfDeserializables(raw.applications, chlk.models.apps.AppAttachment);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.Standard);
                this.studentAnnouncements = SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.StudentAnnouncements);
                this.autoGradeApps = raw.autogradeapps;
                this.owner = SJX.fromDeserializable(raw.owner, chlk.models.people.User);
                this.exempt = SJX.fromValue(raw.exempt, Boolean);
                this.ableToRemoveStandard = SJX.fromValue(raw.canremovestandard, Boolean);
                this.suggestedApps = SJX.fromArrayOfDeserializables(raw.suggestedapps, chlk.models.apps.Application);
                this.appsWithContent = SJX.fromArrayOfDeserializables(raw.appswithcontent, chlk.models.apps.Application);
                this.recipients = SJX.fromArrayOfDeserializables(raw.recipients, chlk.models.announcement.AdminAnnouncementRecipient);
                this.grade = SJX.fromValue(raw.grade, Number);
                this.comment = SJX.fromValue(raw.comment, String);
                this.ableUseExtraCredit = SJX.fromValue(raw.isableuseextracredit, Boolean);

                this.groupIds = SJX.fromValue(raw.groupIds, String);
                this.studentIds = SJX.fromValue(raw.studentIds, String);
                if(raw.selectedItems)
                    this.selectedItems = JSON.parse(SJX.fromValue(raw.selectedItems, String));
                this.attachments = SJX.fromValue(raw.attachments, String);
                this.gradeViewApps = SJX.fromArrayOfDeserializables(raw.gradeviewapps, chlk.models.apps.AppAttachment);
                this.applicationsIds = SJX.fromValue(raw.applicationsids, String);
                this.markingPeriodId = SJX.fromValue(raw.markingperiodid, chlk.models.id.MarkingPeriodId);
                this.categories = SJX.fromArrayOfDeserializables(raw.categories, chlk.models.announcement.CategoryViewData);

                this.submitType = SJX.fromValue(raw.submitType, String);
                this.galleryCategoryId = SJX.fromValue(raw.galleryCategoryId, chlk.models.id.LpGalleryCategoryId);
                this.hiddenFromStudents = this.isFeedItemHidden_(raw);
                this.announcementTypeId = SJX.fromValue(raw.announcementTypeId, Number);
                this.maxScore = SJX.fromValue(raw.maxscore, Number);
                this.weightMultiplier = SJX.fromValue(raw.weightmultiplier, Number);
                this.weightAddition = SJX.fromValue(raw.weightaddition, Number);
                this.gradable = SJX.fromValue(raw.gradable, Boolean);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
                this.startDate = SJX.fromDeserializable(raw.startdate, chlk.models.common.ChlkDate);
                this.endDate = SJX.fromDeserializable(raw.enddate, chlk.models.common.ChlkDate);
                this.classId = SJX.fromValue(raw.classId || raw.classid, chlk.models.id.ClassId);
                this.ableDropStudentScore = SJX.fromValue(raw.candropstudentscore, Boolean);
                this.inGallery = SJX.fromValue(raw.inGallery, Boolean);
                this.galleryCategoryForSearch = SJX.fromValue(raw.galleryCategoryForSearch, chlk.models.id.LpGalleryCategoryId);
                this.filter = SJX.fromValue(raw.filter, String);
                this.recipientIds = SJX.fromValue(raw.recipientIds, String);
                this.announcementForTemplateId = SJX.fromValue(raw.announcementForTemplateId, chlk.models.id.AnnouncementId);

                this.ableEdit = SJX.fromValue(raw.ableedit, Boolean);
                this.imported = SJX.fromValue(raw.imported, Boolean);

                this.requestId = SJX.fromValue(raw.requestId, String);

                if(raw.createdAnnouncements)
                    this.createdAnnouncements = JSON.parse(raw.createdAnnouncements);

                if(this.autoGradeApps && this.autoGradeApps.length){
                    var autoGradeApps = [];
                    this.autoGradeApps.forEach(function(item){
                        var app = autoGradeApps.filter(function(app){return app.id == item.announcementapplicationid})[0];
                        if(!app){

                            autoGradeApps.push({
                                name: this.applications.filter(function(app){
                                    return app.getAnnouncementApplicationId().valueOf() == item.announcementapplicationid
                                })[0].name,
                                id: item.announcementapplicationid,
                                students: [{id:item.studentid, grade:item.grade}]
                            })
                        }else{
                            app.students.push({id:item.studentid, grade:item.grade});
                        }
                    }, this);
                    this.autoGradeApps = autoGradeApps;
                }

                this.studentsAnnApplicationMeta = SJX.fromArrayOfDeserializables(raw.studentsannouncementapplicationmeta,
                    chlk.models.announcement.StudentAnnouncementApplicationMeta);
            },

            ArrayOf(chlk.models.announcement.StudentAnnouncementApplicationMeta), 'studentsAnnApplicationMeta',
            String, 'recipientIds',
            Boolean, 'imported',
            Boolean, 'inGallery',
            Object, 'createdAnnouncements',
            ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData), 'announcementAttributes',
            String, 'announcementAssignedAttrs',
            ArrayOf(chlk.models.attachment.AnnouncementAttachment), 'announcementAttachments',
            ArrayOf(chlk.models.announcement.AnnouncementQnA), 'announcementQnAs',
            ArrayOf(chlk.models.apps.AppAttachment), 'applications',
            ArrayOf(chlk.models.standard.Standard), 'standards',
            chlk.models.announcement.StudentAnnouncements, 'studentAnnouncements',
            Array, 'autoGradeApps',
            chlk.models.people.User, 'owner',
            Boolean, 'exempt',
            Boolean, 'ableToRemoveStandard',
            ArrayOf(chlk.models.apps.Application), 'suggestedApps',
            ArrayOf(chlk.models.apps.Application), 'appsWithContent',
            Number, 'grade',
            String, 'comment',
            ArrayOf(chlk.models.announcement.AdminAnnouncementRecipient), 'recipients',
            ArrayOf(chlk.models.announcement.AnnouncementComment), 'announcementComments',

            Number, 'announcementTypeId',
            ArrayOf(chlk.models.announcement.CategoryViewData), 'categories',
            String, 'groupIds',
            String, 'studentIds',
            Object, 'selectedItems',
            String, 'attachments',
            String, 'applicationsIds',
            String, 'submitType',
            Boolean, 'needButtons',
            Boolean, 'needDeleteButton',
            Boolean, 'ableEdit',
            chlk.models.id.MarkingPeriodId, 'markingPeriodId',
            ArrayOf(chlk.models.apps.AppAttachment), 'gradeViewApps',
            chlk.models.id.LpGalleryCategoryId, 'galleryCategoryId',
            chlk.models.id.LpGalleryCategoryId, 'galleryCategoryForSearch',
            String, 'filter',
            Boolean, 'hiddenFromStudents',
            chlk.models.common.ChlkDate, 'expiresDate',
            chlk.models.common.ChlkDate, 'startDate',
            chlk.models.common.ChlkDate, 'endDate',
            Number, 'maxScore',
            Number, 'weightMultiplier',
            Number, 'weightAddition',
            Boolean, 'ableDropStudentScore',
            Boolean, 'gradable',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AnnouncementId, 'announcementForTemplateId',

            Boolean, 'ableUseExtraCredit',

            chlk.models.classes.Class, 'clazz',

            String, 'requestId',

            [[Object]],
            Boolean, function isFeedItemHidden_(raw){
                var viewData = raw.classannouncementdata;
                if (!viewData)
                    viewData = raw.lessonplandata;
                if (!viewData)
                    viewData = raw.adminannouncementdata;
                if (!viewData)
                    viewData = {hidefromstudents : raw.hidefromstudents || false};
                return SJX.fromValue(viewData.hidefromstudents, Boolean);
            },

            Boolean, function isExtraCreditEnabled(){
                return this.isAbleUseExtraCredit() && this.getClassAnnouncementData().getMaxScore() == 0;
            },

            function getTitleModel(){
                var title = this.getAnnouncementItem().getTitle();
                return new chlk.models.announcement.AnnouncementTitleViewData(title, this.getType());
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
            },

            [[String]],
            Object, function getAssignedAttributesPostData(){
                var announcementAssignedAttrsJson = this.getAnnouncementAssignedAttrs() || '';
                var attrs = announcementAssignedAttrsJson !== '' ? JSON.parse(announcementAssignedAttrsJson) : [];
                return attrs;
            }
        ]);
});
