REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.common.BaseAttachViewData');

NAMESPACE('chlk.models.apps', function () {

    "use strict";
    /** @class chlk.models.apps.InstalledAppsViewData*/
    CLASS(
        'InstalledAppsViewData', EXTENDS(chlk.models.common.BaseAttachViewData), [

            chlk.models.common.PaginatedList, 'apps',

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.id.ClassId,
                chlk.models.common.PaginatedList,
                String,
                Boolean,
                Boolean,
                chlk.models.id.AppId,
                String,
                chlk.models.announcement.AnnouncementTypeEnum
            ]],
            function $(announcementId, classId, apps, appUrlAppend, studyCenterEnabled,
                       canAddStandard, assessmentAppId, announcementTypeName, announcementType){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setApps(apps);
                this.setClassId(classId);
                this.setAppUrlAppend(appUrlAppend);
                this.setFileCabinetEnabled(true);
                this.setStandardAttachEnabled(canAddStandard);
                this.setShowApps(studyCenterEnabled);
                if (studyCenterEnabled && assessmentAppId)
                    this.setAssessmentAppId(assessmentAppId);
                this.setAnnouncementTypeName(announcementTypeName);
                this.setAnnouncementType(announcementType);

            }
        ]);
});
