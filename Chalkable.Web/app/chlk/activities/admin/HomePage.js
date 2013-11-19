REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.admin.AdminHomeTpl');
REQUIRE('chlk.templates.grading.GradeLevelForTopBar');

NAMESPACE('chlk.activities.admin', function () {

    /** @class chlk.activities.admin.HomePage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.admin.AdminHomeTpl)],
        'HomePage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('mouseover mouseleave', '.chart-bg')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function chartHoverStart(node, event) {
                node.parent('.chart-block').find('.chart-container').trigger(event.type);
                return false;
            }
        ]);
});