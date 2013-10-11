REQUIRE('chlk.models.discipline.SetDisciplineModel');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.SetDisciplineListModel*/

    CLASS('SetDisciplineListModel', [

        ArrayOf(chlk.models.discipline.SetDisciplineModel), 'disciplines',

        String, 'disciplinesJson',

        String, 'controller',
        String, 'action',
        String, 'params',

        [[String]],
        VOID, function setDisciplinesJson(disciplinesJsonObj){
                var serializer = new ria.serialize.JsonSerializer();
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