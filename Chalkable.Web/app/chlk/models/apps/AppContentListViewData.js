REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppRecommendedContentViewData');
REQUIRE('chlk.models.common.PaginatedList');


NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppContentListViewData*/
    CLASS(
        'AppContentListViewData', [

            chlk.models.apps.Application, 'application',

            chlk.models.id.AnnouncementId, 'announcementId',
            chlk.models.announcement.AnnouncementTypeEnum, 'announcementType',
            chlk.models.id.ClassId, 'classId',

            chlk.models.common.PaginatedList, 'appContents',

            [[
                chlk.models.apps.Application,
                chlk.models.id.AnnouncementId,
                chlk.models.announcement.AnnouncementTypeEnum,
                chlk.models.id.ClassId,
                chlk.models.common.PaginatedList
            ]],
            function $(app_, annId_, annType_, classId_, appContents_){
                BASE();
                if(app_)
                    this.setApplication(app_);
                if(annId_)
                    this.setAnnouncementId(annId_);
                if(annType_)
                    this.setAnnouncementType(annType_);
                if(classId_)
                    this.setClassId(classId_);

                if(appContents_)
                    this.setAppContents(appContents_);
            }

        ]);
});