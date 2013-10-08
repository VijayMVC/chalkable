REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AnnouncementReminderService');
REQUIRE('chlk.services.AppMarketService');

REQUIRE('chlk.activities.announcement.AdminAnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');
REQUIRE('chlk.activities.apps.AttachAppDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');

REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.Reminder');
REQUIRE('chlk.models.announcement.LastMessages');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.apps.InstalledAppsViewData');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ReminderId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.announcement.QnAForm');
REQUIRE('chlk.models.common.attachments.BaseAttachmentViewData');



NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AttachmentTypeEnum */
    ENUM('AttachmentTypeEnum', {
        DOCUMENT: 0,
        PICTURE: 1
    });

    /** @class chlk.controllers.AnnouncementController */
    CLASS(
        'AnnouncementController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.AnnouncementReminderService, 'announcementReminderService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.PersonService, 'personService',

        [ria.mvc.Inject],
        chlk.services.GradingService, 'gradingService',

        [ria.mvc.Inject],
        chlk.services.AppMarketService, 'appMarketService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',

        function getAnnouncementFormPageType_(){
            if(this.userIsAdmin())
                return chlk.activities.announcement.AdminAnnouncementFormPage;
            if(this.userInRole(chlk.models.common.RoleEnum.TEACHER))
                return chlk.activities.announcement.AnnouncementFormPage;
        },

        [[chlk.models.announcement.Reminder]],
        function editAddReminderAction(model) {
            if(model.isDuplicate()){
                this.ShowMsgBox('This reminder was added before!.', 'fyi.', [{
                    text: Msg.GOT_IT.toUpperCase()
                }])
            }else{
                var result, before = model.getBefore();
                if(model.getId().valueOf()){
                    this.announcementReminderService.editReminder(model.getId(), before);
                }else{
                    result = this.announcementReminderService.addReminder(model.getAnnouncementId(), before)
                        .then(function(announcement){
                            var reminders = announcement.getAnnouncementReminders(), res;
                            res = reminders.filter(function(item){
                                return item.getBefore() == before;
                            });
                            return res[0];
                        });
                    return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
                }
            }
        },

        [[chlk.models.id.ReminderId]],
        function removeReminderAction(announcementReminderId) {
            this.announcementReminderService.deleteReminder(announcementReminderId);
        },

        [[ArrayOf(chlk.models.attachment.Attachment)]],
        function prepareAttachments(attachments){
            attachments.forEach(function(item){
                if(item.getType() == chlk.controllers.AttachmentTypeEnum.PICTURE.valueOf())
                    item.setThumbnailUrl(this.announcementService.getAttachmentUri(item.getId(), false, 170, 110));
            }, this);
        },

        [[chlk.models.announcement.StudentAnnouncement]],
        function updateAnnouncementGradeAction(model){
            var result = this.gradingService.updateItem(model.getId(), model.getGradeValue(), model.getComment(), model.isDropped());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.announcement.AnnouncementForm, Boolean]],
        function addEditAction(model, isEdit){

            var announcement = model.getAnnouncement();
            var reminders = announcement.getAnnouncementReminders() || [];
            var remindersArray = [];

            reminders.forEach(function(item){
                remindersArray.push(item.getBefore());
            });
            model.setReminders(remindersArray);

            if(this.userIsAdmin()){
                this.prepareRecipientsData(model);
            }
            else{
                var classes = this.classService.getClassesForTopBar();
                var classId_ = announcement.getClassId();

                var classesBarData = new chlk.models.classes.ClassesForTopBar(
                    classes,
                    classId_,
                    isEdit
                );

                if(classId_){
                    var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                    model.setClassInfo(classInfo);
                }
                var announcementTypeId_ = announcement.getAnnouncementTypeId();
                if(announcementTypeId_){
                    if(classId_ && classInfo){
                        var types = classInfo.getTypesByClass();
                        var typeId = null;

                        types.forEach(function(item){
                            if(item.getId() == announcementTypeId_)
                                typeId = announcementTypeId_;
                        });
                        if (typeId)
                            model.setSelectedTypeId(typeId);
                        else
                            if(announcementTypeId_){
                                announcementTypeId_ = types[0].getId();
                                announcement.setAnnouncementTypeId(announcementTypeId_);
                                model.setSelectedTypeId(announcementTypeId_);
                            }
                    }
                }
                model.setTopData(classesBarData);
            }

            var attachments = announcement.getAnnouncementAttachments() || [];
            this.prepareAttachments(attachments);
            this.getContext().getSession().set('AnnouncementAttachments', attachments);

            var applications = announcement.getApplications() || [];
            this.getContext().getSession().set('AnnouncementApplications', applications);

            var attachmentsIds = attachments.map(function(item){
                return item.id
            }).join(',');
            announcement.setAttachments(attachmentsIds);
            return new ria.async.DeferredData(model);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, Boolean]],
        function addAction(classId_, announcementTypeId_, date_, noDraft_) {
            this.disableAnnouncementSaving(false);
            this.getView().reset();
            if(classId_ && announcementTypeId_){
                var classInfo = this.classService.getClassAnnouncementInfo(classId_);
                var types = classInfo.getTypesByClass();
                var typeId = null;

                types.forEach(function(item){
                    if(item.getId() == announcementTypeId_)
                        typeId = announcementTypeId_;
                });
                if (!typeId)
                    announcementTypeId_ = types[0].getId();
            }
            var result = this.announcementService
                .addAnnouncement(classId_, announcementTypeId_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var announcement = model.getAnnouncement();
                    if(noDraft_){
                        announcement.setClassId(classId_ || null);
                        announcement.setAnnouncementTypeId(announcementTypeId_ || null);
                    }
                    if(date_){
                        announcement.setExpiresDate(date_);
                    }
                    return this.addEditAction(model, false);
                }.bind(this));
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId]],
        function attachAppAction(announcementId) {
            var userId = this.getCurrentPerson().getId();
            var result = this.appMarketService.getInstalledApps(userId)
                .then(function(data){
                    return new chlk.models.apps.InstalledAppsViewData(userId, announcementId, data);
                })
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.apps.AttachAppDialog, result);
        },

        //todo: join with attachAppAction
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [[chlk.models.id.SchoolPersonId, chlk.models.id.AnnouncementId, Number]],
        function listAvailableForAttachPageTeacherAction(teacherId, announcementId, pageIndex_) {
            var userId = this.getCurrentPerson().getId();
            var result = this.appMarketService
                .getInstalledApps(teacherId, pageIndex_)
                .then(function(data){
                    return new chlk.models.apps.InstalledAppsViewData(userId, announcementId, data);
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.apps.AttachAppDialog, result);
        },

        [[chlk.models.id.AnnouncementId]],
        function editAction(announcementId) {
            var result = this.announcementService
                .editAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.addEditAction(model, true);
                }.bind(this));
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },

        [[chlk.models.id.AnnouncementId]],
        function viewAction(announcementId) {
            this.getView().reset();
            var result = this.announcementService
                .getAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(announcement){
                    var attachments = announcement.getAnnouncementAttachments();
                    this.prepareAttachments(attachments);
                    this.getContext().getSession().set('AnnouncementAttachments', attachments);
                    var apps = announcement.getApplications() || [];
                    var gradeViewApps = apps.filter(function(app){
                        return app.getAppAccess().isVisibleInGradingView();
                    }) || [];
                    announcement.setGradeViewApps(gradeViewApps);
                    announcement.prepareExpiresDateText();
                    announcement.setCurrentUser(this.getCurrentPerson());
                    return new ria.async.DeferredData(announcement);
                }, this);

            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        [[chlk.models.id.AnnouncementId, Object]],
        function uploadAttachmentAction(announcementId, files) {
            var result = this.announcementService
                .uploadAttachment(announcementId, files)
                .then(function(model){
                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    var attachments = model.getAnnouncementAttachments();
                    this.prepareAttachments(attachments);
                    return model;
                }.bind(this));
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'update-attachments');
        },


        [[chlk.models.id.AnnouncementAttachmentId]],
        function viewAttachmentAction(attachmentId){
            var attachments = this.getContext().getSession().get('AnnouncementAttachments') || [];
            attachments = attachments.filter(function(item){
                return item.getId() == attachmentId;
            });

            if (attachments.length == 1){
                var attachmentUrl = attachments[0].getUrl();
                var downloadAttachmentButton = new chlk.models.common.attachments.ToolbarButton(
                    "download-attachment",
                    "Download Attachment",
                    "/AnnouncementAttachment/DownloadAttachment.json?needsDownload=true&announcementAttachmentId=" + attachments[0].getId().valueOf()
                );
                var attachmentViewData = new chlk.models.common.attachments.BaseAttachmentViewData(attachmentUrl, [downloadAttachmentButton]);
                return this.ShadeView(chlk.activities.common.attachments.AttachmentDialog, new ria.async.DeferredData(attachmentViewData));
            }
        },

        [[chlk.models.announcement.Announcement]],
        function addAppAttachmentAction(announcement) {
            announcement.setNeedButtons(true);
            announcement.setNeedDeleteButton(true);
            var attachments = announcement.getAnnouncementAttachments() || [];
            this.prepareAttachments(attachments);
            return this.UpdateView(this.getAnnouncementFormPageType_(), new ria.async.DeferredData(announcement));
        },

        [[chlk.models.id.AnnouncementId, String]],
        function deleteAction(announcementId, typeName) {
            this.disableAnnouncementSaving(true);
            this.ShowMsgBox('You are about to delete this item.\n'+
                    'All grades and attachments for this ' + typeName + ' will\n' +
                    'be gone forever.\n' +
                    'Are you sure?', 'whoa.', [{
                text: "Cancel",
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }, {
                text: 'Delete',
                controller: 'announcement',
                action: 'deleteAnnouncement',
                params: [announcementId.valueOf()],
                color: chlk.models.common.ButtonColor.RED.valueOf()
            }]);
        },

        [[chlk.models.id.AnnouncementId]],
        function deleteAnnouncementAction(announcementId) {
            this.announcementService
                .deleteAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.redirect_('feed', 'list', []);
                }.bind(this));
        },

        [[chlk.models.id.SchoolPersonId]],
        function discardAction(schoolPersonId) {
            this.disableAnnouncementSaving(true);
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
                    var announcementForm = new chlk.models.announcement.AnnouncementForm();
                    announcementForm.setAnnouncement(model);
                    return this.addEditAction(announcementForm, true);
                }.bind(this));
            return this.UpdateView(this.getAnnouncementFormPageType_(), result);
        },

        [[chlk.models.id.AnnouncementApplicationId]],
        function deleteAppAction(announcementAppId) {
            var result = this.announcementService
                .deleteApp(announcementAppId)
                .attach(this.validateResponse_())
                .then(function(model){
                    var announcementForm = new chlk.models.announcement.AnnouncementForm();
                    announcementForm.setAnnouncement(model);
                    return this.addEditAction(announcementForm, true);
                }.bind(this));
            return this.UpdateView(this.getAnnouncementFormPageType_(), result);
        },


        Boolean, function isAnnouncementSavingDisabled(){
            return this.getContext().getSession().get('noSave', false);
        },

        [[Boolean]],
        function disableAnnouncementSaving(val){
            this.getContext().getSession().set('noSave', val);
        },



        /*[[chlk.models.announcement.Announcement, Array]],
        function saveAdminAction(model, recipients_){
            var res = this.announcementService
                .saveAdminAnnouncement(
                    model.getId(),
                    recipients_,
                    model.getSubject(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getAttachments()
                )
                .attach(this.validateResponse_());

        },*/


        //TODO refactor
        [[chlk.models.announcement.Announcement]],
        function saveAction(model) {
            if(!this.isAnnouncementSavingDisabled()){
                this.disableAnnouncementSaving(false);
                var session = this.getContext().getSession();
                var result;
                var submitType = model.getSubmitType();
                var schoolPersonId = model.getPersonId();
                var announcementTypeId = model.getAnnouncementTypeId();
                var announcementTypeName = model.getAnnouncementTypeName();
                var classId = model.getClassId();
                model.setMarkingPeriodId(session.get('markingPeriod').getId());
                if(submitType == 'listLast'){
                    if(!this.userIsAdmin()){
                        result = this.announcementService
                            .listLast(classId, announcementTypeId,schoolPersonId)
                            .attach(this.validateResponse_())
                            .then(function(data){
                                var model = new chlk.models.announcement.LastMessages();
                                model.setItems(data);
                                model.setAnnouncementTypeName(announcementTypeName);
                                return new ria.async.DeferredData(model);
                            }.bind(this));
                        return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
                    }
                }else{
                    if(submitType == 'save'){
                        model.setAnnouncementAttachments(this.getContext().getSession().get('AnnouncementAttachments'));
                        model.setApplications(this.getContext().getSession().get('AnnoucementApplications'));
                        var announcementForm = new chlk.models.announcement.AnnouncementForm();
                        announcementForm.setAnnouncement(model);
                        result = this.addEditAction(announcementForm, false);
                        this.saveAnnouncement(model);
                        return this.UpdateView(this.getAnnouncementFormPageType_(), result);
                    }else{
                        if(submitType == 'saveNoUpdate'){
                            this.saveAnnouncement(model);
                        }else{
                        //TODO nextMarkingPeriod

                            if(!this.userInRole(chlk.models.common.RoleEnum.ADMINEDIT) && !this.userInRole(chlk.models.common.RoleEnum.ADMINVIEW)
                                && session.get('finalizedClassesIds').indexOf(classId.valueOf()) > -1){
                                    var nextMp = model.setMarkingPeriodId(session.get('nextMarkingPeriod'));
                                    if(nextMp){
                                        this.submitAnnouncement(model);
                                        return this.ShadeLoader();
                                    }
                            }else{
                                this.submitAnnouncement(model);
                                return this.ShadeLoader();
                            }
                        }
                    }
                }
            }
        },

        [[chlk.models.announcement.Announcement]],
        VOID, function saveAnnouncement(model){
            if(this.userIsAdmin())
                this.announcementService.saveAdminAnnouncement(
                    model.getId(),
                    model.getAnnRecipients(),
                    model.getSubject(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getAttachments()
                );
            else
                this.announcementService.saveAnnouncement(
                    model.getId(),
                    model.getClassId(),
                    model.getAnnouncementTypeId(),
                    model.getSubject(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getAttachments(),
                    model.getApplications(),
                    model.getMarkingPeriodId()
                );
        },

        [[chlk.models.announcement.Announcement]],
        function submitAnnouncement(model){
            var res;
            if(this.userIsAdmin())
                res = this.announcementService.submitAdminAnnouncement(
                    model.getId(),
                    model.getAnnRecipients(),
                    model.getSubject(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getAttachments()
                );
            else
                res = this.announcementService.submitAnnouncement(
                    model.getId(),
                    model.getClassId(),
                    model.getAnnouncementTypeId(),
                    model.getSubject(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getAttachments(),
                    model.getApplications(),
                    model.getMarkingPeriodId()
                );
            res.then(function(){
                this.redirect_('feed', 'list', []);
            }.bind(this));
        },

        [[chlk.models.id.AnnouncementId, Boolean]],
        function starAction(id, starred_)
        {
            this.announcementService.star(id, starred_);
            return;
        },

        [[chlk.models.announcement.QnAForm]],
        function askQuestionAction(model) {
            var ann = this.announcementService.askQuestion(model.getAnnouncementId(), model.getQuestion());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, ann, 'update-qna');
        },

        [[chlk.models.announcement.QnAForm]],
        function answerQuestionAction(model) {
            var ann;
            if (model.getQuestion())
                ann = this.announcementService.answerQuestion(model.getId(), model.getQuestion(), model.getAnswer());
            else
                ann = this.announcementService.deleteQnA(model.getAnnouncementId(), model.getId());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, ann, 'update-qna');
        },



        //TODO: refactor
        [[chlk.models.announcement.AnnouncementForm]],
        function prepareRecipientsData(model){
            var rolesEnum = chlk.models.common.RoleEnum,
                nameIdModel = chlk.models.common.NameId,
                studentsId = rolesEnum.STUDENT.valueOf(),
                teachersId = rolesEnum.TEACHER.valueOf(),
                gradeLevels = this.gradeLevelService.getGradeLevelsForTopBar(),
                studentsData = [new nameIdModel('0|' + studentsId + '|-1|-1', Msg.All_students)],
                teachersData = [new nameIdModel('0|' + teachersId + '|-1|-1', Msg.All_teachers)];
            gradeLevels.forEach(function(item){
                studentsData.push(new nameIdModel('0|' + studentsId + '|' + item.getId().valueOf() + '|-1', Msg.Student(true) + ' - ' + item.getFullText()));
                teachersData.push(new nameIdModel('0|' + teachersId + '|' + item.getId().valueOf() + '|-1', Msg.Teacher(true) + ' - ' + item.getFullText()));
            });
            model.setAdminRecipientId('0|' + rolesEnum.ADMINEDIT.valueOf() + '|-1|-1,0|'
                + rolesEnum.ADMINGRADE.valueOf() + '|-1|-1,0|'
                + rolesEnum.ADMINVIEW.valueOf() + '|-1|-1');
            var recipientsData = {};
            recipientsData[studentsId] = studentsData;
            recipientsData[teachersId] = teachersData;
            model.setAdminRecipients(new chlk.models.announcement.AdminRecipients([], recipientsData));
         }
    ])
});
