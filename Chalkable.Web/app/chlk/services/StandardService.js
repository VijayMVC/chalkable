REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');

REQUIRE('chlk.models.standard.CommonCoreStandard');
REQUIRE('chlk.models.standard.CCStandardCategory');
REQUIRE('chlk.models.id.CCStandardCategoryId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.StandardService*/
    CLASS(
        'StandardService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getSubjects() {
                return this.get('Standard/GetStandardSubject.json', ArrayOf(chlk.models.standard.StandardSubject), {});
            },

            [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId]],
            ria.async.Future, function getStandards(classId_, subjectId_, standardId_) {
                return this.get('Standard/GetStandards.json', ArrayOf(chlk.models.standard.Standard), {
                    classId: classId_ && classId_.valueOf(),
                    subjectId: subjectId_ && subjectId_.valueOf(),
                    parentStandardId: standardId_ && standardId_.valueOf()
                });
            },

            [[chlk.models.id.CCStandardCategoryId]],
            ria.async.Future, function getCommonCoreStandards(standardCategoryId_) {
                return this.get('Standard/GetCommonCoreStandards.json', ArrayOf(chlk.models.standard.CommonCoreStandard), {
                    standardCategoryId: standardCategoryId_ && standardCategoryId_.valueOf()
                });
            },

            [[chlk.models.id.CCStandardCategoryId]],
            ria.async.Future, function getCCStandardCategories(parentCategoryId_){
                return this.get('Standard/GetCommonCoreStandardCategories.json', ArrayOf(chlk.models.standard.CCStandardCategory), {
                    parentCategoryId: parentCategoryId_ && parentCategoryId_.valueOf()
                });
            }
        ])
});