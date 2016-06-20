REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseClassProfileViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfilePanoramaTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfilePanorama.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfilePanoramaTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [
            function getStudentChartOptions(test){
                var data = [];

                test.getDailyStats().forEach(function(item){
                    data.push(parseInt(item.getSummary(), 10), item.getNumber())
                });

                return {
                    chart:{
                        height: 100
                    },

                    plotOptions:{
                        series:{
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
                        }
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

                    series: [{type:'area',
                        data: [[
                            1352740844000,
                            12
                        ],
                            [
                                1352741744000,
                                10
                            ],
                            [
                                1352742644000,
                                20
                            ],
                            [
                                1352743544000,
                                40
                            ],
                            [
                                1352744444000,
                                50
                            ]
                        ]
                    }]
                }
            }
        ])
});