REQUIRE('chlk.activities.reports.BaseReportWithStudentsDialog');
REQUIRE('chlk.templates.reports.AttendanceProfileReportTpl');

NAMESPACE('chlk.activities.reports', function(){

    /**@class chlk.activities.reports.AttendanceProfileReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.reports.AttendanceProfileReportTpl)],
        'AttendanceProfileReportDialog', EXTENDS(chlk.activities.reports.BaseReportWithStudentsDialog),[

            [ria.mvc.DomEventBind('submit', '.attendance-profile-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var tNode = node.find('#terms-ids'),
                    tArray = node.find('#terms-select').getValue() || [];
                tNode.setValue(tArray.join(','));

                var res = [];
                node.find('.chlk-grid-container')
                    .find('.row:not(.header)')
                    .find('input[type=checkbox]').forEach(function(item){
                        if(item.checked()){
                            res.push(item.parent('.cell').getData('id'));
                        }
                });
                node.find('[name=absenceReasons]').setValue(res.join(','));
            },

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