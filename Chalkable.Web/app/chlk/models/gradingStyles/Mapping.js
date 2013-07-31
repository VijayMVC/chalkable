NAMESPACE('chlk.models.gradingStyles', function () {
    "use strict";
    /** @class chlk.models.gradingStyles.Mapping*/
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
