REQUIRE('chlk.models.id.DisciplineId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.common.ChlkDate');


NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.SetDisciplineModel*/

    CLASS('SetDisciplineModel',[

          chlk.models.id.DisciplineId, 'id',
          chlk.models.id.ClassId, 'classId',
          chlk.models.id.SchoolPersonId, 'studentId',
          chlk.models.common.ChlkDate, 'date',
          String, 'description',
          String, 'disciplineTypeIds',
          Number, 'time',

//          String, 'disciplineJson',


          String, 'controller',
          String, 'action',
          String, 'params',

//          [[String]],
//          VOID, function setDisciplineJson(disciplineJsonObj){
//               var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
//               var disciplinesObj = disciplineJsonObj ? JSON.parse(disciplineJsonObj) : null;
//               var discipline = serializer.deserialize(disciplinesObj, chlk.models.discipline.SetDisciplineModel);
//          },

          Object, function getPostData(){
               return{
                   id: this.getId() && this.getId().valueOf(),
                   classid: this.getClassId() && this.getClassId().valueOf(),
                   studentid: this.getStudentId() && this.getStudentId().valueOf(),
                   date: this.getDate().toStandardFormat(),
                   description: this.getDescription(),
                   time: this.getTime(),
                   infractionsids: this.getDisciplineTypeIds()
               }
          }
    ]);
});