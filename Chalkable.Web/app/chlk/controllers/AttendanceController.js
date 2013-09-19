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
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
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
            return this.ShadeLoader();
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
                    var topModel = new chlk.models.classes.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(topModel, classId, items, date_, true, this.getContext().getSession().get('attendanceReasons', []));
                    this.getContext().getSession().set('attendancePageData', model);
                    return model;
                }, this);
            return this[date_ ? 'UpdateView' : 'PushView'](chlk.activities.attendance.ClassListPage, result);
        },


        //todo refactor this
        [[chlk.models.attendance.ClassAttendance]],
        function setAttendanceAction(model){
            var type = this.changeAttendanceType_(model.getSubmitType(), model.getType());
            var items = this.getContext().getSession().get('attendanceData');
            var item = items.filter(function(item){
                return item.getClassPersonId() == model.getClassPersonId()
            })[0];
            item.setType(type);
            var attReasonId = model.getAttendanceReasonId();
            try{
                if(attReasonId && attReasonId.valueOf() && item.getReasons().filter(function(item){
                    return item.getAttendanceType() == type && item.getId() == attReasonId;
                }).length == 0)
                    console.info('WARNING setAttendance: type = ' + type + ', reasonId = ' + attReasonId);
                else
                    this.attendanceService.setAttendance(model.getClassPersonId(), model.getClassPeriodId(), type, attReasonId, model.getDate());

            }catch(e){
                console.info('ERROR setAttendance: type = ' + type + ', reasonId = ' + attReasonId);
            }
            if(attReasonId && attReasonId.valueOf()){
                if(item.getAttendanceReason()){
                    item.getAttendanceReason().setId(attReasonId);
                    item.getAttendanceReason().setDescription(model.getAttendanceReasonDescription());
                }else{
                    var reason = new chlk.models.attendance.AttendanceReason(attReasonId, model.getAttendanceReasonDescription());
                    item.setAttendanceReason(reason);
                }
            }else{
                item.setAttendanceReason(null);
            }
            var result = new ria.async.DeferredData(item);
            return this.UpdateView(chlk.activities.attendance.ClassListPage, result, window.noLoadingMsg);
        },


        Number,  function changeAttendanceType_(submitType, currentType){
            var attTypeEnum = chlk.models.attendance.AttendanceTypeEnum;
            var types = [
                attTypeEnum.PRESENT.valueOf(),
                attTypeEnum.ABSENT.valueOf(),
                attTypeEnum.EXCUSED.valueOf(),
                attTypeEnum.LATE.valueOf()
            ];
            var index = types.indexOf(currentType);
            var increment = submitType == 'leftArrow' ? -1 : submitType == 'rightArrow' ? 1 : 0;
            if(increment == -1 && index == 0)
                index = types.length;
            else if(increment == 1 && index == types.length - 1)
                index = -1;
            index += increment
            return types[index];
        },
    ])
});
