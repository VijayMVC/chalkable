REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.discipline.ClassDisciplinesTpl');

NAMESPACE('chlk.activities.discipline', function(){
   "use strict";
    /**@class chlk.activities.discipline.ClassDisciplinesPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.ClassDisciplinesTpl)],
        'ClassDisciplinesPage', EXTENDS(chlk.activities.lib.TemplatePage),[

            function $(){
                BASE();
                this._slideTimeout = null;
                this._submitTimeout = null;
                this._TRIANGLE_CLASS = 'triangle';
                this._DOWN_CLASS = 'down';
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.SELECT_ROW.valueOf(), '.disciplines-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function selectStudent(node, event, row, index){
                clearTimeout(this._slideTimeout);
                this._slideTimeout = setTimeout(function(){
                    node.find('.discipline-form-block:eq(' + index + ')').slideDown(500);
                    row.find('.' + this._TRIANGLE_CLASS).addClass(this._DOWN_CLASS);
                }.bind(this), 500);
            },

            [ria.mvc.DomEventBind(chlk.controls.GridEvents.DESELECT_ROW.valueOf(), '.disciplines-individual')],
            [[ria.dom.Dom, ria.dom.Event, ria.dom.Dom, Number]],
            function deSelectStudent(node, event, row, index){
                node.find('.discipline-form-block:eq(' + index + ')').slideUp(250);
                row.find('.' + this._TRIANGLE_CLASS).removeClass(this._DOWN_CLASS);
            },

            [ria.mvc.DomEventBind('change keyup', '.change-discipline')],
            [[ria.dom.Dom, ria.dom.Event]],
            function updateDisciplineItem(node, event){
                clearTimeout(this._submitTimeout);
                this._submitTimeout = setTimeout(function(){
                    this.updateDiscipline_(node);
                }.bind(this), 1000);
            },

            [[ria.dom.Dom]],
            Array, function getDisciplines_(rowNode){
                var res = [];
                var disciplinesNodes = rowNode.find('[name="discipline"]').valueOf();
                var len = disciplinesNodes.length, i, node;
                for(i=0;i<len;i++){
                    node = new ria.dom.Dom(disciplinesNodes[i]);
                    if(node.find(':disabled').valueOf().length == 0) {
                        var descNode = node.find('[name="description"]');
                        var discTypesNode = node.find('[name="disciplineTypes"]');
                        res.push({
                            classPeriodId: node.getData('class-period-id'),
                            classPersonId: node.getData('class-person-id'),
                            date: node.getData('date'),
                            description: descNode.getValue(),
                            disciplineTypeIds: discTypesNode.getValue()
                        });
                    }
                }
                return res;
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function updateDiscipline_(node){
                var form = node.parent('form');
                var disciplinesNode = form.find('input[name="disciplinesJson"]');
                disciplinesNode.setValue(JSON.stringify(this.getDisciplines_(form)));
                form.trigger('submit');
//                setTimeout(function(){
//                    var next = row.next();
//                    if(next.exists()){
//                        row.removeClass('selected');
//                        next.addClass('selected');
//                    }
//                },1);
            }



    ]);
});