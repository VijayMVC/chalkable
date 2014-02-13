REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardSubjectId');

NAMESPACE('chlk.models.standard', function(){
    /**@class chlk.models.standard.StandardsListViewData*/
    CLASS('StandardsListViewData', [

        ArrayOf(chlk.models.standard.Standard), 'itemStandards',

        chlk.models.id.ClassId, 'classId',

        chlk.models.id.StandardSubjectId, 'subjectId',

        String, 'description',

        [[String, chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, ArrayOf(chlk.models.standard.Standard)]],
        function $(description_, classId_, subjectId_, itemStandards_){
            BASE();
            if(itemStandards_)
                this.setItemStandards(itemStandards_);
            if(classId_)
                this.setClassId(classId_);
            if(subjectId_)
                this.setSubjectId(subjectId_);
            if(description_)
                this.setDescription(description_);
        }
    ]);
});