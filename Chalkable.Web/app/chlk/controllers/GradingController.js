REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ReportingService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.GradingPeriodService');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.AttendanceService');

REQUIRE('chlk.activities.grading.GradingClassSummaryPage');
REQUIRE('chlk.activities.grading.GradingClassStandardsPage');
REQUIRE('chlk.activities.grading.GradingTeacherClassSummaryPage');
REQUIRE('chlk.activities.grading.GradingStudentSummaryPage');
REQUIRE('chlk.activities.grading.GradingStudentClassSummaryPage');
REQUIRE('chlk.activities.grading.GradingClassSummaryGridPage');
REQUIRE('chlk.activities.grading.GradingClassStandardsGridPage');
REQUIRE('chlk.activities.grading.StudentAvgPopupDialog');
REQUIRE('chlk.activities.grading.FinalGradesPage');

REQUIRE('chlk.activities.reports.GradeBookReportDialog');
REQUIRE('chlk.activities.reports.WorksheetReportDialog');
REQUIRE('chlk.activities.reports.ProgressReportDialog');
REQUIRE('chlk.activities.reports.ComprehensiveProgressReportDialog');
REQUIRE('chlk.activities.reports.MissingAssignmentsReportDialog');
REQUIRE('chlk.activities.reports.BirthdayReportDialog');
REQUIRE('chlk.activities.reports.SeatingChartReportDialog');
REQUIRE('chlk.activities.reports.GradeVerificationReportDialog');
REQUIRE('chlk.activities.reports.LessonPlanReportDialog');

REQUIRE('chlk.models.grading.GradingSummaryGridSubmitViewData');

REQUIRE('chlk.models.reports.SubmitGradeBookReportViewData');
REQUIRE('chlk.models.reports.SubmitProgressReportViewData');
REQUIRE('chlk.models.reports.SubmitWorksheetReportViewData');
REQUIRE('chlk.models.reports.SubmitComprehensiveProgressViewData');
REQUIRE('chlk.models.reports.SubmitMissingAssignmentsReportViewData');
REQUIRE('chlk.models.reports.SubmitBirthdayReportViewData');
REQUIRE('chlk.models.reports.SubmitGradeVerificationReportViewData');

