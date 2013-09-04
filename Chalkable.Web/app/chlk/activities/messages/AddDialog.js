REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.messages.AddDialog');

NAMESPACE('chlk.activities.messages', function () {

    /** @class chlk.activities.messages.AddDialog */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('MessagePopup')],
        [ria.mvc.TemplateBind(chlk.templates.messages.AddDialog)],
        'AddDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
            [ria.mvc.DomEventBind('click', '#submitbtn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function validate(node, event){
                /*var val = this.dom.find('#recipientId-hidden').getValue();
                if (val)
                    return true;
                this.dom.find('#recipient-required-err').removeClass("x-hidden");
                return false;*/

            }
        ]);
});
