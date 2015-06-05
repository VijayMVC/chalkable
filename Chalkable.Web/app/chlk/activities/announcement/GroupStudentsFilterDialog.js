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
            },

            [ria.mvc.DomEventBind('click', '.clear-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function clearAllClick(node, event){
                this.dom.find('input[type=checkbox]').trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [false]);
            },

            [ria.mvc.DomEventBind('change', 'input[type=checkbox]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function checkChange(node, event){
                node.siblings('.items-container').find('input[type=checkbox]').trigger(chlk.controls.CheckBoxEvents.CHANGE_VALUE.valueOf(), [node.checked()]);
            },

            [ria.mvc.DomEventBind('click', '.filter-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterClick(node, event){
                this.dom.find('.filter-students-form').trigger('submit');
            }
        ]);
});