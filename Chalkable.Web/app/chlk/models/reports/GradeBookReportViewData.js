REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.reports', function (){
   "use strict";

    /**@class chlk.models.reports.GradeBookReportViewData*/
    CLASS('GradeBookReportViewData',  EXTENDS(chlk.models.reports.BaseReportViewData), [

        ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), 'announcements',

        [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            ArrayOf(chlk.models.people.ShortUserInfo), ArrayOf(chlk.models.announcement.ClassAnnouncementViewData), Boolean, Boolean]],
        function $(gradingPeriodId_, classId_, startDate_, endDate_, students_, announcements_, ableDownload_, isAbleToReadSSNumber_){
            BASE(classId_, gradingPeriodId_, startDate_, endDate_, students_, ableDownload_, isAbleToReadSSNumber_);
            if(announcements_)
                this.setAnnouncements(announcements_);
        }
    ]);
});