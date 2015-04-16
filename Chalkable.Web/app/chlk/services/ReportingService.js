REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('ria.async.Observable');
REQUIRE('chlk.models.reports.SubmitComprehensiveProgressViewData');
REQUIRE('chlk.models.reports.SubmitMissingAssignmentsReportViewData');
REQUIRE('chlk.models.reports.SubmitBirthdayReportViewData');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ReportingService */
    CLASS(
        'ReportingService', EXTENDS(chlk.services.BaseService), [

        Number, 'reportType',

        chlk.models.reports.OrderByEnum, 'orderBy',

        chlk.models.reports.StudentIdentifierEnum, 'idToPrint',

        chlk.models.reports.ReportFormatEnum, 'format',

        Boolean, 'displayLetterGrade',

        Boolean, 'displayTotalPoints',

        Boolean, 'displayStudentAverage',

        Boolean, 'includeWithdrawnStudents',

        Boolean, 'includeNonGradedActivities',

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            Number, chlk.models.reports.OrderByEnum, chlk.models.reports.StudentIdentifierEnum,
            chlk.models.reports.ReportFormatEnum, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, String]],
        String, function submitGradeBookReport(classId, gradingPeriodId, startDate, endDate, reportType, orderBy,
                                                         idToPrint, format, displayLetterGrade_, displayTotalPoints_,
                                                         displayStudentAverage_, includeWithdrawnStudents_, includeNonGradedActivities_,
                                                         suppressStudentName_, studentIds_) {
            return this.getUrl('Reporting/GradeBookReport.json', {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                startDate: startDate.toStandardFormat(),
                endDate: endDate.toStandardFormat(),
                reportType: reportType,
                orderBy: orderBy.valueOf(),
                idToPrint: idToPrint.valueOf(),
                format: format.valueOf(),
                displayLetterGrade: displayLetterGrade_,
                displayTotalPoints: displayTotalPoints_,
                displayStudentAverage: displayStudentAverage_,
                includeWithdrawnStudents: includeWithdrawnStudents_,
                includeNonGradedActivities: includeNonGradedActivities_,
                suppressStudentName: suppressStudentName_,
                studentIds: studentIds_
            });
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.BirthdayGroupByMethod, chlk.models.reports.ReportFormatEnum,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Number, Number, Number, Boolean, Boolean, Boolean, Boolean]],
        String, function submitBirthdayReport(classId, gradingPeriodId, groupBy, format, startDate_, endDate_, startMonth_, endMonth_,
                                                    appendOrOverwrite_, includeWithdrawn_, includePhoto_, saveToFilter_, saveAsDefault_) {
            return this.getUrl('Reporting/BirthdayReport.json', {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                groupBy: groupBy.valueOf(),
                format: format.valueOf(),
                startDate: startDate_ && startDate_.toStandardFormat(),
                endDate: endDate_ && endDate_.toStandardFormat(),
                startMonth: startMonth_,
                endMonth: endMonth_,
                appendOrOverwrite: appendOrOverwrite_,
                includeWithdrawn: includeWithdrawn_,
                includePhoto: includePhoto_,
                saveToFilter: saveToFilter_,
                saveAsDefault: saveAsDefault_
            });
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, Boolean]],
        String, function submitSeatingChartReport(classId, gradingPeriodId, displayStudentPhoto_) {
            return this.getUrl('Reporting/SeatingChartReport.json', {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                displayStudentPhoto: displayStudentPhoto_
            });
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.reports.StudentIdentifierEnum,
            String, String, String, String, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, String]],
        String, function submitWorksheetReport(classId, gradingPeriodId, startDate, endDate, idToPrint, announcementIds, title1, title2, title3,
                    title4, title5, printAverage_, printLetterGrade_, printScores_, printStudent_, workingFilter_, appendToExisting_, overwriteExisting_, studentIds_) {
            return this.getUrl('Reporting/WorksheetReport.json', {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                startDate: startDate.toStandardFormat(),
                endDate: endDate.toStandardFormat(),
                idToPrint: idToPrint.valueOf(),
                announcementIds: announcementIds,
                title1: title1,
                title2: title2,
                title3: title3,
                title4: title4,
                title5: title5,
                printAverage: printAverage_,
                printLetterGrade: printLetterGrade_,
                printScores: printScores_,
                printStudent: printStudent_,
                workingFilter: workingFilter_,
                appendToExisting: appendToExisting_,
                overwriteExisting: overwriteExisting_,
                studentIds: studentIds_
            });
        },

        [[chlk.models.id.ClassId, chlk.models.reports.StudentIdentifierEnum, chlk.models.reports.ReportFormatEnum, chlk.models.id.GradingPeriodId,
            String, Boolean, chlk.models.reports.AttendanceDisplayMethodEnum, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean,
            Number, Number, Number, Number, Boolean, String, String, String]],
        String, function submitProgressReport(classId, idToPrint, format, gradingPeriodId, absenceReasonIds, additionalMailings_, dailyAttendanceDisplayMethod,
                displayCategoryAverages_, displayClassAverages_, displayLetterGrade_, displayPeriodAttendance_, displaySignatureLine_, displayStudentComments_,
                displayStudentMailingAddress_, displayTotalPoints_, goGreen_, maxCategoryClassAverage_, maxStandardAverage_, minCategoryClassAverage_,
                minStandardAverage_, printFromHomePortal_, classComment, studentIds, commentsList) {
            return this.getUrl('Reporting/ProgressReport.json', {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                idToPrint: idToPrint.valueOf(),
                format: format.valueOf(),
                absenceReasonIds: absenceReasonIds,
                additionalMailings: additionalMailings_,
                dailyAttendanceDisplayMethod: dailyAttendanceDisplayMethod.valueOf(),
                displayCategoryAverages: displayCategoryAverages_,
                displayClassAverages: displayClassAverages_,
                displayLetterGrade: displayLetterGrade_,
                displayPeriodAttendance: displayPeriodAttendance_,
                displaySignatureLine: displaySignatureLine_,
                displayStudentComments: displayStudentComments_,
                displayStudentMailingAddress: displayStudentMailingAddress_,
                displayTotalPoints: displayTotalPoints_,
                goGreen: goGreen_,
                maxCategoryClassAverage: maxCategoryClassAverage_,
                maxStandardAverage: maxStandardAverage_,
                minCategoryClassAverage: minCategoryClassAverage_,
                minStandardAverage: minStandardAverage_,
                printFromHomePortal: printFromHomePortal_,
                classComment: classComment,
                studentIds: studentIds,
                studentComments: commentsList
                //comments:
            });
        },


        [[chlk.models.id.ClassId, chlk.models.reports.StudentIdentifierEnum, chlk.models.reports.ReportFormatEnum, ArrayOf(chlk.models.id.GradingPeriodId),
            ArrayOf(chlk.models.id.AttendanceReasonId), chlk.models.reports.ComprehensiveProgressOrderByMethod, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            Number, Number, Boolean, Boolean, Boolean, Boolean, chlk.models.reports.AttendanceDisplayMethodEnum, Boolean, Boolean,
            Boolean,  Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, chlk.models.id.SchoolPersonId, String]],

        String, function submitComprehensiveProgressReport(classId, idToPrint, format, gradingPeriodIds, absenceReasonIds, orderBy,  startDate_, endDate_,
               maxStandardAverage_, minStandardAverage_, additionalMailings_, classAverageOnly_, displayCategoryAverages_, displayClassAverage_,
               dailyAttendanceDisplayMethod, displayPeriodAttendance_, displaySignatureLine_, displayStudentComments_, displayStudentMailingAddress_,
               displayTotalPoints_, includePicture_, includeWithdrawn_, windowEnvelope_, goGreen_, studentFilterId_, studentIds_){
            return this.getUrl('Reporting/ComprehensiveProgressReport.json', {
                classId : classId.valueOf(),
                idToPrint: idToPrint.valueOf(),
                format: format.valueOf(),
                gradingPeriodIds : this.arrayToCsv(gradingPeriodIds),
                absenceReasonIds: this.arrayToCsv(absenceReasonIds),
                startDate: startDate_ && startDate_.toStandardFormat(),
                endDate: endDate_ && endDate_.toStandardFormat(),
                maxStandardAverage: maxStandardAverage_,
                minStandardAverage: minStandardAverage_,
                additionalMailings: additionalMailings_,
                classAverageOnly: classAverageOnly_,
                dailyAttendanceDisplayMethod: dailyAttendanceDisplayMethod.valueOf(),
                displayCategoryAverages: displayCategoryAverages_,
                displayClassAverage: displayClassAverage_,
                displayPeriodAttendance: displayPeriodAttendance_,
                displaySignatureLine: displaySignatureLine_,
                displayStudentComment: displayStudentComments_,
                displayStudentMailingAddress: displayStudentMailingAddress_,
                displayTotalPoints: displayTotalPoints_,
                includePicture: includePicture_,
                includeWithdrawn: includeWithdrawn_,
                goGreen: goGreen_,
                windowEnvelope: windowEnvelope_,
                studentFilterId_: studentFilterId_,
                orderBy: orderBy.valueOf(),
                studentIds: studentIds_
            });
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.StudentIdentifierEnum,
            chlk.models.reports.ReportFormatEnum, chlk.models.reports.MissingAssignmentsOrderByMethod, chlk.models.common.ChlkDate,
            chlk.models.common.ChlkDate, ArrayOf(chlk.models.id.AlternateScoreId), Boolean, Boolean, Boolean, Boolean, Boolean, String]],
        String, function submitMissingAssignmentsReport(classId, gradingPeriodId, idToPrint, format, orderBy, startDate, endDate,
                alternateScoreIds_, alternateScoresOnly_, considerZerosAsMissingGrades_, includeWithdrawn_, onePerPage_,
                suppressStudentName_, studentIds_){
            return this.getUrl('Reporting/MissingAssignmentsReport.json',{
                classId: classId.valueOf(),
                gradingPeriodId : gradingPeriodId.valueOf(),
                idToPrint: idToPrint.valueOf(),
                format: format.valueOf(),
                orderBy: orderBy.valueOf(),
                startDate: startDate.toStandardFormat(),
                endDate: endDate.toStandardFormat(),
                alternateScoreIds: this.arrayToCsv(alternateScoreIds_),
                alternateScoresOnly: alternateScoresOnly_,
                considerZerosAsMissingGrades: considerZerosAsMissingGrades_,
                includeWithdrawn: includeWithdrawn_,
                onePerPage: onePerPage_,
                suppressStudentName: suppressStudentName_,
                studentIds: studentIds_
            });
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
        ria.async.Future, function getStudentReportComments(classId, gradingPeriodId) {
            return this.get('Reporting/GetStudentProgressReportComments.json', ArrayOf(chlk.models.reports.UserForReport), {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                start: 0,
                count: 9999
            });
        }
    ])
});