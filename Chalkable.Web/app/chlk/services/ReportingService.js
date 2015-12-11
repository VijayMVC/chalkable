REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('ria.async.Observable');
REQUIRE('chlk.models.reports.SubmitComprehensiveProgressViewData');
REQUIRE('chlk.models.reports.SubmitMissingAssignmentsReportViewData');
REQUIRE('chlk.models.reports.SubmitBirthdayReportViewData');
REQUIRE('chlk.models.reports.SubmitGradeVerificationReportViewData');
REQUIRE('chlk.models.reports.SubmitAttendanceProfileReportViewData');
REQUIRE('chlk.models.reports.SubmitAttendanceRegisterReportViewData');

REQUIRE('chlk.lib.ajax.IframeGetTask');

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

        [[String, Object, Object, Boolean]],
        ria.async.Future, function getWithIframe_(uri, gParams_) {
            return new chlk.lib.ajax.IframeGetTask(uri)
                .params(gParams_ || {})
                .disableCache()
                .checkReadyCookie('chlk-iframe-ready')
                .run()
                .then(this.getResponseProcessor_(null));
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            Number, chlk.models.reports.OrderByEnum, chlk.models.reports.StudentIdentifierEnum,
            chlk.models.reports.ReportFormatEnum, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, String]],
        ria.async.Future, function submitGradeBookReport(classId, gradingPeriodId, startDate, endDate, reportType, orderBy,
                                               idToPrint, format, displayLetterGrade_, displayTotalPoints_,
                                               displayStudentAverage_, includeWithdrawnStudents_, includeNonGradedActivities_,
                                               suppressStudentName_, studentIds_) {

            var url = this.getUrl('Reporting/GradeBookReport.json', {
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

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.BirthdayGroupByMethod, chlk.models.reports.ReportFormatEnum,
            chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Number, Number, Number, Boolean, Boolean, Boolean, Boolean]],
        ria.async.Future, function submitBirthdayReport(classId, gradingPeriodId, groupBy, format, startDate_, endDate_, startMonth_, endMonth_,
                                              appendOrOverwrite_, includeWithdrawn_, includePhoto_, saveToFilter_, saveAsDefault_) {

            var url = this.getUrl('Reporting/BirthdayReport.json', {
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

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.ReportFormatEnum, Boolean]],
        ria.async.Future, function submitSeatingChartReport(classId, gradingPeriodId, format, displayStudentPhoto_) {
            var url = this.getUrl('Reporting/SeatingChartReport.json', {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                format: format.valueOf(),
                displayStudentPhoto: displayStudentPhoto_
            });

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, chlk.models.reports.StudentIdentifierEnum,
            String, String, String, String, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, String]],
        ria.async.Future, function submitWorksheetReport(classId, gradingPeriodId, startDate, endDate, idToPrint, announcementIds, title1, title2, title3,
                    title4, title5, printAverage_, printLetterGrade_, printScores_, printStudent_, workingFilter_, appendToExisting_, overwriteExisting_, studentIds_) {
            var url = this.getUrl('Reporting/WorksheetReport.json', {
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

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.reports.StudentIdentifierEnum, chlk.models.reports.ReportFormatEnum, chlk.models.id.GradingPeriodId,
            String, Boolean, chlk.models.reports.AttendanceDisplayMethodEnum, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean,
            Number, Number, Number, Number, Boolean, String, String, String]],
        ria.async.Future, function submitProgressReport(classId, idToPrint, format, gradingPeriodId, absenceReasonIds, additionalMailings_, dailyAttendanceDisplayMethod,
                displayCategoryAverages_, displayClassAverages_, displayLetterGrade_, displayPeriodAttendance_, displaySignatureLine_, displayStudentComments_,
                displayStudentMailingAddress_, displayTotalPoints_, goGreen_, maxCategoryClassAverage_, maxStandardAverage_, minCategoryClassAverage_,
                minStandardAverage_, printFromHomePortal_, classComment, studentIds) {

            var url = this.getUrl('Reporting/ProgressReport.json', {
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
                studentIds: studentIds
                //comments:
            });

            return this.getWithIframe_(url);
        },


        [[chlk.models.id.ClassId, chlk.models.reports.StudentIdentifierEnum, chlk.models.reports.ReportFormatEnum, ArrayOf(chlk.models.id.GradingPeriodId),
            ArrayOf(chlk.models.id.AttendanceReasonId), chlk.models.reports.ComprehensiveProgressOrderByMethod, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            Number, Number, Boolean, Boolean, Boolean, Boolean, chlk.models.reports.AttendanceDisplayMethodEnum, Boolean, Boolean,
            Boolean,  Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, chlk.models.id.SchoolPersonId, String]],

        ria.async.Future, function submitComprehensiveProgressReport(classId, idToPrint, format, gradingPeriodIds, absenceReasonIds, orderBy,  startDate_, endDate_,
               maxStandardAverage_, minStandardAverage_, additionalMailings_, classAverageOnly_, displayCategoryAverages_, displayClassAverage_,
               dailyAttendanceDisplayMethod, displayPeriodAttendance_, displaySignatureLine_, displayStudentComments_, displayStudentMailingAddress_,
               displayTotalPoints_, includePicture_, includeWithdrawn_, windowEnvelope_, goGreen_, studentFilterId_, studentIds_){

            var url = this.getUrl('Reporting/ComprehensiveProgressReport.json', {
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

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.reports.ReportFormatEnum, ArrayOf(chlk.models.id.GradingPeriodId),
            Array, chlk.models.reports.GradeType, chlk.models.reports.StudentOrder,
            chlk.models.reports.StudentIdentifierEnum, Boolean, Boolean, Boolean, String]],

        ria.async.Future, function submitGradeVerificationReport(classId, format, gradingPeriodIds, studentAverageIds, gradeType, studentOrder,
                numberToDisplay, includeCommentsAndLegends_, includeSignature_, includeWithdrawn_, studentIds_){

            var url = this.getUrl('Reporting/GradeVerificationReport.json', {
                classId : classId.valueOf(),
                format: format.valueOf(),
                gradingPeriodIds : this.arrayToCsv(gradingPeriodIds),
                gradeditemid: this.arrayToCsv(studentAverageIds),
                gradeType: gradeType.valueOf(),
                studentOrder: studentOrder.valueOf(),
                idToPrint: numberToDisplay.valueOf(),
                includeCommentsAndLegend: includeCommentsAndLegends_,
                includeSignature: includeSignature_,
                includeWithdrawn: includeWithdrawn_,
                studentIds: studentIds_
            });

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.ReportFormatEnum, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
            chlk.models.reports.GroupBy, chlk.models.reports.StudentIdentifierEnum, ArrayOf(chlk.models.id.AttendanceReasonId),
            ArrayOf(chlk.models.id.MarkingPeriodId), Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, String]],

        ria.async.Future, function submitAttendanceProfileReport(classId, gradingPeriodId, format, startDate, endDate, groupBy, idToPrint, absenceReasons, terms, displayPeriodAbsences_,
                                                       displayReasonTotals_, includeCheck_, includeUnlisted_, displayNote_, displayWithdrawnStudents_, studentIds_){

            var url = this.getUrl('Reporting/AttendanceProfileReport.json', {
                classId : classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                format: format.valueOf(),
                startDate: startDate.toStandardFormat(),
                endDate: endDate.toStandardFormat(),
                studentIds: studentIds_,
                groupBy: groupBy.valueOf(),
                idToPrint: idToPrint.valueOf(),
                absenceReasons : this.arrayToCsv(absenceReasons),
                markingPeriodIds: this.arrayToCsv(terms),
                displayPeriodAbsences: displayPeriodAbsences_,
                displayReasonTotals: displayReasonTotals_,
                includeCheckInCheckOut: includeCheck_,
                includeUnlisted: includeUnlisted_,
                displayNote: displayNote_,
                displayWithdrawnStudents: displayWithdrawnStudents_
            });

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.ReportFormatEnum,
            chlk.models.reports.StudentIdentifierEnum, chlk.models.reports.ReportType,
            ArrayOf(chlk.models.id.AttendanceReasonId), Number, Boolean, Boolean]],
        ria.async.Future, function submitAttendanceRegisterReport(classId, gradingPeriodId, format,  idToPrint, reportType,
                                                        absenceReasons, monthId, showLocalReasonCode_, includeTardies_){

            var url = this.getUrl('Reporting/AttendanceRegisterReport.json', {
                classId : classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                format: format.valueOf(),
                reportType: reportType.valueOf(),
                idToPrint: idToPrint.valueOf(),
                absenceReasonIds : this.arrayToCsv(absenceReasons),
                monthId: monthId,
                showLocalReasonCode: showLocalReasonCode_,
                includeTardies: includeTardies_
            });

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.reports.StudentIdentifierEnum,
            chlk.models.reports.ReportFormatEnum, chlk.models.reports.MissingAssignmentsOrderByMethod, chlk.models.common.ChlkDate,
            chlk.models.common.ChlkDate, ArrayOf(chlk.models.id.AlternateScoreId), Boolean, Boolean, Boolean, Boolean, Boolean, String]],
        ria.async.Future, function submitMissingAssignmentsReport(classId, gradingPeriodId, idToPrint, format, orderBy, startDate, endDate,
                alternateScoreIds_, alternateScoresOnly_, considerZerosAsMissingGrades_, includeWithdrawn_, onePerPage_,
                suppressStudentName_, studentIds_) {

            var url = this.getUrl('Reporting/MissingAssignmentsReport.json',{
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

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.GradingPeriodId, chlk.models.id.SchoolPersonId]],
        ria.async.Future, function submitStudentComprehensiveProgressReport(gradingPeriodId, studentId) {
            var url = this.getUrl('Reporting/StudentComprehensiveProgressReport.json', {
                gradingPeriodId: gradingPeriodId.valueOf(),
                studentId: studentId.valueOf()
            });

            return this.getWithIframe_(url);
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
        ria.async.Future, function getStudentReportComments(classId, gradingPeriodId) {
            return this.get('Reporting/GetStudentProgressReportComments.json', ArrayOf(chlk.models.reports.UserForReport), {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                start: 0,
                count: 9999
            });
        },

        [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, String]],
        ria.async.Future, function setStudentProgressReportComments(classId, gradingPeriodId, studentComments) {
            return this.post('Reporting/SetStudentProgressReportComments.json', Boolean, {
                classId: classId.valueOf(),
                gradingPeriodId: gradingPeriodId.valueOf(),
                studentComments: studentComments
            });
        },

        ria.async.Future, function getFeedReportSettings(classId_) {
            return this.get('Reporting/FeedReportSettings.json', chlk.models.feed.FeedPrintingViewData, {
                classId: classId_ && classId_.valueOf()
            });
        },

        [[chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Boolean, Boolean, Boolean, Boolean, Boolean, chlk.models.id.ClassId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
        ria.async.Future, function submitFeedReport(startDate, endDate, lessonPlanOnly_, includeAttachments_, includeDetails_,
                                                  includeHiddenAttributes_, includeHiddenActivities_, classId_, importantOnly_, announcementType_) {

            var url = this.getUrl('Reporting/FeedReport.json', {
                startDate: startDate.toStandardFormat(),
                endDate: endDate.toStandardFormat(),
                lessonPlanOnly: lessonPlanOnly_,
                includeAttachments: includeAttachments_,
                includeDetails: includeDetails_,
                includeHiddenAttributes: includeHiddenAttributes_,
                includeHiddenActivities: includeHiddenActivities_,
                classId: classId_ && classId_.valueOf(),
                complete: importantOnly_ ? false : null,
                announcementType: announcementType_ && announcementType_.valueOf()
            });

            return this.getWithIframe_(url);
        }
    ])
});
