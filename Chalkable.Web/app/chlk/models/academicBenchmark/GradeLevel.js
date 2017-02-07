REQUIRE('chlk.models.id.ABSubjectDocumentId');
REQUIRE('chlk.models.id.ABAuthorityId');
REQUIRE('chlk.models.id.ABDocumentId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.GradeLevel*/

    CLASS('GradeLevel', [

        String, 'code',
        String, 'description',
        String, 'low',
        String, 'height',
        chlk.models.id.ABAuthorityId, 'authorityId',
        chlk.models.id.ABDocumentId, 'documentId',
        chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',

        function getName(){
            return this.getCode();
        },

        function getId(){
            return this.getCode();
        }
    ]);
});
