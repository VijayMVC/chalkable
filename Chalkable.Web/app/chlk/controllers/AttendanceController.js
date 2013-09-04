REQUIRE('chlk.controllers.BaseController');
REQUIRE('chlk.services.AttendanceService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.attendance.SummaryPage');
REQUIRE('chlk.activities.attendance.ClassListPage');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AttendanceController */
    CLASS(
        'AttendanceController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AttendanceService, 'attendanceService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [chlk.controllers.SidebarButton('attendance')],
        function summaryAction() {
            var result = this.attendanceService
                .getSummary()
                .attach(this.validateResponse_())
                .then(function(summary){
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.class.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.SummaryPage(topModel, summary);
                    return model;
                }, this);
            return this.PushView(chlk.activities.attendance.SummaryPage, result);
        },

        [[chlk.models.id.ClassId, chlk.models.id.ClassPeriodId, chlk.models.common.ChlkDate]],
        function markAllAction(classId, classPeriodId, date){
            this.attendanceService
                .markAllPresent(classPeriodId, date)
                .then(function(success){
                    this.classListAction(classId, date);
                }, this);
            this.StartLoading(chlk.activities.attendance.ClassListPage);
        },

        [[Boolean]],
        function sortStudentsAction(byLastName){
            var model = this.getContext().getSession().get('attendancePageData');
            model.getItems().sort(function(item1, item2){
                var method = byLastName ? 'getLastName' : 'getFirstName';
                return item1.getStudent()[method]() > item2.getStudent()[method]();
            });
            model.setByLastName(byLastName);
            var result = new ria.async.DeferredData(model);
            return this.UpdateView(chlk.activities.attendance.ClassListPage, result);
        },

        [chlk.controllers.SidebarButton('attendance')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function classListAction(classId, date_) {
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
                    var topModel = new chlk.models.class.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(topModel, classId, items, date_, true, this.getContext().getSession().get('attendanceReasons', []));
                    this.getContext().getSession().set('attendancePageData', model);
                    return model;
                }, this);
            return this[date_ ? 'UpdateView' : 'PushView'](chlk.activities.attendance.ClassListPage, result);
        },

        [[chlk.models.attendance.ClassAttendance]],
        function setAttendanceAction(model){
            var type = model.getType();
            if(model.getSubmitType() == 'leftArrow'){
                if(type > 2)
                    type = type/2;
                else
                    type = 16;
            }
            if(model.getSubmitType() == 'rightArrow'){
                if(type < 16)
                    type = type*2;
                else
                    type = 2;
            }
            var items = this.getContext().getSession().get('attendanceData');
            var item = items.filter(function(item){
                return item.getClassPersonId() == model.getClassPersonId()
            })[0];
            item.setType(type);
            try{
                if(model.getAttendanceReasonId() && model.getAttendanceReasonId().valueOf() && item.getReasons().filter(function(item){
                    return item.getAttendanceType() == type && item.getId() == model.getAttendanceReasonId();
                }).length == 0)
                    console.info('WARNING setAttendance: type = ' + type + ', reasonId = ' + model.getAttendanceReasonId());
                else
                    this.attendanceService.setAttendance(model.getClassPersonId(), model.getClassPeriodId(), type, model.getAttendanceReasonId(), model.getDate());

            }catch(e){
                console.info('ERROR setAttendance: type = ' + type + ', reasonId = ' + model.getAttendanceReasonId());
            }
            if(model.getAttendanceReasonId() && model.getAttendanceReasonId().valueOf()){
                if(item.getAttendanceReason()){
                    item.getAttendanceReason().setId(model.getAttendanceReasonId());
                    item.getAttendanceReason().setDescription(model.getAttendanceReasonDescription());
                }else{
                    var reason = new chlk.models.attendance.AttendanceReason(model.getAttendanceReasonId(), model.getAttendanceReasonDescription());
                    item.setAttendanceReason(reason);
                }
            }else{
                item.setAttendanceReason(null);
            }
            var result = new ria.async.DeferredData(item);
            return this.UpdateView(chlk.activities.attendance.ClassListPage, result, window.noLoadingMsg);
        }
    ])
});
