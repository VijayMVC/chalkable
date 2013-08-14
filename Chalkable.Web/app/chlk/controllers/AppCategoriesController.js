REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.activities.apps.AppCategoryListPage');
REQUIRE('chlk.activities.apps.CategoryDialog');
REQUIRE('chlk.models.id.AppCategoryId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AppCategoriesController */
    CLASS(
        'AppCategoriesController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AppCategoryService, 'appCategoryService',

        [chlk.controllers.SidebarButton('settings')],


        [[Number]],
        function listAction(pageIndex_) {
            var result = this.appCategoryService
                .getCategories(pageIndex_|0)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.apps.AppCategoryListPage, result);
        },

        [[Number]],
        function pageAction(pageIndex) {
            var result = this.appCategoryService
                .getCategories(pageIndex)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AppCategoryListPage, result);
        },


        [[chlk.models.id.AppCategoryId]],
        function updateAction(id) {
            var result = this.appCategoryService
                .getCategory(id)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.apps.CategoryDialog, result);
        },


        function addAction() {
            var result = new ria.async.DeferredData(new chlk.models.apps.AppCategory);
            return this.ShadeView(chlk.activities.apps.CategoryDialog, result);
        },

        [[chlk.models.apps.AppCategory]],
        function saveAction(model){
            var result = this.appCategoryService
                .saveCategory(
                    model.getId(),
                    model.getName(),
                    model.getDescription()
                )
                .attach(this.validateResponse_())
                .then(function (data) {
                    this.view.getCurrent().close();
                    return this.appCategoryService.getCategories(0);
                }.bind(this));

            return this.UpdateView(chlk.activities.apps.AppCategoryListPage, result);
        },



        [[chlk.models.id.AppCategoryId]],
        function deleteAction(id) {
            var result= this.appCategoryService
                .removeCategory(id)
                .attach(this.validateResponse_())
                .then(function (data) {
                    return this.appCategoryService.getCategories(0)
                },this);
            return this.UpdateView(chlk.activities.apps.AppCategoryListPage, result);
        }
    ])
});
