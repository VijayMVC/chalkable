REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('ria.async.Observable');


NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ReportingService */
    CLASS(
        'ReportingService', EXTENDS(chlk.services.BaseService), [

            Number, 'reportType',

        Number, 'orderBy',

        Number, 'idToPrint',

        Number, 'format',

        Boolean, 'displayLetterGrade',

        Boolean, 'displayTotalPoints',

        Boolean, 'displayStudentAverage',

        Boolean, 'includeWithdrawnStudents',

        Boolean, 'includeNonGradedActivities',

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate,
                Number, Number, Number, Number, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean]],
            String, function submitGradeBookReport(classId, gradingPeriodId, startDate, endDate, reportType, orderBy,
                                                             idToPrint, format, displayLetterGrade_, displayTotalPoints_,
                                                             displayStudentAverage_, includeWithdrawnStudents_, includeNonGradedActivities_,
                                                             suppressStudentName_) {
                return this.getUrl('Reporting/GradeBookReport.json', {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf(),
                    startDate: startDate.toStandardFormat(),
                    endDate: endDate.toStandardFormat(),
                    reportType: reportType,
                    orderBy: orderBy,
                    idToPrint: idToPrint,
                    format: format,
                    displayLetterGrade: displayLetterGrade_,
                    displayTotalPoints: displayTotalPoints_,
                    displayStudentAverage: displayStudentAverage_,
                    includeWithdrawnStudents: includeWithdrawnStudents_,
                    includeNonGradedActivities: includeNonGradedActivities_,
                    suppressStudentName: suppressStudentName_
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate, Number,
                String, String, String, String, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean]],
            String, function submitWorksheetReport(classId, gradingPeriodId, startDate, endDate, idToPrint, announcementIds, title1, title2, title3,
                        title4, title5, printAverage_, printLetterGrade_, printScores_, printStudent_, workingFilter_, appendToExisting_, overwriteExisting_) {
                return this.getUrl('Reporting/WorksheetReport.json', {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf(),
                    startDate: startDate.toStandardFormat(),
                    endDate: endDate.toStandardFormat(),
                    idToPrint: idToPrint,
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
                    overwriteExisting: overwriteExisting_
                });
            }
        ])
});