NAMESPACE('chlk.controllers', function (){

    var needStudentReverse, studentIds = [];

    /** @class chlk.controllers.GradingController */
    CLASS(
        'GradingController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.FinalGradeService, 'finalGradeService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.AnnouncementService, 'announcementService',

            [ria.mvc.Inject],
            chlk.services.GradingService, 'gradingService',

            [ria.mvc.Inject],
            chlk.services.ReportingService, 'reportingService',

            [ria.mvc.Inject],
            chlk.services.CalendarService , 'calendarService',

            [ria.mvc.Inject],
            chlk.services.GradingPeriodService, 'gradingPeriodService',

            [ria.mvc.Inject],
            chlk.services.StudentService, 'studentService',

            [ria.mvc.Inject],
            chlk.services.AttendanceService, 'attendanceService',

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES//,
                //[chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                var result = this.gradingService
                    .getClassSummary(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        model.setTopData(topData);
                        model.setGradingPeriodId(gradingPeriod.getId());
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassSummaryPage, result);
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodSummaryAction(model){
                var result = this.gradingService
                    .getClassGradingPeriodSummary(model.getClassId(), model.getGradingPeriodId())
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryPage, result);
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function standardsTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                var model = new chlk.models.grading.GradingClassSummary();
                model.setTopData(topData);
                var result = this.gradingService
                    .getClassStandards(classId_)
                    .attach(this.validateResponse_())
                    .then(function(result){
                        result.forEach(function(mpData){
                            mpData.getItems().forEach(function(item){
                                item.setClassId(classId_);
                            });
                        });
                        model.setSummaryPart(new chlk.models.grading.GradingClassSummaryPart(result));
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        model.setGradingPeriodId(gradingPeriod.getId());
                        model.setAction('standards');
                        model.setGridAction('standardsGrid');
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodFinalGradesSummaryAction(model){
                if(!model.getClassId() || !model.getGradingPeriodId())
                    return null;
                var avgChanged = (this.gradingService.getFinalGradeGPInfo().getGradingPeriod().getId() == model.getGradingPeriodId()) && model.getAverageId();
                var result = this.gradingService
                    .getFinalGradesForPeriod(
                        model.getClassId(),
                        model.getGradingPeriodId(),
                        model.getAverageId()
                    )
                    .then(function(resModel){
                        if(!resModel.getCurrentAverage())
                            this.ShowMsgBox('There are no grading items in selected Grading Period');
                        var index = model.getSelectedIndex();
                        if(index || index == 0)
                            resModel.setSelectedIndex(index);
                        resModel.setAvgChanged(!!avgChanged);
                        return resModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.FinalGradesPage, result, avgChanged ? 'average-change' : 'load-gp');
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodGridSummaryAction(model){
                var result = this.gradingService
                    .getClassSummaryGridForPeriod(
                        model.getClassId(),
                        model.getGradingPeriodId(),
                        model.getStandardId(),
                        model.getCategoryId(),
                        model.isNotCalculateGrid(),
                        model.isAutoUpdate()
                    )
                    .then(function(newModel){
                        /*if(model.isAutoUpdate()){
                            newModel.isAvg = model.isAvg();
                            return new chlk.models.common.SimpleObject(newModel);
                        }*/
                        newModel.setAutoUpdate(model.isAutoUpdate());
                        newModel.setCategoryId(model.getCategoryId());
                        newModel.setStandardId(model.getStandardId());
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        newModel.setSchoolOptions(schoolOptions);
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                        var canEditAvg = canEdit || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_STUDENT_AVERAGES);
                        newModel.setAbleEdit(canEdit);
                        newModel.setAbleEditAvg(canEditAvg);
                        return newModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, result, model.isAutoUpdate() ? chlk.activities.lib.DontShowLoader() : null);
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function finalGradesAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                var alphaGrades = classInfo.getAlphaGrades();
                var gradingComments = this.getContext().getSession().get(ChlkSessionConstants.GRADING_COMMENTS, []);
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId_);
                var result = this.gradingService
                    .getFinalGrades(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        var canEditDirectValue = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_STUDENT_AVERAGES)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                        model.setTopData(topData);
                        model.setGradingPeriodId(gradingPeriod.getId());
                        model.setAlphaGrades(alphaGrades);
                        model.setGradingComments(gradingComments);
                        model.setAbleEdit(canEdit);
                        model.setAbleEditDirectValue(canEditDirectValue);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.FinalGradesPage, result);
            },

            [[chlk.models.reports.BaseSubmitReportViewData]],
            function changeStudentsOrderAction(model){
                studentIds = (model.getStudentIds() || '').split(',');
                var res = [];
                var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                studentIds.forEach(function(id){
                    res.push(students.filter(function(student){return student.getId().valueOf() == id})[0]);
                });
                this.getContext().getSession().set(ChlkSessionConstants.STUDENTS_FOR_REPORT, res);
                //needStudentReverse = !needStudentReverse;

                //students.reverse();
                return null;
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryGridTeacherAction(classId_){
                needStudentReverse = false;
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId_);
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                var alphaGrades = classInfo.getAlphaGrades();
                var alternateScores = this.getContext().getSession().get(ChlkSessionConstants.ALTERNATE_SCORES, []);
                var gradingComments = this.getContext().getSession().get(ChlkSessionConstants.GRADING_COMMENTS, []);
                var result = this.gradingService
                    .getClassSummaryGrid(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        model.setAlphaGrades(alphaGrades);
                        model.setTopData(topData);
                        model.setGradingPeriodId(gradingPeriod.getId());
                        model.setAlternateScores(alternateScores);
                        model.setGradingComments(gradingComments);
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        if(model.getCurrentGradingGrid()){
                            var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                                || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                            var canEditAvg = canEdit || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_STUDENT_AVERAGES);
                            model.getCurrentGradingGrid().setSchoolOptions(schoolOptions);
                            model.getCurrentGradingGrid().setAbleEdit(canEdit);
                            model.getCurrentGradingGrid().setAbleEditAvg(canEditAvg);
                            var students = model.getCurrentGradingGrid().getStudents().map(function (item){return item.getStudentInfo()});
                            this.getContext().getSession().set(ChlkSessionConstants.STUDENTS_FOR_REPORT, students);
                            this.getContext().getSession().set(ChlkSessionConstants.INCLUDE_WITHDRAWN_STUDENTS, model.getCurrentGradingGrid().isIncludeWithdrawnStudents());
                            studentIds = students.map(function(item){return item.getId().valueOf()});
                        }

                        model.setAbleEdit(canEdit);

                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassSummaryGridPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.standard.StandardGrading]],
            function updateStandardGradeFromGridAction(model){
                var result = this.gradingService
                    .updateStandardGrade(
                        model.getClassId(),
                        model.getGradingPeriodId(),
                        model.getStudentId(),
                        model.getStandardId(),
                        model.getGradeId(),
                        model.getComment()
                    )
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassStandardsGridPage, result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function standardsGridTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                var alphaGrades = classInfo.getAlphaGradesForStandards();
                var result = this.gradingService
                    .getClassStandardsGrids(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        model.setAlphaGrades(alphaGrades);
                        model.setTopData(topData);
                        model.setGradingPeriodId(gradingPeriod.getId());
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        if(model.getCurrentGradingGrid()){
                            var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                                || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                            model.getCurrentGradingGrid().setSchoolOptions(schoolOptions);
                            model.getCurrentGradingGrid().setAbleEdit(canEdit);
                            model.getCurrentGradingGrid().setGradable(alphaGrades && (alphaGrades.length || false) && canEdit);
                        }
                        model.setAbleEdit(canEdit);
                        model.setAblePostStandards(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN));
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsGridPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodGridStandardsAction(model){
                var classInfo = this.classService.getClassAnnouncementInfo(model.getClassId());
                var alphaGrades = classInfo.getAlphaGradesForStandards();
                var result = this.gradingService
                    .getClassStandardsGrid(
                        model.getClassId(),
                        model.getGradingPeriodId()
                    )
                    .then(function(newModel){
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        newModel.setSchoolOptions(schoolOptions);
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                        newModel.setAbleEdit(canEdit);
                        newModel.setGradable(alphaGrades && (alphaGrades.length || false) && canEdit);
                        return newModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassStandardsGridPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryStudentAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var studentId = this.getCurrentPerson().getId();
                var result = this.gradingService
                    .getStudentsClassSummary(studentId, classId_)
                    .then(function(model){
                        if(model.getItems()){
                            model.getItems().forEach(function(mpData){
                                mpData.getItems().forEach(function(item){
                                    item.setClassId(classId_);
                                });
                            });
                        }
                        var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        model.setGradingPeriodId(gradingPeriod.getId());
                        model.setClazz(this.classService.getClassById(classId_));
                        model.setTopData(topData);
                        model.setSummaryPart(new chlk.models.grading.GradingClassSummaryPart(model.getItems()));
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingStudentClassSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            function summaryAllTeacherAction(){
                var teacherId = this.getCurrentPerson().getId();
                var result = this.gradingService
                    .getTeacherSummary(teacherId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        var topData = new chlk.models.classes.ClassesForTopBar(null, null);
                        return new chlk.models.grading.GradingTeacherClassSummaryViewDataList(topData, items);
                    }, this);
                return this.PushView(chlk.activities.grading.GradingTeacherClassSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId, Boolean]],
            function summaryAllStudentAction(classId_, update_){
                var studentId = this.getCurrentPerson().getId();
                var result = this.gradingService
                    .getStudentSummary(studentId, classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                        model.setTopData(topData);
                        return model;
                    }, this);
                return update_ ? this.UpdateView(chlk.activities.grading.GradingStudentSummaryPage, result, 'chart-update') :
                    this.PushView(chlk.activities.grading.GradingStudentSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.AnnouncementId]],
            function showChartAction(announcementId){
                var result = this.gradingService.getItemGradingStat(announcementId)
                    .then(function(model){
                        return new chlk.models.common.SimpleObject(model);
                    });
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.SidebarButton('statistic')],
            function getGradeCommentsAction(){
                var result = this.gradingService
                    .getGradeComments()
                    .then(function(comments){
                        return chlk.models.grading.GradingComments.$createFromList(comments);
                    });
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            function getGradeCommentsFromViewAction(){
                return this.getGradeCommentsAction();
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId, chlk.models.id.AnnouncementTypeGradingId, Boolean]],
            function postGradeBookAction(classId, gradingPeriodId, standardId_, categoryId_, finalGrades_){
                var res = this.gradingService.postGradeBook(classId, gradingPeriodId)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        if(finalGrades_)
                            return new chlk.models.Success;
                        var model = new chlk.models.grading.GradingSummaryGridSubmitViewData(classId, gradingPeriodId, true, false, standardId_, categoryId_);
                        this.BackgroundNavigate('grading', 'loadGradingPeriodGridSummary', [model]);
                    }, this);

                if(finalGrades_)
                    return this.UpdateView(this.getView().getCurrent().getClass(), res, chlk.activities.lib.DontShowLoader());
                return null;
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function gradeBookReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', 'Error'), null;
                var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.GRADE_BOOK_REPORT) ||
                    this.hasUserPermission_(chlk.models.people.UserPermissionEnum.GRADE_BOOK_REPORT_CLASSROOM);
                var res = new ria.async.DeferredData(new chlk.models.reports.GradeBookReportViewData(gradingPeriodId, classId, startDate, endDate, students, null, ableDownload));
                return this.ShadeView(chlk.activities.reports.GradeBookReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function birthdayReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', 'Error'), null;
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT) ||
                    this.hasUserPermission_(chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT_CLASSROOM);
                var model = new chlk.models.reports.BirthdayReportViewData();
                model.setGradingPeriodId(gradingPeriodId);
                model.setClassId(classId);
                model.setStartDate(startDate);
                model.setEndDate(endDate);
                model.setAbleDownload(ableDownload);
                var res = new ria.async.DeferredData(model);
                return this.ShadeView(chlk.activities.reports.BirthdayReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function seatingChartReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', 'Error'), null;
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.SEATING_CHART_REPORT);
                var res = new ria.async.DeferredData(new chlk.models.reports.BaseReportViewData(classId, gradingPeriodId, startDate, endDate, null, ableDownload));
                return this.ShadeView(chlk.activities.reports.SeatingChartReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function gradeVerificationReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = ria.async.wait([
                        this.gradingPeriodService.getList(),
                        this.gradingService.getStudentAverages(gradingPeriodId)
                    ])
                    .then(function(data){
                        var periods = data[0];
                        var averages = data[1];
                        var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                        var includeWithdrawn = this.getContext().getSession().get(ChlkSessionConstants.INCLUDE_WITHDRAWN_STUDENTS);
                        var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.GRADE_VERIFICATION_REPORT) ||
                            this.hasUserPermission_(chlk.models.people.UserPermissionEnum.GRADE_VERIFICATION_REPORT_CLASSROOM);
                        var res = new chlk.models.reports.GradeVerificationReportViewData(periods, averages, students, classId, gradingPeriodId, startDate, endDate, ableDownload);
                        res.setIncludeWithdrawnStudents(includeWithdrawn);
                        return res;
                    }, this);
                return this.ShadeView(chlk.activities.reports.GradeVerificationReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function lessonPlanReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', 'Error'), null;
                var classInfo = this.classService.getClassAnnouncementInfo(classId);
                var activityCategories = classInfo.getTypesByClass();
                var res = this.announcementService.getAnnouncementAttributes(true)
                    .then(function(items){
                        var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.LESSON_PLAN_REPORT) ||
                            this.hasUserPermission_(chlk.models.people.UserPermissionEnum.LESSON_PLAN_REPORT_CLASSROOM);
                        return new chlk.models.reports.LessonPlanReportViewData(activityCategories, items, classId, gradingPeriodId, startDate, endDate, ableDownload);
                    }, this);
                return this.ShadeView(chlk.activities.reports.LessonPlanReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function worksheetReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.getWorksheetReportInfo_(gradingPeriodId, classId, startDate, endDate);
                return this.ShadeView(chlk.activities.reports.WorksheetReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function progressReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = ria.async.wait([
                        this.reportingService.getStudentReportComments(classId, gradingPeriodId),
                        this.attendanceService.getAllAttendanceReasons()
                    ])
                    .then(function(data){
                        var students = data[0];
                        var attendanceReasons = data[1];
                        var res = [];
                        studentIds.forEach(function(id){
                            res.push(students.filter(function(student){return student.getId().valueOf() == id})[0]);
                        });
                        var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.PROGRESS_REPORT);
                        return new chlk.models.reports.SubmitProgressReportViewData(attendanceReasons, res, gradingPeriodId, classId, startDate, endDate, ableDownload);
                    }, this);
                return this.ShadeView(chlk.activities.reports.ProgressReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function comprehensiveProgressReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.getComprehensiveProgressReportInfo_(gradingPeriodId, classId, startDate, endDate);
                return this.ShadeView(chlk.activities.reports.ComprehensiveProgressReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function missingAssignmentsReportAction(gradingPeriodId, classId, startDate, endDate){
                var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                var alternateScores = this.getContext().getSession().get(ChlkSessionConstants.ALTERNATE_SCORES, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MISSING_ASSIGNMENTS_REPORT);
                var model = new chlk.models.reports.SubmitMissingAssignmentsReportViewData(classId,
                    gradingPeriodId, startDate, endDate, students, alternateScores, ableDownload);
                return this.ShadeView(chlk.activities.reports.MissingAssignmentsReportDialog, new ria.async.DeferredData(model));
            },



            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.MISSING_ASSIGNMENTS_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitMissingAssignmentsReportViewData]],
            function submitMissingAssignmentsReportAction(reportViewData){
                if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                }

                var src = this.reportingService.submitMissingAssignmentsReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getIdToPrint(),
                    reportViewData.getFormat(),
                    reportViewData.getOrderBy(),
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    this.getIdsList(reportViewData.getAlternateScoreIds(), chlk.models.id.AlternateScoreId),
                    reportViewData.isAlternateScoresOnly(),
                    reportViewData.isConsiderZerosAsMissingGrades(),
                    reportViewData.isIncludeWithdrawnStudents(),
                    reportViewData.isOnePerPage(),
                    reportViewData.isSuppressStudentName(),
                    reportViewData.getStudentIds()
                );
                this.BackgroundCloseView(chlk.activities.reports.MissingAssignmentsReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitComprehensiveProgressViewData]],
            function submitComprehensiveProgressReportAction(reportViewData){
                if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                }

                var src = this.reportingService.submitComprehensiveProgressReport(
                    reportViewData.getClassId(),
                    reportViewData.getIdToPrint(),
                    reportViewData.getFormat(),
                    this.getIdsList(reportViewData.getGradingPeriodIds(), chlk.models.id.GradingPeriodId),
                    this.getIdsList(reportViewData.getAbsenceReasonIds(), chlk.models.id.AttendanceReasonId),
                    reportViewData.getOrderBy(),
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    reportViewData.getMaxStandardAverage(),
                    reportViewData.getMinStandardAverage(),
                    reportViewData.isAdditionalMailings(),
                    reportViewData.isClassAverageOnly(),
                    reportViewData.isDisplayCategoryAverages(),
                    reportViewData.isDisplayClassAverages(),
                    reportViewData.getDailyAttendanceDisplayMethod(),
                    reportViewData.isDisplayPeriodAttendance(),
                    reportViewData.isDisplaySignatureLine(),
                    reportViewData.isDisplayStudentComment(),
                    reportViewData.isDisplayStudentMailingAddress(),
                    reportViewData.isDisplayTotalPoints(),
                    reportViewData.isIncludePicture(),
                    reportViewData.isIncludeWithdrawnStudents(),
                    reportViewData.isWindowEnvelope(),
                    reportViewData.isGoGreen(),
                    reportViewData.getStudentFilterId(),
                    reportViewData.getStudentIds()
                );
                this.BackgroundCloseView(chlk.activities.reports.ComprehensiveProgressReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },

            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.GRADE_VERIFICATION_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitGradeVerificationReportViewData]],
            function submitGradeVerificationReportAction(reportViewData){
                if (!reportViewData.getStudentAverageIds()){
                    return this.ShowAlertBox("You should select at least one graded item", "Error"), null;
                }

                var src = this.reportingService.submitGradeVerificationReport(
                    reportViewData.getClassId(),
                    reportViewData.getFormat(),
                    this.getIdsList(reportViewData.getGradingPeriodIds(), chlk.models.id.GradingPeriodId),
                    reportViewData.getStudentAverageIds().split(','),
                    reportViewData.getGradeType(),
                    reportViewData.getStudentOrder(),
                    reportViewData.getIdToPrint(),
                    reportViewData.isIncludeCommentsAndLegends(),
                    reportViewData.isIncludeSignature(),
                    reportViewData.isIncludeWithdrawn(),
                    reportViewData.getStudentIds()
                );
                this.BackgroundCloseView(chlk.activities.reports.GradeVerificationReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.LESSON_PLAN_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitLessonPlanReportViewData]],
            function submitLessonPlanReportAction(reportViewData){
                if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                }

                if (reportViewData.isIncludeActivities() && !reportViewData.getActivityAttribute()){
                    return this.ShowAlertBox("You should select at least one activity attribute", "Error"), null;
                }

                if (reportViewData.isIncludeActivities() && !reportViewData.getActivityCategory()){
                    return this.ShowAlertBox("You should select at least one activity category", "Error"), null;
                }

                var src = this.reportingService.submitLessonPlanReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getFormat(),
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    reportViewData.getSortActivities(),
                    reportViewData.getPublicPrivateText(),
                    reportViewData.getMaxCount(),
                    reportViewData.isIncludeActivities(),
                    reportViewData.isIncludeStandards(),
                    reportViewData.getActivityAttribute().split(','),
                    reportViewData.getActivityCategory().split(',')
                );
                this.BackgroundCloseView(chlk.activities.reports.LessonPlanReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.GRADE_BOOK_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitGradeBookReportViewData]],
            function submitGradeBookReportAction(reportViewData){

                if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                }

                var src = this.reportingService.submitGradeBookReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    reportViewData.getReportType(),
                    reportViewData.getOrderBy(),
                    reportViewData.getIdToPrint(),
                    reportViewData.getFormat(),
                    reportViewData.isDisplayLetterGrade(),
                    reportViewData.isDisplayTotalPoints(),
                    reportViewData.isDisplayStudentAverage(),
                    reportViewData.isIncludeWithdrawnStudents(),
                    reportViewData.isIncludeNonGradedActivities(),
                    reportViewData.isSuppressStudentName(),
                    reportViewData.getStudentIds()
                );
                this.BackgroundCloseView(chlk.activities.reports.GradeBookReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitBirthdayReportViewData]],
            function submitBirthdayReportAction(reportViewData){

                if (reportViewData.getStartDate() || reportViewData.getEndDate()){

                    if (!reportViewData.getStartDate())
                        return this.ShowAlertBox("Please provide report start date", "Error"), null;
                    if (!reportViewData.getEndDate())
                        return this.ShowAlertBox("Please provide report end date", "Error"), null;

                    if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                        return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                    }
                }


                if (reportViewData.getStartMonth() > reportViewData.getEndMonth()){
                    return this.ShowAlertBox("Start Month must be less than or equal to End Month", "Error"), null;
                }

                var src = this.reportingService.submitBirthdayReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getGroupBy(),
                    reportViewData.getFormat(),
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    reportViewData.getStartMonth(),
                    reportViewData.getEndMonth(),
                    reportViewData.getAppendOrOverwrite(),
                    reportViewData.isIncludeWithdrawn(),
                    reportViewData.isIncludePhoto(),
                    reportViewData.isSaveToFilter(),
                    reportViewData.isSaveAsDefault()
                );
                this.BackgroundCloseView(chlk.activities.reports.BirthdayReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitProgressReportViewData]],
            function downloadProgressReportAction(progressReportViewData){
                progressReportViewData = progressReportViewData.getClassId() ? progressReportViewData
                    : this.getContext().getSession().get('modelForSubmit', null);

                this.reportingService.setStudentProgressReportComments(progressReportViewData.getClassId(),
                    progressReportViewData.getGradingPeriodId(),
                    progressReportViewData.getCommentsList())
                    .then(function(model){
                        var src = this.reportingService.submitProgressReport(
                            progressReportViewData.getClassId(),
                            progressReportViewData.getIdToPrint(),
                            progressReportViewData.getFormat(),
                            progressReportViewData.getGradingPeriodId(),
                            progressReportViewData.getAbsenceReasonIds(),
                            progressReportViewData.isAdditionalMailings(),
                            progressReportViewData.getDailyAttendanceDisplayMethod(),
                            progressReportViewData.isDisplayCategoryAverages(),
                            progressReportViewData.isDisplayClassAverages(),
                            progressReportViewData.isDisplayLetterGrade(),
                            progressReportViewData.isDisplayPeriodAttendance(),
                            progressReportViewData.isDisplaySignatureLine(),
                            progressReportViewData.isDisplayStudentComments(),
                            progressReportViewData.isDisplayStudentMailingAddress(),
                            progressReportViewData.isDisplayTotalPoints(),
                            progressReportViewData.isGoGreen(),
                            progressReportViewData.getMaxCategoryClassAverage(),
                            progressReportViewData.getMaxStandardAverage(),
                            progressReportViewData.getMinCategoryClassAverage(),
                            progressReportViewData.getMinStandardAverage(),
                            progressReportViewData.isPrintFromHomePortal(),
                            progressReportViewData.getClassComment(),
                            progressReportViewData.getStudentIds()
                        );
                        this.getContext().getDefaultView().submitToIFrame(src);
                        return null;
                    }, this);

                this.BackgroundCloseView(chlk.activities.reports.ProgressReportDialog);
                return null;
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.PROGRESS_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitProgressReportViewData]],
            function submitProgressReportAction(model){
                if(!model.getAbsenceReasonIds()){
                    this.ShowMsgBox(Msg.Progress_Report_No_Reasons_msg);
                }
                else{
                    var count = model.getNotSelectedCount();
                    if(!count){
                        return this.downloadProgressReportAction(model);
                    } else {
                        this.getContext().getSession().set('modelForSubmit', model);
                        this.ShowConfirmBox(Msg.Progress_report_msg(count), '', Msg.Yes.toUpperCase())
                            .then(function(data){
                               return this.downloadProgressReportAction(model);
                            }, this);
                    }
                }
                return null;
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.WORKSHEET_REPORT])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.reports.SubmitWorksheetReportViewData]],
            function submitWorksheetReportAction(model){
                if(model.getSubmitType() == 'submit'){
                    var len = 0;
                    for(var i = 1; i <= 5; i++){
                        if(model['getTitle' + i]() && model['getTitle' + i]().trim()) len++;
                    }
                    if(model.getAnnouncementIds().split(',').length + len > 8){
                        return this.ShowAlertBox(Msg.Worksheet_report_msg, "fyi."), null;
                        /*this.ShowMsgBox(Msg.Worksheet_report_msg, 'fyi.', [{
                            text: Msg.GOT_IT.toUpperCase()
                        }]);
                        return this.UpdateView(chlk.activities.reports.WorksheetReportDialog, new ria.async.DeferredData(new chlk.models.reports.GradeBookReportViewData), 'stop');*/
                    }

                    if (Date.compare(model.getStartDate().getDate() , model.getEndDate().getDate()) > 0){
                        return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                    }

                    var src = this.reportingService.submitWorksheetReport(
                        model.getClassId(),
                        model.getGradingPeriodId(),
                        model.getStartDate(),
                        model.getEndDate(),
                        model.getIdToPrint(),
                        model.getAnnouncementIds(),
                        model.getTitle1(),
                        model.getTitle2(),
                        model.getTitle3(),
                        model.getTitle4(),
                        model.getTitle5(),
                        model.isPrintAverage(),
                        model.isPrintLetterGrade(),
                        model.isPrintScores(),
                        model.isPrintStudent(),
                        model.isWorkingFilter(),
                        model.isAppendToExisting(),
                        model.isOverwriteExisting(),
                        model.getStudentIds()
                    );
                    this.BackgroundCloseView(chlk.activities.reports.WorksheetReportDialog);
                    this.getContext().getDefaultView().submitToIFrame(src);
                    return null;
                }
                var res = this.getWorksheetReportInfo_(
                    model.getGradingPeriodId(),
                    model.getClassId(),
                    model.getStartDate(),
                    model.getEndDate()
                );
                return this.UpdateView(chlk.activities.reports.WorksheetReportDialog, res, 'grid');
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.ShortStudentAverageInfo]],
            function updateStudentAvgAction(model){
                if((parseFloat(model.getOldValue()) == parseFloat(model.getAverageValue()) || !model.getAverageValue())
                        && (!model.isExempt() || model.isOldExempt()))
                    return this.updateStudentAvgFromModel(model);

                this.getContext().getSession().set(ChlkSessionConstants.STUDENT_AVG_MODEL, model);
                return this.ShadeView(chlk.activities.grading.StudentAvgPopupDialog, new ria.async.DeferredData(new chlk.models.Success));
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.ShortStudentAverageInfo]],
            function updateStudentAvgFromFinalPageAction(model){
                var result = this.gradingService
                    .updateStudentAverageFromFinalPage(
                        this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CLASS_ID, null),
                        model.getStudentId(),
                        model.getGradingPeriodId(),
                        model.getAverageId(),
                        model.getAverageValue(),
                        model.isExempt(),
                        model.getCodesString() ? JSON.parse(model.getCodesString()) : null,
                        model.getNote()
                    )
                    .catchError(function (error) {
                        this.BackgroundUpdateView(chlk.activities.grading.FinalGradesPage, new chlk.models.grading.StudentFinalGradeViewData(), chlk.activities.lib.DontShowLoader());

                        throw error;
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.grading.FinalGradesPage, result, chlk.activities.lib.DontShowLoader());
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId]],
            function postStandardsAction(classId, gradingPeriodId) {
                var result = this.gradingService
                    .postStandards(classId, gradingPeriodId)
                    .thenCall(this.ShowAlertBox, ['Standards posted successfully.'])
                    .thenBreak();

                return this.UpdateView(chlk.activities.grading.GradingClassStandardsGridPage, result);
            },

            function updateStudentAvgFromModel(model){
                this.BackgroundCloseView(chlk.activities.grading.StudentAvgPopupDialog);
                if(!model.getGradingPeriodId())
                    return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, new ria.async.DeferredData(model), chlk.activities.lib.DontShowLoader());

                var result = this.gradingService
                    .updateStudentAverage(
                        this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CLASS_ID, null),
                        model.getStudentId(),
                        model.getGradingPeriodId(),
                        model.getAverageId(),
                        model.getAverageValue(),
                        model.isExempt(),
                        JSON.parse(model.getCodesString()),
                        model.getNote()
                    ).then(function(newModel){
                        newModel.setGradingPeriodId(model.getGradingPeriodId());
                        return newModel;
                    })
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[Boolean]],
            function updateStudentAvgFromPopupAction(save_){
                var model = this.getContext().getSession().get(ChlkSessionConstants.STUDENT_AVG_MODEL);
                if(!save_)
                    model.setGradingPeriodId(null);
                else{
                    if(!model.getGradingPeriodId())
                        model.setGradingPeriodId(new chlk.models.id.GradingPeriodId(1))
                }
                return this.updateStudentAvgFromModel(model);
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function getWorksheetReportInfo_(gradingPeriodId, classId, startDate, endDate){
                var res = this.calendarService.listByDateRange(startDate, endDate, classId)
                    .then(function(announcements){
                        var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                        var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.WORKSHEET_REPORT);
                        return new ria.async.DeferredData(new chlk.models.reports.GradeBookReportViewData(gradingPeriodId, classId,
                            startDate, endDate, students, announcements, ableDownload));
                    }, this);
                return res;
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            ria.async.Future, function getComprehensiveProgressReportInfo_(selectedGradingPeriodId, classId, startDate, endDate){
                var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                //var attendanceReasons =
                return ria.async.wait([
                            this.gradingPeriodService.getList(),
                            this.attendanceService.getAllAttendanceReasons()
                        ]).then(function (data){
                            var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT) ||
                                this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT_CLASSROOM);
                            return new chlk.models.reports.SubmitComprehensiveProgressViewData(classId,
                                selectedGradingPeriodId, startDate, endDate, data[0], data[1], students, ableDownload)
                        }, this);

                //var res = new chlk.models.reports.SubmitComprehensiveProgressViewData(classId,
                //    selectedGradingPeriodId, startDate, endDate,  this.getReasonsForReport_(), students);
                //return new ria.async.DeferredData(res);
                //return this.studentService.getClassStudents(classId, selectedGradingPeriodId)
                //    .attach(this.validateResponse_())
                //    .then(function(students){
                //        return new chlk.models.reports.SubmitComprehensiveProgressViewData(classId,
                //            selectedGradingPeriodId, startDate, endDate,  this.getReasonsForReport_(), students);
                //    }, this);
            },

            ArrayOf(chlk.models.attendance.AttendanceReason), function getReasonsForReport_(){
                var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
                //return reasons.filter(function(item){
                //    var len = (item.getAttendanceLevelReasons() || []).filter(function(reason){
                //        /*return reason.getLevel() == 'A' || reason.getLevel() == 'AO' ||
                //         reason.getLevel() == 'H' || reason.getLevel() == 'HO';*/
                //        //return reason.getLevel() == 'A';
                //    }).length;
                //    return !!len;
                //});
                return reasons;
            }
        ])
});
