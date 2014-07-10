REQUIRE('chlk.templates.common.PageWithClassesAndGradingPeriodsTpl');
REQUIRE('chlk.models.grading.GradingStudentClassSummaryViewData');
REQUIRE('chlk.templates.announcement.FeedItemTpl');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.GradingStudentClassSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingStudentClassSummary.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingStudentClassSummaryViewData)],
        'GradingStudentClassSummaryTpl', EXTENDS(chlk.templates.common.PageWithClassesAndGradingPeriodsTpl), [
            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.MarkingPeriod, 'currentMarkingPeriod',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingStatsByDateViewData), 'avgPerDate',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.AnnTypeGradeStatsViewData), 'annTypesGradeStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.GradingClassSummaryPart, 'summaryPart',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassForTopBar, 'clazz',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'announcements',

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

            Number, function getAvg(){
                var items = this.getAvgPerDate(), res=0;
                if(!items) return null;
                items.forEach(function(item){
                    res+=item.getAvg();
                });
                return (res - res % items.length) / items.length;
            },

            Object, function getChartOptions(){
                var categories=[], mineData=[], peersData=[];
                var mineAvgs = this.getAvgPerDate();
                var typesInfo = this.getAnnTypesGradeStats();

                if (!mineAvgs || !typesInfo) return null;

                var series = [], that = this;
                if(typesInfo){
                    typesInfo.forEach(function(type, i){
                        var data = [];
                        type.getGradePerDate().forEach(function(item){
                            data.push(item.getAvg() || 0);
                        });
                        series.push({
                            name: type.getTypeName(),
                            marker: {
                                enabled: false,
                                states: {
                                    hover: {
                                        enabled: false
                                    }
                                }
                            },
                            color: 'rgba(193,193,193,0.2)',
                            data: data
                        });
                    });
                }
                if(mineAvgs){
                    mineAvgs.forEach(function(item, index){
                        categories.push(item.getDate().format('M d'));
                        mineData.push(item.getAvg() || 0);
                    });
                }
                series.push({
                    name: 'Mine',
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
                    color: '#4e899e',
                    data: mineData
                });
                return {
                    backgroundColor: 'transparent',
                    chart: {
                        backgroundColor: 'transparent',
                        width: 650,
                        height: 152,
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
                        gridLineWidth: 0
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

                    series: series/*,
                    colors: ['#bbbbbb', '#4e899e']*/
                }
            }
        ]);
});
