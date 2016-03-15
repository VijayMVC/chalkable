REQUIRE('chlk.models.id.ABSubjectDocumentId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.SubjectDocument*/

    CLASS('SubjectDocument', [
        chlk.models.id.ABSubjectDocumentId, 'id',
        String, 'description'
    ]);
});
