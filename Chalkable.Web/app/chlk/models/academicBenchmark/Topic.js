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

        [[chlk.models.id.ABTopicId, String, Boolean]],
        function $(id_, name_, deepest_){
            BASE();
            if(id_)
                this.setId(id_);
            if(name_)
                this.setName(name_);
            if(deepest_)
                this.setDeepest(deepest_);
        },

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