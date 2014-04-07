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

        [chlk.controllers.SidebarButton('attendance')],
        function summaryAction() {
            var result = this.attendanceService
                .getSummary()
                .attach(this.validateResponse_())
                .then(function(summary){
                    var classes = this.classService.getClassesForTopBar(true);
                    if(!this.hasUserPermission_(chlk.models.people.UserPermissionEnum.VIEW_ATTENDANCE)){
                        classes = classes.filter(function(item){return item.getName() == 'All';});
                    }
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
                    return new chlk.models.attendance.SummaryPage(topModel, summary);
                }, this);
            return this.PushView(chlk.activities.attendance.SummaryPage, result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[Boolean, String, Number, chlk.models.common.ChlkDate, String]],
        function summaryAdminAction(update_, gradeLevels_, currentPage_, date_, types_) {
            var markingPeriod = this.getContext().getSession().get('markingPeriod', null), //TODO: use method getCurrentMarkingPeriod
                currentSchoolYearId = this.getContext().getSession().get('currentSchoolYearId', null), //TODO: use method getCurrentSchoolYearId
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
            var markingPeriod = this.getContext().getSession().get('markingPeriod', null);
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

        [[chlk.models.attendance.UpdateSeatingChart]],
        function saveFromSeatingChartAction(model){
            this.attendanceService.postSeatingChart(model.getDate(), JSON.parse(model.getSeatingChartInfo()))
                .then(function(model){
                    this.getView().pop();
                }, this);
            return this.ShadeLoader();
        },

        [[chlk.models.attendance.AdminAttendanceSummary, String, chlk.models.common.ChlkDate, chlk.models.id.MarkingPeriodId,
                chlk.models.id.MarkingPeriodId, chlk.models.common.ChlkDate, chlk.models.common.ChlkDate]],
        chlk.models.attendance.AdminAttendanceSummary,
            function prepareAttendanceSummaryModel(model, gradeLevelsIds_, nowDateTime_, fromMarkingPeriodId, toMarkingPeriodId, startDate_, endDate_){
                var gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(true);
                var topModel = new chlk.models.grading.GradeLevelsForTopBar(gradeLevels, gradeLevelsIds_);
                model.setTopData(topModel);
                model.setFromMarkingPeriodId(fromMarkingPeriodId);
                model.getAttendanceByDayData().setDate(nowDateTime_ ? nowDateTime_ : new chlk.models.common.ChlkDate(getDate()));
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
                    model.setReasons(this.getContext().getSession().get('attendanceReasons', []));
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
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
        function markAllAction(classId, date, isProfile_){
            return this.attendanceService
                .markAllPresent(classId, date)
                .attach(this.validateResponse_())
                .then(function(success){
                    return this.BackgroundNavigate('attendance', 'classList', [classId, date, true, isProfile_]);
                  //  this.classListAction(classId, date, true, isProfile_);
                }, this);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean]],
        function markAllFromSeatingAction(classId, date, isProfile_){

        },

        [[Boolean, Boolean]],
        function sortStudentsAction(byLastName, isProfile_){
            var model = this.getContext().getSession().get('attendancePageData');
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
            return this.UpdateView(this.getActivityClass_(isProfile_), result);
        },

        [[chlk.models.attendance.SeatingChart, chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        chlk.models.attendance.SeatingChart, function prepareSeatingData(model, classId, date_){
            date_ = date_ || new chlk.models.common.ChlkDate(getDate());
            var classes = this.classService.getClassesForTopBar(true);
            var topModel = new chlk.models.classes.ClassesForTopBar(classes);
            topModel.setSelectedItemId(classId);
            model.setAbleRePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE));
//                this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_ATTENDANCE)
                model.setAblePost(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE));
            model.setTopData(topModel);
            model.setDate(date_);
            this.getContext().getSession().set('seatingInfo', model);
            return model;
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function seatingChartAction(classId, date_, isUpdate_){
            if(!classId.valueOf())
                return this.BackgroundNavigate('attendance', 'summary', []);
            var result = this.attendanceService.getSeatingChartInfo(classId, date_)
                .then(function(model){
                    return this.prepareSeatingData(model, classId, date_);
                }, this);
            return this.PushView(chlk.activities.attendance.SeatingChartPage, result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.EditSeatingGridViewData]],
        function showEditGridWindowAction(model){
            var result = new ria.async.DeferredData(model);
            return this.ShadeView(chlk.activities.attendance.EditSeatingGridDialog, result);
        },

        function prepareSeatingChartEdit(model, resModel){
            var classId = model.getClassId();
            resModel = this.prepareSeatingData(resModel, classId, model.getDate());
            if(!resModel.getSeatingList())
                resModel.setSeatingList([]);
            var rows = model.getRows(),
                resRows = resModel.getRows(),
                columns = model.getColumns(),
                resColumns = resModel.getColumns(),
                seatingList = resModel.getSeatingList() || [];
            if(resRows != rows){
                if(resRows > rows){
                    seatingList.splice(rows);
                }else{
                    for(var curRow = resRows + 1; curRow <= rows; curRow++){
                        var newRow = [];
                        for(var curCol = 1; curCol <= columns; curCol++){
                            newRow.push(new chlk.models.attendance.ClassAttendanceWithSeatPlace(
                                curRow,
                                curCol
                            ));
                        }
                        seatingList.push(newRow);
                    }
                }
                resModel.setRows(rows);
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
                            curRow.push(new chlk.models.attendance.ClassAttendanceWithSeatPlace(
                                curRowIndex,
                                curColIndex
                            ));
                        }
                    }
                }
                resModel.setColumns(columns);
            }
            seatingList.forEach(function(items, i){
                items.forEach(function(item, j){
                    item.setIndex(rows * i + j + 1);
                });
            });
            return resModel;
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.EditSeatingGridViewData]],
        function editSeatingGridAction(model){
            var resModel = this.getContext().getSession().get('seatingInfo'), res;
            if(resModel.getColumns() == 0 || resModel.getRows() == 0){
                resModel = this.prepareSeatingChartEdit(model, resModel);
                res = ria.async.DeferredData(resModel);
            }else{
                res = this.attendanceService.postSeatingChart(model.getDate(), JSON.parse(model.getSeatingChartInfo()))
                    .then(function(resModel){
                        resModel = this.prepareSeatingChartEdit(model, resModel);
                        this.getView().pop();
                        return resModel;
                    }, this);
                this.ShadeLoader();
            }
            this.BackgroundCloseView(chlk.activities.attendance.EditSeatingGridDialog);
            return this.UpdateView(chlk.activities.attendance.SeatingChartPage, res);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean, Boolean]],
        function classListAction(classId, date_, isUpdate_, isProfile_) {
            if(!classId.valueOf())
                return this.BackgroundNavigate('attendance', 'summary', []);
            var result = this.attendanceService
                .getClassList(classId, date_)
                .attach(this.validateResponse_())
                .then(function(items){
                    date_ = date_ || new chlk.models.common.ChlkDate(getDate());
                    this.getContext().getSession().set('attendanceData', items);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(
                        topModel,
                        classId,
                        items,
                        date_,
                        true,
                        this.getContext().getSession().get('attendanceReasons', []),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.REPOST_CLASSROOM_ATTENDANCE),
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ATTENDANCE)
                    );
                    this.getContext().getSession().set('attendancePageData', model);
                    return model;
                }, this);

            if(!(date_ && isUpdate_))
                return this.PushView(this.getActivityClass_(isProfile_), result);
            return this.UpdateView(this.getActivityClass_(isProfile_), result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.attendance.SetClassListAttendance]],
        function setClassAttendanceListAction(model){
            if(model.getSubmitType() == 'submit')
                return this.attendanceService.setAttendance(model)
                    .attach(this.validateResponse_())
                    .then(function(res){
                        this.BackgroundNavigate('attendance', 'classList', [model.getClassId(), model.getDate(), true]);
                    }, this);
            return null;
        },

        Object, function getActivityClass_(isProfile_){
            if(isProfile_ === true)
                return chlk.activities.classes.ClassProfileAttendanceListPage;
            return chlk.activities.attendance.ClassListPage;
        }
    ])
});
