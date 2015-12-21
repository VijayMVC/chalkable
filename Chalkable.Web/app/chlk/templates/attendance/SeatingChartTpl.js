REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.SeatingChart');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.SeatingChartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/SeatingChartPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.SeatingChart)],
        'SeatingChartTpl', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inProfile',

            [ria.templates.ModelPropertyBind],
            Number, 'columns',

            [ria.templates.ModelPropertyBind],
            Number, 'rows',

            [ria.templates.ModelPropertyBind],
            Boolean, 'scheduled',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ablePost',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableChangeReasons',

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

            Boolean, function isScheduledInMp(){
                return !!this.getSeatingList() && !!this.getNotSeatingStudents();
            },

            Boolean, function hasStudentOnChart(){
                var seatingList = this.getSeatingList();
                return seatingList && seatingList.length > 0 && seatingList.filter(function(items){
                    return items.filter(function(item){
                        return item.getInfo() != null && item.getInfo() != undefined;
                    }).length > 0
                }).length > 0;
            },

            function getLateReasons(){
                return (this.getReasons() || []).filter(function(item){
                    var len;
                    len = (item.getAttendanceLevelReasons() || []).filter(function(reason){
                        return reason.getLevel() == 'T'
                    }).length;
                    return !!len;
                })
            },

            function getAbsentReasons(){
                return (this.getReasons() || []).filter(function(item){
                    var len;
                    len = (item.getAttendanceLevelReasons() || []).filter(function(reason){
                        return reason.getLevel() == 'A';// || reason.getLevel() == 'AO' ||
                            //reason.getLevel() == 'H' || reason.getLevel() == 'HO';
                    }).length;
                    return !!len;
                })
            },

            String, function getTextForPost(){
                var res = {
                    columns: this.getColumns(),
                    rows: this.getRows(),
                    classId: this.getClassId().valueOf()
                }, seatingList = [];
                this.getSeatingList() && this.getSeatingList().forEach(function(items){
                    var seatings = [];
                    items.forEach(function(item){
                        seatings.push({
                            row: item.getRow(),
                            column: item.getColumn(),
                            studentId: item.getInfo() ? item.getInfo().getStudent().getId().valueOf() : null,
                            index: item.getIndex()
                        })
                    });
                    seatingList.push(seatings);
                });
                res.seatingList = seatingList;
                return JSON.stringify(res);
            },

            Object, function getMinData(){
                var rows = 0, columns = 0;
                this.getSeatingList() && this.getSeatingList().forEach(function(items){
                    items.forEach(function(item){
                        if(item.getInfo()){
                            if(item.getColumn() > columns)
                                columns = item.getColumn();
                            if(item.getRow() > rows)
                                rows = item.getRow();
                        }
                    });
                });
                var res = {
                    rows: rows,
                    columns: columns
                };
                return res;
            }
        ]);
});