REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.discipline.AdminDisciplineSummary');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.AdminDisciplinesSummaryTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/admin-disciplines-summary.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.AdminDisciplineSummary)],
        'AdminDisciplinesSummaryTpl', EXTENDS(chlk.templates.common.PageWithGrades), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartItem), 'dayStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartItem), 'mpStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartItem), 'nowStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineStudents), 'disciplinesByType',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            String, 'markingPeriodName',

            [ria.templates.ModelPropertyBind],
            Number, 'nowCount',

            [ria.templates.ModelPropertyBind],
            Number, 'dayCount',

            [ria.templates.ModelPropertyBind],
            Number, 'mpCount',

            [ria.templates.ModelPropertyBind],
            String, 'gradeLevelsIds',

            [[ArrayOf(chlk.models.common.ChartItem), Object]],
            Object, function getChartData(items, summaryConverter_){
                var categories = [], data = [], avgs = [];
                items.forEach(function(item){
                    categories.push(summaryConverter_ ? summaryConverter_(item.getSummary()) : item.getSummary());
                    data.push(item.getValue());
                    avgs.push(item.getAvg());
                });
                var res = {
                    data: data,
                    avgs: avgs,
                    categories: categories
                };
                return res;
            },

            [[ArrayOf(chlk.models.common.ChartItem), Object]],
            Object, function getNowChartData(items, summaryConverter_){
                var categories = [], data = [], avgs = [];
                items.forEach(function(item){
                    categories.push(summaryConverter_ ? summaryConverter_(item.getSummary()) : item.getSummary());
                    data.push(item.getValue());
                    avgs.push(item.getAvg());
                });
                var res = {
                    data: data,
                    avgs: avgs,
                    categories: categories
                };
                return res;
            },

            function getMpChartConfigs(){
                var configs = this.getChartData(this.getMpStats(), function(item){
                    return new chlk.models.common.ChlkSchoolYearDate.$createServerTime(item).format('D d');
                });
                var res =  {
                    chart: {
                        type: 'area'
                    },
                    xAxis: {
                        categories: configs.categories
                    },
                    yAxis:{
                        tickInterval: 25,
                        endOnTick: false,
	                    maxPadding: 1
                    },
                    series: [{
                        name: '',
                        lineWidth: 0,
                        data: configs.data
                    }]
                };
                return res;
            },

            function isNowHidden(){
                var date = this.getDate();
                return date && date.getDate() < this.getSchoolYearServerDate();
            },

            function getDayChartConfigs(){
                var configs = this.getChartData(this.getDayStats()), max = 0;
                configs.data.forEach(function(item, i){
                    if(item > max)
                        max = item;
                    if(configs.avgs[i] > max)
                        max = configs.avgs[i];
                });
                max = Math.ceil(max/25) * 25;
                var res =  {
                    xAxis: {
                        categories: configs.categories
                    },
                    yAxis:{
                        //tickInterval: 25
                    },
                    chart: {
                        type: 'column'
                    },
                    tooltip: {
                        enabled: false
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
                    colors: ['#2f7790', '#d8d8d8'],

                    series: [{
                        name: '',
                        data: configs.data,
                        pointWidth: 20
                    }, {
                        name: '',
                        data: configs.avgs,
                        pointWidth: 20
                    }]
                };
                return res;
            },

            function getNowChartConfigs(){
                var configs = this.getChartData(this.getNowStats()), max = 0;
                configs.data.forEach(function(item, i){
                    if(item > max)
                        max = item;
                    if(configs.avgs[i] > max)
                        max = configs.avgs[i];
                });
                max = Math.ceil(max/25) * 25;
                var res =  {
                    xAxis: {
                        categories: configs.categories
                    },
                    chart: {
                        type: 'column'
                    },
                    tooltip: {
                        enabled: false
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
                    colors: ['#2f7790', '#d8d8d8'],

                    series: [{
                        name: '',
                        data: configs.data,
                        pointWidth: 20
                    }]
                };
                return res;
            }

        ]);
});