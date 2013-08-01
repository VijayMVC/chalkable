NAMESPACE('chlk.models.apps', function () {
    "use strict";
    /** @class chlk.models.apps.Application*/
    CLASS(
        'Application', [
            Number, 'id',
            String, 'name',
            String, 'url',
            [ria.serialize.SerializeProperty('videodemourl')],
            String, 'videoModeUrl',
            [ria.serialize.SerializeProperty('shortdescription')],
            String, 'shortDescription',
            String, 'description',
            [ria.serialize.SerializeProperty('isinternal')],
            Boolean, 'isInternal'
        ]);
});
