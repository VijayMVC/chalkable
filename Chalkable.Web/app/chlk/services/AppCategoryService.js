REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.apps.AppCategory');
REQUIRE('chlk.models.id.AppCategoryId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.AppCategoryService */
    CLASS(
        'AppCategoryService', EXTENDS(chlk.services.BaseService), [
            [[Number]],
            ria.async.Future, function getCategories(pageIndex_) {

                var cachedCategories = this.getContext().getSession().get(ChlkSessionConstants.CACHED_APP_CATEGORIES);

                return cachedCategories ? ria.async.DeferredData(cachedCategories)
                                        : this.getPaginatedList('Category/List.json', chlk.models.apps.AppCategory, {
                                            start: pageIndex_|0
                                        })
                                        .then(function(data){
                                            this.getContext().getSession().set(ChlkSessionConstants.CACHED_APP_CATEGORIES, data);
                                            return data;
                                        }, this);
            },

            [[String, String]],
            ria.async.Future, function addCategory(name, description) {
                return this
                    .post('Category/Add.json', chlk.models.apps.AppCategory, {
                        name: name,
                        description: description
                    })
                    .then(function(data){
                        this.getContext().getSession().set(ChlkSessionConstants.CACHED_APP_CATEGORIES, null);
                    }, this);
            },

            [[chlk.models.id.AppCategoryId, String, String]],
            ria.async.Future, function updateCategory(id, name, description) {
                return this
                    .post('Category/Update.json', chlk.models.apps.AppCategory, {
                        categoryId: id.valueOf(),
                        name: name,
                        description: description
                    })
                    .then(function(data){
                        this.getContext().getSession().set(ChlkSessionConstants.CACHED_APP_CATEGORIES, null);
                    }, this);
            },

            [[chlk.models.id.AppCategoryId, String, String]],
            ria.async.Future, function saveCategory(id_, name, description) {
                if (id_ && id_.valueOf())
                    return this.updateCategory(id_, name, description);

                return this.addCategory(name, description);
            },

            [[chlk.models.id.AppCategoryId]],
            ria.async.Future, function removeCategory(id) {
                return this
                    .post('Category/Delete.json', chlk.models.apps.AppCategory, {
                        categoryId: id.valueOf()
                    })
                    .then(function(data){
                        this.getContext().getSession().set(ChlkSessionConstants.CACHED_APP_CATEGORIES, null);
                    }, this);
            },
            [[chlk.models.id.AppCategoryId]],
            ria.async.Future, function getCategory(id) {
                return this.post('Category/GetInfo.json', chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf()
                });
            }
        ])
});