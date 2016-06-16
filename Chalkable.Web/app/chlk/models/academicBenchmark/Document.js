REQUIRE('chlk.models.id.ABDocumentId');
REQUIRE('chlk.models.id.ABAuthorityId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Document*/

    CLASS('Document', [
        chlk.models.id.ABDocumentId, 'id',
        String, 'code',
        String, 'description',
        chlk.models.id.ABAuthorityId, 'authorityId',

        function getName(){
            return this.getCode() || this.getDescription();
        }
    ]);
});
