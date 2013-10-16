REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.developer.ApiExplorerTpl');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.ApiExplorerPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.developer.ApiExplorerTpl)],
        'ApiExplorerPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.header')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function toggleDetails(node, event){
                node.find('.description').toggleClass('long');
                jQuery(node.parent().find('.details').valueOf()).slideToggle();
            },

            [ria.mvc.DomEventBind('click', '.collapse-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseAll(node, event){
                var parentNode = jQuery(node.valueOf()).parent().parent();
                parentNode.find('.details').slideUp();
                parentNode.find('.header').find('.description').removeClass('long');
            },
        ]);
});