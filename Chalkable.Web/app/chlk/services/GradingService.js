REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.grading.ItemGradingStat');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.grading.GradingStudentClassSummaryViewData');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradingService*/
    CLASS(
        'GradingService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.StudentAnnouncementId, String, String, Boolean, Boolean, Boolean, Boolean, Boolean,
                Boolean, Boolean, String, String]],
            ria.async.Future, function updateItem(studentAnnouncementId, gradeValue, comment, dropped, late, absent,
                      incomplete, exempt, passed, complete, standardIds, standardGrades) {
                return this.get('Grading/UpdateItem', chlk.models.announcement.StudentAnnouncement, {
                    studentAnnouncementId: studentAnnouncementId.valueOf(),
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
            ria.async.Future, function getClassSummaryGrid(classId) {
                return this.get('Grading/ClassSummaryGrid', ArrayOf(chlk.models.grading.GradingClassSummaryGridItems), {
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