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

    var announcementAttachments;

    /** @class chlk.controllers.AnnouncementController */
    CLASS(
        'AnnouncementController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',

        [[chlk.models.announcement.AnnouncementForm, Boolean]],
        function addEditAction(model, isEdit){
            var classes = this.classService.getClassesForTopBar();
            var topModel = new chlk.models.class.ClassesForTopBar();
            var announcement = model.getAnnouncement();
            var attachments = announcement.getAnnouncementAttachments();
            this.getContext().getSession().set('AnnouncementAttachments', attachments);
            announcement.setAttachments(attachments.map(function(item){return item.id}).join(','));
            topModel.setTopItems(classes);
            topModel.setDisabled(isEdit);
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
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number]],
        function addAction(classId_, announcementTypeId_) {
            var result = this.announcementService
                .addAnnouncement(classId_, announcementTypeId_)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.addEditAction(model, false);
                }.bind(this));
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [[chlk.models.id.AnnouncementId]],
        function editAction(announcementId) {
            var result = this.announcementService
                .editAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.addEditAction(model, true);
                }.bind(this));
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [[chlk.models.id.AnnouncementId]],
        function viewAction(announcementId) {
            var result = this.announcementService
                .getAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    var now = getDate(), days;
                    var expires = model.getExpiresDate();
                    var expiresDate = expires.getDate();
                    var date = expires.format('(D m/d)');
                    model.setExpiresDateColor('blue');
                    if(formatDate(now, 'dd-mm-yy') == expires.format('dd-mm-yy')){
                        model.setExpiresDateColor('blue');
                        model.setExpiresDateText(Msg.Due_today);
                    }else{
                        if(now > expires.getDate()){
                            model.setExpiresDateColor('red');
                            days = Math.ceil((now - expiresDate)/1000/3600/24);
                            if(days == 1){
                                model.setExpiresDateText(Msg.Due_yesterday + " " + date);
                            }else{
                                model.setExpiresDateText(Msg.Due_days_ago(days) + " " + date);
                            }
                        }else{
                            days = Math.ceil((expiresDate - now)/1000/3600/24);
                            if(days == 1){
                                model.setExpiresDateText(Msg.Due_tomorrow + " " + date);
                            }else{
                                model.setExpiresDateText(Msg.Due_in_days(days) + " " + date);
                            }
                        }
                    }
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
                    this.getContext().getSession().set('AnnouncementAttachments', attachments);
                    model.setAttachments(attachments.map(function(item){return item.id}).join(','));
                    return new ria.async.DeferredData(model);
                }.bind(this));
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, result);
        },

        [[chlk.models.id.AnnouncementId]],
        function deleteAction(announcementId) {
            this.announcementService
                .deleteAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.redirect_('feed', 'list', []);
                }.bind(this));
        },

        [[chlk.models.id.SchoolPersonId]],
        function discardAction(schoolPersonId) {
            this.announcementService
                .deleteDrafts(schoolPersonId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.redirect_('feed', 'list', []);
                }.bind(this));
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
                    model.setAnnouncementAttachments(this.getContext().getSession().get('AnnouncementAttachments'));
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
                        return this.redirect_('feed', 'list', []);
                    }
                }
            }
        }
    ])
});
