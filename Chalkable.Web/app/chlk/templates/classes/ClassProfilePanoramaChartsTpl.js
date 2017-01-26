REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.profile.ClassPanoramaViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfilePanoramaChartsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfilePanoramaCharts.jade')],
        [ria.templates.ModelBind(chlk.models.profile.ClassPanoramaViewData)],
        'ClassProfilePanoramaChartsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.profile.ClassDistributionSectionViewData, 'classDistributionSection',

            function columnSelectDeselectHandler_(chart, data){
                setTimeout(function(){
                    var cnt = jQuery(chart.graphic.element).parents('.chart-container');
                    var selected = cnt.highcharts().getSelectedPoints();
                    var ids = [];
                    selected.forEach(function(column){
                        ids = ids.concat(data[column.index].getStudentIds())
                    });

                    cnt.trigger('columnselect', [ids])
                })
            },

            [[Object, String]],
            function getDistributionChartOptions_(distribution, color){
                if(!distribution)
                    return null;

                var data = distribution.getDistributionStats(), classAvg = distribution.getClassAvg(),
                    categories = [], columnData = [], that = this, useUnSelect = true;

                data.forEach(function(item){
                    categories.push(item.getSummary());
                    columnData.push(item.getCount());
                });

                return {
                    legend:{
                        enabled: true
                    },

                    chart:{
                        height: 200
                    },

                    plotOptions:{
                        line: {
                            marker: {
                                enabled: false
                            }
                        }
                    },

                    xAxis: {
                        categories: categories,
                        gridLineWidth:0,
                        lineWidth:0
                    },

                    yAxis: {
                        gridLineWidth: 1,
                        lineWidth: 1,
                        lineColor: '#ebebeb',
                        gridLineColor: '#ebebeb',
                        gridLineDashStyle: 'solid',
                        startOnTick: true,
                        showFirstLabel: true//,
                        //min: 0
                    },

                    series: [{
                        type: 'column',
                        color: color,
                        name: '',
                        showInLegend: false,
                        data: columnData,
                        allowPointSelect: true,
                        point: {
                            events: {
                                select: function(){
                                    that.columnSelectDeselectHandler_(this, data);
                                    useUnSelect = false;
                                    setTimeout(function(){
                                        useUnSelect = true;
                                    }, 5)
                                },

                                unselect: function(){
                                    if(useUnSelect){
                                        that.columnSelectDeselectHandler_(this, data);
                                        useUnSelect = false;
                                        setTimeout(function(){
                                            useUnSelect = true;
                                        }, 5)
                                    }
                                }
                            }
                        }
                    }]
                }
            },

            function getAbsencesDistributionChartOptions(){
                return this.getDistributionChartOptions_(this.getClassDistributionSection().getAbsencesDistribution(), '#fbc649');
            },

            function getDisciplineDistributionChartOptions(){
                return this.getDistributionChartOptions_(this.getClassDistributionSection().getDisciplineDistribution(), '#fb6149');
            },

            function getGradeAverageDistributionChartOptions(){
                return this.getDistributionChartOptions_(this.getClassDistributionSection().getGradeAverageDistribution(), '#31859b');
            }
        ])
});