REQUIRE('chlk.models.announcement.CategoryViewData');

NAMESPACE('chlk.models.announcement', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.announcement.CategoriesListViewData*/

    CLASS('CategoriesListViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.announcement.CategoryViewData), 'categories',

        VOID, function deserialize(raw){
            this.categories = SJX.fromArrayOfDeserializables(raw.categories, chlk.models.announcement.CategoryViewData);
        },

        [[ArrayOf(chlk.models.announcement.CategoryViewData)]],
        function $(categories_){
            BASE();
            if(categories_)
                this.setCategories(categories_);
        }
    ]);
});