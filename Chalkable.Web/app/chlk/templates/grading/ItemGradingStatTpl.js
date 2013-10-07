REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.grading.ItemGradingStat');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ItemGradingStatTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/PopupChart.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ItemGradingStat)],
        'ItemGradingStatTpl', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            Number, 'gradingStyle',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GraphPoint), 'graphPoints',

            function l(x,j,XY){
                var res= 1,i;
                for(i=0;i<5;i++)
                    if(i != j){
                        res= res*(x - XY[i][0])/(XY[j][0] - XY[i][0]);
                    }
                return res;
            },

            function interpolate(XY,x){
                var res = 0,i;
                for(i=0;i<5;i++)
                    res = res + XY[i][1] * this.l(x,i,XY);
                return res < 0 ? 0 : res;
            },

            Object, function getGraphConfigs(){
                var graphpoints = this.getGraphPoints();
                var tickPositions = [], ticksLetters = [], graphdata=[], max = 0;

                if(graphpoints && graphpoints.length){
                    var XY = [];
                    graphpoints.forEach(function(item){
                        XY.push([item.getGrade(), item.getStudentCount()]);
                    });


                    XY.unshift([0,0]);
                    var x = graphpoints[0].getStartInterval();
                    graphdata.push([x, this.interpolate(XY, x)]);
                    tickPositions.push(x);
                    ticksLetters[x] = GradingStyler.getFromMapped(graphpoints[0].getGradingStyle(), graphpoints[0].getMappedStartInterval());
                    graphpoints.forEach(function(item){
                        x = item.getEndInterval();
                        graphdata.push([item.getGrade(), item.getStudentCount()]);
                        if(item.getStudentCount() > max)
                            max = item.getStudentCount();
                        var fx = this.interpolate(XY, x);
                        tickPositions.push(x);
                        ticksLetters[x] = GradingStyler.getFromMapped(graphpoints[0].getGradingStyle(), item.getMappedEndInterval());
                        graphdata.push([x, fx]);
                        if(fx > max)
                            max = fx;
                    }.bind(this));

                    var tickYInterval = ((max/2 > 10) ? 10 : Math.floor(max/2)) || 1;

                    return {
                        backgroundColor: 'transparent',
                        chart: {
                            type: 'spline',
                            backgroundColor: 'transparent',
                            width: 180,
                            height: 100,
                            style: {
                                fontFamily: 'Arial',
                                fontSize: '10px',
                                color: '#a6a6a6'
                            }

                        },
                        credits: {
                            enabled: false
                        },
                        title: {
                            text: ''
                        },
                        xAxis: {
                            showLastLabel: true,
                            showFirstLabel: true,
                            startOnTick: true,
                            tickPositions: tickPositions,
                            labels: {
                                formatter : function(){
                                    return ticksLetters[this.value];
                                }
                            },
                            tickInterval: (graphpoints[0].getEndInterval() -  graphpoints[0].getStartInterval()),
                            min: graphpoints[0].getStartInterval()
                        },
                        yAxis: {
                            title: {
                                text: ''
                            },
                            min: 0,
                            tickInterval: tickYInterval,
                            gridLineWidth: 0
                        },
                        legend:{
                            enabled: false
                        },
                        tooltip: {
                            enabled: false
                        },
                        plotOptions: {
                            spline: {
                                lineWidth: 1,
                                marker: {
                                    enabled: false,
                                    states: {
                                        hover: {
                                            enabled: false
                                        }
                                    }
                                },
                                pointStart: graphpoints[0].getStartInterval()
                            }
                        },
                        colors: ['#3f8198'],

                        series: [{
                            name: '',
                            data: graphdata
                        }],

                        labels: {
                            style: {
                                fontSize: '9px',
                                fontWeight: 'bold'
                            }
                        }
                    }
                }else
                    return null;
            }
        ]);
});
