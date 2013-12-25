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
            ArrayOf(ArrayOf(chlk.models.attendance.StudentSummaryItem)), function getGroupedStudents(students){
                var res = [], res1=[];
                students.forEach(function(item, index){
                    if(index && index % 18 == 0){
                        res.push(res1);
                        res1=[];
                    }
                    res1.push(item);
                });
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
                    backgroundColor: 'transparent',
                    chart: {
                        type: 'area',
                        backgroundColor: 'transparent',
                        width: 630,
                        height: 216,
                        plotBorderWidth: 0
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: ''
                    },
                    xAxis: {
                        categories: names,
                        lineWidth:0
                    },
                    yAxis: {
                        title: {
                            text: ''
                        },
                        showFirstLabel: false,
                        showLastLabel: false,
                        gridLineDashStyle: 'dot'
                    },
                    legend:{
                        enabled: false
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<b class="chart-text">{point.y}</b>',
                        borderWidth: 0,
                        borderRadius: 2,
                        useHTML: true,
                        positioner: function (labelWidth, labelHeight, point) {
                            return { x: point.plotX + 7, y: point.plotY - 37 };
                        },
                        style: {
                            display: 'none'
                        }
                    },
                    plotOptions: {
                        area: {
                            marker: {
                                enabled: false,
                                states: {
                                    hover: {
                                        enabled: false
                                    }
                                }
                            }
                        },
                        series:{
                            lineWidth: 0
                        }
                    },
                    colors: ['#d8d8d8'],

                    series: [{
                        name: '',
                        marker: {
                            enabled: true,
                            symbol: 'circle',
                            radius: 3,
                            fillColor: '#ffffff',
                            lineWidth: 2,
                            lineColor: '#a6a6a6',
                            states: {
                                hover: {
                                    radius: 6,
                                    lineWidth: 2,
                                    enabled: true,
                                    lineColor: '#2F7790'
                                }
                            }
                        },
                        data: series
                    }]
                }
            }
        ])
});