REQUIRE('chlk.models.id.ABStandardId');
REQUIRE('chlk.models.id.ABCourseId');
REQUIRE('chlk.models.academicBenchmark.Authority');
REQUIRE('chlk.models.academicBenchmark.Document');
REQUIRE('chlk.models.academicBenchmark.SubjectDocument');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Standard*/

    CLASS('Standard', [

        chlk.models.id.ABAuthorityId, 'authorityId',
        chlk.models.id.ABDocumentId, 'documentId',
        chlk.models.id.ABSubjectDocumentId, 'subjectDocumentId',
        String, 'gradeLevel',
        chlk.models.id.ABCourseId, 'standardCourseId',

        [ria.serialize.SerializeProperty('id')],
        chlk.models.id.ABStandardId, 'standardId',
        chlk.models.id.ABStandardId, 'parentStandardId',
        String, 'code',
        String, 'description',
        String, 'tooltip',

        [ria.serialize.SerializeProperty('isdeepest')],
        Boolean, 'deepest',

        Number, 'level',

        [ria.serialize.SerializeProperty('isactive')],
        Boolean, 'active',

        chlk.models.academicBenchmark.Authority, 'authority',
        chlk.models.academicBenchmark.Document, 'document',
        chlk.models.academicBenchmark.SubjectDocument, 'subject',

        [[chlk.models.id.ABStandardId, String, String, Boolean]],
        function $(standardId_, name_, description_, deepest_){
            BASE();
            if(standardId_)
                this.setStandardId(standardId_);
            if(name_)
                this.setName(name_);
            if(description_)
                this.setDescription(description_);
            if(deepest_)
                this.setDeepest(deepest_);
        },

        function getName(){
            return this.getCode();
        },

        String, function displayTitle(){
            var name = this.getName();
            if(name && (!name.trim || name.trim() != '')) return name;
            return this.getDescription();
        },

        Object, function serialize() {
            return {
                id: this.standardId.valueOf(),
                standardCode: this.code,
                description: this.description
            }
        }


    ]);
});
