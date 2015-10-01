REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AttachDialogTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AttachDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AttachDialogTpl)],
        'AttachDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('click', '.app-icon-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function appIconClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.is('.action-link')) {
                    var link = node.find('.app-link');
                    if(link.exists()){
                        link.trigger('click');
                    }
                }
            }
        ]);
});