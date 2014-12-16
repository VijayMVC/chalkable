REQUIRE('chlk.templates.common.SimpleObjectTpl');
REQUIRE('chlk.models.common.SimpleObject');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ItemGradingStatTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/PopupChart.jade')],
        [ria.templates.ModelBind(chlk.models.common.SimpleObject)],
        'ItemGradingStatTpl', EXTENDS(chlk.templates.common.SimpleObjectTpl), [
            function interpolate(XY,x,len){
                var res = 0,i;
                for(i=0;i<=len;i++)
                    res = res + XY[i][1] * this.l(x,i,XY, len);
                return res < 0 ? 0 : res;
            },

            function l(x,j,XY,len){
                var res= 1,i;
                for(i=0;i<=len;i++)
                    if(i != j){
                        res= res*(x - XY[i][0])/(XY[j][0] - XY[i][0]);
                    }
                return res;
            },

            function getClassAvg(){
                var gradedStudentCount = 0, sum = 0, numericGrade, gradeValue;
                var items = this.getValue().studentannouncements || [], classAvg = null;
                items.forEach(function(item){
                    numericGrade = item.numericscore;
                    gradeValue = item.scorevalue;
                    if(!item.dropped
                        && !item.isincomplete
                        && (gradeValue && gradeValue.toLowerCase() != 'ps'
                            && gradeValue.toLowerCase() != 'wd'
                            && gradeValue.toLowerCase() != 'nc')
                        && item.includeinaverage
                        && (numericGrade || numericGrade == 0 || gradeValue == 0 || gradeValue)){
                            gradedStudentCount++;
                            sum += (numericGrade || 0);
                    }
                });
                if(gradedStudentCount){
                    classAvg = (sum / gradedStudentCount).toFixed(2);
                }
                return classAvg;
            },

            Object, function getGraphConfigs(){
                var graphpoints = this.getValue().graphpoints;
                var len = graphpoints.length;
                var tickPositions = [], ticksLetters = [], graphdata=[], max = 0;

                if(graphpoints && len){
                    var XY = [];
                    graphpoints.forEach(function(item){
                        XY.push([item.grade, item.studentcount]);
                    });

                    XY.unshift([0,0]);
                    var x = graphpoints[0].startinterval;
                    graphdata.push([x, this.interpolate(XY, x, len)]);
                    tickPositions.push(x);
                    ticksLetters[x] = GradingStyler.getFromMapped(graphpoints[0].gradingstyle, graphpoints[0].gradingstyle);
                    graphpoints.forEach(function(item){
                        x = item.endinterval;
                        graphdata.push([item.grade, item.studentcount]);
                        if(item.studentcount > max)
                            max = item.studentcount;
                        var fx = this.interpolate(XY, x, len);
                        tickPositions.push(x);
                        ticksLetters[x] = GradingStyler.getFromMapped(graphpoints[0].gradingstyle, item.mappedendinterval);
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
                                formatter: function(){
                                    return '<div class="chart-label">' + ticksLetters[this.value] + '</div>';
                                },
                                useHTML: true
                            },
                            tickInterval: (graphpoints[0].endinterval -  graphpoints[0].startinterval),
                            min: graphpoints[0].startinterval
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
                                pointStart: graphpoints[0].startinterval
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
