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
                return this.getPaginatedList('Category/List.json', chlk.models.apps.AppCategory, {
                    start: pageIndex_|0
                });
            },

            [[String, String]],
            ria.async.Future, function addCategory(name, description) {
                return this.post('Category/Create.json', chlk.models.apps.AppCategory, {
                    name: name,
                    description: description
                });
            },

            [[chlk.models.id.AppCategoryId, String, String]],
            ria.async.Future, function updateCategory(id, name, description) {
                return this.post('Category/Update.json', chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf(),
                    name: name,
                    description: description
                });
            },

            [[chlk.models.id.AppCategoryId, String, String]],
            ria.async.Future, function saveCategory(id_, name, description) {
                if (id_ && id_.valueOf()) return this.updateCategory(id_, name, description);
                return this.addCategory(name, description);
            },

            [[chlk.models.id.AppCategoryId]],
            ria.async.Future, function removeCategory(id) {
                return this.post('Category/Delete.json', chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf()
                });
            },
            [[chlk.models.id.AppCategoryId]],
            ria.async.Future, function getCategory(id) {
                return this.post('Category/Info.json', chlk.models.apps.AppCategory, {
                    categoryId: id.valueOf()
                });
            }
        ])
});