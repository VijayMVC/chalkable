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

            [[chlk.models.id.ClassId]],
            ria.async.Future, function getSubjects(classId_) {
                return this.get('Standard/GetStandardSubject.json', ArrayOf(chlk.models.standard.StandardSubject), {
                    classId: classId_ && classId_.valueOf()
                });
            },

            [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, chlk.models.id.StandardId]],
            ria.async.Future, function getStandards(classId_, subjectId_, standardId_) {
                return this.get('Standard/GetStandards.json', ArrayOf(chlk.models.standard.Standard), {
                    classId: classId_ && classId_.valueOf(),
                    subjectId: subjectId_ && subjectId_.valueOf(),
                    parentStandardId: standardId_ && standardId_.valueOf()
                });
            },

            [[chlk.models.id.CCStandardCategoryId, chlk.models.id.CommonCoreStandardId, Boolean]],
            ria.async.Future, function getCommonCoreStandards(standardCategoryId_, parentStandardId_, allStandards_) {
                return this.get('Standard/GetCommonCoreStandards.json', ArrayOf(chlk.models.standard.CommonCoreStandard), {
                    standardCategoryId: standardCategoryId_ && standardCategoryId_.valueOf(),
                    parentStandardId: parentStandardId_ && parentStandardId_.valueOf(),
                    allStandards: allStandards_
                });
            },

            ria.async.Future, function getCCStandardCategories(){
                return this.get('Standard/GetCommonCoreStandardCategories.json', ArrayOf(chlk.models.standard.CCStandardCategory), {
                });
            }
        ])
});