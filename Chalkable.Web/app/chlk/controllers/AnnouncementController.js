REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.LastMessages');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AnnouncementController */
    CLASS(
        'AnnouncementController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number]],
        function addAction(classId_, announcementTypeId_) {
            var result = this.announcementService
                .addAnnouncement(classId_, announcementTypeId_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var classes = this.classService.getClassesForTopBar();
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    var announcement = model.getAnnouncement();
                    var attachments = announcement.getAnnouncementAttachments();
                    announcement.setAttachments(attachments.map(function(item){return item.id}).join(','));
                    topModel.setTopItems(classes);
                    classId_ = announcement.getRecipientId();
                    if(classId_){
                        topModel.setSelectedItemId(classId_);
                        var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                        model.setClassInfo(classInfo);
                    }
                    announcementTypeId_ = announcement.getAnnouncementTypeId();
                    if(announcementTypeId_){
                        model.setSelectedTypeId(announcementTypeId_);
                    }
                    model.setTopData(topModel);

                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId]],
        function editAction(announcementId) {
            var result = this.announcementService
                .updateAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    var classes = this.classService.getClassesForTopBar();
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    var announcement = model.getAnnouncement();
                    var attachments = announcement.getAnnouncementAttachments();
                    announcement.setAttachments(attachments.map(function(item){return item.id}).join(','));
                    topModel.setTopItems(classes);
                    var classId_ = announcement.getRecipientId();
                    if(classId_){
                        topModel.setSelectedItemId(classId_);
                        var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                        model.setClassInfo(classInfo);
                    }
                    var announcementTypeId_ = announcement.getAnnouncementTypeId();
                    if(announcementTypeId_){
                        model.setSelectedTypeId(announcementTypeId_);
                    }
                    model.setTopData(topModel);

                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId]],
        function viewAction(announcementId) {
            var result = this.announcementService
                .readAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    var attachments = model.getAnnouncementAttachments();
                    model.setAttachments(attachments.map(function(item){return item.id}).join(','));
                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        [[chlk.models.id.AnnouncementId, Object]],
        function uploadAttachmentAction(announcementId, files) {
            var result = this.announcementService
                .uploadAttachment(announcementId, files)
                .attach(this.validateResponse_())
                .then(function(model){
                    var attachments = model.getAnnouncementAttachments();
                    model.setAttachments(attachments.map(function(item){return item.id}).join(','));
                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [[chlk.models.id.AttachmentId]],
        function deleteAttachmentAction(attachmentId) {
            var result = this.announcementService
                .deleteAttachment(attachmentId)
                .attach(this.validateResponse_())
                .then(function(model){
                    var attachments = model.getAnnouncementAttachments();
                    model.setAttachments(attachments.map(function(item){return item.id}).join(','));
                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [[chlk.models.announcement.Announcement]],
        function saveAction(model) {

            function save(){
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
            }

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
                    save.call(this);
                    return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
                }else{
                    if(submitType == 'saveNoUpdate'){
                        save.call(this);
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
        }
    ])
});
