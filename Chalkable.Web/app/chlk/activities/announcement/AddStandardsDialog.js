REQUIRE('chlk.activities.common.standards.BaseStandardDialog');
REQUIRE('chlk.templates.announcement.AddStandardsTpl');
REQUIRE('chlk.templates.standard.StandardsListTpl');

NAMESPACE('chlk.activities.announcement', function(){

    /**@class chlk.activities.announcement.AddStandardsDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsListTpl, '', '.standards-row', ria.mvc.PartialUpdateRuleActions.Append)],
        [ria.mvc.TemplateBind(chlk.templates.announcement.AddStandardsTpl)],

        'AddStandardsDialog', EXTENDS(chlk.activities.common.standards.BaseStandardDialog),[


            [ria.mvc.DomEventBind('change', '.search-standard')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function searchStandard(node, event, selected_){
                var name = node.getValue();
                var id = this.dom.find('[type=hidden][name=filter]').getValue();
                if(id && id.trim() != ''){
                    var formNode = this.dom.find('#get-standardt-tree-form');
                    var standardIdNode = formNode.find('[name=standardid]');
                    standardIdNode.setValue(id);
                    formNode.trigger('submit');
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.standard.StandardsListTpl, 'rebuild-standard-tree', '', ria.mvc.PartialUpdateRuleActions.Replace)],
            [[Object, Object, String]],
            VOID, function reBuildStandardTree(tpl, model, msg_) {
                this.onPrepareTemplate_(tpl, model, msg_);
                tpl.assign(model);
                var row = this.dom.find('.standards-row');
                var columns = row.find('.name-td').valueOf();
                row.empty();
                tpl.renderTo(row);
                var firstColumn = ria.dom.Dom(columns[0]);
                firstColumn.find('.standard-name').forEach(function(node){ node.removeClass('active')});
                firstColumn.prependTo(row);
                if(model.getSubjectId()){
                    var subjectId = model.getSubjectId().valueOf();
                    var subjectNode = firstColumn.find('.standard-name')
                        .filter(function(item){ return item.getData('subject-id') == subjectId; });
                    subjectNode.addClass('active');
                }
                columns = this.dom.find('.standards-row').find('.name-td').valueOf();
                var lastColumn = ria.dom.Dom(columns[columns.length - 1]);
                this.afterCellActivate_(lastColumn.find('.standard-name.active'));
            }
        ]);
});