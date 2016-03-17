REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.GroupStudentsFilterTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.GroupStudentsFilterDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.GroupStudentsFilterTpl)],
        'GroupStudentsFilterDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('submit', '.filter-students-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var ids = [];
                this.dom.find('.class-check').forEach(function(node){
                    if(node.checked())
                        ids.push(node.getData('id'));
                });
                this.dom.find('.class-ids').setValue(ids.join(','));
                if(!ids.length)
                    return false;
            },

            [ria.mvc.DomEventBind('click', '.clear-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function clearAllClick(node, event){
                this.setCheckBoxValue(this.dom.find('input[type=checkbox]').valueOf(), false);
                this.dom.find('.filter-button').setProp('disabled', true);
            },

            function setCheckBoxValue(node, value){
                if(Array.isArray(node)){
                    var that = this;
                    node.forEach(function(item){that.setCheckBoxValue(item, value)});
                }else{
                    var jNode = jQuery(node);
                    jNode.prop('checked', value);
                    if(value)
                        node.setAttribute('checked', 'checked');
                    else
                        node.removeAttribute('checked');
                    value && node.setAttribute('checked', 'checked');
                    var hidden = jNode.parent().find('.hidden-checkbox');
                    hidden.val(value);
                    hidden.data('value', value);
                    hidden.attr('data-value', value);
                }


            },

            [ria.mvc.DomEventBind('change', 'input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkChange(node, event){
                //node.siblings('.items-container').find('input[type=checkbox]').trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [node.checked()]);

                var value = node.checked(), that = this;
                jQuery(node.valueOf()).parents('.column-cell').siblings('.items-container').find('input[type=checkbox]')
                    .each(function(index, item){
                        if(jQuery(item).is(':checked') != !!value){
                            that.setCheckBoxValue(this, value);
                        }
                    });

                var parentCheck = jQuery(node.valueOf()).parents('.items-container:eq(0)').parents('.main-item:eq(0)').find('input[type=checkbox]:eq(0)');
                while(parentCheck[0]){
                    var items = parentCheck.parents('.column-cell:eq(0)').siblings('.items-container').find('>DIV.main-item').find('input[type=checkbox]:eq(0)'),
                        checked = 0, partially = 0;

                    items.each(function(index, item){
                        var jNode = jQuery(item);
                        if(jNode.is(':checked')){
                            if(jNode.hasClass('partially-checked'))
                                partially++;
                            else
                                checked++;
                        }
                    });

                    if(!checked && !partially){
                        this.setCheckBoxValue(parentCheck[0], false);
                        parentCheck.removeClass('partially-checked');
                    }else{
                        if(partially || checked != items.length){
                            this.setCheckBoxValue(parentCheck[0], true);
                            parentCheck.addClass('partially-checked');
                        }else{
                            this.setCheckBoxValue(parentCheck[0], true);
                            parentCheck.removeClass('partially-checked');
                        }
                    }


                    parentCheck = parentCheck.parents('.items-container:eq(0)').parents('.main-item:eq(0)').find('input[type=checkbox]:eq(0)');
                }

                this.dom.find('.filter-button').setProp('disabled', this.dom.find('input[type=checkbox]:checked').count() == 0);
            },

            [ria.mvc.DomEventBind('click', '.filter-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterClick(node, event){
                this.dom.find('.filter-students-form').trigger('submit');
            }
        ]);
});