REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.WorksheetReportTpl');
REQUIRE('chlk.templates.grading.WorksheetReportGridTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.WorksheetReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.grading.WorksheetReportTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.WorksheetReportGridTpl, 'grid', '.grid-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        'WorksheetReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('change', '.report-date-picker')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function reportDatePickerChange(node, event){
                node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('change', '.all-checkboxes')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allAnnouncementsChange(node, event){
                var value = node.checked(), jNode;
                jQuery(node.valueOf()).parents('form')
                    .find('.grid-container')
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
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.WorksheetReportGridTpl, 'stop')],
            function stopWorking(tpl, model, msg){
                this.dom.find('.report-form').removeClass('working');
            },

            [ria.mvc.DomEventBind('submit', 'form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var res = [];
                node.find('.grid-container')
                    .find('.row:not(.header)')
                    .find('input[type=checkbox]').forEach(function(item){
                        if(item.checked()){
                            res.push(item.parent('.cell').getData('id'));
                        }
                });
                this.dom.find('.blank-columns').find('.checkbox').forEach(function(item){
                    if(!item.checked())
                        item.parent().find('.label-input').setValue('');
                });
                node.find('[name=announcementIds]').setValue(res.join(','));
            }
        ]);
});