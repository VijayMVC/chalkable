REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.announcement', function(){

    var SJX = ria.serialize.SJX;
    /** @class chlk.models.announcement.CategoriesListViewData*/

    CLASS('CategoriesListViewData', IMPLEMENTS(ria.serialize.IDeserializable), [

        ArrayOf(chlk.models.common.NameId), 'categories',

        VOID, function deserialize(raw){
            this.categories = SJX.fromArrayOfDeserializables(raw.categories, chlk.models.common.NameId);
        },

        [[ArrayOf(chlk.models.common.NameId)]],
        function $(categories_){
            BASE();
            if(categories_)
                this.setCategories(categories_);
        }
    ]);
});