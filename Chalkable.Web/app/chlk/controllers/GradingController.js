REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ReportingService');
REQUIRE('chlk.services.CalendarService');

REQUIRE('chlk.activities.grading.TeacherSettingsPage');
REQUIRE('chlk.activities.grading.GradingClassSummaryPage');
REQUIRE('chlk.activities.grading.GradingClassStandardsPage');
REQUIRE('chlk.activities.grading.GradingTeacherClassSummaryPage');
REQUIRE('chlk.activities.grading.GradingStudentSummaryPage');
REQUIRE('chlk.activities.grading.GradingStudentClassSummaryPage');
REQUIRE('chlk.activities.grading.GradingClassSummaryGridPage');
REQUIRE('chlk.activities.grading.GradingClassStandardsGridPage');
REQUIRE('chlk.activities.grading.GradeBookReportDialog');
REQUIRE('chlk.activities.grading.WorksheetReportDialog');
REQUIRE('chlk.activities.grading.ProgressReportDialog');
REQUIRE('chlk.activities.grading.StudentAvgPopupDialog');
REQUIRE('chlk.activities.grading.FinalGradesPage');

REQUIRE('chlk.models.grading.GradingSummaryGridSubmitViewData');
REQUIRE('chlk.models.grading.SubmitGradeBookReportViewData');
REQUIRE('chlk.models.grading.SubmitProgressReportViewData');
REQUIRE('chlk.models.grading.SubmitWorksheetReportViewData');

