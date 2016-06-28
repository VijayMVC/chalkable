REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.discipline.ClassDisciplineStatsViewData');

NAMESPACE('chlk.templates.discipline',function(){
   "use strict";
    /**@class chlk.templates.discipline.ClassDisciplineStatsTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/ClassDisciplineStats.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.ClassDisciplineStatsViewData)],
        'ClassDisciplineStatsTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.DateTypeEnum, 'dateType',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartDateItem), 'dailySummaries',

            Object, function getChartOptions(){
                var data = [], categories = [];
                this.getDailySummaries().forEach(function(item){
                    categories.push(item.getSummary());
                    data.push(item.getNumber());
                });

                return {
                    chart: {
                        height: 200
                    },

                    xAxis: {
                        categories: categories,
                        gridLineWidth: 1,
                        gridLineColor: '#f0f0f0'
                    },

                    yAxis: {
                        gridLineWidth: 1,
                        gridLineColor: '#f0f0f0'
                    },

                    tooltip: {
                        positioner: function (labelWidth, labelHeight, point) {
                            return { x: point.plotX - 3, y: point.plotY - 47 };
                        },
                        formatter: function () {
                            return this.y + (this.y == 1 ? ' Infraction' : ' Infractions');
                        }
                    },

                    colors: ['#2f7790'],

                    series: [{
                        name: '',
                        color: 'rgba(47,119,144,1)',
                        data: data
                    }]
                }
            }
    ]);
});