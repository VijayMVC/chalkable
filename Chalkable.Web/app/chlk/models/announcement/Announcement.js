REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.announcement.LessonPlanViewData');
REQUIRE('chlk.models.announcement.AdminAnnouncementViewData');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.models.announcement.SupplementalAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.Announcement*/
    CLASS(
        UNSAFE, 'Announcement',
        EXTENDS(chlk.models.announcement.BaseAnnouncementViewData),
        IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.lessonPlanData = SJX.fromDeserializable(raw.lessonplandata, chlk.models.announcement.LessonPlanViewData);
                this.adminAnnouncementData = SJX.fromDeserializable(raw.adminannouncementdata, chlk.models.announcement.AdminAnnouncementViewData);
                this.classAnnouncementData = SJX.fromDeserializable(raw.classannouncementdata, chlk.models.announcement.ClassAnnouncementViewData);
                this.supplementalAnnouncementData = SJX.fromDeserializable(raw.supplementalannouncementdata, chlk.models.announcement.SupplementalAnnouncementViewData);
                this.shortContent = SJX.fromValue(raw.shortcontent, String);
                this.complete = SJX.fromValue(raw.complete, Boolean);
                this.attachmentsCount = SJX.fromValue(raw.attachmentscount, Number);
                this.attachmentNames = SJX.fromValue((raw.attachmentnames || []).join('\n'), String);
                this.ownerAttachmentsCount = SJX.fromValue(raw.ownerattachmentscount, Number);
                this.canAddStandard = SJX.fromValue(raw.canaddstandard, Boolean);
                this.studentAnnouncementId = SJX.fromValue(raw.studentannouncementid, chlk.models.id.StudentAnnouncementId);
                this.gradingStudentsCount = SJX.fromValue(raw.gradingstudentscount, Number);
                this.applicationsCount = SJX.fromValue(raw.applicationscount, Number);
                this.applicationName = SJX.fromValue(raw.applicationname, String);
                this.showGradingIcon = SJX.fromValue(raw.showgradingicon, Boolean);
                this.assessmentApplicationId = SJX.fromValue(raw.assessmentapplicationid, chlk.models.id.AppId);
            },

            chlk.models.announcement.LessonPlanViewData, 'lessonPlanData',
            chlk.models.announcement.AdminAnnouncementViewData, 'adminAnnouncementData',
            chlk.models.announcement.ClassAnnouncementViewData, 'classAnnouncementData',
            chlk.models.announcement.SupplementalAnnouncementViewData, 'supplementalAnnouncementData',
            String, 'shortContent',
            String, 'attachmentNames',
            Boolean, 'complete',
            Number, 'attachmentsCount',
            Number, 'ownerAttachmentsCount',
            Boolean, 'canAddStandard',
            chlk.models.id.StudentAnnouncementId, 'studentAnnouncementId',
            Number, 'gradingStudentsCount',
            Number, 'applicationsCount',
            String, 'applicationName',
            Boolean, 'showGradingIcon',
            chlk.models.id.AppId, 'assessmentApplicationId',

            function getAnnouncementItem(){
                var type = this.getType();
                switch(type){
                    case chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT:
                        return this.getClassAnnouncementData();
                    case chlk.models.announcement.AnnouncementTypeEnum.ADMIN:
                        return this.getAdminAnnouncementData();
                    case chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN:
                        return this.getLessonPlanData();
                    case chlk.models.announcement.AnnouncementTypeEnum.SUPPLEMENTAL_ANNOUNCEMENT:
                        return this.getSupplementalAnnouncementData();
                }
            }
    ])
});


































