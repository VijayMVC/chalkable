REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.LpGalleryCategoryService');

REQUIRE('chlk.activities.announcement.AddNewCategoryDialog');
REQUIRE('chlk.activities.announcement.LessonPlanFormPage');

REQUIRE('chlk.models.announcement.CategoriesListViewData');
REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');


NAMESPACE('chlk.controllers', function () {

    /** @class chlk.controllers.LpGalleryCategoryController */
    CLASS(
        'LpGalleryCategoryController', EXTENDS(chlk.controllers.BaseController), [


            [ria.mvc.Inject],
            chlk.services.LpGalleryCategoryService, 'lpGalleryCategoryService',

            function editCategoriesAction(){
                var res = this.lpGalleryCategoryService.list()
                    .then(function(list){
                        return new chlk.models.announcement.CategoriesListViewData(list);
                    })
                    .attach(this.validateResponse_());
                return this.ShadeView(chlk.activities.announcement.AddNewCategoryDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            function addAction(){
                var res = new ria.async.DeferredData(new chlk.models.announcement.CategoryViewData);

                return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.LpGalleryCategoryId]],
            function tryDeleteAction(categoryId) {
                this.ShowConfirmBox('Are you sure you want to delete this category?', "whoa.", null, 'negative-button')
                    .then(function (data) {
                        return this.BackgroundNavigate('lpgallerycategory', 'delete', [categoryId]);
                    }, this);
                return null;
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.id.LpGalleryCategoryId]],
            function deleteAction(categoryId){
                var res = this.lpGalleryCategoryService.deleteCategory(categoryId)
                    .thenCall(this.lpGalleryCategoryService.list, [])
                    .then(function(list){
                        this.afterCategoryEdit_(list);
                        return new chlk.models.announcement.CategoriesListViewData(list);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.announcement.CategoryViewData]],
            function createAction(model){
                var res = this.lpGalleryCategoryService.create(model.getName())
                    .then(function(data){
                        if(!data)
                            this.ShowMsgBox("Category with this name already exists");
                        return this.lpGalleryCategoryService.list();
                    }, this)
                    .then(function(list){
                        this.afterCategoryEdit_(list);
                        return new chlk.models.announcement.CategoriesListViewData(list);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
            },

            [chlk.controllers.NotChangedSidebarButton()],
            [[chlk.models.announcement.CategoryViewData]],
            function editNameAction(model){
                var res = this.lpGalleryCategoryService.update(model.getId(), model.getName())
                    .then(function(data){
                        if(!data)
                            this.ShowMsgBox("Category with this name already exists");
                        return this.lpGalleryCategoryService.list();
                    }, this)
                    .then(function(list){
                        this.afterCategoryEdit_(list);
                        return new chlk.models.announcement.CategoriesListViewData(list);
                    }, this)
                    .attach(this.validateResponse_());

                return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
            },

            function afterCategoryEdit_(list){
                var model = new chlk.models.announcement.FeedAnnouncementViewData();
                model.setCategories(list);
                this.BackgroundUpdateView(chlk.activities.announcement.LessonPlanFormPage, model, 'right-categories');
                this.BackgroundUpdateView(chlk.activities.announcement.LessonPlanFormPage, model, 'categories');
            }
        ]);
});