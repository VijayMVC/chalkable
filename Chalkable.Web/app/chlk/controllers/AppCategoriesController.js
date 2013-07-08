REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AppCategoryService');
REQUIRE('chlk.activities.apps.AppCategoryListPage');
REQUIRE('chlk.activities.apps.AddCategoryDialog');

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
                .getCategories(pageIndex_ | 0)
                .attach(this.validateResponse_());
            /* Put activity in stack and render when result is ready */
            return this.PushView(chlk.activities.apps.AppCategoryListPage, result);
        },
        function addCategoryAction(form_){
            var model = new chlk.models.apps.AppCategory;
            if (form_){
                model.setName(form_.name);
                model.setDescription(form_.description);
            }
            return this.ShadeView(chlk.activities.apps.AddCategoryDialog, ria.async.DeferredData(model));
        },
        [[Number]],
        function deleteAction(id) {
        },
        [[Number]],
        function detailsAction(id) {
        }

    ])
});
