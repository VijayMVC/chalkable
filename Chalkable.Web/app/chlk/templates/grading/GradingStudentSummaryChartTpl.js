REQUIRE('chlk.templates.grading.GradingStudentSummaryTpl');
REQUIRE('chlk.models.grading.GradingStudentSummaryViewData');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingStudentSummaryChartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingStudentSummaryChart.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingStudentSummaryViewData)],
        'GradingStudentSummaryChartTpl', EXTENDS(chlk.templates.grading.GradingStudentSummaryTpl), [

            Object, function getChartOptions(){
                var categories=[], mineData=[], peersData=[];
                var totalAvgs = this.getTotalAvgPerDate();
                var peersAvgs = this.getPeersAvgPerDate();
                totalAvgs.forEach(function(item, index){
                    categories.push(item.getDate().format('M d'));
                    mineData.push(item.getAvg() || 0);
                    peersData.push(peersAvgs[index].getAvg() || 0);
                });
                return {
                    backgroundColor: 'transparent',
                    chart: {
                        type: 'area',
                        backgroundColor: 'transparent',
                        width: 650,
                        height: 192,
                        style: {
                            fontFamily: 'Arial',
                            fontSize: '10px',
                            color: '#a6a6a6'
                        }

                    },
                    labels: {
                        style: {
                            color: '#a6a6a6',
                            textOverflow: 'ellipsis',
                            fontSize: '9px'
                        }
                    },
                    credits: {
                        enabled: false
                    },
                    title: {
                        text: ''
                    },
                    xAxis: {
                        categories: categories
                    },
                    yAxis: {
                        title: {
                            text: ''
                        },
                        lineWidth:0,
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
                            return { x: point.plotX + 17, y: point.plotY - 37 };
                        },
                        style: {
                            display: 'none'
                        }
                    },
                    plotOptions: {
                        area: {
                            fillOpacity: 0.5
                        }
                    },

                    series: [{
                        lineWidth: 0,
                        data: peersData,
                        marker: {
                            enabled: false,
                            states: {
                                hover: {
                                    enabled: false
                                }
                            }
                        }
                    }, {
                        name: '',
                        marker: {
                            enabled: true,
                            symbol: 'circle',
                            radius: 3,
                            fillColor: '#ffffff',
                            lineWidth: 2,
                            lineColor: '#4e899e',
                            states: {
                                hover: {
                                    radius: 6
                                }
                            }
                        },
                        data: mineData
                    }],
                    colors: ['#bbbbbb', '#4e899e']
                }
            }
        ]);
});
