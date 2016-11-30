REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.apps.ApplicationContent');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppRecommendedContentViewData*/
    CLASS(
        'AppRecommendedContentViewData', [

            chlk.models.id.AppId, 'applicationId',
            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',
            chlk.models.id.ClassId, 'classId',
            chlk.models.apps.ApplicationContent, 'Content',

            [[
                chlk.models.id.AppId,
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.ClassId,
                chlk.models.apps.ApplicationContent
            ]],
            function $(appId_, annId_, annType_, classId_, content_){
                BASE();

                if(appId_)
                    this.setApplicationId(appId_);
                if(annId_)
                    this.setAnnouncementId(annId_);
                if(annType_)
                    this.setAnnouncementType(annType_);
                if(classId_)
                    this.setClassId(classId_);
                if(content_)
                    this.setContent(content_);
            }
    ]);
})