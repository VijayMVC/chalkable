REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.common.ChlkDate');


NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.SetDisciplineModel*/

    CLASS('SetDisciplineModel',[
          chlk.models.id.ClassPeriodId, 'classPeriodId',
          chlk.models.id.ClassPersonId, 'classPersonId',
          chlk.models.common.ChlkDate, 'date',
          String, 'description',
          String, 'disciplineTypeIds',

          Object, function getPostData(){
               return{
                   classperiodid: this.getClassPeriodId().valueOf(),
                   classpersonid: this.getClassPersonId().valueOf(),
                   date: this.getDate().getDate(),
                   description: this.getDescription(),
                   disciplinetypeids: this.getDisciplineTypeIds()
               }
          }
    ]);
});