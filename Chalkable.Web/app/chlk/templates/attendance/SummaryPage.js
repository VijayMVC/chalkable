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

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

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

            [[chlk.models.attendance.AbsentLateSummaryItem]],
            Object, function getChartOptions(data){
                var classesInfo = data.getClassesStats();
                var series = [];
                var today = new chlk.models.common.ChlkDate(getDate());
                var gpStart = this.getGradingPeriod().getStartDate();
                var start = today.add(chlk.models.common.ChlkDateEnum.MONTH, -1);
                var startDate = start.add(chlk.models.common.ChlkDateEnum.DAY, -1).getDate();
                var todayDate = getDate();
                classesInfo && classesInfo.forEach(function(type, i){
                    var data = [];
                    type.getDayStats().slice(-31).forEach(function(item, i){
                        var cur = getDate(item.date);
                        if(cur >= startDate && cur <= todayDate){
                            var time = cur.getTime();
                            data.push([time, item.studentcount || 0]);
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


                var min = Date.UTC(start.getDate().getFullYear(), start.getDate().getMonth(), start.getDate().getDate()),
                    max = Date.UTC(today.getDate().getFullYear(), today.getDate().getMonth(), today.getDate().getDate()),
                    needLabel = ((max - min) / 1000 / 3600 / 24) % 2 == 0,
                    chartMin = min - 1000 * 60 * 60 * 10;
                var needFirstLabel = needLabel;

                if(gpStart.getDate() > chartMin)
                    min = gpStart.getDate();

                return {
                    chart: {
                        width: 800,
                        height: 200,
                        events: {
                            load: function(){
                                this && this.series && this.series[0] && this.series[0].update({});
                            }
                        }
                    },
                    xAxis: {
                        type: 'datetime',
                        min: min - 1000 * 60 * 60 * 10,
                        max: max + 1000 * 60 * 60 * 10,
                        tickInterval:  24 * 3600 * 1000,
                        labels: {
                            formatter: function() {
                                if(this.isFirst)
                                    needLabel = needFirstLabel;
                                var res = needLabel ? Highcharts.dateFormat('%b %e', this.value) : '';
                                needLabel = !needLabel;
                                return res;
                            },
                            step: 1,
                            rotation: 0
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