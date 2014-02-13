REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');

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
            }
        ])
});