REQUIRE('chlk.activities.person.PersonGrid');
REQUIRE('chlk.templates.discipline.ClassDisciplinesTpl');
REQUIRE('chlk.templates.discipline.DisciplineTpl');

NAMESPACE('chlk.activities.discipline', function(){
   "use strict";
    /**@class chlk.activities.discipline.ClassDisciplinesPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.ClassDisciplinesTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.discipline.ClassDisciplinesTpl, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassDisciplinesPage', EXTENDS(chlk.activities.person.PersonGrid),[

            function $(){
                BASE();
                this._slideTimeout = null;
                this._submitTimeout = null;
                this._TRIANGLE_CLASS = 'triangle';
                this._DOWN_CLASS = 'down';
                this._isAblePostDiscipline = true;
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

            [[Object]],
            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this._isAblePostDiscipline = model.isAblePostDiscipline();
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                this._isAblePostDiscipline = model.isAblePostDiscipline();
                new ria.dom.Dom('.change-discipline').on('scroll', function(node, event){
                    node.parent().setCss('background-position', '0 ' +  (-jQuery(node.valueOf()).scrollTop()) + 'px')
                });
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
                            id: node.getData('id'),
                            studentId: node.getData('student-id'),
                            classId: node.getData('class-id'),
                            date: node.getData('date'),
                            description: descNode.getValue(),
                            disciplineTypeIds: discTypesNode.getValue(),
                            time: rowNode.find('[name="time"]').getValue()
                        });
                    }
                }
                var row = rowNode.previous();
                var tooltipNode = row.find('.with-discipline');
                if(discTypesNode.getValue()){
                    var text = rowNode.find(':checked').next().valueOf().map(function(item){
                        return item.innerHTML.capitalize()
                    }).join(', ');
                    if(tooltipNode.exists())
                        tooltipNode
                            .show()
                            .setData('tooltip', text);
                    else{
                        new ria.dom.Dom()
                            .fromHTML('<div class="with-discipline" data-tooltip="' + text + '"></div>')
                            .appendTo(row.find('.tooltip-container'));
                    }
                }else{
                    if(tooltipNode.exists())
                        tooltipNode.hide();
                }

                return res;
            },

            [[ria.dom.Dom, Boolean]],
            VOID, function updateDiscipline_(node){
                if(this._isAblePostDiscipline){
                    var form = node.parent('form');
                    var time = getDate().getTime();
                    form.find('.save-time').setValue(time);
                    form.previous()
                        .setAttr('time', time)
                        .removeClass('saved')
                        .addClass('saving');
                    var disciplinesNode = form.find('input[name="disciplinesJson"]');
                    disciplinesNode.setValue(JSON.stringify(this.getDisciplines_(form)));
                    form.trigger('submit');
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.discipline.DisciplineTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function doUpdateItem(tpl, model, msg_) {
                var row = this.dom.find('.row[time=' + model.getTime() + ']');
                if(row.exists()){
                    row.removeClass('saving').addClass('saved');
                }
            },

            [ria.mvc.DomEventBind('submit', 'form.discipline-form-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function submitForm(node, event){
                return this._isAblePostDiscipline;
            }
    ]);
});