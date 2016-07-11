REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.AdminMessagingTpl');

NAMESPACE('chlk.activities.settings', function () {

    var timer, needToSubmit;

    /** @class chlk.activities.settings.AdminPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.settings.AdminMessagingTpl)],
        'AdminPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('change', '[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function checkboxSelect(node, event, selected_){
                clearTimeout(timer);

                needToSubmit = true;

                timer = setTimeout(function(){
                    node.parent('form').trigger('submit');
                    needToSubmit = false;
                }, 2000);
            },

            OVERRIDE, VOID, function onStop_() {
                if(needToSubmit)
                    this.dom.find('form').trigger('submit');
                needToSubmit = false;
                BASE();
            },

            [ria.mvc.DomEventBind('change', '.all-students, .teachers-to-students')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function mainCheckChange(node, event, selected_){
                var node2 = node.hasClass('all-students') ? this.dom.find('[name=studenttoclassmessagingonly]') : this.dom.find('[name=teachertoclassmessagingonly]');
                var checkbox2 = node.hasClass('all-students') ? this.dom.find('[type=checkbox][name=studenttoclassmessagingonly]') : this.dom.find('[type=checkbox][name=teachertoclassmessagingonly]');
                if(node.checked()){
                    node2.trigger(chlk.controls.CheckBoxEvents.DISABLED_STATE.valueOf(), [false]);
                }
                else{
                    node2.trigger(chlk.controls.CheckBoxEvents.DISABLED_STATE.valueOf(), [true]);
                    checkbox2.setValue(false);
                }
            }
        ]);
});