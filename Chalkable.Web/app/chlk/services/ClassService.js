REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.classes.ClassForWeekMask');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.classes.ClassSummary');
REQUIRE('chlk.models.classes.ClassInfo');
REQUIRE('chlk.models.classes.ClassSchedule');
REQUIRE('chlk.models.classes.ClassGradingViewData');
REQUIRE('chlk.models.classes.AllSchoolsActiveClasses');
REQUIRE('chlk.models.attendance.ClassAttendanceStatsViewData');
REQUIRE('chlk.models.school.SchoolClassesStatisticViewData');
REQUIRE('chlk.models.classes.ClassDisciplinesSummary');
REQUIRE('chlk.models.classes.ClassAttendanceSummary');

REQUIRE('chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData');
REQUIRE('chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData');
REQUIRE('chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData');
REQUIRE('chlk.models.classes.ClassGradingSummary');
REQUIRE('chlk.models.grading.FinalGradesViewData');

REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.ClassService*/
    CLASS(
        'ClassService', EXTENDS(chlk.services.BaseService), [

            Array, function getClassesForTopBarSync() {
                return this.getContext().getSession().get(ChlkSessionConstants.CLASSES_TO_FILTER);
            },

            [[chlk.models.id.ClassId]],
            ArrayOf(chlk.models.id.MarkingPeriodId), function getMarkingPeriodRefsOfClass(classId) {
                var classInfo = _GLOBAL.classesToFilter.filter(function (_) { return _.id == classId.valueOf()})[0];
                Assert(classInfo, 'Class with is should exist');
                return classInfo.markingperiodsid.map(function (_) { return chlk.models.id.MarkingPeriodId(_) });
            },

            [[chlk.models.id.ClassId]],
            chlk.models.classes.Class, function getClassById(classId) {
                var classes = this.getContext().getSession().get(ChlkSessionConstants.CLASSES_TO_FILTER), res = null;
                classes.forEach(function(item){
                    if(item.getId() == classId)
                        res = item;
                });
                return res;
            },

            //TODO: refactor
            [[chlk.models.id.ClassId]],
            chlk.models.classes.ClassForWeekMask, function getClassAnnouncementInfo(id){
                //var res = window.classesInfo[id.valueOf()];
                var res = this.getContext().getSession().get(ChlkSessionConstants.CLASSES_INFO, {})[id.valueOf()];
                if(!res) {
                    var cls = chlk.models.classes.ClassForWeekMask();
                    cls.setAlphaGrades([]);
                    cls.setMask([]);
                    cls.setClassId(id);
                    cls.setTypesByClass([]);
                    cls.setAlphaGradesForStandards([]);
                    return cls;
                }
                return res;
                //res.classId = id.valueOf();
                //res = new chlk.lib.serialize.ChlkJsonSerializer().deserialize(res, chlk.models.classes.ClassForWeekMask);
                //return res;
            },

            [[ArrayOf(chlk.models.id.ClassId)]],
            ria.async.Future, function updateClassAnnouncementTypes(classIds){
                return this.getContext().getService(chlk.services.AnnouncementTypeService)
                    .list(classIds)
                    .then(function (classAnnTypes){
                        var classesInfoMap = this.getContext().getSession().get(ChlkSessionConstants.CLASSES_INFO, {});
                        for(var i = 0; i < classIds.length; i++){
                            var classInfo = classesInfoMap[classIds[i].valueOf()];
                            if(classInfo){
                                classInfo.setTypesByClass(classAnnTypes.filter(function (annType){
                                    return annType.getClassId() == classIds[i];
                                }));
                            }
                        }
                        this.getContext().getSession().set(ChlkSessionConstants.CLASSES_INFO, classesInfoMap);
                        return classesInfoMap;
                    }, this);
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getSummary(classId) {
                return this.get('Class/Summary.json', chlk.models.classes.ClassSummary, {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.classes.DateTypeEnum]],
            ria.async.Future, function getDisciplinesStats(classId, dateType_){
                return this.get('Class/DisciplineSummary.json', chlk.models.discipline.ClassDisciplineStatsViewData,{
                    classId: classId.valueOf(),
                    dateType: dateType_ && dateType_.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.classes.DateTypeEnum]],
            ria.async.Future, function getAttendanceStats(classId, dateType_){
                return this.get('Class/AttendanceSummary.json', chlk.models.attendance.ClassAttendanceStatsViewData,{
                    classId: classId.valueOf(),
                    dateType: dateType_ && dateType_.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getDisciplinesSummary(classId) {
                return this.get('Class/Summary.json', chlk.models.classes.ClassDisciplinesSummary, {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getAttendanceSummary(classId) {
                return this.get('Class/Summary.json', chlk.models.classes.ClassAttendanceSummary, {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGradingItemsGridSummary(classId) {
                return this.get('Class/Grading.json', chlk.models.classes.ClassGradingSummary.OF(chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGradingItemsBoxesSummary(classId) {
                return this.get('Class/Summary.json', chlk.models.classes.ClassGradingSummary.OF(chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGradingStandardsGridSummary(classId) {
                return this.get('Class/Grading.json', chlk.models.classes.ClassGradingSummary.OF(chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGradingStandardsBoxesSummary(classId) {
                return this.get('Class/Summary.json', chlk.models.classes.ClassGradingSummary.OF(chlk.models.grading.GradingClassSummary), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGradingFinalGradesSummary(classId) {
                return this.get('Class/Grading.json', chlk.models.classes.ClassGradingSummary.OF(chlk.models.grading.FinalGradesViewData), {
                    classId: classId.valueOf()
                });
            },

            [[chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
            ria.async.Future, function detailedCourseTypes(schoolYearId, gradeLevelId) {
                return this.get('Class/DetailedCourseTypes.json', ArrayOf(chlk.models.classes.CourseType), {
                    schoolYearId: schoolYearId.valueOf(),
                    gradeLevelId: gradeLevelId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getInfo(classId){
                return this.get('Class/ClassInfo.json', chlk.models.classes.ClassInfo,{
                    classId: classId && classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getExplorer(classId){
                return this.get('Class/Explorer.json', chlk.models.classes.ClassExplorerViewData,{
                    classId: classId && classId.valueOf()
                });
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getGrading(classId){
                return this.get('Class/ClassGrading.json', chlk.models.classes.ClassGradingViewData,{
                    classId: classId && classId.valueOf()
                })
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function getSchedule(classId, date_){
                return this.get('Class/ClassSchedule.json', chlk.models.classes.ClassSchedule,{
                    classId: classId && classId.valueOf(),
                    date: date_ && date_.toString('mm-dd-yy')
                })
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getAttendance(classId){
                return this.get('Class/ClassAttendance.json', chlk.models.classes.ClassAttendanceSummary,{
                    classId: classId && classId.valueOf()
                })
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getAppsInfo(classId){
                return this.get('Class/ClassApps.json', chlk.models.classes.Class,{
                    classId: classId && classId.valueOf()
                })
            },

            ria.async.Future, function getAllSchoolsActiveClasses(){
                return this.get('Class/AllSchoolsActiveClasses.json', chlk.models.classes.AllSchoolsActiveClasses, {})
            },

            [[chlk.models.id.SchoolYearId, Number, String, chlk.models.id.SchoolPersonId, Number]],
            ria.async.Future, function getClassesStatistic(schoolYearId, start_, filter_, teacherId_, sortType_) {
                return this.get('Class/ClassesStats.json', ArrayOf(chlk.models.school.SchoolClassesStatisticViewData.OF(chlk.models.id.SchoolId)), {
                    schoolYearId: schoolYearId.valueOf(),
                    start:start_ || 0,
                    count: 10,
                    sortType: sortType_,
                    teacherId: teacherId_ && teacherId_.valueOf(),
                    filter: filter_
                });
            }
        ])
});