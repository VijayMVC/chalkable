REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.grading.ItemGradingStat');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.grading.GradingStudentClassSummaryViewData');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.standard.StandardGradings');

REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.GradeId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradingService*/
    CLASS(
        'GradingService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.AnnouncementId, chlk.models.id.SchoolPersonId, String, String, Boolean, Boolean, Boolean, Boolean, Boolean,
                Boolean, Boolean, String, String]],
            ria.async.Future, function updateItem(announcementId, studentId, gradeValue, comment, dropped, late, absent,
                      incomplete, exempt, passed, complete, standardIds, standardGrades) {
                return this.get('Grading/UpdateItem', chlk.models.announcement.StudentAnnouncement, {
                    announcementId: announcementId && announcementId.valueOf(),
                    studentId: announcementId && studentId.valueOf(),
                    gradeValue: gradeValue,
                    comment: comment,
                    dropped: dropped,
                    late: late,
                    absent: absent,
                    incomplete: incomplete,
                    exempt: exempt,
                    passed: passed,
                    complete: complete,
                    standardIds: standardIds,
                    standardGrades: standardGrades
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function applyAutoGrade(announcementId) {
                return this.get('Grading/ApplyAutoGrade', chlk.models.announcement.StudentAnnouncements, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.SchoolPersonId, chlk.models.id.StandardId,
                chlk.models.id.GradeId, String]],
            ria.async.Future, function updateStandardGrade(classId, gradingPeriodId
            , studentId, standardId, alphaGradeId_, note_) {
                return this.get('Grading/UpdateStandardGrade', chlk.models.announcement.StudentAnnouncements, {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf(),
                    studentId: studentId.valueOf(),
                    standardId: standardId.valueOf(),
                    alphaGradeId: alphaGradeId_ & alphaGradeId_.valueOf(),
                    note: note_
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function applyManualGrade(announcementId) {
                return this.get('Grading/ApplyManualGrade', chlk.models.announcement.StudentAnnouncements, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassSummary(classId) {
                return this.get('Grading/ClassSummary', ArrayOf(chlk.models.grading.GradingClassSummaryItems), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassStandards(classId) {
                return this.get('Grading/ClassStandardSummary', ArrayOf(chlk.models.grading.GradingClassSummaryItems), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassSummaryGrid(classId) {
                return this.get('Grading/ClassSummaryGrid', ArrayOf(chlk.models.grading.GradingClassSummaryGridItems.OF(chlk.models.announcement.ShortAnnouncementViewData)), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassStandardsGrid(classId) {
                return this.get('Grading/ClassStandardGrid', ArrayOf(chlk.models.grading.GradingClassSummaryGridItems.OF(chlk.models.standard.StandardGradings)), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId]],
            ria.async.Future, function getStudentsClassSummary(studentId, classId) {
                return this.get('Grading/StudentClassSummary', chlk.models.grading.GradingStudentClassSummaryViewData, {
                    studentId: studentId.valueOf(),
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getTeacherSummary(teacherId) {
                return this.get('Grading/TeacherSummary', ArrayOf(chlk.models.grading.GradingTeacherClassSummaryViewData), {
                    teacherId: teacherId.valueOf()
                });
            },

            [[chlk.models.id.SchoolPersonId, chlk.models.id.ClassId]],
            ria.async.Future, function getStudentSummary(studentId, classId_) {
                return this.get('Grading/StudentSummary', chlk.models.grading.GradingStudentSummaryViewData, {
                    studentId: studentId.valueOf(),
                    classId: classId_ && classId_.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function getItemGradingStat(announcementId) {
                return this.get('Grading/ItemGradingStat', chlk.models.grading.ItemGradingStat, {
                    announcementId: announcementId.valueOf()
                });
            }
        ])
});