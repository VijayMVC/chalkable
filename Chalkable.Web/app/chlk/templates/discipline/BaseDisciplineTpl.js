REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.discipline.DisciplineList');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.BaseDisciplineTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.discipline.DisciplineList)],
        'BaseDisciplineTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',

            [[Number]],
            String, function getTypesForToolTip(number){
                var res = '',
                    types = this.getDisciplines()[number].getDisciplineTypes(), otherLen, len = types.length;
                if(len > 0){
                    if(len > 3)
                        otherLen = len - 3;
                    res = types.slice(0,3).map(function(item){
                        return item.getName() ? item.getName().capitalize() : item.getName()
                    }).join(',\n');
                    if(otherLen)
                        res += ',\n+ {' + otherLen + '} more';
                }
                return res;
            }

        ]);
});