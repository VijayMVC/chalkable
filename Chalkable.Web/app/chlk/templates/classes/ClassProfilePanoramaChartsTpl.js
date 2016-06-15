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

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStatsByClass',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'selectStandardizedTestsStats',

            [[Object, String]],
            function getDistributionChartOptions_(distribution, color){
                if(!distribution)
                    return null;

                var data = distribution.getDistributionStats(), classAvg = distribution.getClassAvg(),
                    categories = [], columnData = [], avgData = [];

                data.forEach(function(item){
                    categories.push(item.getSummary());
                    avgData.push(classAvg);
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
                        gridLineDashStyle: 'solid'
                    },

                    series: [{
                        type: 'column',
                        color: color,
                        name: '',
                        showInLegend: false,
                        data: columnData
                    }, {
                        type: 'line',
                        color: '#f8b681',
                        name: 'Class Mean',
                        data: avgData
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
            },

            function getTestsChartOptions_(){
                var standardizedTestsStatsByClass = this.getStandardizedTestsStatsByClass() || [],
                    selectStandardizedTestsStats = this.getSelectStandardizedTestsStats() || [],
                    categories = [], series = [];

                if(!standardizedTestsStatsByClass.length && !selectStandardizedTestsStats.length)
                    return null;

                standardizedTestsStatsByClass.forEach(function(item, index){
                    var data = item.getDailyStats(), columnData = [];

                    data.forEach(function(stat){
                        if(!index)
                            categories.push(stat.getSummary());
                        columnData.push(stat.getNumber());
                    });

                    series.push({
                        type: 'line',
                        name: item.getStandardizedTest().getDisplayName() + ' | ' + item.getComponent().getName() + ' | ' + item.getScoreType().getName() + ' - class avg',
                        data: columnData
                    })

                });

                selectStandardizedTestsStats.forEach(function(item, index){
                    var data = item.getDailyStats(), columnData = [];

                    data.forEach(function(stat){
                        columnData.push(stat.getNumber());
                    });

                    series.push({
                        type: 'line',
                        name: item.getStandardizedTest().getDisplayName() + ' | ' + item.getComponent().getName() + ' | ' + item.getScoreType().getName() + ' - selected avg',
                        data: columnData
                    })

                });

                return {
                    chart:{
                        height: 200
                    },

                    legend:{
                        enabled: true
                    },

                    plotOptions:{
                        line: {
                            marker: {
                                symbol: 'triangle-down'
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
                        gridLineDashStyle: 'solid'
                    },

                    series: series
                }
            }
        ])
});