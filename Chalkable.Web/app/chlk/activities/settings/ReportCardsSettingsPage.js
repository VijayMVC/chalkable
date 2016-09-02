REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.ReportCardsSettingsTpl');

NAMESPACE('chlk.activities.settings', function(){

    /**@class chlk.activities.settings.ReportCardsSettingsPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.ReportCardsSettingsTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.settings.ReportCardsSettingsTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ReportCardsSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage),[


            [ria.mvc.DomEventBind('click', '.panorama-filter-form .add-school-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function addSchoolClick(node, event) {
                //var form = node.parent('form');
                //var tpl = new chlk.templates.controls.PanoramaFilterBlockTpl();
                //var model = new chlk.models.controls.PanoramaFilterBlockViewData(this.getStandardizedTests());
                //tpl.assign(model);
                //var dom = new ria.dom.Dom().fromHTML(tpl.render());
                //var target = form.find('.filters-container');
                //dom.appendTo(target);
                //this.context.getDefaultView().notifyControlRefreshed();
            },
        ]);
});