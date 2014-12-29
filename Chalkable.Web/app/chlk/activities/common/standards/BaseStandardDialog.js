REQUIRE('chlk.activities.lib.TemplateDialog');

NAMESPACE('chlk.activities.common.standards', function(){


    ASSET('~/assets/jade/common/standards/BaseStandardDialog.jade')();
    /**@class chlk.activities.common.standards.BaseStandardDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],

        'BaseStandardDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [[ria.dom.Dom, Array]],
            function removeColumnId_(column, standardIds){
                var columnId = column.getData('id') || '';
                var standardIdIndex = columnId ? standardIds.indexOf(columnId.toString()) : - 1;
                if (standardIdIndex > -1) {
                    standardIds.splice(standardIdIndex, 1);
                }
                return standardIds;
            },

            [ria.mvc.DomEventBind('click', '.column-cell')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function cellClick(node, event){
                if(node.hasClass('active'))
                    return false;

                var standardIdsNode = this.dom.find('.standard-ids');
                var value = standardIdsNode.getValue();
                var standardIds = value ? value.split(',') : [];

                var prevColumn = node.parent('.column').find('.column-cell.active');
                prevColumn.removeClass('active');

                standardIds = this.removeColumnId_(prevColumn, standardIds);

                node.addClass('active');
                var id = node.getData('id') || '';
                var idIndex = id ? standardIds.indexOf(id.toString()) : - 1;
                var btnAddContainer = this.dom.find('.add-standard-btn');
                var btn = btnAddContainer.find('button');

                var childNodes = node.parent('td').find('~ td');
                var childColumns = childNodes
                    .find('.column-cell')
                    .forEach(function(column){
                        standardIds = this.removeColumnId_(column, standardIds);
                    }.bind(this));

                childNodes.remove();

                if(idIndex == -1 && id != ''){
                    this.dom.find('input[name=standardid]').setValue(id);
                    standardIds.push(id);
                }

                if (standardIds.length == 0){
                    btnAddContainer.addClass('disabled');
                    btnAddContainer.setAttr('disabled', true);
                    btn.setAttr('disabled', true);
                }else{
                    btnAddContainer.removeClass('disabled');
                    btnAddContainer.removeAttr('disabled');
                    btn.removeAttr('disabled');
                }

                standardIdsNode.setValue(standardIds.join(','));
                return true;
            },

            [ria.mvc.DomEventBind('click', '.add-standard-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                setTimeout(function(){
                    node.setAttr('disabled', true);
                }, 1);
            }
        ]);
});