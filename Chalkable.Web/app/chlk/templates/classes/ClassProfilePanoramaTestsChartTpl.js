REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.profile.ClassPanoramaViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfilePanoramaTestsChartTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfilePanoramaTestsChart.jade')],
        [ria.templates.ModelBind(chlk.models.profile.ClassPanoramaViewData)],
        'ClassProfilePanoramaTestsChartTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStatsByClass',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'selectStandardizedTestsStats',

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
                        name: item.getFullName() + ' - class avg',
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
                                fillColor: '#ffffff',
                                symbol: 'circle',
                                radius: 5,
                                lineWidth: 3,
                                enabled: true,
                                lineColor: null
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

                    series: series
                }
            }
        ])
});