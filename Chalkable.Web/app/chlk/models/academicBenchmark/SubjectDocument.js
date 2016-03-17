REQUIRE('chlk.models.id.ABSubjectDocumentId');
REQUIRE('chlk.models.id.ABAuthorityId');
REQUIRE('chlk.models.id.ABDocumentId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.SubjectDocument*/

    CLASS('SubjectDocument', [
        chlk.models.id.ABSubjectDocumentId, 'id',
        String, 'description',
        chlk.models.id.ABAuthorityId, 'authorityId',
        chlk.models.id.ABDocumentId, 'documentId',

        function getName(){
            return this.getDescription();
        }
    ]);
});
