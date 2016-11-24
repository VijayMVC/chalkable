REQUIRE('chlk.models.reports.BaseAdminReportViewData');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.reports.CustomReportTemplate');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.group.Group');


NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ReportCardsStandards*/
    ENUM('ReportCardsStandards', {
        NONE: 0,
        GRADED: 1,
        ALL: 2
    });

    /** @class chlk.models.reports.ReportCardsOrderBy*/
    ENUM('ReportCardsOrderBy',{
        GRADE_LEVEL: 0,
        HOME_ROOM: 1,
        POST_CODE: 2,
        STUDENT_DISPLAY_NAME: 3,
        STUDENT_IDENTIFIER: 4
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

    /** @class chlk.models.reports.ReportCardsAdditionalOptions*/
    ENUM('ReportCardsAdditionalOptions',{
        ANNOUNCEMENT: 0,
        ATTENDANCE:1,
        GRADING_PERIOD_NOTES:2,
        GRADING_SCALE_TRADITIONAL: 3,
        GRADING_SCALE_STANDARDS: 4,
        MERIT_DEMERIT: 5,
        PARENT_SIGNATURE: 6,
        PROMOTION_STATUS: 7,
        WITHDRAWN_STUDENTS: 8,
        YEAR_TO_DATE_INFORMATION: 9,
        COMMENTS: 10
    });

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.reports.SubmitReportCardsViewData*/

    CLASS('SubmitReportCardsViewData', EXTENDS(chlk.models.reports.BaseAdminReportViewData), IMPLEMENTS(ria.serialize.IDeserializable),  [

        chlk.models.id.CustomReportTemplateId, 'customReportTemplateId',
        chlk.models.id.GradingPeriodId, 'gradingPeriodId',
        String, 'attendanceReasonIds',
        String, 'includeOptions',
        chlk.models.reports.ReportCardsLogoType, 'logo',
        chlk.models.reports.ReportCardsRecipientType, 'recipient',
        chlk.models.reports.ReportCardsOrderBy, 'orderBy',
        chlk.models.reports.ReportCardsStandards, 'standardType',
        ArrayOf(chlk.models.reports.CustomReportTemplate), 'templates',
        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',
        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',
        ArrayOf(chlk.models.group.Group), 'groups',
        Boolean, 'ableToReadSSNumber',

        OVERRIDE, VOID, function deserialize(raw) {
            BASE(raw);
            this.customReportTemplateId = SJX.fromValue(raw.customReportTemplateId, chlk.models.id.CustomReportTemplateId);
            this.gradingPeriodId = SJX.fromValue(raw.gradingPeriodId, chlk.models.id.GradingPeriodId);
            this.attendanceReasonIds = SJX.fromValue(raw.attendanceReasonIds, String);
            this.includeOptions = SJX.fromValue(raw.includeOptions, String);
            this.logo = SJX.fromValue(raw.logo, chlk.models.reports.ReportCardsLogoType);
            this.recipient = SJX.fromValue(raw.recipient, chlk.models.reports.ReportCardsRecipientType);
            this.orderBy = SJX.fromValue(raw.orderBy, chlk.models.reports.ReportCardsOrderBy);
            this.standardType = SJX.fromValue(raw.standardType, chlk.models.reports.ReportCardsStandards);
            this.templates = SJX.fromArrayOfDeserializables(raw.templates, chlk.models.reports.CustomReportTemplate);
            this.reasons = SJX.fromArrayOfDeserializables(raw.reasons, chlk.models.attendance.AttendanceReason);
            this.gradingPeriods = SJX.fromArrayOfDeserializables(raw.gradingPeriods, chlk.models.schoolYear.GradingPeriod);
            this.groups = SJX.fromArrayOfDeserializables(raw.groups, chlk.models.group.Group);
            this.ableToReadSSNumber = SJX.fromValue(raw.ableToReadSSNumber, Boolean);
        },

        [[ArrayOf(chlk.models.reports.CustomReportTemplate), ArrayOf(chlk.models.schoolYear.GradingPeriod)
            , ArrayOf(chlk.models.attendance.AttendanceReason), Boolean, Boolean]],
        function $(templates_, gradingPeriods_, reasons_, ableDownload_, isAbleToReadSSNumber_, defaultIdToPrint_){
            BASE(ableDownload_, defaultIdToPrint_);
            if(templates_)
                this.setTemplates(templates_);
            if(reasons_)
                this.setReasons(reasons_);
            if(gradingPeriods_)
                this.setGradingPeriods(gradingPeriods_);
            if(isAbleToReadSSNumber_)
                this.setAbleToReadSSNumber(isAbleToReadSSNumber_);
            else this.setIdToPrint(chlk.models.reports.StudentIdentifierEnum.NONE);
        }
    ]);
});
