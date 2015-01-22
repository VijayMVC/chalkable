REQUIRE('chlk.models.discipline.SetDisciplineModel');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.SetDisciplineListModel*/

    CLASS('SetDisciplineListModel', [

        ArrayOf(chlk.models.discipline.SetDisciplineModel), 'disciplines',

        String, 'disciplinesJson',

        chlk.models.common.ChlkDate, 'date',

        String, 'controller',
        String, 'action',
        String, 'params',
        Boolean, 'newStudent',

        [[String]],
        VOID, function setDisciplinesJson(disciplinesJsonObj){
                var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                var disciplinesObj = disciplinesJsonObj ? JSON.parse(disciplinesJsonObj) : null;
                this.setDisciplines(serializer.deserialize(disciplinesObj, ArrayOf(chlk.models.discipline.SetDisciplineModel)));
        },

        Object, function getPostData(){
            var res = [];
            var disciplines = this.getDisciplines();
            for(var i = 0; i < disciplines.length; i++){
                res.push(disciplines[i].getPostData())
            }
            return res;
        }
    ]);
});