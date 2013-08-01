NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.Mapping*/
    CLASS(
        'Mapping', [
            [ria.serialize.SerializeProperty('gradingabcf')],
            ArrayOf(Number), 'gradingAbcf',
            [ria.serialize.SerializeProperty('gradingcomplete')],
            ArrayOf(Number), 'gradingComplete',
            [ria.serialize.SerializeProperty('gradingcheck')],
            ArrayOf(Number), 'gradingCheck'
        ]);
});
