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
REQUIRE('chlk.models.profile.ClassPanoramaViewData');
REQUIRE('chlk.models.classes.CourseType');

REQUIRE('chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData');
REQUIRE('chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData');
REQUIRE('chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData');
REQUIRE('chlk.models.classes.ClassGradingSummary');
REQUIRE('chlk.models.grading.FinalGradesViewData');

REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.lunchCount.LunchCountGrid');

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

            ria.async.Future, function getCourseTypes() {
                return this.get('Class/CourseTypes.json', ArrayOf(chlk.models.classes.CourseType));
            },

            //[[chlk.models.id.ClassId, Object]],
            ria.async.Future, function getPanorama(classId, data_, selectedStudents_) {
                return this.post('Class/Panorama.json', chlk.models.profile.ClassPanoramaViewData, {
                    classId: classId.valueOf(),
                    standardizedTestFilters: data_ && data_.standardizedTestFilters,
                    acadYears: data_ && data_.acadYears,
                    selectedStudents: selectedStudents_
                }).then(function(model){
                    this.getContext().getSession().set(ChlkSessionConstants.CURRENT_PANORAMA, model);
                    return model;
                }, this);
            },

            ria.async.Future, function restorePanorama(classId) {
                return this.post('Class/RestorePanoramaSettings.json', Object, {
                    classId: classId.valueOf()
                });
            },

            //[[Object, Object]],
            ria.async.Future, function savePanoramaSettings(classId, data) {
                return this.post('Class/SavePanoramaSettings.json', Boolean, {
                    classId : classId,
                    standardizedTestFilters: data.standardizedTestFilters,
                    acadYears: data.acadYears
                });
            },

            [[chlk.models.profile.ClassPanoramaSortType, Boolean]],
            ria.async.Future, function sortPanoramaStudents(orderBy_, descending_) {
                orderBy_ = orderBy_ || chlk.models.profile.ClassPanoramaSortType.NAME;
                descending_ = descending_ || false;
                var panorama = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PANORAMA, null);

                if(!panorama)
                    throw 'No panorama saved';

                var students = panorama.getStudents(), propRef, propName,
                    ref = ria.reflection.ReflectionClass(chlk.models.panorama.StudentDetailsViewData);

                switch (orderBy_){
                    case chlk.models.profile.ClassPanoramaSortType.NAME: propName = 'displayName'; break;
                    case chlk.models.profile.ClassPanoramaSortType.GRADE_AVG: propName = 'gradeAvg'; break;
                    case chlk.models.profile.ClassPanoramaSortType.ABSENCES: propName = 'absences'; break;
                    case chlk.models.profile.ClassPanoramaSortType.DISCIPLINE: propName = 'discipline'; break;
                    //case chlk.models.profile.ClassPanoramaSortType.ETHNICITY: propName = 'displayName'; break;
                    case chlk.models.profile.ClassPanoramaSortType.HISPANIC: propName = 'hispanic'; break;
                    case chlk.models.profile.ClassPanoramaSortType.IEP_ACTIVE: propName = 'IEPActive'; break;
                    case chlk.models.profile.ClassPanoramaSortType.RETAINED: propName = 'retainedFromPrevSchoolYear'; break;
                }

                if(propName)
                    propRef = ref.getPropertyReflector(propName);

                students = students.sort(function(s1, s2){
                    var a, b;

                    if(orderBy_ == chlk.models.profile.ClassPanoramaSortType.ETHNICITY){
                        if(!s1.getStudent().getEthnicity() || !s2.getStudent().getEthnicity())
                            return 0;

                        a = parseInt(s1.getStudent().getEthnicity().getCode(), 10);
                        b = parseInt(s2.getStudent().getEthnicity().getCode(), 10);
                    }else{
                        a = propRef.invokeGetterOn(s1.getStudent());
                        b = propRef.invokeGetterOn(s2.getStudent());
                    }

                    if (a > b)
                        return descending_ ? -1 : 1;
                    if (a < b)
                        return descending_ ? 1 : -1;
                    return 0
                });

                panorama.setOrderBy(orderBy_);
                panorama.setDescending(descending_);

                return ria.async.DeferredData(panorama);
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getScheduledDays(classId) {
                return this.get('Class/Days.json', ArrayOf(chlk.models.common.ChlkDate), {
                    classId: classId.valueOf()
                });
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
            },

            [[chlk.models.id.SchoolId, chlk.models.id.GradeLevelId]],
            ria.async.Future, function getClassesBySchool(schoolId, gradeLevel_) {
                return this.get('Class/ClassesBySchool.json', ArrayOf(chlk.models.classes.Class), {
                    schoolId: schoolId.valueOf(),
                    gradeLevel: gradeLevel_ && gradeLevel_.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
            ria.async.Future, function getLunchCount(classId, date, includeGuests) {
                return this.get('Class/LunchCount.json', chlk.models.lunchCount.LunchCountGrid, {
                    classId: classId.valueOf(),
                    date: date.toStandardFormat(),
                    includeGuests: includeGuests.valueOf()
                });
            }
        ])
});