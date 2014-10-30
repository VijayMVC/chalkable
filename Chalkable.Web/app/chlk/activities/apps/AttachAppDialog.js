REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AttachAppDialog');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AttachAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AttachAppDialog)],
        'AttachAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('click', '.app-icon-link')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function appIconClick(node, event){
                var target = new ria.dom.Dom(event.target);
                if(!target.is('.action-link')){
                    var link = node.find('.app-link');
                    if(link.exists()){
                        link.trigger('click');
                    }
                }
            }
        ]);
});