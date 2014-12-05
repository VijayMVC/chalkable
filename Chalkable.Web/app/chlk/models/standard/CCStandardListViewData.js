REQUIRE('chlk.models.standard.CommonCoreStandard');
REQUIRE('chlk.models.standard.CCStandardCategory');
REQUIRE('chlk.models.id.CCStandardCategoryId');

NAMESPACE('chlk.models.standard', function(){
    /**@class chlk.models.standard.CCStandardListViewData*/

    CLASS('CCStandardListViewData', [

        ArrayOf(chlk.models.standard.CCStandardCategory), 'categories',

        chlk.models.id.CCStandardCategoryId, 'categoryId',

        ArrayOf(chlk.models.standard.CommonCoreStandard), 'standards',

        String, 'description',

        [[String, chlk.models.id.CCStandardCategoryId
            , ArrayOf(chlk.models.standard.CCStandardCategory)
            , ArrayOf(chlk.models.standard.CommonCoreStandard)]],
        function $(description_, categoryId_, categories_, standards_){
            BASE();
            if(categories_)
                this.setCategories(categories_);
            if(categoryId_)
                this.setCategoryId(categoryId_);
            if(description_)
                this.setDescription(description_);
            if(standards_)
                this.setStandards(standards_);
        }
    ]);
});