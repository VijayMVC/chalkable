REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.TeacherSettings');

NAMESPACE('chlk.activities.settings', function () {

    /** @class chlk.activities.settings.TeacherPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.TeacherSettings)],
        'TeacherPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '#changePasswordLink')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function resetPwdClick(node, event){
                var link = this.dom.find('#changePasswordLink');
                var form = this.dom.find('#changePasswordForm');
                link.addClass('x-hidden');
                form.removeClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '#cancell-edit-pwd-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelResetPwdClick(node, event){
                var link = this.dom.find('#changePasswordLink');
                var form = this.dom.find('#changePasswordForm');
                link.removeClass('x-hidden');
                form.addClass('x-hidden');
            }


        ]);
});