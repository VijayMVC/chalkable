REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.GradingPeriodService */
    CLASS(
        'GradingPeriodService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.SchoolYearId]],
            ria.async.Future, function getList(schoolYearId_) {
                return this.get('GradingPeriod/List.json', ArrayOf(chlk.models.schoolYear.GradingPeriod), {
                    schoolYearId: schoolYearId_ && schoolYearId_.valueOf()
                }).then(function(gps){
                    var currentGp = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIOD, null);
                    if(currentGp){
                        gps.forEach(function(gp){
                            gp.setCurrent(gp.getId() == currentGp.getId());
                        });
                    };
                    return gps;
                }, this);
            },

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getListByClassId(classId){
                return this.get('GradingPeriod/ListByClassId.json', ArrayOf(chlk.models.schoolYear.GradingPeriod), {
                    classId: classId && classId.valueOf()
                });
            }
        ])
});