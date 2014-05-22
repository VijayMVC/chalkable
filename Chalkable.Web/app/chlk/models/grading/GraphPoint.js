NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GraphPoint*/
    CLASS(
        'GraphPoint', [
            Number, 'grade',
            [ria.serialize.SerializeProperty('studentcount')],
            Number, 'studentCount',
            [ria.serialize.SerializeProperty('startinterval')],
            Number, 'startInterval',
            [ria.serialize.SerializeProperty('endinterval')],
            Number, 'endInterval',
            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',
            [ria.serialize.SerializeProperty('mappedstartinterval')],
            Number, 'mappedStartInterval',
            [ria.serialize.SerializeProperty('mappedendinterval')],
            Number, 'mappedEndInterval'
        ]);
});
