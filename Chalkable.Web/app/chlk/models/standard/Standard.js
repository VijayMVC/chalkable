REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.Standard*/
    CLASS(
        'Standard', [
             String, 'name',

             String, 'description',

             chlk.models.id.AnnouncementId, 'announcementId',

             [ria.serialize.SerializeProperty('standardid')],
             chlk.models.id.StandardId, 'standardId',

             String, 'grade',

            [[chlk.models.id.StandardId, String, String]],
            function $(standardId_, name_, grade_){
                BASE();
                if(standardId_)
                    this.setStandardId(standardId_);
                if(name_)
                    this.setName(name_);
                if(grade_)
                    this.setGrade(grade_);
            }
        ]);
});
