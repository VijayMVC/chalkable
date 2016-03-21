REQUIRE('chlk.models.id.ABTopicId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Topic*/

    CLASS('Topic', [

        chlk.models.id.ABTopicId, 'id',

        String, 'description',

        Number, 'level',

        [ria.serialize.SerializeProperty('isdeepest')],
        Boolean, 'deepest',

        chlk.models.id.ABTopicId, 'parentid',

        [ria.serialize.SerializeProperty('isactive')],
        Boolean, 'active',
    ]);
});