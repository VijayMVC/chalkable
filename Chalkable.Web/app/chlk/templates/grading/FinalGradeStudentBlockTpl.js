REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.StudentFinalGradeViewData');
REQUIRE('chlk.models.grading.AvgComment');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.FinalGradeStudentBlockTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/FinalGradeStudentBlock.jade')],
        [ria.templates.ModelBind(chlk.models.grading.StudentFinalGradeViewData)],
        'FinalGradeStudentBlockTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.ShortUserInfo, 'student',

            [ria.templates.ModelPropertyBind],
            chlk.models.grading.ShortStudentAverageInfo, 'currentStudentAverage',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.ShortStudentAverageInfo), 'studentAverages',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.StudentFinalAttendanceSummaryViewData, 'attendance',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineTypeSummaryViewData), 'disciplines',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentGradingByTypeStatsViewData), 'statsByType',

            Boolean, 'selected',

            Number, 'index',

            chlk.models.id.GradingPeriodId, 'gradingPeriodId',

            ArrayOf(chlk.models.grading.AvgComment), 'gradingComments',

            function getSortedDisciplines(){
                var res = [], d = [], len = this.getDisciplines().length;
                this.getDisciplines().forEach(function(item, i){
                    d.push(item);
                    if(i % 3 == 2 || i == len - 1){
                        res.push(d);
                        d = [];
                    }
                });
                return res;
            },

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

            [[chlk.models.grading.StudentGradingByTypeStatsViewData]],
            function getClassAnnouncementTypeName(stat){
                var name = stat.getClassAnnouncementTypeName();
                if(name)
                    return name;
                var id = stat.getClassAnnouncementTypeId().valueOf(), res = '';
                window.classesAdvancedData.forEach(function(item){
                    item.typesbyclass.forEach(function(type){
                        if(type.id == id)
                            res = type.name;
                    });
                });
                return res;
            },

            function getPresentWidth(item){
                var all = item.getPresentCount() + item.getLateCount() + item.getAbsentCount();
                if(!all || !item.getPresentCount())
                    return 0;
                return Math.ceil(item.getPresentCount() * 100 /all) + '%';
            },

            function getLateWidth(item){
                var all = item.getPresentCount() + item.getLateCount() + item.getAbsentCount();
                if(!all || !item.getLateCount())
                    return 0;
                return Math.ceil(item.getLateCount() * 100 /all) + '%';
            },

            function getAbsentWidth(item){
                var all = item.getPresentCount() + item.getLateCount() + item.getAbsentCount();
                if(!all || !item.getAbsentCount())
                    return 0;
                return 100 - parseInt(this.getPresentWidth(item)) - parseInt(this.getLateWidth(item)) + '%';
            },

            Object, function getChartOptions(){
                //var categories=[];
                var typesInfo = this.getStatsByType();
                var series = [], tooltips = [];
                typesInfo.forEach(function(type, i){
                    var data = [], sTooltips = {};
                    type.getStudentGradingStats().forEach(function(item, i){
                        var time = item.getDate().getDate().getTime();
                        data.push([time, item.getGrade() || 0]);
                        var textArray = [];
                        item.getAnnouncementGrades().forEach(function(item){
                            var grade = item.getStudentAnnouncements().getItems()[0].getNumericGradeValue();
                            textArray.push(grade + '<br>' + item.getTitle());
                        });
                        sTooltips[time] = textArray.join('<br>');
                    });

                    tooltips.push(sTooltips);
                    series.push({
                        name: type.getClassAnnouncementTypeName(),
                        marker: {
                            enabled: false
                        },
                        color: 'rgba(193,193,193,0.5)',
                        data: data
                    });
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
                        type: 'datetime',
                        dateTimeLabelFormats: {
                           day: '%b %e'
                        }
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
                        //pointFormat: '<b class="chart-text">{point.y}</b>',
                        formatter: function(){
                            var index = this.series.index;
                            return '<b class="chart-text">' + tooltips[index][this.x] + '<div class="triangle"></div></b>';
                        },
                        borderWidth: 0,
                        borderRadius: 2,
                        useHTML: true,
                        positioner: function (labelWidth, labelHeight, point) {
                            return { x: point.plotX - jQuery('.chart-text:visible').width()/2 + 30, y: point.plotY - jQuery('.chart-text:visible').height() - 15 };
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

                    series: series
                }
            }
        ]);
});
