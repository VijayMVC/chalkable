REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.profile.ClassPanoramaViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfilePanoramaStudentsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfilePanoramaStudents.jade')],
        [ria.templates.ModelBind(chlk.models.profile.ClassPanoramaViewData)],
        'ClassProfilePanoramaStudentsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.profile.StandardizedTestStatsViewData), 'standardizedTestsStatsByClass',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.panorama.StudentStandardizedTestStats), 'students',

            [ria.templates.ModelPropertyBind],
            chlk.models.profile.ClassPanoramaSortType, 'orderBy',

            [ria.templates.ModelPropertyBind],
            Boolean, 'descending',

            function getStudentChartOptions(test){
                var data = [];

                test.getDailyStats().forEach(function(item){
                    data.push([item.getDate().getDate().getTime(), item.getNumber()])
                });

                return {
                    chart:{
                        height: 50
                    },

                    plotOptions:{
                        area:{
                            enableMouseTracking:false,
                            lineWidth:1,
                            shadow:false,
                            marker:{
                                enabled: false
                            }
                        }
                    },

                    xAxis: {
                        labels:{
                            enabled:false
                        },
                        maxPadding:0,
                        minPadding:0,
                        gridLineWidth: 0,
                        endOnTick:false,
                        showFirstLabel:false
                    },

                    yAxis: {
                        maxPadding:0,
                        minPadding:0,
                        gridLineWidth: 0,
                        endOnTick:false,
                        labels:{
                            enabled:false
                        }
                    },

                    tooltip:{
                        enabled:false
                    },

                    series: [{
                        type:'area',
                        data: data
                    }]
                }
            }
        ])
});