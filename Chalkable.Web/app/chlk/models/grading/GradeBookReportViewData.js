REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.grading', function (){
   "use strict";

    /**@class chlk.models.grading.GradeBookReportViewData*/
    CLASS('GradeBookReportViewData',  [
        chlk.models.id.GradingPeriodId, 'gradingPeriodId',

        chlk.models.id.ClassId, 'classId',

        chlk.models.common.ChlkDate, 'startDate',

        chlk.models.common.ChlkDate, 'endDate',

        ArrayOf(chlk.models.announcement.BaseAnnouncementViewData), 'announcements',

        [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            ArrayOf(chlk.models.announcement.BaseAnnouncementViewData)]],
        function $(gradingPeriodId_, classId_, startDate_, endDate_, announcements_){
            BASE();
            if(gradingPeriodId_)
                this.setGradingPeriodId(gradingPeriodId_);
            if(classId_)
                this.setClassId(classId_);
            if(startDate_)
                this.setStartDate(startDate_);
            if(endDate_)
                this.setEndDate(endDate_);
            if(announcements_)
                this.setAnnouncements(announcements_);
        }
    ]);
});