REQUIRE('chlk.models.id.ABSubjectDocumentId');
REQUIRE('chlk.models.id.ABCourseId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Course*/

    CLASS('Course', [

        chlk.models.id.ABCourseId, 'id',
        String, 'description',
        chlk.models.id.ABAuthorityId, 'authorityId',
        chlk.models.id.ABDocumentId, 'documentId',
        chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',
        String, 'gradeLevel',

        function getName(){
            return this.getDescription();
        }
    ]);
});
