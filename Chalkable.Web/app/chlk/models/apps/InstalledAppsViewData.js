REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');

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
            String, 'appUrlAppend',

            Boolean, 'fileCabinetEnabled',
            Boolean, 'standardAttachEnabled',
            Boolean, 'attributesEnabled',
            Boolean, 'showApps',

            [[
                chlk.models.id.AnnouncementId,
                chlk.models.id.ClassId,
                chlk.models.common.PaginatedList,
                String,
                Boolean,
                Boolean,
                chlk.models.id.AppId,
                String
            ]],
            function $(announcementId, classId, apps, appUrlAppend, studyCenterEnabled, canAddStandard, assessmentAppId, announcementTypeName){
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
                if (assessmentAppId)
                    this.setAssessmentAppId(assessmentAppId);
                this.setAnnouncementTypeName(announcementTypeName);
            },

            [[
                chlk.models.id.AnnouncementId
            ]],
            function $createForAdmin(announcementId){
                BASE();
                this.setAnnouncementId(announcementId);
                this.setAttributesEnabled(true);
                this.setFileCabinetEnabled(false);
                this.setStandardAttachEnabled(false);
                this.setShowApps(false);
            }
        ]);
});
