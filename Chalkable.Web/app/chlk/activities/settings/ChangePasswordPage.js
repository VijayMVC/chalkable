REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.ChangePasswordTpl');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.ChangePasswordPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.ChangePasswordTpl)],
        'ChangePasswordPage', EXTENDS(chlk.activities.lib.TemplatePage), [


            [ria.mvc.DomEventBind('click', '#changePasswordLink')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                node.addClass('x-hidden');
                node.parent().parent().find('#changePasswordForm').removeClass('x-hidden');
            }
        ]);
});