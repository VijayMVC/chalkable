REQUIRE('chlk.models.id.ABTopicId');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Topic*/

    CLASS('Topic', [

        chlk.models.id.ABTopicId, 'id',

        [ria.serialize.SerializeProperty('description')],
        String, 'name',

        Number, 'level',

        String, 'tooltip',

        [ria.serialize.SerializeProperty('isdeepest')],
        Boolean, 'deepest',

        chlk.models.id.ABTopicId, 'parentid',

        [ria.serialize.SerializeProperty('isactive')],
        Boolean, 'active',

        [ria.serialize.SerializeProperty('courseid')],
        chlk.models.id.ABCourseId, 'courseId',

        [ria.serialize.SerializeProperty('subjectdocumentid')],
        chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',

        function getDescription(){
            return null;
        },

        Object, function serialize() {
            return {
                id: this.id.valueOf(),
                description: this.name
            }
        }
    ]);
});