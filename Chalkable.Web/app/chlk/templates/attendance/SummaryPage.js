REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.SummaryPage');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.SummaryPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/SummaryPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.SummaryPage)],
        'SummaryPage', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceSummary, 'summary',

            [[ArrayOf(chlk.models.attendance.StudentSummaryItem)]],
            ArrayOf(Array), function getGroupedStudents(students){
                var res = [], res1=[];
                students.forEach(function(item, index){
                    if(index && index % 18 == 0){
                        res.push(res1);
                        res1=[];
                    }
                    res1.push(item);
                });
                var max = 6 - res1.length % 6;
                if(max != 6)
                    for(var i=0; i < max; i++)
                        res1.push(false);
                res.push(res1);
                return res;
            },

            [[chlk.models.attendance.AbsentLateSummaryItem]],
            Object, function getChartOptions(data){
                var stats = data.getStat(), names = [], series = [];
                stats.forEach(function(item){
                    names.push(item.getSummary());
                    series.push(item.getStudentCount());
                });

                return {
                    chart: {
                        type: 'area',
                        width: 704,
                        height: 179
                    },
                    xAxis: {
                        categories: names
                    },
                    colors: ['#d8d8d8'],

                    series: [{
                        name: '',
                        data: series
                    }]
                }
            }
        ])
});