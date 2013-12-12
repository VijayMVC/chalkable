REQUIRE('chlk.templates.common.PageWithGrades');
REQUIRE('chlk.models.attendance.AdminAttendanceSummary');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminAttendanceSummaryTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminSummaryPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AdminAttendanceSummary)],
        'AdminAttendanceSummaryTpl', EXTENDS(chlk.templates.common.PageWithGrades), [
            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceNowSummary, 'nowAttendanceData',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceDaySummary, 'attendanceByDayData',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceMpSummary, 'attendanceByMpData',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

            [ria.templates.ModelPropertyBind],
            Boolean, 'renderNow',

            [ria.templates.ModelPropertyBind],
            Number, 'currentPage',

            [ria.templates.ModelPropertyBind],
            Boolean, 'renderDay',

            [ria.templates.ModelPropertyBind],
            Boolean, 'renderMp',

            [ria.templates.ModelPropertyBind],
            String, 'gradeLevelsIds',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'nowDateTime',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.MarkingPeriodId, 'fromMarkingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.MarkingPeriodId, 'toMarkingPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'startDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'endDate',

            [ria.templates.ModelPropertyBind],
            String, 'attendanceTypes',

            [[Boolean, String]],
            function getMarkerConfigs_(enabled, color_){
                return enabled ? {
                    enabled: true,
                    symbol: 'circle',
                    radius: 3,
                    fillColor: '#ffffff',
                    lineWidth: 2,
                    lineColor: color_,
                    states: {
                        hover: {
                            radius: 6,
                            lineWidth: 2,
                            enabled: true
                        }
                    }
                } : {
                    enabled: false,
                    states: {
                        hover: {
                            enabled: false
                        }
                    }
                };
            },

            [[ArrayOf(chlk.models.people.User), Boolean]],
            function getPreparedStudents(studentsList, needPlus_){
                return studentsList;
            },

            [[ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), Object]],
            function getMainChartConfigs(attendancestats, width_){
                var categories = [], series = [], absent=[], excused=[], late=[];
                attendancestats && attendancestats.forEach(function(item){
                    categories.push(item.getSummary());
                    absent.push(item.getAbsentCount());
                    excused.push(item.getExcusedCount());
                    late.push(item.getLateCount());
                });
                series = [{
                    name: Msg.Late,
                    data: late,
                    zIndex: 10,
                    marker: this.getMarkerConfigs_(true, '#e49e3c'),
                    enableMouseTracking: true
                }, {
                    name: Msg.Absent,
                    data: absent,
                    zIndex: 1,
                    marker: this.getMarkerConfigs_(false),
                    enableMouseTracking: false
                }, {
                    name: Msg.Excused,
                    data: excused,
                    zIndex: 1,
                    marker: this.getMarkerConfigs_(false),
                    enableMouseTracking: false
                }];
                return {
                    backgroundColor: 'transparent',
                    chart: {
                        backgroundColor: 'transparent',
                        width: width_ || 660,
                        height: 178,
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
                        gridLineDashStyle: 'dot',
                        min: 0
                    },
                    tooltip: {
                        headerFormat: '',
                        pointFormat: '<b class="chart-text">{point.y}</b>',
                        borderWidth: 0,
                        borderRadius: 2,
                        useHTML: true,
                        positioner: function (labelWidth, labelHeight, point) {
                            return { x: point.plotX + 7, y: point.plotY - 37 };
                        },
                        style: {
                            display: 'none'
                        }
                    },
                    legend:{
                        enabled: false
                    },
                    colors: ['#e49e3c', '#c1c1c1', '#c1c1c1'],
                    //colors: ['#e49e3c', '#b93838', '#5093a7'],

                    series: series
                }
            },

            [[ArrayOf(chlk.models.attendance.AdminAttendanceStatItem)]],
            function getMpChartConfigs(attendancestats){
                var categories = [], series = [], absent=[], excused=[], late=[];
                attendancestats && attendancestats.forEach(function(item){
                    categories.push(item.getSummary());
                    absent.push(item.getAbsentCount());
                    excused.push(item.getExcusedCount());
                    late.push(item.getLateCount());
                });
                series = [{
                    name: Msg.Late,
                    data: late
                }, {
                    name: Msg.Absent,
                    data: absent,
                    visible: false
                }, {
                    name: Msg.Excused,
                    data: excused,
                    visible: false
                }];
                return {
                    backgroundColor: 'transparent',
                    chart: {
                        backgroundColor: 'transparent',
                        width: 660,
                        type: 'column',
                        height: 215,
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
                    plotOptions: {
                        series: {
                            events: {
                                legendItemClick: function(event) {
                                    if(this.visible)
                                        return false;
                                    var index = this.index;
                                    this.chart.series.forEach(function(item){
                                        if(item.index != index)
                                            item.hide();
                                    });
                                }
                            }
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
                        gridLineDashStyle: 'dot',
                        min: 0
                    },
                    colors: ['#e49e3c', '#b93838', '#5093a7'],

                    series: series
                }
            }
        ])
});