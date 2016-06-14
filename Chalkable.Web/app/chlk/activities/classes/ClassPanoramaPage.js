REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaTpl');
REQUIRE('chlk.templates.classes.ClassProfilePanoramaChartsTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassPanoramaPage */

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfilePanoramaTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfilePanoramaChartsTpl, '', '.charts-part', ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassPanoramaPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.PartialUpdateRule(null, 'save-filters')],
            VOID, function afterFiltersSave(tpl, model, msg_) {

            },

            [ria.mvc.DomEventBind(chlk.controls.TabEvents.TAB_CHANGED.valueOf(), '.tabs-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function lessStandardsClick(node, event){
                this.updateChartsSize_();
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                this.updateChartsSize_();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                this.updateChartsSize_();
            },

            function updateChartsSize_(){
                jQuery(window).trigger('resize');
            }
        ]);
});