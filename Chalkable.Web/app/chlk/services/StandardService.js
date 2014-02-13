REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.StandardSubjectId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.StandardService*/
    CLASS(
        'StandardService', EXTENDS(chlk.services.BaseService), [
            ria.async.Future, function getSubjects() {
                return this.get('Standard/GetStandardSubject.json', ArrayOf(chlk.models.standard.StandardSubject), {});
            },

            [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId]],
            ria.async.Future, function getStandards(classId_, subjectId_) {
                return this.get('Standard/GetStandards.json', ArrayOf(chlk.models.standard.Standard), {
                    classId_: classId_ && classId_.valueOf(),
                    subjectId_: subjectId_ && subjectId_.valueOf()
                });
            }
        ])
});