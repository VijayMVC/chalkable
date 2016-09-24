REQUIRE('chlk.models.reports.BaseSubmitReportViewData');
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
        YEAR_TO_DATE_INFORMATION: 9
    });

    /** @class chlk.models.reports.SubmitReportCardsViewData*/

    CLASS('SubmitReportCardsViewData',  [

        chlk.models.id.CustomReportTemplateId, 'customReportTemplateId',
        String, 'title',
        chlk.models.id.GradingPeriodId, 'gradingPeriodId',
        String, 'attendanceReasonIds',
        String, 'groupIds',
        String, 'studentIds',
        String, 'submitType',
        String, 'includeOptions',
        chlk.models.reports.ReportCardsLogoType, 'logo',
        chlk.models.reports.ReportCardsRecipientType, 'recipient',
        chlk.models.reports.ReportCardsOrderBy, 'orderBy',
        chlk.models.reports.StudentIdentifierEnum, 'idToPrint',
        chlk.models.reports.ReportCardsStandards, 'standardType',
        ArrayOf(chlk.models.reports.CustomReportTemplate), 'templates',
        ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',
        ArrayOf(chlk.models.schoolYear.GradingPeriod), 'gradingPeriods',
        ArrayOf(chlk.models.group.Group), 'groups',
        Boolean, 'ableDownload',
        Boolean, 'ableToReadSSNumber',
        String, 'selectedItems',
        String, 'reportRecipient',
        
        function getParsedSelected(){
            var selectedItems = this.selectedItems ? JSON.parse(this.selectedItems ) : {groups:[], students:[]};

            selectedItems.groups = selectedItems.groups.map(function(group){
                return new chlk.models.group.Group(group.name, new chlk.models.id.GroupId(group.id));
            });

            selectedItems.students = selectedItems.students.map(function(student){
                return new chlk.models.people.ShortUserInfo(null, null, new chlk.models.id.SchoolPersonId(student.id), student.displayname, student.gender);
            });
            
            return selectedItems;
        },

        /*VOID, function deserialize(raw) {
            this.customReportTemplateId = SJX.fromValue(raw.customReportTemplateId, chlk.models.id.CustomReportTemplateId);
            this.title = SJX.fromValue(raw.title, String);
            this.gradingPeriodId = SJX.fromValue(raw.gradingPeriodId, chlk.models.id.GradingPeriodId);
            this.attendanceReasonIds = SJX.fromValue(raw.attendanceReasonIds, String);
            this.groupIds = SJX.fromValue(raw.groupIds, String);
            this.studentIds = SJX.fromValue(raw.studentIds, String);
            this.submitType = SJX.fromValue(raw.submitType, String);
            this.includeOptions = SJX.fromValue(raw.includeOptions, String);
            this.groupId = SJX.fromValue(raw.groupId, chlk.models.id.GroupId);
            if(raw.selectedItems)
                this.selectedItems = JSON.parse(SJX.fromValue(raw.selectedItems, String));
        },*/

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
