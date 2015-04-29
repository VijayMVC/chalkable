REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.CategoriesSetupTpl');

NAMESPACE('chlk.activities.setup', function () {

    var interval;

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
                    .find('[type=checkbox]')
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
            }
        ]);
});