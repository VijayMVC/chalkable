REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {
    var charts = {};

    /** @class chlk.controls.ChartControl */
    CLASS(
        'ChartControl', EXTENDS(ria.mvc.DomControl), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/chart.jade')(this);
            },

            [[Object]],
            Object, function processAttrs(attrs) {
                attrs.id = attrs.id || ria.dom.NewGID();
                var options = attrs['data-options'];
                this.queueReanimation_(attrs.id, options);
                delete attrs['data-options'];
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
            VOID, function reanimate_(node, id, options, activity, model) {
                options.chart.renderTo = node.valueOf()[0];
                charts[id] = new Highcharts.Chart(options);
                node.setData('chart', charts[id]);
            }
        ]);
});