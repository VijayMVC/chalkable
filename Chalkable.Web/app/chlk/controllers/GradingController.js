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

REQUIRE('chlk.activities.classes.grading.ClassProfileGradingItemsGridPage');
REQUIRE('chlk.activities.classes.grading.ClassProfileGradingItemsBoxesPage');
REQUIRE('chlk.activities.classes.grading.ClassProfileGradingStandardsGridPage');
REQUIRE('chlk.activities.classes.grading.ClassProfileGradingStandardsBoxesPage');
REQUIRE('chlk.activities.classes.grading.ClassProfileGradingFinalGradesPage');

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

            [ria.mvc.Inject],
            chlk.services.ClassAnnouncementService, 'classAnnouncementService',


            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            function indexTeacherAction() {
                var classId = this.getCurrentClassId();
                if(classId && classId.valueOf())
                    return this.Redirect('grading', 'summaryGrid', [classId]);

                return this.Redirect('grading', 'summaryAll');
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
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
                        model.setTopData(topData);
                        this.prepareItemsBoxesModel(model, classId_);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassSummaryPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
            ])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodSummaryAction(model){
                var result = this.gradingService
                    .getClassGradingPeriodSummary(model.getClassId(), model.getGradingPeriodId())
                    .attach(this.validateResponse_());
                return this.UpdateView(this.getView().getCurrent().getClass(), result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function standardsTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);

                var result = this.gradingService
                    .getClassStandards(classId_)
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var model = this.createStandardsBoxesModel(result, classId_);
                        var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                        model.setTopData(topData);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsPage, result);
            },

            [chlk.controllers.NotChangedSidebarButton()],
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
                            this.ShowMsgBox('There are no grading items in selected Grading Period', null, null, 'leave-msg');
                        var index = model.getSelectedIndex();
                        if(index || index == 0)
                            resModel.setSelectedIndex(index);
                        resModel.setAvgChanged(!!avgChanged);
                        return resModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(this.getView().getCurrent().getClass(), result, avgChanged ? 'average-change' : 'load-gp');
            },

            [chlk.controllers.NotChangedSidebarButton()],
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
                        studentIds = newModel.getStudents().map(function(item){return item.getStudentInfo().getId().valueOf()});
                        this.getContext().getSession().set(ChlkSessionConstants.STUDENTS_FOR_REPORT, newModel.getStudents().map(function(item){return item.getStudentInfo()}));
                        newModel.setAutoUpdate(model.isAutoUpdate());
                        newModel.setCategoryId(model.getCategoryId());
                        newModel.setStandardId(model.getStandardId());
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        newModel.setSchoolOptions(schoolOptions);
                        var canEdit = model.isAbleEdit();
                        var canEditAvg = canEdit || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_STUDENT_AVERAGES);
                        newModel.setAbleEdit(canEdit);
                        newModel.setAbleEditAvg(canEditAvg);
                        return newModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(this.getView().getCurrent().getClass(), result, model.isAutoUpdate() ? chlk.activities.lib.DontShowLoader() : null);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function finalGradesAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);

                var result = this.gradingService
                    .getFinalGrades(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                        var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                        var alphaGrades = classInfo.getAlphaGrades();
                        model.setTopData(topData);
                        model.setAlphaGrades(alphaGrades);
                        this.prepareFinalGradesModel(model, classId_);
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

            [[chlk.models.grading.GradingClassSummaryGridForCurrentPeriodViewData, chlk.models.id.ClassId, chlk.models.id.SchoolId]],
            function prepareItemsGridModel(model, classId, schoolId, clazz_){
                var alternateScores = this.getContext().getSession().get(ChlkSessionConstants.ALTERNATE_SCORES, []);
                var gradingPeriod = this.getCurrentGradingPeriod();
                var canEdit = false;
                model.setClassId(classId);
                model.setGradingPeriodId(gradingPeriod.getId());
                model.setAlternateScores(alternateScores);
                model.setHasAccessToLE(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM));
                var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                if(model.getCurrentGradingGrid()){
                    canEdit = !this.isPageReadonly_('MAINTAIN_CLASSROOM', 'MAINTAIN_CLASSROOM_ADMIN', clazz_);
                    var canEditAvg = canEdit ||  !this.isPageReadonly_('MAINTAIN_STUDENT_AVERAGES', 'MAINTAIN_CLASSROOM_ADMIN', clazz_);
                    model.getCurrentGradingGrid().setSchoolOptions(schoolOptions);
                    model.getCurrentGradingGrid().setAbleEdit(canEdit);
                    model.getCurrentGradingGrid().setAbleEditAvg(canEditAvg);
                    var students = model.getCurrentGradingGrid().getStudents().map(function (item){return item.getStudentInfo()});
                    this.getContext().getSession().set(ChlkSessionConstants.STUDENTS_FOR_REPORT, students);
                    this.getContext().getSession().set(ChlkSessionConstants.INCLUDE_WITHDRAWN_STUDENTS, model.getCurrentGradingGrid().isIncludeWithdrawnStudents());
                    studentIds = students.map(function(item){return item.getId().valueOf()});
                }
                var gradingComments = this.getContext().getSession().get(ChlkSessionConstants.GRADING_COMMENTS, []);

                if(this.userIsAdmin() && schoolId != null)
                    for(var i = gradingComments.length - 1; i >= 0; i--)
                        if(gradingComments[i].getSchoolId() != schoolId)
                            gradingComments.splice(i, 1);

                model.setGradingComments(gradingComments);

                model.setAbleEdit(canEdit);
            },

            [[chlk.models.grading.GradingClassStandardsGridForCurrentPeriodViewData, chlk.models.id.ClassId]],
            function prepareStandardsGridModel(model, classId, clazz_){
                var gradingPeriod = this.getCurrentGradingPeriod();
                var alphaGrades = model.getAlphaGrades();
                var canEdit = false;
                model.setClassId(classId);
                model.setGradingPeriodId(gradingPeriod.getId());
                var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                if(model.getCurrentGradingGrid()){
                    canEdit = !this.isPageReadonly_('MAINTAIN_CLASSROOM', 'MAINTAIN_CLASSROOM_ADMIN', clazz_);
                    model.getCurrentGradingGrid().setSchoolOptions(schoolOptions);
                    model.getCurrentGradingGrid().setAbleEdit(canEdit);
                    model.getCurrentGradingGrid().setGradable(alphaGrades && (alphaGrades.length || false) && canEdit);
                }
                model.setAbleEdit(canEdit);
                model.setAblePostStandards(canEdit);
                model.setHasAccessToLE(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM));
            },

            [[chlk.models.grading.GradingClassSummaryForCurrentPeriodViewData, chlk.models.id.ClassId]],
            function prepareItemsBoxesModel(model, classId){
                var gradingPeriod = this.getCurrentGradingPeriod();
                model.setClassId(classId);
                model.setGradingPeriodId(gradingPeriod.getId());
                model.setHasAccessToLE(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM));
            },

            [[ArrayOf(chlk.models.grading.GradingClassStandardsItems), chlk.models.id.ClassId]],
            chlk.models.grading.GradingClassSummary, function createStandardsBoxesModel(standards, classId){
                var model = new chlk.models.grading.GradingClassSummary();
                standards.forEach(function(mpData){
                    mpData.getItems().forEach(function(item){
                        item.setClassId(classId);
                    });
                });
                model.setClassId(classId);
                model.setSummaryPart(new chlk.models.grading.GradingClassSummaryPart(standards));
                var gradingPeriod = this.getCurrentGradingPeriod();
                model.setGradingPeriodId(gradingPeriod.getId());
                model.setHasAccessToLE(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM));
                return model;
            },

            [[chlk.models.grading.FinalGradesViewData, chlk.models.id.ClassId]],
            function prepareFinalGradesModel(model, classId, clazz_){
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId);
                var canEdit = !this.isPageReadonly_('MAINTAIN_CLASSROOM', 'MAINTAIN_CLASSROOM_ADMIN', clazz_);
                var canEditDirectValue = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_STUDENT_AVERAGES) || canEdit;
                var gradingComments = this.getContext().getSession().get(ChlkSessionConstants.GRADING_COMMENTS, []);
                var gradingPeriod = this.getCurrentGradingPeriod();
                model.setClassId(classId);
                model.setGradingPeriodId(gradingPeriod.getId());
                model.setGradingComments(gradingComments);
                model.setAbleEdit(canEdit);
                model.setAbleEditDirectValue(canEditDirectValue);
                model.setHasAccessToLE(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM));
            },

            /* CLASS PROFILE */

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function finalGradesClassProfileAction(classId){

                var result = ria.async.wait(
                    this.gradingService.getFinalGrades(classId),
                    this.classService.getGradingFinalGradesSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var gradingModel = result[0], classModel = result[1];
                        gradingModel.setAlphaGrades(classModel.getAlphaGrades() || []);
                        this.prepareFinalGradesModel(gradingModel, classId, classModel);
                        gradingModel.setInProfile(true);
                        classModel.setGradingPart(gradingModel);

                        var res = new chlk.models.classes.BaseGradingClassProfileViewData(
                            this.getCurrentRole(), classModel, this.getUserClaims_(),
                            this.isAssignedToClass_(classId), chlk.models.classes.GradingPageTypeEnum.FINAL_GRADES
                        );

                        return res;
                    }, this);
                return this.PushView(chlk.activities.classes.grading.ClassProfileGradingFinalGradesPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function summaryGridClassProfileAction(classId){
                needStudentReverse = false;
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId);
                var result = ria.async.wait(
                    this.gradingService.getClassSummaryGrid(classId),
                    this.classService.getGradingItemsGridSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var gradingModel = result[0], classModel = result[1];
                        gradingModel.setAlphaGrades(classModel.getAlphaGrades() || []);
                        var schoolYear = classModel && classModel.getSchoolYear();
                        var schoolId = schoolYear && schoolYear.getSchoolId();
                        this.prepareItemsGridModel(gradingModel, classId, schoolId, classModel);
                        gradingModel.setInProfile(true);
                        classModel.setGradingPart(gradingModel);

                        var res = new chlk.models.classes.BaseGradingClassProfileViewData(
                            this.getCurrentRole(), classModel, this.getUserClaims_(),
                            this.isAssignedToClass_(classId), chlk.models.classes.GradingPageTypeEnum.ITEMS_GRID
                        );

                        return res;
                    }, this);
                return this.PushView(chlk.activities.classes.grading.ClassProfileGradingItemsGridPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function standardsGridClassProfileAction(classId){
                var result = ria.async.wait(
                    this.gradingService.getClassStandardsGrids(classId),
                    this.classService.getGradingStandardsGridSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var gradingModel = result[0], classModel = result[1];
                        gradingModel.setAlphaGrades(classModel.getAlphaGradesForStandards() || []);
                        this.prepareStandardsGridModel(gradingModel, classId, classModel);
                        gradingModel.setInProfile(true);
                        classModel.setGradingPart(gradingModel);

                        var res = new chlk.models.classes.BaseGradingClassProfileViewData(
                            this.getCurrentRole(), classModel, this.getUserClaims_(),
                            this.isAssignedToClass_(classId), chlk.models.classes.GradingPageTypeEnum.STANDARDS_GRID
                        );

                        return res;
                    }, this);
                return this.PushView(chlk.activities.classes.grading.ClassProfileGradingStandardsGridPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function summaryBoxesClassProfileAction(classId){
                var result = ria.async.wait(
                    this.gradingService.getClassSummary(classId),
                    this.classService.getGradingItemsBoxesSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var gradingModel = result[0], classModel = result[1];
                        this.prepareItemsBoxesModel(gradingModel, classId);
                        gradingModel.setInProfile(true);
                        classModel.setGradingPart(gradingModel);

                        var res = new chlk.models.classes.BaseGradingClassProfileViewData(
                            this.getCurrentRole(), classModel, this.getUserClaims_(),
                            this.isAssignedToClass_(classId), chlk.models.classes.GradingPageTypeEnum.ITEMS_BOXES
                        );

                        return res;
                    }, this);
                return this.PushView(chlk.activities.classes.grading.ClassProfileGradingItemsBoxesPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function standardsBoxesClassProfileAction(classId){
                var result = ria.async.wait(
                    this.gradingService.getClassStandards(classId),
                    this.classService.getGradingStandardsBoxesSummary(classId)
                )
                    .attach(this.validateResponse_())
                    .then(function(result){
                        var classModel = result[1];
                        var gradingModel = this.createStandardsBoxesModel(result[0], classId);
                        gradingModel.setInProfile(true);
                        classModel.setGradingPart(gradingModel);

                        var res = new chlk.models.classes.BaseGradingClassProfileViewData(
                            this.getCurrentRole(), classModel, this.getUserClaims_(),
                            this.isAssignedToClass_(classId), chlk.models.classes.GradingPageTypeEnum.STANDARDS_BOXES
                        );

                        return res;
                    }, this);
                return this.PushView(chlk.activities.classes.grading.ClassProfileGradingStandardsBoxesPage, result);
            },

            /* END CLASS PROFILE */

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
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

                var result = this.gradingService
                    .getClassSummaryGrid(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        model.setTopData(topData);
                        model.setAlphaGrades(alphaGrades);

                        this.prepareItemsGridModel(model, classId_, null);

                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassSummaryGridPage, result);
            },

            [[chlk.models.announcement.SubmitDroppedAnnouncementViewData]],
            function setAnnouncementDroppedAction(model){
                (model.isDropped()
                        ? this.classAnnouncementService.dropAnnouncement(model.getAnnouncementId())
                        : this.classAnnouncementService.unDropAnnouncement(model.getAnnouncementId())
                )
                .attach(this.validateResponse_())
                .then(function(data){
                        var redirectModel = new chlk.models.grading.GradingSummaryGridSubmitViewData(model.getClassId(), model.getGradingPeriodId(), true, false, model.getStandardId(), model.getCategoryId(), true);
                        this.BackgroundNavigate('grading', 'loadGradingPeriodGridSummary', [redirectModel]);
                }, this);
               return null;
            },

            [chlk.controllers.NotChangedSidebarButton()],
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
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN,
                    chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
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
                        model.setAlphaGrades(alphaGrades);
                        model.setTopData(topData);
                        this.prepareStandardsGridModel(model, classId_);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsGridPage, result);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodGridStandardsAction(model){
                var result = this.gradingService
                    .getClassStandardsGrid(
                        model.getClassId(),
                        model.getGradingPeriodId()
                    )
                    .then(function(newModel){
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        newModel.setSchoolOptions(schoolOptions);
                        newModel.setAbleEdit(model.isAbleEdit());
                        newModel.setGradable(model.isGradable());
                        return newModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(this.getView().getCurrent().getClass(), result);
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

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.AnnouncementId]],
            function showChartAction(announcementId){
                var result = this.gradingService.getItemGradingStat(announcementId)
                    .then(function(model){
                        return new chlk.models.common.SimpleObject(model);
                    });
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.NotChangedSidebarButton()],
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

            [chlk.controllers.NotChangedSidebarButton()],
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

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function gradeBookReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', null, null, 'error'), null;
                var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.GRADE_BOOK_REPORT) ||
                    this.hasUserPermission_(chlk.models.people.UserPermissionEnum.GRADE_BOOK_REPORT_CLASSROOM);
                var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                var res = new ria.async.DeferredData(new chlk.models.reports.GradeBookReportViewData(gradingPeriodId, classId, startDate, endDate, students, null, ableDownload, isAbleToReadSSNumber));
                return this.ShadeView(chlk.activities.reports.GradeBookReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function birthdayReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', null, null, 'error'), null;
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT) ||
                    this.hasUserPermission_(chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT_CLASSROOM);
                //var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                var model = new chlk.models.reports.BirthdayReportViewData();
                model.setGradingPeriodId(gradingPeriodId);
                model.setClassId(classId);
                model.setStartDate(startDate);
                model.setEndDate(endDate);
                model.setAbleDownload(ableDownload);
               // model.setIsAbleToReadSSNumber(isAbleToReadSSNumber);
                var res = new ria.async.DeferredData(model);
                return this.ShadeView(chlk.activities.reports.BirthdayReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function seatingChartReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', null, null, 'error'), null;
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.SEATING_CHART_REPORT);
                var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                var res = new ria.async.DeferredData(new chlk.models.reports.BaseReportViewData(classId, gradingPeriodId, startDate, endDate, null, ableDownload, isAbleToReadSSNumber));
                return this.ShadeView(chlk.activities.reports.SeatingChartReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
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
                        var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                        var res = new chlk.models.reports.GradeVerificationReportViewData(periods, averages, students, classId, gradingPeriodId, startDate, endDate, ableDownload, isAbleToReadSSNumber);
                        res.setIncludeWithdrawnStudents(includeWithdrawn);
                        return res;
                    }, this);
                return this.ShadeView(chlk.activities.reports.GradeVerificationReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function worksheetReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.getWorksheetReportInfo_(gradingPeriodId, classId, startDate, endDate);
                return this.ShadeView(chlk.activities.reports.WorksheetReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
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
                        var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                        return new chlk.models.reports.SubmitProgressReportViewData(attendanceReasons, res, gradingPeriodId, classId, startDate, endDate, ableDownload, isAbleToReadSSNumber);
                    }, this);
                return this.ShadeView(chlk.activities.reports.ProgressReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function comprehensiveProgressReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.getComprehensiveProgressReportInfo_(gradingPeriodId, classId, startDate, endDate);
                return this.ShadeView(chlk.activities.reports.ComprehensiveProgressReportDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function missingAssignmentsReportAction(gradingPeriodId, classId, startDate, endDate){
                var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                var alternateScores = this.getContext().getSession().get(ChlkSessionConstants.ALTERNATE_SCORES, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MISSING_ASSIGNMENTS_REPORT);
                var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                var model = new chlk.models.reports.SubmitMissingAssignmentsReportViewData(classId,
                    gradingPeriodId, startDate, endDate, students, alternateScores, ableDownload, isAbleToReadSSNumber);
                return this.ShadeView(chlk.activities.reports.MissingAssignmentsReportDialog, new ria.async.DeferredData(model));
            },



            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.MISSING_ASSIGNMENTS_REPORT])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitMissingAssignmentsReportViewData]],
            function submitMissingAssignmentsReportAction(reportViewData){
                if (reportViewData.getStartDate().compare(reportViewData.getEndDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", null, null, 'leave-msg'), null;
                }

                var result = this.reportingService.submitMissingAssignmentsReport(
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
                    )
                    .attach(this.validateResponse_())
                    .then(function () {
                        this.BackgroundCloseView(chlk.activities.reports.MissingAssignmentsReportDialog);
                    }, this)
                    .thenBreak();

                return this.UpdateView(chlk.activities.reports.MissingAssignmentsReportDialog, result);
            },

            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitComprehensiveProgressViewData]],
            function submitComprehensiveProgressReportAction(reportViewData){
                if (reportViewData.getStartDate().compare(reportViewData.getEndDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", null, null, 'leave-msg'), null;
                }

                var result = this.reportingService.submitComprehensiveProgressReport(
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
                    )
                    .attach(this.validateResponse_())
                    .then(function () {
                        this.BackgroundCloseView(chlk.activities.reports.ComprehensiveProgressReportDialog);
                    }, this)
                    .thenBreak();

                return this.UpdateView(chlk.activities.reports.ComprehensiveProgressReportDialog, result);
            },

            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.GRADE_VERIFICATION_REPORT])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitGradeVerificationReportViewData]],
            function submitGradeVerificationReportAction(reportViewData){
                if (!reportViewData.getStudentAverageIds()){
                    return this.ShowAlertBox("You should select at least one graded item", null, null, 'leave-msg'), null;
                }

                var result = this.reportingService.submitGradeVerificationReport(
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
                    )
                    .attach(this.validateResponse_())
                    .then(function () {
                        this.BackgroundCloseView(chlk.activities.reports.GradeVerificationReportDialog);
                    }, this)
                    .thenBreak();

                return this.UpdateView(chlk.activities.reports.GradeVerificationReportDialog, result);
            },

            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.GRADE_BOOK_REPORT])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitGradeBookReportViewData]],
            function submitGradeBookReportAction(reportViewData){

                if (reportViewData.getStartDate().compare(reportViewData.getEndDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", null, null, 'leave-msg'), null;
                }

                var result = this.reportingService.submitGradeBookReport(
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
                    )
                    .attach(this.validateResponse_())
                    .then(function () {
                        this.BackgroundCloseView(chlk.activities.reports.GradeBookReportDialog);
                    }, this)
                    .thenBreak();

                return this.UpdateView(chlk.activities.reports.GradeBookReportDialog, result);
            },


            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT, chlk.models.people.UserPermissionEnum.BIRTHDAY_LISTING_REPORT_CLASSROOM]
            ])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitBirthdayReportViewData]],
            function submitBirthdayReportAction(reportViewData){

                if (reportViewData.getStartDate() || reportViewData.getEndDate()){

                    if (!reportViewData.getStartDate())
                        return this.ShowAlertBox("Please provide report start date", null, null, 'leave-msg'), null;
                    if (!reportViewData.getEndDate())
                        return this.ShowAlertBox("Please provide report end date", null, null, 'leave-msg'), null;

                    if (reportViewData.getStartDate().compare(reportViewData.getEndDate()) > 0){
                        return this.ShowAlertBox("Report start time should be less than report end time", null, null, 'leave-msg'), null;
                    }
                }


                if (reportViewData.getStartMonth() > reportViewData.getEndMonth()){
                    return this.ShowAlertBox("Start Month must be less than or equal to End Month", null, null, 'leave-msg'), null;
                }

                var result = this.reportingService.submitBirthdayReport(
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
                    )
                    .attach(this.validateResponse_())
                        .then(function () {
                        this.BackgroundCloseView(chlk.activities.reports.BirthdayReportDialog);
                    }, this)
                    .thenBreak();

                return this.UpdateView(chlk.activities.reports.BirthdayReportDialog, result);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitProgressReportViewData]],
            function downloadProgressReportAction(progressReportViewData){
                progressReportViewData = progressReportViewData.getClassId() ? progressReportViewData
                    : this.getContext().getSession().get('modelForSubmit', null);

                var result = this.reportingService
                    .setStudentProgressReportComments(progressReportViewData.getClassId(),
                        progressReportViewData.getGradingPeriodId(),
                        progressReportViewData.getCommentsList())
                    .thenCall(this.reportingService.submitProgressReport, [
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
                    ])
                    .attach(this.validateResponse_())
                    .then(function () {
                        this.BackgroundCloseView(chlk.activities.reports.ProgressReportDialog);
                    }, this)
                    .thenBreak();

                return this.UpdateView(chlk.activities.reports.ProgressReportDialog, result);
            },


            [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.PROGRESS_REPORT])],
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitProgressReportViewData]],
            function submitProgressReportAction(model){
                if(!model.getAbsenceReasonIds()){
                    this.ShowMsgBox(Msg.Progress_Report_No_Reasons_msg, null, null, 'leave-msg');
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
            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.reports.SubmitWorksheetReportViewData]],
            function submitWorksheetReportAction(model){
                if(model.getSubmitType() == 'submit'){
                    var len = 0;
                    for(var i = 1; i <= 5; i++){
                        if(model['getTitle' + i]() && model['getTitle' + i]().trim()) len++;
                    }
                    if(model.getAnnouncementIds().split(',').length + len > 8){
                        return this.ShowAlertBox(Msg.Worksheet_report_msg, null, null, 'leave-msg'), null;
                        /*this.ShowMsgBox(Msg.Worksheet_report_msg, 'fyi.', [{
                            text: Msg.GOT_IT.toUpperCase()
                        }]);
                        return this.UpdateView(chlk.activities.reports.WorksheetReportDialog, new ria.async.DeferredData(new chlk.models.reports.GradeBookReportViewData), 'stop');*/
                    }

                    if (model.getStartDate().compare(model.getEndDate()) > 0){
                        return this.ShowAlertBox("Report start time should be less than report end time", null, null, 'leave-msg'), null;
                    }

                    var result = this.reportingService.submitWorksheetReport(
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
                        )
                        .attach(this.validateResponse_())
                        .then(function () {
                            this.BackgroundCloseView(chlk.activities.reports.WorksheetReportDialog);
                        }, this)
                        .thenBreak();

                    return this.UpdateView(chlk.activities.reports.WorksheetReportDialog, result);
                }
                var res = this.getWorksheetReportInfo_(
                    model.getGradingPeriodId(),
                    model.getClassId(),
                    model.getStartDate(),
                    model.getEndDate()
                );
                return this.UpdateView(chlk.activities.reports.WorksheetReportDialog, res, 'grid');
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.grading.ShortStudentAverageInfo]],
            function updateStudentAvgAction(model){
                if((parseFloat(model.getOldValue()) == parseFloat(model.getAverageValue()) || !model.getAverageValue())
                        && (!model.isExempt() || model.isOldExempt()))
                    return this.updateStudentAvgFromModel(model);

                this.getContext().getSession().set(ChlkSessionConstants.STUDENT_AVG_MODEL, model);
                return this.ShadeView(chlk.activities.grading.StudentAvgPopupDialog, new ria.async.DeferredData(new chlk.models.Success));
            },

            [chlk.controllers.NotChangedSidebarButton()],
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
                        this.BackgroundUpdateView(this.getView().getCurrent().getClass(), new chlk.models.grading.StudentFinalGradeViewData(), chlk.activities.lib.DontShowLoader());

                        throw error;
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId]],
            function postStandardsAction(classId, gradingPeriodId) {
                var result = this.gradingService
                    .postStandards(classId, gradingPeriodId)
                    .thenCall(this.ShowAlertBox, ['Standards posted successfully.', null, null, 'ok'])
                    .thenBreak();

                return this.UpdateView(this.getView().getCurrent().getClass(), result);
            },

            function updateStudentAvgFromModel(model){
                this.BackgroundCloseView(chlk.activities.grading.StudentAvgPopupDialog);
                var activityClass = chlk.activities.grading.GradingClassSummaryGridPage;
                if(!model.getGradingPeriodId())
                    return this.UpdateView(activityClass, new ria.async.DeferredData(model), chlk.activities.lib.DontShowLoader());

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
                    )
                    .attach(this.validateResponse_())
                    .then(function(newModel){
                        newModel.setGradingPeriodId(model.getGradingPeriodId());
                        return newModel;
                    });
                return this.UpdateView(activityClass, result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.NotChangedSidebarButton()],
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

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId, chlk.models.id.SchoolPersonId]],
            function standardPopupAction(classId, gradingPeriodId, standardId, studentId){
                var result = this.gradingService
                    .getStudentClassGradingByStandard(classId, gradingPeriodId, standardId, studentId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        var model = new chlk.models.grading.StandardsPopupViewData(studentId, standardId, items);
                        this.BackgroundUpdateView(chlk.activities.grading.GradingClassStandardsGridPage, model, 'popup-items');
                    }, this);

                return null;
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function getWorksheetReportInfo_(gradingPeriodId, classId, startDate, endDate){
                var res = this.calendarService.listClassAnnsByDateRange(startDate, endDate, classId)
                    .then(function(announcements){
                        var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
                        var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.WORKSHEET_REPORT);
                        var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                        return new ria.async.DeferredData(new chlk.models.reports.GradeBookReportViewData(gradingPeriodId, classId,
                            startDate, endDate, students, announcements, ableDownload, isAbleToReadSSNumber));
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
                            var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                            return new chlk.models.reports.SubmitComprehensiveProgressViewData(classId,
                                selectedGradingPeriodId, startDate, endDate, data[0], data[1], students, ableDownload, isAbleToReadSSNumber)
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
