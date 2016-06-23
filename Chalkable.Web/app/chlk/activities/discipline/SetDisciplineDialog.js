REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.discipline.SetDisciplineDialog');

NAMESPACE('chlk.activities.discipline', function(){

    /** @class chlk.activities.discipline.SetDisciplineDialog */

    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.SetDisciplineDialog)],

        'SetDisciplineDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            Array, function getDisciplines_(){
                var res = [];
                var disciplinesNodes = this.dom.find('[name="discipline"]').valueOf();
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

            [ria.mvc.DomEventBind('click', '#submit-discipline-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                var disciplinesNode = this.dom.find('input[name="disciplinesJson"]');
                disciplinesNode.setValue(JSON.stringify(this.getDisciplines_()));
            }
        ]);
});