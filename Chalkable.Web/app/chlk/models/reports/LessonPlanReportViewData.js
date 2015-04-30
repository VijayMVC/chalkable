REQUIRE('chlk.models.announcement.ClassAnnouncementType');
REQUIRE('chlk.models.announcement.AnnouncementAttributeViewData');

NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.LessonPlanReportViewData*/
    CLASS('LessonPlanReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [
        ArrayOf(chlk.models.announcement.ClassAnnouncementType), 'activityCategories',

        Array, 'activityAttributes',

        [[ArrayOf(chlk.models.announcement.ClassAnnouncementType), ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData),
            chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        function $(activityCategories_, activityAttributes_, classId_, gradingPeriodId_, startDate_, endDate_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_);
            if(activityCategories_)
                this.setActivityCategories(activityCategories_);
            if(activityAttributes_)
                this.setActivityAttributes(activityAttributes_);
        }
    ]);
});