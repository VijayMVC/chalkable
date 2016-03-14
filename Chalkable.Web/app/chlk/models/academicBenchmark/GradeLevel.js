REQUIRE('chlk.models.id.ABAuthorityId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.GradeLevel*/

    CLASS('GradeLevel', [

        String, 'code',
        String, 'description',
        String, 'low',
        String, 'height'
    ]);
});
