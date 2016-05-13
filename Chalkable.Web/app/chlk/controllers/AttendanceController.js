REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AttendanceService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.ReportingService');

REQUIRE('chlk.activities.attendance.SummaryPage');
REQUIRE('chlk.activities.attendance.ClassListPage');
REQUIRE('chlk.activities.attendance.StudentDayAttendancePopup');
REQUIRE('chlk.activities.classes.ClassProfileAttendanceListPage');
REQUIRE('chlk.activities.classes.ClassProfileAttendanceSeatingChartPage');
REQUIRE('chlk.activities.attendance.SeatingChartPage');
REQUIRE('chlk.activities.attendance.EditSeatingGridDialog');
REQUIRE('chlk.activities.reports.SeatingChartAttendanceReportDialog');
REQUIRE('chlk.activities.reports.AttendanceProfileReportDialog');
REQUIRE('chlk.activities.reports.AttendanceRegisterReportDialog');

REQUIRE('chlk.models.attendance.AttendanceList');
REQUIRE('chlk.models.attendance.AttendanceStudentBox');
REQUIRE('chlk.models.attendance.UpdateSeatingChart');
REQUIRE('chlk.models.reports.SubmitSeatingChartReportViewData');
REQUIRE('chlk.models.id.ClassId');

REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.controllers', function (){

    var studentAttendanceTimeout, canUpdateStudentAttendance = true, currentStudentId;

    /** @class chlk.controllers.AttendanceController */
    CLASS(
        'AttendanceController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AttendanceService, 'attendanceService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.ReportingService, 'reportingService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.MarkingPeriodService, 'markingPeriodService',


        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE,
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('attendance')],
        function indexAction() {
            var classId = this.getCurrentClassId();
            if(classId && classId.valueOf())
                return this.Redirect('attendance', 'classList', [classId]);

            return this.Redirect('attendance', 'summary');
        },


        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId]],
        function seatingChartReportAction(gradingPeriodId, classId){

            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var res = new ria.async.DeferredData(new chlk.models.reports.BaseReportViewData(classId, gradingPeriodId,
                null, null, null, this.hasUserPermission_(chlk.models.people.UserPermissionEnum.SEATING_CHART_REPORT)));
            return this.ShadeView(chlk.activities.reports.SeatingChartAttendanceReportDialog, res);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId]],
        function attendanceProfileReportAction(classId){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var gp = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, null);
            var gradingPeriodId = gp.getId();

            var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
            var markingPeriods = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIODS, []);
            var students = this.getContext().getSession().get(ChlkSessionConstants.STUDENTS_FOR_REPORT, []);
            var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, null);
            var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.ATTENDANCE_PROFILE_REPORT) ||
                    this.hasUserPermission_(chlk.models.people.UserPermissionEnum.ATTENDANCE_PROFILE_REPORT_CLASSROOM);

            var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
            var res = new ria.async.DeferredData(new chlk.models.reports.AttendanceProfileReportViewData(markingPeriods, reasons, students
                , classId, gradingPeriodId, schoolYear.getStartDate(), schoolYear.getEndDate(), ableDownload, isAbleToReadSSNumber));
            return this.ShadeView(chlk.activities.reports.AttendanceProfileReportDialog, res);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.GradingPeriodId, chlk.models.id.ClassId]],
        function attendanceRegisterReportAction(gradingPeriodId, classId){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
            var res = this.attendanceService.getAttendanceMonths()
                .then(function(items){
                    var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.CLASSROOM_ATTENDANCE_REGISTER_REPORT);
                    var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                    return new chlk.models.reports.AttendanceRegisterReportViewData(reasons, items, classId, gradingPeriodId, ableDownload, isAbleToReadSSNumber)
                }, this);

            return this.ShadeView(chlk.activities.reports.AttendanceRegisterReportDialog, res);
        },

        [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.SEATING_CHART_REPORT])],
        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.reports.SubmitSeatingChartReportViewData]],
        function submitSeatingChartReportAction(reportViewData){

            var result = this.reportingService.submitSeatingChartReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getFormat(),
                    reportViewData.isDisplayStudentPhoto()
                )
                .attach(this.validateResponse_())
                .then(function () {
                    this.BackgroundCloseView(chlk.activities.reports.SeatingChartAttendanceReportDialog);
                    this.BackgroundCloseView(chlk.activities.reports.SeatingChartReportDialog);
                }, this)
                .thenBreak();

            return this.UpdateView(this.getView().getCurrent().getClass(), result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.ATTENDANCE_PROFILE_REPORT, chlk.models.people.UserPermissionEnum.ATTENDANCE_PROFILE_REPORT_CLASSROOM]
        ])],
        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.reports.SubmitAttendanceProfileReportViewData]],
        function submitAttendanceProfileReportAction(reportViewData){

            if (Date.compare(reportViewData.getStartDate().getDate() , reportViewData.getEndDate().getDate()) > 0){
                    return this.ShowAlertBox("Report start time should be less than report end time", "Error"), null;
                }

            if (!reportViewData.getAbsenceReasons()){
                return this.ShowAlertBox("Absence Reasons is a required field. Please make sure that you enter data in all required fields", "Error"), null;
            }

            if (!reportViewData.getTerms()){
                return this.ShowAlertBox("You should select at least one term", "Error"), null;
            }

            var result = this.reportingService.submitAttendanceProfileReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getFormat(),
                    reportViewData.getStartDate(),
                    reportViewData.getEndDate(),
                    reportViewData.getGroupBy(),
                    reportViewData.getIdToPrint(),
                    this.getIdsList(reportViewData.getAbsenceReasons(), chlk.models.id.AttendanceReasonId),
                    this.getIdsList(reportViewData.getTerms(), chlk.models.id.MarkingPeriodId),
                    reportViewData.isDisplayPeriodAbsences(),
                    reportViewData.isDisplayReasonTotals(),
                    reportViewData.isIncludeCheck(),
                    reportViewData.isIncludeUnlisted(),
                    reportViewData.isDisplayNote(),
                    reportViewData.isDisplayWithdrawnStudents(),
                    reportViewData.getStudentIds()
                )
                .attach(this.validateResponse_())
                .then(function () {
                    this.BackgroundCloseView(chlk.activities.reports.AttendanceProfileReportDialog);
                }, this)
                .thenBreak();

            return this.UpdateView(chlk.activities.reports.AttendanceProfileReportDialog, result);
        },

        [chlk.controllers.Permissions([chlk.models.people.UserPermissionEnum.CLASSROOM_ATTENDANCE_REGISTER_REPORT])],
        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.reports.SubmitAttendanceRegisterReportViewData]],
        function submitAttendanceRegisterReportAction(reportViewData){

            if (!reportViewData.getAbsenceReasons()){
                return this.ShowAlertBox("Absence Reasons is a required field. Please make sure that you enter data in all required fields", "Error"), null;
            }

            var result = this.reportingService.submitAttendanceRegisterReport(
                    reportViewData.getClassId(),
                    reportViewData.getGradingPeriodId(),
                    reportViewData.getFormat(),
                    reportViewData.getIdToPrint(),
                    reportViewData.getReportType(),
                    this.getIdsList(reportViewData.getAbsenceReasons(), chlk.models.id.AttendanceReasonId),
                    reportViewData.getMonthId(),
                    reportViewData.isShowLocalReasonCode(),
                    reportViewData.isIncludeTardies()
                )
                .attach(this.validateResponse_())
                .then(function () {
                    this.BackgroundCloseView(chlk.activities.reports.AttendanceRegisterReportDialog);
                }, this)
                .thenBreak();

            return this.UpdateView(chlk.activities.reports.AttendanceRegisterReportDialog, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE,
                chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('attendance')],
        function summaryAction() {
            var result = this.attendanceService
                .getSummary()
                .attach(this.validateResponse_())
                .then(function(summary) {
                    var res = this.attendanceService
                        .getNotTakenAttendanceClasses()
                        .attach(this.validateResponse_())
                        .then(function(items){
                            return new chlk.models.attendance.NotTakenAttendanceClassesViewData(items);
                        }.bind(this));
                    this.BackgroundUpdateView(chlk.activities.attendance.SummaryPage, res, chlk.activities.lib.DontShowLoader());
                    var gp = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, null);

                    var topModel = new chlk.models.classes.ClassesForTopBar(null);
                    return new chlk.models.attendance.SummaryPage(topModel, summary, gp);
                }, this);

            return this.PushView(chlk.activities.attendance.SummaryPage, result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.AttendanceList]],
        function setAttendanceForListAction(model){
            if(model.getClassIds()){
                this.attendanceService.setAttendanceForList(model.getPersonId(), model.getClassIds(), model.getAttendanceTypes(), model.getAttReasons(), model.getDate())
                    .attach(this.validateResponse_())
                    .then(function(){
                        var controller = model.getController();
                        if(controller){
                            var action = model.getAction();
                            var params = JSON.parse(model.getParams());
                            if(model.isNewStudent()){
                                params.push(model.getAttendanceTypes());
                            }
                            return this.BackgroundNavigate(controller, action, params);
                        }
                    }, this);
            }

        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.attendance.UpdateSeatingChart]],
        function saveFromSeatingChartAction(model){
            var o = JSON.parse(model.getSeatingChartInfo());
            var result = this.attendanceService.postSeatingChart(model.getDate(), o)
                .then(function(res){
                    if(model.isInProfile()){
                        //this.BackgroundCloseView(chlk.activities.lib.PendingActionDialog);
                        this.redirectToPage_('class', 'attendanceSeatingChart', [new chlk.models.id.ClassId(o.classId), model.getDate(), true]);
                        return null;
                    }

                    return new chlk.models.attendance.SeatingChart(model.isInProfile());
                }, this);
            if(model.isInProfile())
                return null;//this.ShadeLoader();

            return this.UpdateView(chlk.activities.attendance.SeatingChartPage, result, 'savedChart');
        },

        VOID, function addStudentClickAction(){

        },

        [[chlk.models.attendance.AttendanceStudentBox]],
        VOID, function showStudentBoxAction(model) {
            var date = model.getDate() ? model.getDate().format('mm-dd-yy') : '';
            this.showStudentAttendanceAction(model.getId(), model.getDate(), 'attendance', 'summary',
                JSON.stringify([true, model.getGradeLevelsIds(), model.getCurrentPage(), date]), true);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean, Boolean, Boolean]],
        function markAllAction(classId, date, isInProfile_, isDailyAttendancePeriod, isSeatingChart_){
            var activityClass = this.getView().getCurrent().getClass();
            var res =  this.ShowConfirmBox('Are you sure you want to Mark All Present?', '')
                    .thenCall(this.attendanceService.markAllPresent, [classId, date, isDailyAttendancePeriod])
                .attach(this.validateResponse_())
                .then(function(success){
                    this.BackgroundUpdateView(activityClass, null, 'mark-all');

                    if(isSeatingChart_){
                        if(isInProfile_)
                            return this.redirectToPage_('class', 'attendanceSeatingChart', [classId, date, true]);

                        return ria.async.BREAK;
                    }

                    return this.redirectToPage_(isInProfile_ ? 'class' : 'attendance',
                        isInProfile_ ? 'attendanceList' : 'classList',
                        [classId, date]);

                }, this);

            return this.UpdateView(activityClass, res);
        },

        [[Boolean, Boolean]],
        function sortStudentsAction(byLastName){
            var model = this.getContext().getSession().get(ChlkSessionConstants.CLASS_LIST_DATA);
            model.getItems().sort(function(item1, item2){
                var sortField1 = "";
                var sortField2 = "";

                if (byLastName){
                    sortField1 = item1.getStudent().getLastName();
                    sortField2 = item2.getStudent().getLastName();
                }
                else{
                    sortField1 = item1.getStudent().getFirstName();
                    sortField2 = item2.getStudent().getFirstName();
                }
                return strcmp(sortField1, sortField2);
            });
            model.setByLastName(byLastName);
            var result = new ria.async.DeferredData(model);
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'sort');
        },

        [[chlk.models.attendance.SeatingChart, chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        chlk.models.attendance.SeatingChart, function prepareSeatingData(model, classId, date_){
            date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
            var topModel = new chlk.models.classes.ClassesForTopBar(null);
            topModel.setSelectedItemId(classId);
            model.setClassId(classId);
            model.setAbleRePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE));
            model.setAbleChangeReasons(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS)
                || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN));
            model.setAblePost(this.isClassTeacher_(classId) && this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE)
                || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN));
            model.setTopData(topModel);
            model.setDate(date_);
            model.setReasons(this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []));
            return model;
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function seatingChartAction(classId, date_, isUpdate_){
            if(!classId.valueOf())
                return this.BackgroundNavigate('attendance', 'summary', []);
            var result = this.attendanceService.getSeatingChartInfo(classId, date_)
                .then(function(model){
                    if(!model)
                        model = new chlk.models.attendance.SeatingChart();
                    var res = this.attendanceService
                        .getNotTakenAttendanceClasses(date_)
                        .attach(this.validateResponse_())
                        .then(function(items){
                            return new chlk.models.attendance.NotTakenAttendanceClassesViewData(items);
                        }.bind(this));
                    this.BackgroundUpdateView(chlk.activities.attendance.SeatingChartPage, res, chlk.activities.lib.DontShowLoader());
                    return this.prepareSeatingData(model, classId, date_);
                }, this);
            return this.PushView(chlk.activities.attendance.SeatingChartPage, result);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.attendance.EditSeatingGridViewData]],
        function showEditGridWindowAction(model){
            var result = new ria.async.DeferredData(model);
            this.BackgroundUpdateView(model.isInProfile() ? chlk.activities.classes.ClassProfileAttendanceSeatingChartPage : chlk.activities.attendance.SeatingChartPage,
                new ria.async.DeferredData(new chlk.models.attendance.SeatingChart(model.isInProfile())), 'savedChart');
            return this.ShadeView(chlk.activities.attendance.EditSeatingGridDialog, result);
        },

        [[chlk.models.attendance.EditSeatingGridViewData, Object]],
        function prepareSeatingChartEdit(model, resObject){
            var rows = model.getRows(),
                resRows = resObject.rows,
                columns = model.getColumns(),
                resColumns = resObject.columns,
                seatingList = resObject.seatingList || [];
            if(resRows != rows){
                if(resRows > rows){
                    seatingList.splice(rows);
                }else{
                    for(var curRow = resRows + 1; curRow <= rows; curRow++){
                        var newRow = [];
                        for(var curCol = 1; curCol <= columns; curCol++){
                            newRow.push({
                                row: curRow,
                                column: curCol
                            });
                        }
                        seatingList.push(newRow);
                    }
                }
                resObject.rows = rows;
            }
            if(resColumns != columns){
                if(resColumns > columns){
                    seatingList.forEach(function(items){
                        items.splice(columns);
                    });
                }else{
                    for(var curRowIndex = 0; curRowIndex < rows; curRowIndex++){
                        var curRow = seatingList[curRowIndex];
                        var len = curRow.length;
                        for(var curColIndex = len + 1; curColIndex <= columns; curColIndex++){
                            curRow.push({
                                row: curRowIndex,
                                column: curColIndex
                            });
                        }
                    }
                }
                resObject.columns = columns;
            }
            seatingList.forEach(function(items, i){
                items.forEach(function(item, j){
                    item.index = rows * i + j + 1;
                });
            });
            return resObject;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.attendance.EditSeatingGridViewData]],
        function editSeatingGridAction(model){
            var seatingChartInfo = JSON.parse(model.getSeatingChartInfo());
            var postInfo = this.prepareSeatingChartEdit(model, seatingChartInfo);
            var page = model.isInProfile() ? chlk.activities.classes.ClassProfileAttendanceSeatingChartPage : chlk.activities.attendance.SeatingChartPage;
            var res = this.attendanceService.postSeatingChartWithInfo(model.getDate(), postInfo)
                .then(function(resModel){
                    if(!model.isInProfile()){
                        var res = this.attendanceService
                            .getNotTakenAttendanceClasses(model.getDate())
                            .attach(this.validateResponse_())
                            .then(function(items){
                                return new chlk.models.attendance.NotTakenAttendanceClassesViewData(items);
                            }.bind(this));
                        this.BackgroundUpdateView(chlk.activities.attendance.SeatingChartPage, res, chlk.activities.lib.DontShowLoader());
                    }

                    resModel.setInProfile(model.isInProfile());

                    return this.prepareSeatingData(resModel, model.getClassId(), model.getDate());
                }, this);
            this.BackgroundCloseView(chlk.activities.attendance.EditSeatingGridDialog);
            return this.UpdateView(page, res);
        },

        [[chlk.models.id.ClassId]],
        function isClassTeacher_(classId){
            var len = this.classService.getClassesForTopBarSync().filter(function(clazz){
                return clazz.getId() == classId;
            }).length;

            return len > 0;
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
        function classListAction(classId, date_, byPostButton_) {
            if(!classId.valueOf())
                return this.BackgroundNavigate('attendance', 'summary', []);
            var result = this.attendanceService
                .getClassList(classId, date_)
                .attach(this.validateResponse_())
                .then(function(items){
                    var students = items.map(function(item){return item.getStudent()});
                    this.getContext().getSession().set(ChlkSessionConstants.STUDENTS_FOR_REPORT, students);
                    var res = this.attendanceService
                        .getNotTakenAttendanceClasses(date_)
                        .attach(this.validateResponse_())
                        .then(function(items){
                            return new chlk.models.attendance.NotTakenAttendanceClassesViewData(items);
                        }.bind(this));
                    this.BackgroundUpdateView(chlk.activities.attendance.ClassListPage, res, chlk.activities.lib.DontShowLoader());

                    date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
                    var topModel = new chlk.models.classes.ClassesForTopBar(null);
                    var model = new chlk.models.attendance.ClassList(
                        topModel,
                        classId,
                        items,
                        date_,
                        true,
                        this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE) && !this.userIsAdmin(),
                        (this.isClassTeacher_(classId) && this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN)) && !this.userIsAdmin(),
                        (this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN)) && !this.userIsAdmin(),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM)
                    );
                    this.getContext().getSession().set(ChlkSessionConstants.CLASS_LIST_DATA, model);
                    return model;
                }, this);

            return this.PushOrUpdateView(chlk.activities.attendance.ClassListPage, result, byPostButton_ ? 'saved' : '');
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.SetClassListAttendance]],
        function setClassAttendanceListAction(model){
            return this.attendanceService.setAttendance(model)
                .attach(this.validateResponse_())
                .then(function(res){
                    return this.Redirect(model.isInProfile() ? 'class' : 'attendance',
                        model.isInProfile() ? 'attendanceList' : 'classList',
                        [model.getClassId(), model.getDate(), true]);
                }, this);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.attendance.SetClassListAttendance]],
        function setClassAttendanceListFromSeatingChartAction(model){
            var result = this.attendanceService.setAttendance(model)
                .attach(this.validateResponse_())
                .then(function(res){
                    return new chlk.models.attendance.SeatingChart(model.isInProfile());
                }, this);
            return this.UpdateView(model.isInProfile() ? chlk.activities.classes.ClassProfileAttendanceSeatingChartPage : chlk.activities.attendance.SeatingChartPage, result, 'saved');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.attendance.SetClassListAttendance]],
        function setClassAttendanceListFromPopUpAction(model){
            var result = this.attendanceService.setAttendance(model)
                .attach(this.validateResponse_())
                .then(function(res){
                    return new chlk.models.attendance.SeatingChart(model.isInProfile());
                }, this);
            return this.UpdateView(model.isInProfile() ? chlk.activities.classes.ClassProfileAttendanceSeatingChartPage : chlk.activities.attendance.SeatingChartPage, result, 'saved');
        }
    ])
});
