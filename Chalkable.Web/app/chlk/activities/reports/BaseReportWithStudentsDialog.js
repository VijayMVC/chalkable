REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.reports.ProgressReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.BaseReportWithStudentsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.ProgressReportTpl)],
        'BaseReportWithStudentsDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('submit', '.report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function reportFormSubmit(node, event){
                var studentIdsNode = node.find('#student-ids-value'),
                    valuesArray = [];
                node.find('.student-chk').forEach(function(item){
                    if(item.is(':checked'))
                        valuesArray.push(item.getData('id'));
                });

                studentIdsNode.setValue(valuesArray.join(','));
            },

            [ria.mvc.DomEventBind('change', '#select-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allStudentsChange(node, event){
                var value = node.checked(), jNode;
                jQuery(node.valueOf()).parents('td')
                    .find('.students-block')
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