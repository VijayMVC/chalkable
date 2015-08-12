REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementAssignedAttributeId');

NAMESPACE('chlk.models.apps', function () {

    "use strict";
    /** @class chlk.models.apps.InstalledAppsViewData*/
    CLASS(
        'InstalledAppsViewData', [

            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.id.ClassId, 'classId',
            chlk.models.id.AppId, 'assessmentAppId',
            String, 'announcementTypeName',
            chlk.models.common.PaginatedList, 'apps',
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',
            String, 'appUrlAppend',

            Boolean, 'fileCabinetEnabled',
            Boolean, 'standardAttachEnabled',
            Boolean, 'attributesEnabled',
            Boolean, 'showApps',

            chlk.models.id.AnnouncementAssignedAttributeId, 'assignedAttributeId',

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.id.ClassId,
                chlk.models.common.PaginatedList,
                String,
                Boolean,
                Boolean,
                chlk.models.id.AppId,
                String,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.AnnouncementAssignedAttributeId
            ]],
            function $(announcementId, classId, apps, appUrlAppend, studyCenterEnabled,
                       canAddStandard, assessmentAppId, announcementTypeName, announcementType){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setApps(apps);
                this.setClassId(classId);
                this.setAppUrlAppend(appUrlAppend);
                this.setFileCabinetEnabled(true);
                this.setAttributesEnabled(true);
                this.setStandardAttachEnabled(canAddStandard);
                var hasApps = (apps.getItems() || []).length > 0 && studyCenterEnabled;
                this.setShowApps(hasApps);
                if (studyCenterEnabled && assessmentAppId)
                    this.setAssessmentAppId(assessmentAppId);
                this.setAnnouncementTypeName(announcementTypeName);
                this.setAnnouncementType(announcementType);

            },

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.AnnouncementAssignedAttributeId
            ]],
            function $createForAttribute(announcementId, announcementType, assignedAttributeId_){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setAnnouncementType(announcementType);
                this.setAttributesEnabled(false);
                this.setFileCabinetEnabled(true);
                this.setStandardAttachEnabled(false);
                this.setShowApps(false);
                if(assignedAttributeId_)
                    this.setAssignedAttributeId(assignedAttributeId_);
            },

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.AppId
            ]],
            function $createForAdmin(announcementId, announcementType, assessmentAppId_){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setAttributesEnabled(true);
                this.setFileCabinetEnabled(false);
                this.setStandardAttachEnabled(false);
                this.setShowApps(false);
                this.setAnnouncementType(announcementType);
                if (assessmentAppId_)
                    this.setAssessmentAppId(assessmentAppId_);
            }
        ]);
});
