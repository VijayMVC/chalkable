REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.setup.CategoriesImportTpl');
REQUIRE('chlk.templates.setup.CategoriesImportItemsTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.CategoriesImportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.setup.CategoriesImportTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.setup.CategoriesImportItemsTpl, 'list-update', '.items-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'CategoriesImportDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [
            [ria.mvc.DomEventBind('change', '.item-check')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function itemSelect(node, event, selected_){
                this.updateSubmitBtn_();
            },

            [ria.mvc.DomEventBind('change', '.copy-to-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function classSelect(node, event, selected_){
                this.dom.find('.list-update-btn').trigger('click');
            },

            function updateSubmitBtn_(){
                var btn = this.dom.find('.import-btn');
                if(this.dom.find('.item-check:checked').count() > 0){
                    btn.removeAttr('disabled');
                    btn.setProp('disabled', false);
                }else{
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                }
            },

            [ria.mvc.DomEventBind('submit', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var announcements = [];
                this.dom.find('.item-check:checked').forEach(function(node){
                    announcements.push(node.getData('id'));
                });

                var value = announcements.join(',');
                this.dom.find('.items-to-copy').setValue(value);
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_){
                BASE(model, msg_);
                this.updateSubmitBtn_();
            }
        ]);
});