REQUIRE('chlk.models.id.ABDocumentId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Document*/

    CLASS('Document', [
        chlk.models.id.ABDocumentId, 'id',
        String, 'code',
        String, 'description',

        function getName(){
            return this.getCode();
        }
    ]);
});
