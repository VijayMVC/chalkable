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
REQUIRE('chlk.activities.grading.StudentAvgPopupDialog');

REQUIRE('chlk.models.grading.GradingSummaryGridSubmitViewData');
REQUIRE('chlk.models.grading.SubmitGradeBookReportViewData');
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

            //TODO: refactor
            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function teacherSettingsAction(classId_){
                var classes = this.classService.getClassesForTopBar();
                var model = new chlk.models.setup.TeacherSettings();
                var classBarMdl= new chlk.models.classes.ClassesForTopBar(classes, classId_, true);
                model.setTopData(classBarMdl);
                var result;
                if(classId_){
                    result = this.finalGradeService
                        .getFinalGrades(classId_, false)
                        .attach(this.validateResponse_())
                        .then(function(result){
                            var gradesInfo = result.getFinalGradeAnnType(), sum=0;
                            gradesInfo.forEach(function(item, index){
                                item.setIndex(index);
                                sum+=(item.getValue() || 0);
                            });
                            gradesInfo.sort(function(a,b){
                                return b.getValue() > a.getValue();
                            });
                            sum+=(result.getAttendance() || 0);
                            sum+=(result.getParticipation() || 0);
                            sum+=(result.getDiscipline() || 0);
                            model.setPercentsSum(sum);
                            model.setGradingInfo(result);
                            return model;
                        }, this);
                }else{
                    result = new ria.async.DeferredData(model);
                }
                return this.PushView(chlk.activities.grading.TeacherSettingsPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classes = this.classService.getClassesForTopBar(true, true);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                var result = this.gradingService
                    .getClassSummary(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, {});
                        model.setTopData(topData);
                        model.setGradingPeriodId(gradingPeriod.getId());
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodSummaryAction(model){
                var result = this.gradingService
                    .getClassGradingPeriodSummary(model.getClassId(), model.getGradingPeriodId())
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function standardsTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classes = this.classService.getClassesForTopBar(true, true);
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
                        var gradingPeriod = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, {});
                        model.setGradingPeriodId(gradingPeriod.getId());
                        model.setAction('standards');
                        model.setGridAction('standardsGrid');
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.grading.GradingSummaryGridSubmitViewData]],
            function loadGradingPeriodGridSummaryAction(model){
                var result = this.gradingService
                    .getClassSummaryGridForPeriod(model.getClassId(), model.getGradingPeriodId(), model.getStandardId(), model.getCategoryId(), model.isNotCalculateGrid())
                    .then(function(newModel){
                        newModel.setAutoUpdate(model.isAutoUpdate());
                        newModel.setCategoryId(model.getCategoryId());
                        newModel.setStandardId(model.getStandardId());
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        newModel.setSchoolOptions(schoolOptions);
                        return newModel;
                    }, this)
                    .attach(this.validateResponse_());
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, result, model.isAutoUpdate() ? chlk.activities.lib.DontShowLoader() : null);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryGridTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId_);
                var classes = this.classService.getClassesForTopBar(true, true);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                var alphaGrades = classInfo.getAlphaGrades();
                var alternateScores = this.getContext().getSession().get(ChlkSessionConstants.ALTERNATE_SCORES, []);
                var gradingComments = this.getContext().getSession().get(ChlkSessionConstants.GRADING_COMMENTS, []);
                var result = this.gradingService
                    .getClassSummaryGrid(classId_)
                    .attach(this.validateResponse_())
                    .then(function(model){
                        var gradingPeriod = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, {});
                        model.setAlphaGrades(alphaGrades);
                        model.setTopData(topData);
                        model.setGradingPeriodId(gradingPeriod.getId());
                        model.setAlternateScores(alternateScores);
                        model.setGradingComments(gradingComments);
                        var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                        model.getCurrentGradingGrid() && model.getCurrentGradingGrid().setSchoolOptions(schoolOptions);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassSummaryGridPage, result);
            },

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

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function standardsGridTeacherAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var classes = this.classService.getClassesForTopBar(true, true);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                var alphaGrades = classInfo.getAlphaGradesForStandards();
                var result = this.gradingService
                    .getClassStandardsGrid(classId_)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        var gradingPeriod = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, {});
                        var model = new chlk.models.grading.GradingClassSummaryGridViewData(chlk.models.standard.StandardGradings,
                            gradingPeriod.getId(), topData, null, items, alphaGrades);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingClassStandardsGridPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function summaryStudentAction(classId_){
                if(!classId_ || !classId_.valueOf())
                    return this.BackgroundNavigate('grading', 'summaryAll', []);
                var studentId = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON).getId();
                var result = this.gradingService
                    .getStudentsClassSummary(studentId, classId_)
                    .then(function(model){
                        model.getItems().forEach(function(mpData){
                            mpData.getItems().forEach(function(item){
                                item.setClassId(classId_);
                            });
                        });
                        var classes = this.classService.getClassesForTopBar(true, true);
                        var topData = new chlk.models.classes.ClassesForTopBar(classes, classId_);
                        var gradingPeriod = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, {});
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
                var teacherId = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON).getId();
                var result = this.gradingService
                    .getTeacherSummary(teacherId)
                    .attach(this.validateResponse_())
                    .then(function(items){
                        var classes = this.classService.getClassesForTopBar(true, true);
                        var topData = new chlk.models.classes.ClassesForTopBar(classes, null);
                        var model = new chlk.models.grading.GradingTeacherClassSummaryViewDataList(topData, items);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.grading.GradingTeacherClassSummaryPage, result);
            },

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId, Boolean]],
            function summaryAllStudentAction(classId_, update_){
                var studentId = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON).getId();
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

            [[chlk.models.id.AnnouncementId]],
            function unDropFromPopupAction(announcementId){
                var result = this.announcementService.unDropAnnouncement(announcementId);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [[chlk.models.id.AnnouncementId]],
            function dropFromPopupAction(announcementId){
                var result = this.announcementService.dropAnnouncement(announcementId);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [[chlk.models.id.AnnouncementId]],
            function showChartAction(announcementId){
                var result = this.gradingService.getItemGradingStat(announcementId);
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            function getGradeCommentsAction(){
                var result = this.gradingService.getGradeComments().then(function(comments){
                    return new chlk.models.grading.GradingComments(comments);
                });
                return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
            },

            [[chlk.models.id.ClassId, chlk.models.id.GradingPeriodId, chlk.models.id.StandardId, chlk.models.id.AnnouncementTypeGradingId]],
            function postGradeBookAction(classId, gradingPeriodId, standardId_, categoryId_){
                var res = this.gradingService.postGradeBook(classId, gradingPeriodId)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var model = new chlk.models.grading.GradingSummaryGridSubmitViewData(classId, gradingPeriodId, true, false, standardId_, categoryId_);
                        this.BackgroundNavigate('grading', 'loadGradingPeriodGridSummary', [model]);
                    }, this);
                return null;
            },

            [[chlk.models.grading.Final]],
            function teacherSettingsEditAction(model){
                var finalGradeAnnouncementTypes = [], item, ids = model.getFinalGradeAnnouncementTypeIds().split(','),
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

                this.finalGradeService.update(
                        model.getId(),
                        model.getParticipation(),
                        model.getAttendance(),
                        model.getDropLowestAttendance(),
                        model.getDiscipline(),
                        model.getDropLowestDiscipline(),
                        model.getGradingStyle(),
                        finalGradeAnnouncementTypes,
                        model.isNeedsTypesForClasses())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return this.BackgroundNavigate('grading', 'teacherSettings', []);
                    }, this);
                return this.ShadeLoader();
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function gradeBookReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = new ria.async.DeferredData(new chlk.models.grading.GradeBookReportViewData(gradingPeriodId, classId, startDate, endDate));
                return this.ShadeView(chlk.activities.grading.GradeBookReportDialog, res);
            },

            [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
            function worksheetReportAction(gradingPeriodId, classId, startDate, endDate){
                var res = this.getWorksheetReportInfo(gradingPeriodId, classId, startDate, endDate);
                return this.ShadeView(chlk.activities.grading.WorksheetReportDialog, res);
            },

            [[chlk.models.grading.SubmitGradeBookReportViewData]],
            function submitGradeBookReportAction(model){
                var src = this.reportingService.submitGradeBookReport(model.getClassId(), model.getGradingPeriodId(), model.getStartDate(),
                    model.getEndDate(), model.getReportType(), model.getOrderBy(), model.getIdToPrint(), model.getFormat(),
                    model.isDisplayLetterGrade(), model.isDisplayTotalPoints(), model.isDisplayStudentAverage(),
                    model.isIncludeWithdrawnStudents(), model.isIncludeNonGradedActivities(), model.isSuppressStudentName());
                this.BackgroundCloseView(chlk.activities.grading.GradeBookReportDialog);
                this.getContext().getDefaultView().submitToIFrame(src);
                return null;
            },

            [[chlk.models.grading.SubmitWorksheetReportViewData]],
            function submitWorksheetReportAction(model){
                if(model.getSubmitType() == 'submit'){
                    var src = this.reportingService.submitWorksheetReport(model.getClassId(), model.getGradingPeriodId(), model.getStartDate(),
                        model.getEndDate(), model.getIdToPrint(), model.getAnnouncementIds(), model.getTitle1(), model.getTitle2(), model.getTitle3(),
                        model.getTitle4(), model.getTitle5(), model.isPrintAverage(), model.isPrintLetterGrade(), model.isPrintScores(),
                        model.isPrintStudent(), model.isWorkingFilter(), model.isAppendToExisting(), model.isOverwriteExisting());
                    this.BackgroundCloseView(chlk.activities.grading.WorksheetReportDialog);
                    this.getContext().getDefaultView().submitToIFrame(src);
                    return null;
                }
                var res = this.getWorksheetReportInfo(model.getGradingPeriodId(), model.getClassId(), model.getStartDate(), model.getEndDate());
                return this.UpdateView(chlk.activities.grading.WorksheetReportDialog, res, 'grid');
            },

            [[chlk.models.grading.ShortStudentAverageInfo]],
            function updateStudentAvgAction(model){
                if((parseFloat(model.getOldValue()) == parseFloat(model.getAverageValue()) || !model.getAverageValue())
                        && (!model.isExempt() || model.isOldExempt()))
                    return this.updateStudentAvgFromModel(model);

                this.getContext().getSession().set(ChlkSessionConstants.STUDENT_AVG_MODEL, model);
                return this.ShadeView(chlk.activities.grading.StudentAvgPopupDialog, new ria.async.DeferredData(new chlk.models.Success));
            },

            function updateStudentAvgFromModel(model){
                var result = this.gradingService
                    .updateStudentAverage(
                        this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CLASS_ID, null),
                        model.getStudentId(),
                        model.getGradingPeriodId(),
                        model.getAverageId(),
                        model.getAverageValue(),
                        model.isExempt(),
                        JSON.parse(model.getCodesString())
                    )
                    .attach(this.validateResponse_());
                this.BackgroundCloseView(chlk.activities.grading.StudentAvgPopupDialog);
                return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, result, chlk.activities.lib.DontShowLoader());
            },

            function updateStudentAvgFromPopupAction(){
                var model = this.getContext().getSession().get(ChlkSessionConstants.STUDENT_AVG_MODEL);
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
