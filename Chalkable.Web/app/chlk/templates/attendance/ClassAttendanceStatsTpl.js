REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.ClassAttendanceStatsViewData');

NAMESPACE('chlk.templates.attendance',function(){
   "use strict";
    /**@class chlk.templates.attendance.ClassAttendanceStatsTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassAttendanceStats.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendanceStatsViewData)],
        'ClassAttendanceStatsTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.DateTypeEnum, 'dateType',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartDateItem), 'absences',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartDateItem), 'lates',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.ChartDateItem), 'presents',

            Object, function getChartOptions(){
                var absences = [], lates = [], presents = [], categories = [];
                this.getAbsences().forEach(function(item){
                    categories.push(item.getSummary());
                    absences.push(item.getNumber());
                });

                this.getLates().forEach(function(item){
                    lates.push(item.getNumber());
                });

                this.getPresents().forEach(function(item){
                    presents.push(item.getNumber());
                });

                return {
                    chart: {
                        width: 800,
                        height: 200
                    },

                    plotOptions:{
                        line: {
                            marker: {
                                symbol: 'circle'
                            }
                        }
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
                            return { x: point.plotX - 3, y: point.plotY - 20 - this.label.div.children[0].clientHeight };
                        },
                        formatter: function () {
                            var pointIndex = this.point.index,
                                res = [];

                            if(absences[pointIndex] === this.y)
                                res.push(this.y + ' Absent');

                            if(lates[pointIndex] === this.y)
                                res.push(this.y + ' Late');

                            if(presents[pointIndex] === this.y)
                                res.push(this.y + ' Present');

                            return res.join('\n');
                        }
                    },

                    colors: ['#2f7790'],

                    series: [{
                        name: '',
                        color: 'rgba(212,81,81,1)',
                        data: absences
                    }, {
                        name: '',
                        color: 'rgba(248,230,165,1)',
                        data: lates
                    }, {
                        name: '',
                        color: 'rgba(65,183,104,1)',
                        data: presents
                    }]
                }
            }
    ]);
});