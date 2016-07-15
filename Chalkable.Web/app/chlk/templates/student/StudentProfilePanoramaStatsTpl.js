REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.student.StudentPanoramaViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfilePanoramaStatsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfilePanoramaStats.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentPanoramaViewData)],
        'StudentProfilePanoramaStatsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            function getTestsChartOptions_(){
                var standardizedTestsStats = this.getModel().getPanoramaInfo().getStandardizedTestsStats() || [],
                    categories = [], series = [];

                if(!standardizedTestsStats.length)
                    return null;

                standardizedTestsStats.forEach(function(item, index){
                    var data = item.getDailyStats(), columnData = [];

                    data.forEach(function(stat){
                        columnData.push(stat.getNumber());
                        categories.push(stat.getSummary());
                    });

                    series.push({
                        type: 'line',
                        name: item.getStandardizedTest().getDisplayName() + ' | ' + item.getComponent().getName() + ' | ' + item.getScoreType().getName(),
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
            },

            function getAbsencesChartOptions_(){
                var stats = this.getModel().getPanoramaInfo().getStudentAbsenceStats() || [],
                    columnData = [], absenceLevels = ['Absent', 'Half Day', 'All Day'],
                    categoriesNames = ['0.0', '0.5', '1.0'];

                if(!stats.length)
                    return null;

                stats.forEach(function(item, index){
                    columnData.push([item.getDate().getDate().getTime(), absenceLevels.indexOf(item.getAbsenceLevel())]);
                });

                return {
                    chart:{
                        height: 200
                    },

                    xAxis: {
                        type: 'datetime',
                        labels: {
                            formatter: function() {
                                return Highcharts.dateFormat('%b %Y', this.value);
                            }
                        },
                        gridLineWidth:0,
                        lineWidth:0
                    },

                    yAxis: {
                        gridLineWidth: 1,
                        lineWidth: 1,
                        lineColor: '#ebebeb',
                        gridLineColor: '#ebebeb',
                        gridLineDashStyle: 'solid',
                        labels: {
                            formatter: function () {
                                return categoriesNames[this.value];
                            }
                        },
                        startOnTick: true,
                        showFirstLabel: true//,
                        //min: 0
                    },

                    tooltip: {
                        formatter: function () {
                            return '<b>' + absenceLevels[this.y] + '</b>';
                        }
                    },

                    series: [{
                        data: columnData,
                        color: '#41b768'
                    }]
                }
            },

            function getDisciplinesChartOptions_(){
                var stats = this.getModel().getPanoramaInfo().getDailyDisciplines() || [],
                    columnData = [];

                if(!stats.length)
                    return null;

                stats.forEach(function(item, index){
                    columnData.push([item.getDate().getDate().getTime(), item.getNumber()]);
                });

                return {
                    chart:{
                        height: 200
                    },

                    xAxis: {
                        type: 'datetime',
                        labels: {
                            formatter: function() {
                                return Highcharts.dateFormat('%b %Y', this.value);
                            }
                        },
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
                        data: columnData,
                        color: '#f8e6a5'
                    }]
                }
            }
        ]);
});