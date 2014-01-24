REQUIRE('chlk.models.apps.AppGeneralInfoViewData');
REQUIRE('chlk.models.apps.Application');
REQUIRE('chlk.models.apps.AppRating');
REQUIRE('chlk.models.apps.AppState');
REQUIRE('chlk.models.developer.HomeAnalytics');

NAMESPACE('chlk.templates.apps', function () {

    ASSET('~/assets/jade/activities/apps/app-rating.jade')();
    /** @class chlk.templates.apps.AppGeneral*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-general-info.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppGeneralInfoViewData)],
        'AppGeneral', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'draftAppId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AppId, 'liveAppId',

            [ria.templates.ModelPropertyBind],
            String, 'appStatus',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppState, 'appState',

            [ria.templates.ModelPropertyBind],
            String, 'appName',

            [ria.templates.ModelPropertyBind],
            Boolean, 'appLive',

            [ria.templates.ModelPropertyBind],
            String, 'appThumbnail',

            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppRating, 'appRating',

            [ria.templates.ModelPropertyBind],
            chlk.models.developer.HomeAnalytics, 'appAnalytics',

            [[chlk.models.developer.DeveloperBalance]],
            function getCurrentBalanceChartData(devBalance){

                var percent = Math.floor(devBalance.getDaysToPayout() / devBalance.getDaysInMonth() * 100);

                var data = [{
                    y: 100 - percent,
                    color: {
                        radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
                        stops: [
                            [0, '#296882'],
                            [1, '#2e738d']
                        ]
                    },
                    borderColor: '#134252',
                    drilldown: {
                        name: 'MSIE versions',
                        categories: [''],
                        data: [100 - percent],
                        color: '#30708a',
                        border: 'black'
                    }
                }, {
                    y: percent,
                    color: 'transparent',
                    borderWidth: 0,
                    drilldown: {
                        name: 'MSIE versions',
                        categories: [''],
                        data: [percent],
                        color: 'transparent'
                    }
                }];

                var versionsData = [];
                for (var i = 0; i < data.length; i++) {
                    for (var j = 0; j < data[i].drilldown.data.length; j++) {
                        versionsData.push({
                            name: data[i].drilldown.categories[j],
                            y: data[i].drilldown.data[j],
                            color: data[i].color,
                            borderColor: data[i].borderColor,
                            borderWidth: data[i].borderWidth
                        });
                    }
                }
                return versionsData;
            },

            [[chlk.models.developer.DeveloperBalance]],
            function prepareBalanceChartOptions(devBalance){
                var balanceData = this.getCurrentBalanceChartData(devBalance);
                return {
                    chart: {
                        type: 'pie',
                        backgroundColor: 'transparent',
                        width: 250,
                        height: 250
                    },
                    credits: {enabled: false },
                    title: {text: ''},
                    plotOptions: {
                        pie: {shadow: false, dataLabels: {enabled: false}}
                    },
                    tooltip: {valueSuffix: '%'},
                    series: [{
                        name: '',
                        data: balanceData,
                        innerSize: '74%',
                        size: '81%'
                    }]
                };
            },

            [[chlk.models.apps.AppInstallStats]],
            function prepareAppInstallsChartData(appInstallStats){
                var categories = [],
                    data = [];

                var stats = appInstallStats.getStats() || [];

                stats.forEach(function(item){
                    categories.push(item.getSummary());
                    data.push(item.getInstallCount());
                });
                var max0 = 20;
                var maxLength = 30;
                var dataLen = data.length;
                if(dataLen > maxLength){
                    var resData = [],
                        resCategories = [];

                    for(var i = 0; i < maxLength; i++){
                        resData.push(data[parseInt(dataLen * i / maxLength, 10)]);
                        resCategories.push(categories[parseInt(dataLen * i / maxLength, 10)]);
                    }
                    resData.push(data[dataLen - 1]);
                    resCategories.push(categories[dataLen - 1]);
                    data = resData;
                    categories = resCategories;
                }

                data.forEach(function(item, i){
                    if(item > max0)
                        max0 = item;
                });
                var max = Math.ceil(max0 / 10) * 10;
                return {
                    categories : categories,
                    data : data,
                    max : max
                }
            },

            [[chlk.models.apps.AppViewStats]],
            function prepareAppViewsChartData(appViewStats){
                var categories = [],
                    data = [];

                var stats = appViewStats.getStats() || [];

                stats.forEach(function(item){
                    categories.push(item.getSummary());
                    data.push(item.getViewsCount());
                });
                var max0 = 20;
                var maxLength = 30;
                var dataLen = data.length;
                if(dataLen > maxLength){
                    var resData = [],
                        resCategories = [];

                    for(var i = 0; i < maxLength; i++){
                        resData.push(data[parseInt(dataLen * i / maxLength, 10)]);
                        resCategories.push(categories[parseInt(dataLen * i / maxLength, 10)]);
                    }
                    resData.push(data[dataLen - 1]);
                    resCategories.push(categories[dataLen - 1]);
                    data = resData;
                    categories = resCategories;
                }

                data.forEach(function(item, i){
                    if(item > max0)
                        max0 = item;
                });
                var max = Math.ceil(max0 / 10) * 10;
                return {
                    categories : categories,
                    data : data,
                    max : max
                }
            },

            [[Object]],
            function prepareAppViewsChartOptions(configs){
                return {
                    backgroundColor: 'transparent',
                    chart: {
                        type: 'area',
                        backgroundColor: 'transparent',
                        width: 350,
                        height: 181,
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
                    credits: {enabled: false},
                    title: {text: ''},
                    xAxis: {categories: configs.categories},
                    yAxis: {
                        title: {text: ''},
                        lineWidth:0,
                        showFirstLabel: false,
                        showLastLabel: false,
                        gridLineDashStyle: 'dot',
                        tickInterval: Math.floor((configs.max + 10) / 30) * 10,
                        max: configs.max + 10
                    },
                    legend:{enabled: false},
                    tooltip: {enabled: true},
                    plotOptions: {
                        area: {
                            marker: {
                                enabled: false,
                                states: {hover: {enabled: false}}
                            }
                        }
                    },
                    colors: ['#d8d8d8'],
                    series: [{
                        name: '',
                        lineWidth: 0,
                        data: configs.data
                    }]
                };
            },

            [[Object]],
            function prepareAppInstallsChartOptions(configs){
                return {
                    backgroundColor: 'transparent',
                    chart: {
                        type: 'area',
                        backgroundColor: 'transparent',
                        width: 690,
                        height: 145,
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
                    credits: {enabled: false},
                    title: {text: ''},
                    xAxis: {categories: configs.categories},
                    yAxis: {
                        title: {text: ''},
                        lineWidth:0,
                        showFirstLabel: false,
                        showLastLabel: false,
                        gridLineDashStyle: 'dot',
                        tickInterval: Math.floor((configs.max + 10) / 30) * 10,
                        max: configs.max + 10
                    },
                    legend:{enabled: false},
                    tooltip: {enabled: false},
                    plotOptions: {
                        area: {
                            marker: {
                                enabled: false,
                                states: {hover: {enabled: false}}
                            }
                        }
                    },
                    colors: ['#d8d8d8'],
                    series: [{
                        name: '',
                        lineWidth: 0,
                        data: configs.data
                    }]
                };
            }

        ])
});