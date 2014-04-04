REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.SeatingChart');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.SeatingChartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/SeatingChartPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.SeatingChart)],
        'SeatingChartTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            Number, 'columns',

            [ria.templates.ModelPropertyBind],
            Number, 'rows',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ablePost',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableRePost',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'notSeatingStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(ArrayOf(chlk.models.attendance.ClassAttendanceWithSeatPlace)), 'seatingList',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            String, function getTextForPost(){
                var res = {
                    columns: this.getColumns(),
                    rows: this.getRows(),
                    classId: this.getTopData().getSelectedItemId().valueOf()
                }, seatsList = [];
                this.getSeatingList().forEach(function(items){
                    items.forEach(function(item){
                        seatsList.push({
                            row: item.getRow(),
                            column: item.getColumn(),
                            studentId: item.getInfo() ? item.getInfo().getStudent().getId().valueOf() : null,
                            index: item.getIndex()
                        })
                    })
                });
                res.seatsList = seatsList;
                return JSON.stringify(res);
            }
        ]);
});