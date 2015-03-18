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

            [[Number, Number]],
            String, function getColor(i, opacity_){
                var colors =  ['rgba(204, 0, 0, 1)', 'rgba(204, 102, 0, 1)', 'rgba(0, 204, 204, 1)',
                    'rgba(102, 204, 0, 1)', 'rgba(102, 0, 204, 1)', 'rgba(204, 204, 0, 1)',
                    'rgba(0, 102, 204, 1)', 'rgba(0, 51, 0, 1)', 'rgba(255, 51, 153, 1)',
                    'rgba(102, 51, 0, 1)', 'rgba(0, 102, 102, 1)'];
                var color = colors[i % colors.length];
                if(opacity_)
                    color = color.replace(/, 1\)/, ', ' + opacity_.toString() + ')');
                return color;
            },

            /*[[ArrayOf(chlk.models.attendance.StudentSummaryItem)]],
            ArrayOf(Array), function getGroupedStudents(students){
                if(!students.length) return [];
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
            },*/

            [[chlk.models.attendance.AbsentLateSummaryItem]],
            Object, function getChartOptions(data){
                var classesInfo = data.getClassesStats();
                var series = [];
                var dt = getDate();
                dt.setMinutes(0);
                dt.setHours(0);
                dt.setSeconds(0);
                dt.setMilliseconds(0);
                var today = new chlk.models.common.ChlkDate(getDate());
                var start = today.add(chlk.models.common.ChlkDateEnum.MONTH, -1);
                classesInfo && classesInfo.forEach(function(type, i){
                    var data = [], sTooltips = {};
                    type.getDayStats().forEach(function(item, i){
                        var cur = item.getDate().getDate();
                        if(cur >= start.add(chlk.models.common.ChlkDateEnum.DAY, -1).getDate() && cur <= today.getDate()){
                            var time = item.getDate().getDate().getTime();
                            data.push([time, item.getStudentCount() || 0]);
                        }
                    });

                    series.push({
                        name: '',
                        marker: {
                            enabled: false
                        },
                        color: 'rgba(193,193,193,0.5)',
                        data: data
                    });
                });

                return {
                    chart: {
                        width: 800,
                        height: 200
                    },
                    xAxis: {
                        type: 'datetime',
                        min: Date.UTC(start.getDate().getFullYear(), start.getDate().getMonth(), start.getDate().getDate()),
                        max: Date.UTC(today.getDate().getFullYear(), today.getDate().getMonth(), today.getDate().getDate()),
                        labels: {
                            formatter: function() {
                                return Highcharts.dateFormat('%b %e', this.value);
                            }
                        }
                    },

                    yAxis: {
                        min: 0,
                        title: {text:'Student Count'}
                    },

                    series: series
                }
            }
        ])
});