REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.discipline.DisciplineDayPopupTpl');

NAMESPACE('chlk.activities.discipline', function(){

    /** @class chlk.activities.discipline.DisciplineDayPopup */

    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(true)],
        [ria.mvc.ActivityGroup('DayDisciplinePopup')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.DisciplineDayPopupTpl)],

        'DisciplineDayPopup', EXTENDS(chlk.activities.lib.TemplatePopup),[

            function $(){
                BASE();
                this._HIDDEN_CLASS = 'x-hidden';
            },

            Array, function getDisciplines_(){
                var res = [];
                var disciplinesNodes = this.dom.find('[name="discipline"]').valueOf();
                var len = disciplinesNodes.length, i, node;
                for(i=0;i<len;i++){
                    node = new ria.dom.Dom(disciplinesNodes[i]);
                    if(node.find(':disabled').valueOf().length == 0) {
                        var descNode = node.find('[name="description"]');
                        var discTypesNodes = node.find('.list-item');
                        discTypesNodes = discTypesNodes.filter(function(_){return !_.hasClass(this._HIDDEN_CLASS);}.bind(this));
                        var discTypesIds = discTypesNodes.valueOf().map(function(_){
                           return (new ria.dom.Dom(_)).getData('disciplinetypeid');
                        });

                        res.push({
                            classPeriodId: node.getData('class-period-id'),
                            classPersonId: node.getData('class-person-id'),
                            date: node.getData('date'),
                            description: descNode.getValue(),
                            disciplineTypeIds: discTypesIds.join(',')
                        });
                    }
                }
                return res;
            },

            [ria.mvc.DomEventBind('change', '.discipline-type-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function changeDisciplineType(node, event, options){
                var selectedNode = node.find('[selected="selected"]');
                var value = selectedNode.getValue();
                var items = node.parent('.row').find('.disciplines-container .editable-list .list-item').valueOf();
                for(var i = 0; i < items.length; i++){
                    var itemNode = (new ria.dom.Dom(items[i]));
                    if(itemNode.getData('disciplinetypeid') == value
                        && itemNode.hasClass(this._HIDDEN_CLASS)){
                       itemNode.removeClass(this._HIDDEN_CLASS);
                    }
                }
            },

            [ria.mvc.DomEventBind('click', '.list-item .del')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function removeDisciplineType(node, event){
                node.parent().addClass(this._HIDDEN_CLASS);
            },

            [ria.mvc.DomEventBind('click', '.save-btn')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function saveClick(node, event){
                var disciplinesNode = this.dom.find('input[name="disciplinesJson"]');
                disciplinesNode.setValue(JSON.stringify(this.getDisciplines_()));
                this.dom.find('form').trigger('submit');
                this.close();
            }
        ]);
});