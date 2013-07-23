REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.models.announcement.AnnouncementForm');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementController */
    CLASS(
        'AnnouncementController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.class.ClassId, Number]],
        function addAction(classId_, announcementTypeId_) {
            var model = new chlk.models.announcement.AnnouncementForm();
            var classes = this.classService.getClassesForTopBar();
            var topModel = new chlk.models.class.ClassesForTopBar();
            topModel.setTopItems(classes);
            if(classId_){
                topModel.setSelectedItemId(classId_);
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                model.setClassInfo(classInfo);
            }
            if(announcementTypeId_){
                model.setSelectedTypeId(announcementTypeId_);
                model.setAnnouncementTypeId(announcementTypeId_)
            }
            model.setTopData(topModel);
            var result = new ria.async.DeferredData(model);
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        }
    ])
});
