REQUIRE('chlk.models.id.ABSubjectDocumentId');
REQUIRE('chlk.models.id.ABCourseId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Course*/

    CLASS('Course', [

        chlk.models.id.ABCourseId, 'id',
        String, 'description',
        chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',

        function getName(){
            return this.getDescription();
        },
    ]);
});
