REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.CategoriesSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.CategoriesSetupPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.setup.CategoriesSetupTpl)],
        'CategoriesSetupPage', EXTENDS(chlk.activities.lib.TemplatePage), [
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
                }.bind(this), 1);
            }
        ]);
});