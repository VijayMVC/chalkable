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

        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function classListAction(classId, date_) {
            var result = this.attendanceService
                .getClassList(classId, date_)
                .attach(this.validateResponse_())
                .then(function(items){
                    this.getContext().getSession().set('attendanceData', items);
                    date_ = date_ || new chlk.models.common.ChlkDate(getDate());
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.class.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(topModel, classId, items, date_, true, this.getContext().getSession().get('attendanceReasons', []));
                    return model;
                }, this);
            return this.PushView(chlk.activities.attendance.ClassListPage, result);
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
            this.attendanceService.setAttendance(model.getClassPersonId(), model.getClassPeriodId(), type, model.getAttendanceReasonId(), model.getDate());
            var items = this.getContext().getSession().get('attendanceData');
            var item = items.filter(function(item){
                return item.getClassPersonId() == model.getClassPersonId()
            })[0];
            item.setType(type);
            if(model.getAttendanceReasonId()){
                if(item.getAttendanceReason()){
                    item.getAttendanceReason().setId(model.getAttendanceReasonId());
                    item.getAttendanceReason().setDescription(model.getAttendanceReasonDescription());
                }else{
                    var reason = new chlk.models.attendance.AttendanceReason(model.getAttendanceReasonId(), model.getAttendanceReasonDescription());
                    item.setAttendanceReason(reason);
                }
            }
            var result = new ria.async.DeferredData(item);
            return this.UpdateView(chlk.activities.attendance.ClassListPage, result, window.noLoadingMsg);
        }
    ])
});
