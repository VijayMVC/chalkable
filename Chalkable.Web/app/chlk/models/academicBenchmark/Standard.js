REQUIRE('chlk.models.id.ABStandardId');
REQUIRE('chlk.models.academicBenchmark.Authority');
REQUIRE('chlk.models.academicBenchmark.Document');
REQUIRE('chlk.models.academicBenchmark.SubjectDocument');

NAMESPACE('chlk.models.academicBenchmark', function(){

    /**@class chlk.models.academicBenchmark.Standard*/

    CLASS('Standard', [

        [ria.serialize.SerializeProperty('id')],
        chlk.models.id.ABStandardId, 'standardId',
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

        function getName(){
            return this.getCode();
        }


    ]);
});
