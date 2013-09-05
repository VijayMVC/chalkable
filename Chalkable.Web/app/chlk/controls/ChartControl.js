REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

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

            VOID, function queueReanimation_(id, options) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.reanimate_(ria.dom.Dom('#' + id), options, activity, model)
                    }.bind(this));
            },

            [[ria.dom.Dom, Object, ria.mvc.IActivity, Object]],
            VOID, function reanimate_(node, options, activity, model) {
                options.chart.renderTo = node.valueOf()[0];
                new Highcharts.Chart(options);
            }
        ]);
});