NAMESPACE('chlk.controllers', function (){

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

            Array, function getClassForGrading_(withAll_, forCurrentMp_){
                return this.classService.getClassesForTopBar(withAll_, forCurrentMp_);
            },

            //TODO: refactor
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function teacherSettingsAction(classId_){
                var classes = this.getClassForGrading_();
                var model = new chlk.models.setup.TeacherSettings();
                var classBarMdl= new chlk.models.classes.ClassesForTopBar(classes, classId_, true);
                model.setTopData(classBarMdl);
                var result;
                if(classId_){
                    result = this.finalGradeService
                        .getFinalGrades(classId_, false)
                        .attach(this.validateResponse_())
                        .then(function(result){
                            var gradesInfo = result.getFinalGradeAnnType(), sum = 0;
                            gradesInfo.forEach(function(item, index){
                                item.setIndex(index);
                                sum += (item.getValue() || 0);
                            });
                            gradesInfo.sort(function(a,b){
                                return b.getValue() > a.getValue();
                            });
                            sum += (result.getAttendance() || 0);
                            sum += (result.getParticipation() || 0);
                            sum += (result.getDiscipline() || 0);
                            model.setPercentsSum(sum);
                            model.setGradingInfo(result);
                            return model;
                        }, this);
                }else{
                    result = new ria.async.DeferredData(model);
                }
                return this.PushView(chlk.activities.grading.TeacherSettingsPage, result);
            },


            chlk.models.schoolYear.GradingPeriod, function getCurrentGradingPeriod(){
                return this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD);
            },

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classes = this.getClassForGrading_(true, true);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
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
                var classes = this.getClassForGrading_(true, true);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
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
                        if(model.isAutoUpdate()){
                            newModel.isAvg = model.isAvg();
                            return new chlk.models.common.SimpleObject(newModel);
                        }
                        newModel.setAutoUpdate(model.isAutoUpdate());
                        newModel.setCategoryId(model.getCategoryId());
                        newModel.setStandardId(model.getStandardId());
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        newModel.setSchoolOptions(schoolOptions);
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM);
                        newModel.setAbleEdit(canEdit);
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
                var classes = this.getClassForGrading_(true, true);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                var alphaGrades = classInfo.getAlphaGrades();
                var gradingComments = this.getContext().getSession().get(ChlkSessionConstants.GRADING_COMMENTS, []);
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId_);
                var result = this.gradingService
                    .getFinalGrades(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getContext().getSession().get('gradingPeriod', {});
                        var canEditDirectValue = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_STUDENT_AVERAGES);
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM);
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

            [chlk.controllers.Permissions([
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_GRADES
            ])],
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryGridTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId_);
                var classes = this.getClassForGrading_(true, true);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
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
                            var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM);
                            model.getCurrentGradingGrid().setSchoolOptions(schoolOptions);
                            model.getCurrentGradingGrid().setAbleEdit(canEdit);
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
                var classes = this.getClassForGrading_(true, true);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                var alphaGrades = classInfo.getAlphaGradesForStandards();
                var result = this.gradingService
                    .getClassStandardsGrid(classId_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        var gradingPeriod = this.getCurrentGradingPeriod();
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM);
                        var summmaryGridViewData = new chlk.models.grading.GradingClassSummaryGridViewData(
                            chlk.models.standard.StandardGradings,
                            gradingPeriod.getId(),
                            topData,
                            null,
                            items,
                            alphaGrades,
                            null,
                            canEdit
                        );
                        return summmaryGridViewData;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsGridPage, result);
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
                        var classes = this.classService.getClassesForTopBar(true, true);
                        var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
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
                        var classes = this.getClassForGrading_(true, true);
                        var topData = new chlk.models.classes.ClassesForTopBar(classes, null);
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
                        var classes = this.classService.getClassesForTopBar(true, true);
                        var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                        model.setTopData(topData);
                        return model;
                    }, this);
                return update_ ? this.UpdateView(chlk.activities.grading.GradingStudentSummaryPage, result, 'chart-update') :
                    this.PushView(chlk.activities.grading.GradingStudentSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.AnnouncementId]],
            function unDropFromPopupAction(announcementId){
                var result = this.announcementService.unDropAnnouncement(announcementId);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.AnnouncementId]],
            function dropFromPopupAction(announcementId){
                var result = this.announcementService.dropAnnouncement(announcementId);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
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

            function getGradeCommentsAction(){
                var result = this.gradingService.getGradeComments().then(function(comments){
                    return new chlk.models.grading.GradingComments(comments);
                });
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
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

            [[chlk.models.grading.Final]],
            function teacherSettingsEditAction(model){
                var finalGradeAnnouncementTypes = [], item,
                    ids = model.getFinalGradeAnnouncementTypeIds().split(','),
                    percents = model.getPercents().split(','),
                    dropLowest = model.getDropLowest().split(','),
                    gradingStyle = model.getGradingStyleByType().split(',');
                ids.forEach(function(id, i){
                    item = {};
                    item.finalGradeAnnouncementTypeId = id;
                    item.percentValue = JSON.parse(percents[i]);
                    item.dropLowest = JSON.parse(dropLowest[i]);
                    item.gradingStyle =JSON.parse(gradingStyle[i]);
                    finalGradeAnnouncementTypes.push(item)
                });

                return this.finalGradeService.update(
                        model.getId(),
                        model.getParticipation(),
                        model.getAttendance(),
                        model.getDropLowestAttendance(),
                        model.getDiscipline(),
                        model.getDropLowestDiscipline(),
                        model.getGradingStyle(),
                        finalGradeAnnouncementTypes,
                        model.isNeedsTypesForClasses()
                    )
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return this.BackgroundNavigate('grading', 'teacherSettings', []);
                    }, this);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function gradeBookReportAction(gradingPeriodId, classId, startDate, endDate){
                if (this.isDemoSchool())
                    return this.ShowMsgBox('Not available for demo', 'Error');
                var res = new ria.async.DeferredData(new chlk.models.grading.GradeBookReportViewData(gradingPeriodId, classId, startDate, endDate));
                return this.ShadeView(chlk.activities.grading.GradeBookReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function worksheetReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.getWorksheetReportInfo(gradingPeriodId, classId, startDate, endDate);
                return this.ShadeView(chlk.activities.grading.WorksheetReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function progressReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.reportingService.getStudentsForReport(classId, gradingPeriodId)
                    .then(function(students){
                        var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
                        var absenceReasons = reasons.filter(function(item){
                            var len = (item.getAttendanceLevelReasons() || []).filter(function(reason){
                                /*return reason.getLevel() == 'A' || reason.getLevel() == 'AO' ||
                                    reason.getLevel() == 'H' || reason.getLevel() == 'HO';*/
                                return reason.getLevel() == 'A';
                            }).length;
                            return !!len;
                        });
                        return new chlk.models.grading.SubmitProgressReportViewData(absenceReasons, students, gradingPeriodId, classId, startDate, endDate);
                    }, this);
                return this.ShadeView(chlk.activities.grading.ProgressReportDialog, res);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.SubmitGradeBookReportViewData]],
            function submitGradeBookReportAction(reportViewData){
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
                    reportViewData.isSuppressStudentName()
                );
                this.BackgroundCloseView(chlk.activities.grading.GradeBookReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.SubmitProgressReportViewData]],
            function downloadProgressReportAction(progressReportViewData){
                progressReportViewData = progressReportViewData.getClassId() ? progressReportViewData
                    : this.getContext().getSession().get('modelForSubmit', null);

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
                    progressReportViewData.getStudentIds(),
                    progressReportViewData.getCommentsList()
                );
                this.BackgroundCloseView(chlk.activities.grading.ProgressReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.SubmitProgressReportViewData]],
            function submitProgressReportAction(model){
                var count = model.getNotSelectedCount();
                if(!count){
                    return this.downloadProgressReportAction(model);
                } else {
                    this.getContext().getSession().set('modelForSubmit', model);
                    this.ShowMsgBox(Msg.Progress_report_msg(count), '', [{
                        text: Msg.Yes.toUpperCase(),
                        controller: 'grading',
                        action: 'downloadProgressReport',
                        params: [model],
                        color: chlk.models.common.ButtonColor.RED.valueOf()
                    }, {
                        text: Msg.Cancel.toUpperCase(),
                        color: chlk.models.common.ButtonColor.GREEN.valueOf()
                    }]);
                }
                return null;
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.SubmitWorksheetReportViewData]],
            function submitWorksheetReportAction(model){
                var len = 0;
                for(var i = 1; i <= 5; i++){
                    if(model['getTitle' + i]() && model['getTitle' + i]().trim()) len++;
                }
                if(model.getAnnouncementIds().split(',').length + len > 8){
                    this.ShowMsgBox(Msg.Worksheet_report_msg, 'fyi.', [{
                        text: Msg.GOT_IT.toUpperCase()
                    }]);
                    return this.UpdateView(chlk.activities.grading.WorksheetReportDialog, new ria.async.DeferredData(new chlk.models.grading.GradeBookReportViewData), 'stop');
                }
                if(model.getSubmitType() == 'submit'){
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
                        model.isOverwriteExisting()
                    );
                    this.BackgroundCloseView(chlk.activities.grading.WorksheetReportDialog);
                    this.getContext().getDefaultView().submitToIFrame(src);
                    return null;
                }
                var res = this.getWorksheetReportInfo(
                    model.getGradingPeriodId(),
                    model.getClassId(),
                    model.getStartDate(),
                    model.getEndDate()
                );
                return this.UpdateView(chlk.activities.grading.WorksheetReportDialog, res, 'grid');
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
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.FinalGradesPage, result, chlk.activities.lib.DontShowLoader());
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
                    )
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, result, chlk.activities.lib.DontShowLoader());
            },

            function updateStudentAvgFromPopupAction(save_){
                var model = this.getContext().getSession().get(ChlkSessionConstants.STUDENT_AVG_MODEL);
                if(!save_)
                    model.setGradingPeriodId(null);
                return this.updateStudentAvgFromModel(model);
            },

            function getWorksheetReportInfo(gradingPeriodId, classId, startDate, endDate){
                var res = this.calendarService.listByDateRange(startDate, endDate, classId)
                    .then(function(announcements){
                        return new ria.async.DeferredData(new chlk.models.grading.GradeBookReportViewData(gradingPeriodId, classId, startDate, endDate, announcements));
                    });
                return res;
            }
        ])
});
