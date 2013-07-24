REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.LastMessages');

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
            var result = this.announcementService
                .addAnnouncement(classId_, announcementTypeId_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var classes = this.classService.getClassesForTopBar();
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    classId_ = model.getAnnouncement().getRecipientId();
                    if(classId_){
                        topModel.setSelectedItemId(classId_);
                        var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                        model.setClassInfo(classInfo);
                    }
                    announcementTypeId_ = model.getAnnouncement().getAnnouncementTypeId();
                    if(announcementTypeId_){
                        model.setSelectedTypeId(announcementTypeId_);
                model.setAnnouncementTypeId(announcementTypeId_)
                    }
                    model.setTopData(topModel);
                    //announcementTypeId_ && data.setAnnouncementTypeId(announcementTypeId_);
                    //model.setAnnouncement(data);

                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [[chlk.models.announcement.Announcement]],
        function saveAction(model) {
            var result;
            var submitType = model.getSubmitType();
            var schoolPersonId = model.getSchoolPersonRef();
            var announcementTypeId = model.getAnnouncementTypeId();
            var announcementTypeName = model.getAnnouncementTypeName();
            var classId = model.getClassId();
            if(submitType == 'listLast'){
                result = this.announcementService
                    .listLast(classId, announcementTypeId,schoolPersonId)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var model = new chlk.models.announcement.LastMessages();
                        model.setItems(data);
                        model.setAnnouncementTypeName(announcementTypeName);
                        return new ria.async.DeferredData(model);
                    }.bind(this));
                return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
            }else{
                if(submitType == 'save'){
                    result = new ria.async.DeferredData(model);
                    /*this.announcementService.saveAnnouncement(
                        model.getId(),
                        model.getClassId(),
                        model.getAnnouncementTypeId(),
                        model.getSubject(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getAttachments(),
                        model.getApplications(),
                        model.getMarkingPeriodId()
                    );*/
                    return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
                }else{
                    /*this.announcementService.submitAnnouncement(
                        model.getId(),
                        model.getClassId(),
                        model.getAnnouncementTypeId(),
                        model.getSubject(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getAttachments(),
                        model.getApplications(),
                        model.getMarkingPeriodId()
                    ).then(function(){
                        this.redirect('feed', 'list', []);
                    }.bind(this));*/
                    this.redirect_('feed', 'list', []);
                }
            }
        }
    ])
});
