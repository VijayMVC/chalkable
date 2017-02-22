REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.schoolYear.Year');
REQUIRE('chlk.models.schoolYear.YearAndClasses');

NAMESPACE('chlk.services', function () {
    "use strict";


    /** @class chlk.services.SchoolYearService */
    CLASS(
        'SchoolYearService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolId]],
            ria.async.Future, function list(schoolId_) {
                return this.get('SchoolYear/List.json', ArrayOf(chlk.models.schoolYear.Year), {
                    schoolId: schoolId_ && schoolId_.valueOf()
                });
            },

            ria.async.Future, function listOfSchoolYearClasses() {
                return this.get('SchoolYear/ListOfSchoolYearClasses.json', ArrayOf(chlk.models.schoolYear.YearAndClasses), {})
                    .then(function(items){
                        items = items.filter(function(item){
                            var sY = item.getSchoolYear();
                            return sY.getStartDate() && sY.getEndDate();
                        });
                        return items;
                    });
            }
        ])
});