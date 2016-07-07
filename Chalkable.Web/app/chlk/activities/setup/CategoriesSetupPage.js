REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.CategoriesSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.CategoriesSetupPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.CategoriesSetupTpl)],
        'CategoriesSetupPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.PartialUpdateRule(null, 'copy', '')],
            [[Object, Object, String]],
            VOID, function copyUpdate(tpl, model, msg_){
                var text = 'Selected categories were copied to ' + model.getToClassName() + ', year ' + model.getCopyToYearName();
                var cnt = this.dom.find('.green-info-msg');
                cnt.setHTML(text);
                cnt.removeClass('x-hidden');
                this.dom.find('.cancel-copy').trigger('click');
                setTimeout(function(){
                    cnt.addClass('x-hidden');
                }, 5000)
            },

            [ria.mvc.DomEventBind('click', '.setup-copy')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function copyClick(node, event){
                this.dom.find('.setup-page').toggleClass('copy-mode');
                node.toggleClass('active');
                this.dom.find('.green-info-msg').addClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '.cancel-copy')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelCopyClick(node, event){
                this.dom.find('.setup-page').removeClass('copy-mode');
                node.removeClass('active');
            },

            [ria.mvc.DomEventBind('change', '.copy-to-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function classSelect(node, event, selected_){
                this.updateCopySubmitBtn_();
            },

            function updateCopySubmitBtn_(){
                var btn = this.dom.find('.copy-submit');
                if(this.dom.find('.item-check:checked').count() > 0 && this.dom.find('[name="toClassId"]').getValue()){
                    btn.removeAttr('disabled');
                    btn.setProp('disabled', false);
                }else{
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                }
            },

            [ria.mvc.DomEventBind('change', '.all-checkboxes')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allReasonsChange(node, event){
                var value = node.checked(), jNode;
                jQuery(node.valueOf()).parents('form')
                    .find('.chlk-grid-container')
                    .find('.row:not(.header)')
                    .find('[type=checkbox]:not(:disabled)')
                    .each(function(index, item){
                        jNode = jQuery(this);
                        if(!!item.getAttribute('checked') != !!value){
                            jNode.prop('checked', value);
                            value ? this.setAttribute('checked', 'checked') : this.removeAttribute('checked');
                            value && this.setAttribute('checked', 'checked');
                            var node = jNode.parent().find('.hidden-checkbox');
                            node.val(value);
                            node.data('value', value);
                            node.attr('data-value', value);
                        }
                    });
            },

            [ria.mvc.DomEventBind('change', '[type="checkbox"]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkboxChange(node, event){
                setTimeout(function(){
                    var ids = [];
                    this.dom.find('.item-check').forEach(function(item){
                        if(item.checked())
                            ids.push(item.parent('.check-container').getData('id'));
                    });
                    this.dom.find('.delete-button').setAttr('disabled', ids.length ? false : 'disabled');
                    this.dom.find('.ids-to-delete').setValue(ids.join(','));
                    this.updateCopySubmitBtn_();
                }.bind(this), 1);
            }
        ]);
});