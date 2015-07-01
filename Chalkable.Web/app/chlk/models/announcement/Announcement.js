REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.announcement.LessonPlanViewData');
REQUIRE('chlk.models.announcement.AdminAnnouncementViewData');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');

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
                this.shortContent = SJX.fromValue(raw.shortcontent, String);
                this.complete = SJX.fromValue(raw.complete, Boolean);
                this.attachmentsCount = SJX.fromValue(raw.attachmentscount, Number);
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
            String, 'shortContent',
            Boolean, 'complete',
            Number, 'attachmentsCount',
            Number, 'ownerAttachmentsCount',
            Boolean, 'canAddStandard',
            chlk.models.id.StudentAnnouncementId, 'studentAnnouncementId',
            Number, 'gradingStudentsCount',
            Number, 'applicationsCount',
            String, 'applicationName',
            Boolean, 'showGradingIcon',
            chlk.models.id.AppId, 'assessmentApplicationId'
    ])
});


































