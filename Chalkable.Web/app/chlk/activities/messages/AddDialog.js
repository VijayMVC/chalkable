REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.messages.AddDialog');

NAMESPACE('chlk.activities.messages', function () {

    /** @class chlk.activities.messages.AddDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('MessagePopup')],
        [ria.mvc.TemplateBind(chlk.templates.messages.AddDialog)],
        'AddDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
            [ria.mvc.DomEventBind('submit', '#add-message-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function addFormSubmit(node, event) {
                var p = true;
                node.find('[class^=validate]').forEach(function(item){
                    p = p && !!item.getValue();
                });
                return p;
            }
        ]);
});
