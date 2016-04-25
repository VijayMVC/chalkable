REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.BaseReportViewData*/
    CLASS('BaseReportViewData', [

        chlk.models.id.ClassId, 'classId',
        chlk.models.id.GradingPeriodId, 'gradingPeriodId',
        chlk.models.common.ChlkDate, 'startDate',
        chlk.models.common.ChlkDate, 'endDate',
        ArrayOf(chlk.models.people.ShortUserInfo), 'students',
        Boolean, 'ableDownload',

        Boolean, 'ableToReadSSNumber',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, ArrayOf(chlk.models.people.ShortUserInfo), Boolean, Boolean]],
        function $(classId_, gradingPeriodId_, startDate_, endDate_, students_, ableDownload_, isAbleToReadSSNumber_){
            BASE();
            if(gradingPeriodId_)
                this.setGradingPeriodId(gradingPeriodId_);
            if(classId_)
                this.setClassId(classId_);
            if(startDate_)
                this.setStartDate(startDate_);
            if(endDate_)
                this.setEndDate(endDate_);
            if(students_)
                this.setStudents(students_);
            if(ableDownload_)
                this.setAbleDownload(ableDownload_);
            if(isAbleToReadSSNumber_)
                this.setAbleToReadSSNumber(isAbleToReadSSNumber_);
        }
    ]);
});
