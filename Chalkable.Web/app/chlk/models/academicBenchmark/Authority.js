REQUIRE('chlk.models.id.ABAuthorityId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Authority*/

    CLASS('Authority', [
        chlk.models.id.ABAuthorityId, 'id',
        String, 'code',
        String, 'description',

        function getName(){
            return this.getCode();
        }
    ]);
});
