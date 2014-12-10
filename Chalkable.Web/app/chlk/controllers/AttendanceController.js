REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AttendanceService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.MarkingPeriodService');

REQUIRE('chlk.activities.attendance.SummaryPage');
REQUIRE('chlk.activities.attendance.ClassListPage');
REQUIRE('chlk.activities.attendance.AdminAttendanceSummaryPage');
REQUIRE('chlk.activities.attendance.StudentDayAttendancePopup');
REQUIRE('chlk.activities.classes.ClassProfileAttendanceListPage');
REQUIRE('chlk.activities.attendance.SeatingChartPage');
REQUIRE('chlk.activities.attendance.EditSeatingGridDialog');

REQUIRE('chlk.models.attendance.AttendanceList');
REQUIRE('chlk.models.attendance.AttendanceStudentBox');
REQUIRE('chlk.models.attendance.UpdateSeatingChart');
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
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.MarkingPeriodService, 'markingPeriodService',

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('attendance')],
        function summaryAction() {
            var result = this.attendanceService
                .getSummary()
                .attach(this.validateResponse_())
                .then(function(summary){
                    var res = this.attendanceService
                        .getNotTakenAttendanceClasses()
                        .attach(this.validateResponse_())
                        .then(function(items){
                            return new chlk.models.attendance.NotTakenAttendanceClassesViewData(items);
                        }.bind(this));
                    this.BackgroundUpdateView(chlk.activities.attendance.SummaryPage, res, chlk.activities.lib.DontShowLoader());

                    var topModel = new chlk.models.classes.ClassesForTopBar(this.classService.getClassesForTopBar(true));
                    return new chlk.models.attendance.SummaryPage(topModel, summary);
                }, this);
            return this.PushView(chlk.activities.attendance.SummaryPage, result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[Boolean, String, Number, chlk.models.common.ChlkDate, String]],
        function summaryAdminAction(update_, gradeLevels_, currentPage_, date_, types_) {
            var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD, null), //TODO: use method getCurrentMarkingPeriod
                currentSchoolYearId = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_SCHOOL_YEAR_ID, null), //TODO: use method getCurrentSchoolYearId
                fromMarkingPeriodId = markingPeriod.getId(),
                toMarkingPeriodId = markingPeriod.getId();
            var res = ria.async.wait([
                    this.attendanceService.getAdminAttendanceSummary(true, true, true, gradeLevels_, date_, fromMarkingPeriodId, toMarkingPeriodId),
                    this.markingPeriodService.list(currentSchoolYearId)
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    model.setMarkingPeriods(result[1]);
                    if(currentPage_)
                        model.setCurrentPage(currentPage_);
                    if(types_)
                        model.setAttendanceTypes(types_);
                    return this.prepareAttendanceSummaryModel(model, gradeLevels_, date_, fromMarkingPeriodId, toMarkingPeriodId);
                }, this);
            return /*!update_ && this.PushView(chlk.activities.attendance.AdminAttendanceSummaryPage, res); */update_ ?
                this.UpdateView(chlk.activities.attendance.AdminAttendanceSummaryPage, res) :
                this.PushView(chlk.activities.attendance.AdminAttendanceSummaryPage, res);
        },

        [[chlk.models.attendance.AdminAttendanceSummary]],
        function updateSummaryAdminAction(model) {
            var markingPeriod = this.getContext().getSession().get(ChlkSessionConstants.MARKING_PERIOD, null);
            var renderNow = model.isRenderNow(),
                renderDay = model.isRenderDay(),
                renderMp = model.isRenderMp(),
                gradeLevelsIds = model.getGradeLevelsIds(),
                nowDateTime = model.getNowDateTime(),
                fromMarkingPeriodId = model.getFromMarkingPeriodId() || markingPeriod.getId(),
                toMarkingPeriodId = model.getToMarkingPeriodId() || markingPeriod.getId(),
                startDate = model.getStartDate(),
                endDate = model.getEndDate();
            var res = this.attendanceService
                .getAdminAttendanceSummary(renderNow, renderDay, renderMp, gradeLevelsIds, nowDateTime,
                    fromMarkingPeriodId, toMarkingPeriodId, startDate, endDate)
                .attach(this.validateResponse_())
                .then(function(model){
                    var result =  this.prepareAttendanceSummaryModel(model, gradeLevelsIds, nowDateTime,
                        fromMarkingPeriodId, toMarkingPeriodId, startDate, endDate);
                    return renderNow ? model.getNowAttendanceData() : (renderDay ? model.getAttendanceByDayData() : model.getAttendanceByMpData())
                }, this);
            return this.UpdateView(chlk.activities.attendance.AdminAttendanceSummaryPage, res);
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

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.UpdateSeatingChart]],
        function saveFromSeatingChartAction(model){
            var result = this.attendanceService.postSeatingChart(model.getDate(), JSON.parse(model.getSeatingChartInfo()))
                .then(function(model){
                    //this.getView().pop();
                    return new chlk.models.attendance.SeatingChart();
                });
            //return this.ShadeLoader();
            return this.UpdateView(chlk.activities.attendance.SeatingChartPage, result, 'savedChart');
        },

        [[chlk.models.attendance.AdminAttendanceSummary, String, chlk.models.common.ChlkDate, chlk.models.id.MarkingPeriodId,
                chlk.models.id.MarkingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        chlk.models.attendance.AdminAttendanceSummary,
            function prepareAttendanceSummaryModel(model, gradeLevelsIds_, nowDateTime_, fromMarkingPeriodId, toMarkingPeriodId, startDate_, endDate_){
                var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
                var topModel = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevelsIds_);
                model.setTopData(topModel);
                model.setFromMarkingPeriodId(fromMarkingPeriodId);
                model.getAttendanceByDayData().setDate(nowDateTime_ ? nowDateTime_ : new chlk.models.common.ChlkSchoolYearDate());
                model.setToMarkingPeriodId(toMarkingPeriodId);
                if(gradeLevelsIds_)
                    model.setGradeLevelsIds(gradeLevelsIds_);
                if(startDate_)
                    model.setStartDate(startDate_);
                if(endDate_)
                    model.setEndDate(endDate_);
                return model;
        },

        VOID, function addStudentClickAction(){

        },

        [[chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate, String, String, String, Boolean]],
        function showStudentAttendanceAction(studentId, date_, controller_, action_, params_, isNew_) {
            var result = this.attendanceService
                .getStudentAttendance(studentId, date_)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setTarget(chlk.controls.getActionLinkControlLastNode());
                    model.setReasons(this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []));
                    model.setAbleEdit(this.userIsAdmin() || this.userInRole(chlk.models.common.RoleEnum.TEACHER));
                    if(controller_)
                        model.setController(controller_);
                    if(action_)
                        model.setAction(action_);
                    if(params_)
                        model.setParams(params_);
                    if(isNew_)
                        model.setNewStudent(true);
                    return model;
                }, this);
            return this.ShadeView(chlk.activities.attendance.StudentDayAttendancePopup, result);
        },

        [[chlk.models.attendance.AttendanceStudentBox]],
        VOID, function showStudentBoxAction(model) {
            var date = model.getDate() ? model.getDate().format('mm-dd-yy') : '';
            this.showStudentAttendanceAction(model.getId(), model.getDate(), 'attendance', 'summary',
                JSON.stringify([true, model.getGradeLevelsIds(), model.getCurrentPage(), date]), true);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean, Boolean]],
        function markAllAction(classId, date, isProfile_, isSeatingChart_){
            this.attendanceService
                .markAllPresent(classId, date)
                .attach(this.validateResponse_())
                .then(function(success){
                    if(isSeatingChart_)
                        return null;
                    return this.BackgroundNavigate('attendance', 'classList', [classId, date, true, isProfile_]);
                  //  this.classListAction(classId, date, true, isProfile_);
                }, this);
            return null;
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
        function markAllFromSeatingAction(classId, date, isProfile_){

        },

        [[Boolean, Boolean]],
        function sortStudentsAction(byLastName, isProfile_){
            var model = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_PAGE_DATA);
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
            return this.UpdateView(this.getActivityClass_(isProfile_), result, 'sort');
        },

        [[chlk.models.attendance.SeatingChart, chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        chlk.models.attendance.SeatingChart, function prepareSeatingData(model, classId, date_){
            date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
            var classes = this.classService.getClassesForTopBar(true);
            var topModel = new chlk.models.classes.ClassesForTopBar(classes);
            topModel.setSelectedItemId(classId);
            model.setAbleRePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE));
            model.setAbleChangeReasons(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS));
            model.setAblePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE)
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
                    return this.prepareSeatingData(model, classId, date_);
                }, this);
            return this.PushView(chlk.activities.attendance.SeatingChartPage, result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.EditSeatingGridViewData]],
        function showEditGridWindowAction(model){
            var result = new ria.async.DeferredData(model);
            this.BackgroundUpdateView(chlk.activities.attendance.SeatingChartPage, new ria.async.DeferredData(new chlk.models.attendance.SeatingChart()), 'savedChart');
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

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.EditSeatingGridViewData]],
        function editSeatingGridAction(model){
            var seatingChartInfo = JSON.parse(model.getSeatingChartInfo());
            var postInfo = this.prepareSeatingChartEdit(model, seatingChartInfo);
            var res = this.attendanceService.postSeatingChartWithInfo(model.getDate(), postInfo)
                .then(function(resModel){
                    return this.prepareSeatingData(resModel, model.getClassId(), model.getDate());
                }, this);
            this.BackgroundCloseView(chlk.activities.attendance.EditSeatingGridDialog);
            return this.UpdateView(chlk.activities.attendance.SeatingChartPage, res);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.common.ChlkDate, chlk.models.id.ClassId]],
        function classListFromBarAction(date_, classId_) {
            return this.classListAction(classId_, date_);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ATTENDANCE_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean, Boolean, Boolean]],
        function classListAction(classId, date_, isUpdate_, isProfile_, byPostButton_) {
            if(!classId.valueOf())
                return this.BackgroundNavigate('attendance', 'summary', []);
            var result = this.attendanceService
                .getClassList(classId, date_)
                .attach(this.validateResponse_())
                .then(function(items){
                    if(!isProfile_){
                        var res = this.attendanceService
                            .getNotTakenAttendanceClasses(date_)
                            .attach(this.validateResponse_())
                            .then(function(items){
                                return new chlk.models.attendance.NotTakenAttendanceClassesViewData(items);
                            }.bind(this));
                        this.BackgroundUpdateView(chlk.activities.attendance.ClassListPage, res, chlk.activities.lib.DontShowLoader());
                    }

                    date_ = date_ || new chlk.models.common.ChlkSchoolYearDate();
                    this.getContext().getSession().set(ChlkSessionConstants.ATTENDANCE_DATA, items);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(
                        topModel,
                        classId,
                        items,
                        date_,
                        true,
                        this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE)
                            || this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE_ADMIN),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ABSENCE_REASONS)
                    );
                    this.getContext().getSession().set(ChlkSessionConstants.ATTENDANCE_PAGE_DATA, model);
                    return model;
                }, this);

            if(!(date_ && isUpdate_))
                return this.PushView(this.getActivityClass_(isProfile_), result);
            return this.UpdateView(this.getActivityClass_(isProfile_), result, byPostButton_ ? 'saved' : '');
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.SetClassListAttendance]],
        function setClassAttendanceListAction(model){
            this.attendanceService.setAttendance(model)
                .attach(this.validateResponse_())
                .then(function(res){
                    this.BackgroundNavigate('attendance', 'classList', [model.getClassId(), model.getDate(), true, null, true]);
                }, this);
            return null;
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.SetClassListAttendance]],
        function setClassAttendanceListFromSeatingChartAction(model){
            var result = this.attendanceService.setAttendance(model)
                .attach(this.validateResponse_())
                .then(function(res){
                    return new chlk.models.attendance.SeatingChart();
                }, this);
            return this.UpdateView(chlk.activities.attendance.SeatingChartPage, result, 'saved');
        },

        Object, function getActivityClass_(isProfile_){
            if(isProfile_ === true)
                return chlk.activities.classes.ClassProfileAttendanceListPage;
            return chlk.activities.attendance.ClassListPage;
        }
    ])
});
