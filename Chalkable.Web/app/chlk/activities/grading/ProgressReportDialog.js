REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.grading.ProgressReportTpl');

NAMESPACE('chlk.activities.grading', function(){

    /**@class chlk.activities.grading.ProgressReportDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.ActivityGroup('ReportDialog')],
        [ria.mvc.TemplateBind(chlk.templates.grading.ProgressReportTpl)],
        'ProgressReportDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[
            [ria.mvc.DomEventBind('change', '.category-average')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function categoryAverageChange(node, event){
                node.parent('.item-3').find('.small-input').setAttr('disabled', !node.checked());
            },

            [ria.mvc.DomEventBind('change', '#select-all')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allAnnouncementsChange(node, event){
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
            },

            [ria.mvc.DomEventBind('change', '.reasons-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function reasonsSelectChange(node, event, options_){
                console.info(node.getValue(), options_);
            },

            [ria.mvc.DomEventBind('submit', '.progress-report-form')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function formSubmit(node, event){
                var studentIdsNode = node.find('#student-ids-value'),
                    commentsNode = node.find('#coments-list'),
                    notSelectedNode = node.find('#not-selected-count'),
                    valuesArray = [], commentsArray = [], comment, notSelectedCount = 0;
                node.find('.student-chk').forEach(function(item){
                    comment = item.parent('.student-item').find('.student-comment').getValue();
                    if(item.is(':checked')){
                        valuesArray.push(item.getData('id'));
                        commentsArray.push(comment);
                    }else{
                        if(comment)
                            notSelectedCount++;
                    }
                });

                notSelectedNode.setValue(notSelectedCount);

                if(valuesArray.length){
                    studentIdsNode.setValue(valuesArray.join(','));
                    commentsNode.setValue(commentsArray.join(','));
                }

                var yearToDate = node.find('#year-to-date-chk').checked();
                var gradingPeriod = node.find('#grading-period-chk').checked();
                var dailyAttendanceDisplayMethodNode = node.find('#daily-attendance-display-method');
                var reasonsNode = node.find('#absence-reasons'),
                    reasonsArray = node.find('.reasons-select').getValue();
                if(reasonsArray && reasonsArray.length)
                    reasonsNode.setValue(reasonsArray.join(','));
                if(yearToDate){
                    dailyAttendanceDisplayMethodNode.setValue(3);
                    if(gradingPeriod){
                        dailyAttendanceDisplayMethodNode.setValue(1);
                    }
                }else{
                    if(gradingPeriod){
                        dailyAttendanceDisplayMethodNode.setValue(2);
                    }
                }
            }
        ]);
});