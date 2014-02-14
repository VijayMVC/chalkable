REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.developer.PayPalSettingsTpl');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.DeveloperDocsPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.developer.PayPalSettingsTpl)],
        'PayPalSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.paypal-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function togglePayPalForm(node, event){
                this.dom.find('.paypal-info').toggleClass('x-hidden');
                node.toggleClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '.cancel-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            function cancelFormEditing(node, event){
                this.dom.find('.paypal-info').toggleClass('x-hidden');
                this.dom.find('.cancel-btn').toggleClass('x-hidden');
                node.toggleClass('x-hidden');
                this.dom.find('.paypal-btn').toggleClass('x-hidden');
                event.preventDefault();
            }



        ]);
});