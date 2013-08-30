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
                    date_ = date_ || new chlk.models.common.ChlkDate(getDate());
                    var classes = this.classService.getClassesForTopBar(true);
                    var topModel = new chlk.models.class.ClassesForTopBar(classes);
                    var model = new chlk.models.attendance.ClassList(topModel, classId, items, date_, true, this.getContext().getSession().get('attendanceReasons', []));
                    return model;
                }, this);
            return this.PushView(chlk.activities.attendance.ClassListPage, result);
        }
    ])
});
