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

REQUIRE('chlk.models.attendance.AttendanceList');
REQUIRE('chlk.models.attendance.AttendanceStudentBox');

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
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.SummaryPage(topModel, summary);
                    return model;
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
            if(model.getClassPersonIds()){
                this.attendanceService.setAttendanceForList(model.getClassPersonIds(), model.getClassPeriodIds(), model.getAttendanceTypes(), model.getAttReasons(), model.getDate())
                    .then(function(){
                        var controller = model.getController();
                        if(controller){
                            var action = model.getAction();
                            var params = JSON.parse(model.getParams());
                            if(model.isNewStudent()){
                                params.push(model.getAttendanceTypes());
                            }
                            this.Redirect(controller, action, params);
                        }
                    }, this);
            }

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
        VOID, function showStudentAttendanceAction(studentId, date_, controller_, action_, params_, isNew_) {
            var result = this.attendanceService.getStudentAttendance(studentId, date_)
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
            this.showStudentAttendanceAction(model.getId(), model.getDate(), 'attendance', 'summary',
                JSON.stringify([true, model.getGradeLevelsIds(), model.getCurrentPage(), model.getDate().format('mm-dd-yy')]), true);
        },

        [[chlk.models.id.ClassId, chlk.models.id.ClassPeriodId, chlk.models.common.ChlkDate, Boolean]],
        function markAllAction(classId, classPeriodId, date, isProfile_){
            this.attendanceService
                .markAllPresent(classPeriodId, date)
                .then(function(success){
                    this.classListAction(classId, date, true, isProfile_);
                }, this);
            return this.ShadeLoader();
        },

        [[Boolean, Boolean]],
        function sortStudentsAction(byLastName, isProfile_){
            var model = this.getContext().getSession().get('attendancePageData');
            model.getItems().sort(function(item1, item2){
                //todo : change this... it will not work on production
                var method = byLastName ? 'getLastName' : 'getFirstName';
                return item1.getStudent()[method]() > item2.getStudent()[method]();
            });
            model.setByLastName(byLastName);
            var result = new ria.async.DeferredData(model);
            return this.UpdateView(this.getActivityClass_(isProfile_), result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Boolean, Boolean]],
        function classListAction(classId, date_, isUpdate_, isProfile_) {
            if(!classId.valueOf())
                return this.Redirect('attendance', 'summary', []);
            var result = this.attendanceService
                .getClassList(classId, date_)
                .attach(this.validateResponse_())
                .then(function(items){
                    date_ = date_ || new chlk.models.common.ChlkDate(getDate());
                    if(items.length == 0)
                        this.ShowMsgBox(Msg.No_class_scheduled(date_.format('mm/dd/yy')), null, [{
                            text: Msg.GOT_IT.toUpperCase()
                        }]);
                    this.getContext().getSession().set('attendanceData', items);
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(topModel, classId, items, date_, true, this.getContext().getSession().get('attendanceReasons', []));
                    this.getContext().getSession().set('attendancePageData', model);
                    return model;
                }, this);

            if(!(date_ && isUpdate_))
                return this.PushView(this.getActivityClass_(isProfile_), result);
            return this.UpdateView(this.getActivityClass_(isProfile_), result);
        },

        Object, function getActivityClass_(isProfile_){
            if(isProfile_ === true)
                return chlk.activities.classes.ClassProfileAttendanceListPage;
            return chlk.activities.attendance.ClassListPage;
        },


        [[chlk.models.attendance.ClassAttendance]],
        function setAttendanceAction(model){
            if(canUpdateStudentAttendance || currentStudentId != model.getStudentId()){
                currentStudentId = model.getStudentId();
                canUpdateStudentAttendance = false;
                studentAttendanceTimeout = setTimeout(function(){
                    canUpdateStudentAttendance = true;
                },5);
                var activityClass = this.getView().getCurrent().getClass();
                return this.UpdateView(activityClass, this.setAttendance_(model), chlk.activities.lib.DontShowLoader());
            }
            return null;
        },

        [[chlk.models.attendance.ClassAttendance]],
        ria.async.Future, function setAttendance_(model){
            var type = this.changeAttendanceType_(model.getSubmitType(), model.getType());
            var items = this.getContext().getSession().get('attendanceData');
            var item = items.filter(function(item){
                return item.getStudentId() == model.getStudentId()
            })[0];
            item.setType(type);
            var level = item.getLevel();
            var attReasonId = model.getAttendanceReasonId();
            if(!attReasonId || !attReasonId.valueOf()){
                var reasons = item.getReasons().filter(function(item){return item.isDefaultReason(level);});
                if(reasons.length > 0){
                    attReasonId =  reasons[0].getId();
                    model.setAttendanceReasonId(attReasonId);
                }
            }
            try{
                if(attReasonId && attReasonId.valueOf() && item.getReasons().filter(function(item){
                    return item.hasLevel(level) && item.getId() == attReasonId;
                }).length == 0)
                    console.info('WARNING setAttendance: type = ' + level + ', reasonId = ' + attReasonId);
                else
                    this.attendanceService.setAttendance(model.getStudentId(), model.getClassId(), level, attReasonId, model.getDate());

            }catch(e){
                console.info('ERROR setAttendance: type = ' + type + ', reasonId = ' + attReasonId);
            }
            if(attReasonId && attReasonId.valueOf()){
                if(item.getAttendanceReason()){
                    item.getAttendanceReason().setId(attReasonId);
                    item.getAttendanceReason().setName(model.getAttendanceReasonDescription());
                    item.getAttendanceReason().setDescription(model.getAttendanceReasonDescription());
                }else{
                    var reason = new chlk.models.attendance.AttendanceReason(attReasonId, model.getAttendanceReasonDescription());
                    item.setAttendanceReason(reason);
                }
            }else{
                item.setAttendanceReason(null);
            }
            return new ria.async.DeferredData(item);
        },

        Number,  function changeAttendanceType_(submitType, currentType){
            var attTypeEnum = chlk.models.attendance.AttendanceTypeEnum;
            var types = [
                attTypeEnum.PRESENT.valueOf(),
                attTypeEnum.ABSENT.valueOf(),
//                attTypeEnum.EXCUSED.valueOf(),
                attTypeEnum.LATE.valueOf()
            ];
            var index = types.indexOf(currentType);
            var increment = submitType == 'leftArrow' ? -1 : submitType == 'rightArrow' ? 1 : 0;
            if(increment == -1 && index == 0)
                index = types.length;
            else if(increment == 1 && index == types.length - 1)
                index = -1;
            index += increment;
            return types[index];
        }
    ])
});
