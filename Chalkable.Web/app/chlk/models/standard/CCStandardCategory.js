REQUIRE('chlk.models.id.CCStandardCategoryId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.CCStandardCategory*/
    CLASS(
        'CCStandardCategory', [

            chlk.models.id.CCStandardCategoryId, 'id',

            [ria.serialize.SerializeProperty('parentcategoryid')],
            chlk.models.id.CCStandardCategoryId, 'parentCategoryId',

            String, 'name'
        ]);
});
