REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.settings.AdminMessagingTpl');

NAMESPACE('chlk.activities.settings', function () {

    var timer;

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

                timer = setTimeout(function(){
                    node.parent('form').trigger('submit');
                }, 2000);
            },

            [ria.mvc.DomEventBind('change', '.all-students, .teachers-to-students')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function mainCheckChange(node, event, selected_){
                var node2 = node.hasClass('all-students') ? this.dom.find('[name=studenttoclassmessagingonly]') : this.dom.find('[name=teachertoclassmessagingonly]');
                var checkbox2 = node.hasClass('all-students') ? this.dom.find('[type=checkbox][name=studenttoclassmessagingonly]') : this.dom.find('[type=checkbox][name=teachertoclassmessagingonly]');
                if(node.checked()){
                    node2.removeAttr('readonly');
                    node2.parent('.slide-checkbox').removeAttr('readonly');
                }
                else{
                    node2.setAttr('readonly', 'readonly');
                    node2.parent('.slide-checkbox').setAttr('readonly', 'readonly');
                    checkbox2.setValue(false);
                }
            }
        ]);
});