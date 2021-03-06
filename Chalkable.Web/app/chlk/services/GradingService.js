REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.grading.GradingClassSummaryItems');
REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');
REQUIRE('chlk.models.grading.ItemGradingStat');
REQUIRE('chlk.models.announcement.StudentAnnouncements');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.standard.StandardGradings');
REQUIRE('chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData');
REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.models.grading.FinalGradesViewData');
REQUIRE('chlk.models.grading.GradingScale');

REQUIRE('chlk.models.id.StudentAnnouncementId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.GradingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.GradeId');
REQUIRE('chlk.models.id.AnnouncementTypeGradingId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradingService*/
    CLASS(
        'GradingService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.AnnouncementId, chlk.models.id.SchoolPersonId, String, String, Boolean, Boolean, Boolean, Boolean, Boolean, Boolean, Object]],
            ria.async.Future, function updateItem(announcementId, studentId, gradeValue, comment, dropped, late, absent,
                      incomplete, exempt, commentWasChanged, model_) {
                return this.get('Grading/UpdateItem', model_ || chlk.models.announcement.StudentAnnouncement, {
                    announcementId: announcementId && announcementId.valueOf(),
                    studentId: studentId && studentId.valueOf(),
                    gradeValue: gradeValue,
                    comment: comment,
                    dropped: dropped,
                    late: late,
                    absent: absent,
                    incomplete: incomplete,
                    exempt: exempt,
                    commentWasChanged: commentWasChanged,
                    callFromGradeBook: !!model_
                });
            },

            [[chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getStudentAverages(gradingPeriodId) {
                return this.get('Grading/GradedItemsList', ArrayOf(chlk.models.grading.GradedItemViewData), {
                    gradingPeriodId: gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId, Number, String, Boolean, Object, String]],
            ria.async.Future, function updateStudentAverage(classId, studentId, gradingPeriodId, averageId, averageValue, exempt_, codes, note) {
                return this.post('Grading/UpdateStudentAverage', chlk.models.grading.ShortStudentAverageInfo, {
                    classId: classId && classId.valueOf(),
                    studentId: studentId && studentId.valueOf(),
                    gradingPeriodId: gradingPeriodId && gradingPeriodId.valueOf(),
                    averageId: averageId,
                    averageValue: averageValue,
                    exempt: exempt_,
                    codes: codes,
                    note: note
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.SchoolPersonId, chlk.models.id.GradingPeriodId, Number, String, Boolean, Object, String]],
            ria.async.Future, function updateStudentAverageFromFinalPage(classId, studentId, gradingPeriodId, averageId, averageValue, exempt_, codes, note) {
                var res =  this.updateStudentAverage(classId, studentId, gradingPeriodId, averageId, averageValue, exempt_, codes, note)
                    .then(function(model){
                        var gp = this.getFinalGradeGPInfo(), curIndex = 0;
                        var currentModel = gp.getStudentFinalGrades().filter(function(item){return model.getStudentId() == item.getStudent().getId()})[0];
                        currentModel.setCurrentStudentAverage(model);
                        setTimeout(function(){
                            currentModel.getStudentAverages().forEach(function(item, index){
                                if(item.getAverageId() == model.getAverageId())
                                    curIndex = index;
                            });
                            currentModel.getStudentAverages()[curIndex] = model;
                        }, 1);
                        return currentModel;
                    }, this);
                return res;
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
                return this.get('Grading/UpdateStandardGrade', chlk.models.standard.StandardGrading, {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf(),
                    studentId: studentId.valueOf(),
                    standardId: standardId.valueOf(),
                    alphaGradeId: alphaGradeId_ ? (alphaGradeId_.valueOf() || '') : '',
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
                return this.get('Grading/ClassSummary', chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData, {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getClassGradingPeriodSummary(classId, gradingPeriodId) {
                return this.get('Grading/ClassGradingPeriodSummary', chlk.models.grading.GradingClassSummaryItems, {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassStandards(classId) {
                return this.get('Grading/ClassStandardSummary', ArrayOf(chlk.models.grading.GradingClassStandardsItems), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassSummaryGrid(classId) {
                return this.get('Grading/ClassSummaryGrids', chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData, {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getFinalGrades(classId) {
                return this.get('Grading/FinalGrade', chlk.models.grading.FinalGradesViewData, {
                    classId: classId.valueOf()
                }).then(function(model){
                    this.setFinalGradeGPInfo(model.getCurrentFinalGrade());
                    return model;
                }, this);
            },

            chlk.models.grading.GradingPeriodFinalGradeViewData, 'finalGradeGPInfo',

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, Number]],
            ria.async.Future, function getFinalGradesForPeriod(classId, gradingPeriodId, averageId_) {
                var gp = this.getFinalGradeGPInfo();
                if(averageId_ && gradingPeriodId == gp.getGradingPeriod().getId()){
                    var currentStudentAvg;
                    var currentAvg = gp.getAverages().filter(function(item){return averageId_ == item.getAverageId()})[0];
                    gp.setCurrentAverage(currentAvg);
                    gp.getStudentFinalGrades().forEach(function(item){
                        currentStudentAvg = item.getStudentAverages().filter(function(item){return averageId_ == item.getAverageId()})[0];
                        item.setCurrentStudentAverage(currentStudentAvg);
                    });
                    return new ria.async.DeferredData(gp);
                }
                return this.get('Grading/GradingPeriodFinalGrade', chlk.models.grading.GradingPeriodFinalGradeViewData, {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf()
                }).then(function(model){
                    this.setFinalGradeGPInfo(model);
                    return model;
                }, this);
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId, chlk.models.id.AnnouncementTypeGradingId, Boolean, Boolean]],
            ria.async.Future, function getClassSummaryGridForPeriod(classId, gradingPeriodId, standardId_, classAnnouncementTypeId_, notCalculateGrid_, autoUpdate_) {
                return this.get('Grading/ClassGradingGrid', chlk.models.grading.ShortGradingClassSummaryGridItems, {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf(),
                    standardId: standardId_ && standardId_.valueOf(),
                    classAnnouncementTypeId: classAnnouncementTypeId_ && classAnnouncementTypeId_.valueOf(),
                    notCalculateGrid: notCalculateGrid_
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId, chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getStudentClassGradingByStandard(classId, gradingPeriodId, standardId, studentId) {
                return this.get('Grading/StudentClassGradingByStandard', null, {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf(),
                    standardId: standardId.valueOf(),
                    studentId: studentId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function getClassStandardsGrid(classId, gradingPeriodId) {
                return this.get('Grading/ClassStandardGrid', chlk.models.grading.GradingClassSummaryGridItems.OF(Object), {
                    classId: classId.valueOf(),
                    gradingPeriodId: gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getClassStandardsGrids(classId) {
                return this.get('Grading/ClassStandardGrids', chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData, {
                    classId: classId.valueOf()
                });
            },

            [[Number, chlk.models.id.ClassId]],
            ria.async.Future, function getRecentlyGradedItems(pageIndex_, classId_){
                return this.get('Grading/RecentlyGradedItems', ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), {
                    start: pageIndex_|0,
                    classId: classId_ ? classId_.valueOf() : null
                });
            },

            [[chlk.models.id.SchoolPersonId]],
            ria.async.Future, function getTeacherSummary(teacherId) {
                return this.get('Grading/TeacherSummary', ArrayOf(chlk.models.grading.GradingTeacherClassSummaryViewData), {
                    teacherId: teacherId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementId]],
            ria.async.Future, function getItemGradingStat(announcementId) {
                return this.get('Grading/ItemGradingStat', null, {
                    announcementId: announcementId.valueOf()
                });
            },

            [[String]],
            ria.async.Future, function getGradeComments(query_) {
                return this.get('Grading/GetGridComments', ArrayOf(String), {
                    schoolYearId: this.getContext().getSession().get(ChlkSessionConstants.CURRENT_SCHOOL_YEAR_ID, null).valueOf()
                }).then(function(data){
                    var comments = data || [];
                    if (query_){
                        query_ = query_.toLowerCase();
                        comments = comments.filter(function(item){
                            return item != null && item.toLowerCase().indexOf(query_) != -1;
                        });
                    }
                    return comments.map(function(item){
                        return new chlk.models.grading.GradingComment(item);
                    });
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function postGradeBook(classId, gradingPeriodId){
                return this.post('Grading/PostGradebook', Boolean, {
                    classId: classId && classId.valueOf(),
                    gradingPeriodId: gradingPeriodId && gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId]],
            ria.async.Future, function postStandards(classId, gradingPeriodId){
                return this.post('Grading/PostStandards', Boolean, {
                    classId: classId && classId.valueOf(),
                    gradingPeriodId: gradingPeriodId && gradingPeriodId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementApplicationId]],
            ria.async.Future, function discardAutoGrades(announcementApplicationId){
                return this.get('Grading/DiscardAutoGrades', null, {
                    announcementApplicationId: announcementApplicationId.valueOf()
                });
            },

            [[chlk.models.id.AnnouncementApplicationId]],
            ria.async.Future, function applyAutoGrades(announcementApplicationId){
                return this.get('Grading/ApplyAutoGrades', null, {
                    announcementApplicationId: announcementApplicationId.valueOf(),
                    lastSettedGradeTime: new chlk.models.common.ChlkDate().toString('mm-dd-yy hh:min:ss')
                });
            },

            ria.async.Future, function getGradingScales(){
                return this.get('Grading/GradingScalesList.json', ArrayOf(chlk.models.grading.GradingScale), {
                });
            }

        ])
});