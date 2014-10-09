REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {
    /** @class chlk.controls.ChartEvents */
    ENUM('ChartEvents', {
        CHART_UPDATE: 'chartupdate'
    });

    var charts = {};

    var defaultConfigs = {
        backgroundColor: 'transparent',
        chart: {
            backgroundColor: 'transparent',
            width: 630,
            height: 178,
            style: {
                fontFamily: 'Arial',
                fontSize: '10px',
                color: '#a6a6a6'
            },
            plotBorderWidth: 0

        },
        labels: {
            style: {
                color: '#a6a6a6',
                textOverflow: 'ellipsis',
                fontSize: '9px',
                width: '1000px'
            }
        },
        credits: {
            enabled: false
        },
        title: {
            text: ''
        },
        xAxis: {
            lineWidth:0
        },
        yAxis: {
            title: {
                text: ''
            },
            showFirstLabel: false,
            gridLineDashStyle: 'dot',
            labels: {
                formatter: function(){
                    return '<div class="chart-label-y">' + this.value + '</div>';
                },
                useHTML: true
            }
        },
        legend:{
             enabled: false
        },
        tooltip: {
            headerFormat: '',
            pointFormat: '<b class="chart-text">{point.y}</b>',
            borderWidth: 0,
            borderRadius: 2,
            useHTML: true,
            positioner: function (labelWidth, labelHeight, point) {
                return { x: point.plotX + 18, y: point.plotY - 37 };
            },
            style: {
                display: 'none'
            }
        },
        plotOptions:{
            area: {
                marker: {
                    enabled: true,
                    symbol: 'circle',
                    radius: 3,
                    fillColor: '#ffffff',
                    lineWidth: 2,
                    lineColor: '#a6a6a6',
                    states: {
                        hover: {
                            radius: 6,
                            lineWidth: 2,
                            enabled: true,
                            lineColor: '#2F7790'
                        }
                    }
                }
            }
        },
        colors: ['#d8d8d8'],

        series: [{
            name: '',
            data: []
        }]
    };

    /** @class chlk.controls.ChartControl */
    CLASS(
        'ChartControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/chart.jade')(this);
            },

            [ria.mvc.DomEventBind(chlk.controls.ChartEvents.CHART_UPDATE.valueOf(), '.chart-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function chartUpdate(node, event) {
                var options = node.getData('options');
                var id = node.getAttr('id');
                this.reanimate_(node, id, options);
            },

            [[Object]],
            Object, function processAttrs(attrs) {
                attrs.id = attrs.id || ria.dom.Dom.GID();
                var options = attrs['data-options'];
                if(!options.chart.width && attrs.scroll){
                    var len = options.series[0].data.length;
                    var width = len * 80;
                    if(width > 630){
                        options.chart.width = width;
                        attrs.needsScroll = true;
                    }
                }
                var res = {};
                if(!attrs.noExtends){
                    if(!options.xAxis || !options.xAxis.dateTimeLabelFormats)
                        defaultConfigs.xAxis.labels= {
                            formatter: function(){
                                return '<div class="chart-label">' + this.value + '</div>';
                            },
                            useHTML: true
                        }
                    jQuery.extend(true, res, defaultConfigs,options);
                    var interval = res.yAxis.tickInterval;
                    if(interval && !res.yAxis.max){
                        var max = 0;
                        res.series.forEach(function(s){
                            s.data.forEach(function(item){
                                if(item > max)
                                    max = item;
                            });
                        });
                        max = Math.ceil(max/interval) * interval;
                        res.yAxis.max = max;
                    }
                    if(res.legend.enabled && !options.chart.height)
                        res.chart.height = 215;
                }else{
                    res = options;
                }
                res.plotOptions.series = {
                    events: {
                        mouseOver: function(event) {
                            new ria.dom.Dom('#' + attrs.id).trigger('seriemouseover', [this, event]);
                        },
                        mouseOut: function(event) {
                            new ria.dom.Dom('#' + attrs.id).trigger('seriemouseleave', [this, event]);
                        }
                    }
                };
                this.queueReanimation_(attrs.id, res);
                //delete attrs['data-options'];
                return attrs;
            },

            [ria.mvc.DomEventBind('mouseover', '.chart-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            function chartHoverStart(node, event) {
                this.updateMarkers(node, {
                    enabled: true,
                    lineWidth:2,
                    fillColor: '#FFFFFF',
                    radius: 3,
                    lineColor: '#d8d8d8'
                });
            },

            [[ria.dom.Dom, Object]],
            function updateMarkers(node, params){
                if(node.getData('showmarkeronhover')){
                    var chart = charts[node.getAttr('id')];
                    if(chart.series){
                        chart = chart.series[0];
                        chart.options.marker = jQuery.extend(chart.options.marker, params);
                        chart.redraw();
                    }
                }
            },

            [ria.mvc.DomEventBind('mouseleave', '.chart-container')],
            [[ria.dom.Dom, ria.dom.Event]],
            function chartHoverEnd(node, event) {
                this.updateMarkers(node, {
                    enabled: true,
                    lineWidth:0,
                    fillColor: 'transparent',
                    radius: 0,
                    lineColor: 'transparent'
                });
            },

            VOID, function queueReanimation_(id, options) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.reanimate_(ria.dom.Dom('#' + id), id, options, activity, model)
                    }.bind(this));
            },

            [[ria.dom.Dom, String, Object, ria.mvc.IActivity, Object]],
            VOID, function reanimate_(node, id, options, activity_, model_) {
                var container = node.find('.third-container');
                if(container.exists())
                    node = container;
                options.chart.renderTo = node.valueOf()[0];
                charts[id] = new Highcharts.Chart(options);
                node.setData('chart', charts[id]);
                setTimeout(function(node){
                    node.addClass('processed');
                }.bind(this, node), 1);
            }
        ]);
});