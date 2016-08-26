REQUIRE('chlk.models.reports.BaseSubmitReportViewData');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.reports.CustomReportTemplate');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ReportCardsOrderBy*/
    ENUM('ReportCardsOrderBy',{
        GRADE_LEVEL: 0,
        HOME_ROOM: 1,
        POST_CODE: 2,
        STUDENT_DISPLAY_NAME: 3,
        STUDNET_IDENTIFIER: 4
    });

    /** @class chlk.models.reports.ReportCardsLogoType*/
    ENUM('ReportCardsLogoType',{
        NONE:0,
        DISTRICT: 1,
        SCHOOL: 2
    });

    /** @class chlk.models.reports.ReportCardsRecipientType*/
    ENUM('ReportCardsRecipientType',{
        STUDENTS: 0,
        CUSTODIANS: 1,
        MAILING_CONTACTS: 2
    });

    /** @class chlk.models.reports.ReportCardsAddionalOptions*/
    ENUM('ReportCardsAddionalOptions',{
        ANNOUNCEMENT: 0,
        ATTENDANCE:1,
        GRADING_PERIOD_NOTES:2,
        GRADING_SCALE_TRADITIONAL: 3,
        GRADING_SCALE_STANDARDS: 4,
        MERIT_DEMERIT: 5,
        PARENT_SIGNATURE: 6,
        PROMOTION_STATUS: 7,
        WITHDRAWN_STUDENTS: 8,
        YEAR_TO_DATE_INFORMATION: 9
    });

    /** @class chlk.models.reports.SubmitReportCardsViewData*/

    CLASS('SubmitReportCardsViewData',  [

        chlk.models.id.CustomReportTemplateId, 'customReportTemplateId',

        String, 'title',

        String, 'gradingPeriodIds',

        String, 'absenceReasonIds',

        String, 'groupIds',

        chlk.models.reports.ReportCardsLogoType, 'logo',

        chlk.models.reports.ReportCardsRecipientType, 'recipient',

        chlk.models.reports.ReportCardsOrderBy, 'orderBy',

        chlk.models.reports.StudentIdentifierEnum, 'idToPrint',

        Boolean, 'includeGradedStandardsOnly',

        ArrayOf(chlk.models.reports.CustomReportTemplate), 'templates',

        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',

        Boolean, 'ableDownload',

        Boolean, 'ableToReadSSNumber',

        [[ArrayOf(chlk.models.reports.CustomReportTemplate), ArrayOf(chlk.models.schoolYear.GradingPeriod)
            , ArrayOf(chlk.models.attendance.AttendanceReason), Boolean, Boolean]],
        function $(templates_, gradingPeriods_, reasons_, ableDownload_, isAbleToReadSSNumber_){
            BASE();
            if(templates_)
                this.setTemplates(templates_);
            if(reasons_)
                this.setReasons(reasons_);
            if(gradingPeriods_)
                this.setGradingPeriods(gradingPeriods_);
            if(ableDownload_)
                this.setAbleDownload(ableDownload_);
            if(isAbleToReadSSNumber_)
                this.setAbleToReadSSNumber(isAbleToReadSSNumber_);
        }
    ]);
});
