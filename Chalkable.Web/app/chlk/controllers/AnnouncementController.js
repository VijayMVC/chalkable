REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AnnouncementReminderService');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.StandardService');

REQUIRE('chlk.activities.announcement.AdminAnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');
REQUIRE('chlk.activities.apps.AttachAppDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');
REQUIRE('chlk.activities.announcement.AddStandardsDialog');
REQUIRE('chlk.activities.announcement.AddDuplicateAnnouncementDialog');

REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.Reminder');
REQUIRE('chlk.models.announcement.LastMessages');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.apps.InstalledAppsViewData');
REQUIRE('chlk.models.announcement.ShowGradesToStudents');
REQUIRE('chlk.models.standard.Standard');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.ReminderId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.announcement.QnAForm');
REQUIRE('chlk.models.common.attachments.BaseAttachmentViewData');
REQUIRE('chlk.models.announcement.AddStandardViewData');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.standard.StandardsListViewData');
REQUIRE('chlk.models.announcement.AddDuplicateAnnouncementViewData');

REQUIRE('chlk.lib.exception.AppErrorException');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.AttachmentTypeEnum */
    ENUM('AttachmentTypeEnum', {
        DOCUMENT: 0,
        PICTURE: 1,
        OTHER: 2
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

        [ria.mvc.Inject],
        chlk.services.StandardService, 'standardService',

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
                var before = model.getBefore();
                if(model.getId().valueOf()){
                    return this.announcementReminderService
                        .editReminder(model.getId(), before)
                        .attach(this.validateResponse_());
                }else{
                    var result = this.announcementReminderService
                        .addReminder(model.getAnnouncementId(), before)
                        .attach(this.validateResponse_())
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
            this.announcementReminderService
                .deleteReminder(announcementReminderId)
                .attach(this.validateResponse_());
        },

        [[chlk.models.announcement.ShowGradesToStudents]],
        function setShowGradesToStudentsAction(model) {
            this.announcementService
                .setShowGradesToStudents(model.getAnnouncementId(), model.isShowToStudents())
                .attach(this.validateResponse_());
        },

        function prepareAttachment(attachment){
            if(attachment.getType() == chlk.controllers.AttachmentTypeEnum.PICTURE.valueOf()){
                attachment.setThumbnailUrl(this.announcementService.getAttachmentUri(attachment.getId(), false, 170, 110));
                attachment.setUrl(this.announcementService.getAttachmentUri(attachment.getId(), false, null, null));
            }
            if(attachment.getType() == chlk.controllers.AttachmentTypeEnum.OTHER.valueOf()){
                attachment.setUrl(this.announcementService.getAttachmentUri(attachment.getId(), true, null, null));
            }
        },

        [[chlk.models.announcement.Announcement]],
        function prepareAttachments(announcement){
            var attachments = announcement.getAnnouncementAttachments() || [], ids = [];
            attachments.forEach(function(item){
                ids.push(item.getId().valueOf());
                this.prepareAttachment(item);
            }, this);
            announcement.setAttachments(ids.join(','));
            this.cacheAnnouncementAttachments(attachments);
        },

        [[chlk.models.announcement.StudentAnnouncement, Boolean]],
        function setAnnouncementGrade(studentAnnouncement, fromGrid_){
            var result = this.gradingService
                .updateItem(
                    studentAnnouncement.getAnnouncementId(),
                    studentAnnouncement.getStudentId(),
                    studentAnnouncement.getGradeValue(),
                    studentAnnouncement.getComment(),
                    studentAnnouncement.isDropped(),
                    studentAnnouncement.isLate(),
                    studentAnnouncement.isAbsent(),
                    studentAnnouncement.isIncomplete(),
                    studentAnnouncement.isExempt(),
                    fromGrid_ ? chlk.models.announcement.ShortStudentAnnouncementViewData : null
                )
                .attach(this.validateResponse_());
            return result;
        },

        [chlk.controllers.SidebarButton('statistic')],
        [[chlk.models.announcement.StudentAnnouncement]],
        function updateAnnouncementGradeFromGridAction(model){
            var result = this.setAnnouncementGrade(model, true);
            return this.UpdateView(chlk.activities.grading.GradingClassSummaryGridPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.announcement.StudentAnnouncement]],
        function updateAnnouncementGradeAction(model){
            var result = this.setAnnouncementGrade(model)
                .then(function(currentItem){
                    var announcement = this.getCachedAnnouncement();
                    announcement.getStudentAnnouncements().getItems().forEach(function(item){
                        if(item.getStudentId() == currentItem.getStudentId()){
                            item.setGradeValue(currentItem.getGradeValue());
                            item.setNumericGradeValue(currentItem.getNumericGradeValue());
                            item.setAbsent(currentItem.isAbsent());
                            item.setExempt(currentItem.isExempt());
                            item.setIncomplete(currentItem.isIncomplete());
                            item.setLate(currentItem.isLate());
                            item.setDropped(currentItem.isDropped());
                            item.setIncludeInAverage(currentItem.isIncludeInAverage());
                        }
                    });
                    announcement.calculateGradesAvg();
                    this.cacheAnnouncement(announcement);
                    return new chlk.activities.announcement.UpdateAnnouncementItemViewModel(announcement, currentItem);
            }, this);
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.announcement.AnnouncementForm, Boolean]],
        function addEditAction(model, isEdit){
            this.disableAnnouncementSaving(false);
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
                var classes = this.classService.getClassesForTopBar(false, true);
                var classId_ = announcement.getClassId(), classInfo, types;
                classes.forEach(function(item){
                    var currentClassInfo = this.classService.getClassAnnouncementInfo(item.getId());
                    types = currentClassInfo ? currentClassInfo.getTypesByClass() : [];
                    if(types.length)
                        item.setDefaultAnnouncementTypeId(types[0].getId());
                    if(currentClassInfo && classId_ && classId_ == item.getId()){
                        classInfo = currentClassInfo;
                        model.setClassInfo(classInfo);
                    }
                }, this);

                var classesBarData = new chlk.models.classes.ClassesForTopBar(
                    classes,
                    classId_,
                    isEdit
                );

                var announcementTypeId_ = announcement.getAnnouncementTypeId();
                if(announcementTypeId_){
                    if(classId_ && classInfo){
                        types = classInfo.getTypesByClass();
                        if(types.length > 0){
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
                }
                model.setTopData(classesBarData);
            }

            this.prepareAttachments(announcement);

            var applications = announcement.getApplications() || [];
            this.cacheAnnouncementApplications(applications);

            var applicationsIds = applications.map(function(item){
                return item.getId().valueOf()
            }).join(',');
            announcement.setApplicationsIds(applicationsIds);

            this.saveStandardIds(announcement);

            return model;
        },

        VOID, function saveStandardIds(announcement){
            var standardsIds = [], standards = announcement.getStandards() || [];
            standards.forEach(function(item){
                standardsIds.push(item.getStandardId().valueOf());
            });

            this.getContext().getSession().set(ChlkSessionConstants.STANDARD_IDS, standardsIds);
        },

        [[chlk.models.common.ChlkDate, Boolean, chlk.models.id.ClassId]],
        function addViaCalendarAction(date_, noDraft_, classId_){
            return this.addAction(classId_, null, date_, noDraft_);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, Boolean]],
        function addAction(classId_, announcementTypeId_, date_, noDraft_) {
            this.getView().reset();
            var classes = this.classService.getClassesForTopBar(false, true);
            var classesBarData = new chlk.models.classes.ClassesForTopBar(classes), p = false;
            classes.forEach(function(item){
                if(!p && item.getId() == classId_)
                    p = true;
            });
            if(!p)
                classId_ = null;
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
                .catchError(function(error){
                    if(error.getStatus && error.getStatus() == 500){
                        var res = JSON.parse(error.getResponse());
                        if(res.exceptiontype == 'NoClassAnnouncementTypeException')
                            return this.redirectToErrorPage_(error.toString(), 'error', 'createAnnouncementError', []);
                    }
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    if(model && model.getAnnouncement()){
                        var announcement = model.getAnnouncement();
                        if(noDraft_){
                            announcement.setClassId(classId_ || null);
                            announcement.setAnnouncementTypeId(announcementTypeId_ || null);
                        }
                        if(!announcement.getClassId() || !announcement.getClassId().valueOf())
                            announcement.setAnnouncementTypeId(null);
                        if(date_){
                            announcement.setExpiresDate(date_);
                        }
                        return this.addEditAction(model, false);
                    }
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true);
                },this);
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, Number]],
        function attachAppAction(announcementId, pageIndex_) {
            var userId = this.getCurrentPerson().getId();

            var result = this.appMarketService
                .getInstalledApps(userId, pageIndex_ | 0, null, null, true)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.apps.InstalledAppsViewData(userId, announcementId, data);
                });
            if (pageIndex_ || pageIndex_ == 0)
                return this.UpdateView(chlk.activities.apps.AttachAppDialog, result);
            else
                return this.ShadeView(chlk.activities.apps.AttachAppDialog, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [[chlk.models.id.AnnouncementId]],
        function editAction(announcementId) {
            var result = this.announcementService
                .editAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.addEditAction(model, true);
                }, this);
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },

        [[chlk.models.id.AnnouncementId]],
        function viewAction(announcementId) {
            this.getView().reset();
            var result = this.announcementService
                .getAnnouncement(announcementId)

                .attach(this.validateResponse_())
                .then(function(announcement){
                    this.prepareAttachments(announcement);
                    var studentAnnouncements = announcement.getStudentAnnouncements();
                    if(studentAnnouncements){
                        var studentItems = studentAnnouncements.getItems(), that = this;
                        studentItems.forEach(function(item){
                            item.getAttachments().forEach(function(attachment){
                                that.prepareAttachment(attachment);
                            });
                        });
                    }
                    if(!this.userInRole(chlk.models.common.RoleEnum.STUDENT)){
                        var classInfo = this.classService.getClassAnnouncementInfo(announcement.getClassId());
                        var alphaGrades = classInfo ? classInfo.getAlphaGrades() : [];
                        var alternateScores = this.getContext().getSession().get(ChlkSessionConstants.ALTERNATE_SCORES, []);
                        announcement.setAlphaGrades(alphaGrades);
                        announcement.setAlternateScores(alternateScores);
                    }
                    var apps = announcement.getApplications() || [];
                    var gradeViewApps = apps.filter(function(app){
                        return app.getAppAccess().isVisibleInGradingView();
                    }) || [];
                    announcement.setGradeViewApps(gradeViewApps);
                    announcement.prepareExpiresDateText();
                    announcement.setCurrentUser(this.getCurrentPerson());
                    var hasMCPermission = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM) ||
                        this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
                    if(!hasMCPermission){
                        announcement.setAbleToGrade(false);
                    }
                    announcement.setAbleEdit(announcement.isAnnOwner() && hasMCPermission);
//                    announcement.setAbleChangeDate(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.CHANGE_ACTIVITY_DATES));
                    announcement.calculateGradesAvg();
                    var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
                    if(studentAnnouncements)
                        announcement.getStudentAnnouncements().setSchoolOptions(schoolOptions);

                    this.cacheAnnouncement(announcement);
                    return announcement;
                }, this)
                //.catchError(this.handleNoAnnouncementException_, this);
            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        function handleNoAnnouncementException_(error){
            if(error.getStatus && error.getStatus() == 500){
                var res = JSON.parse(error.getResponse());
                if(res.exceptiontype == 'NoAnnouncementException')
                    return this.redirectToErrorPage_(error.toString(), 'error', 'viewAnnouncementError', []);
            }
        },

        [[chlk.models.id.AnnouncementId, Object]],
        function uploadAttachmentAction(announcementId, files) {
            var result = this.announcementService
                .uploadAttachment(announcementId, files)
                .attach(this.validateResponse_())
                .then(function(announcement){
                    announcement.setNeedButtons(true);
                    announcement.setNeedDeleteButton(true);
                    this.prepareAttachments(announcement);
                    this.cacheAnnouncement(announcement);
                    return announcement;
                }, this);
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId]],
        function applyAutoGradeAction(announcementId){
            var result = this.gradingService
                .applyAutoGrade(announcementId)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId]],
        function gradeManuallyAction(announcementId){
            var result = this.gradingService
                .applyManualGrade(announcementId)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId]],
        function applyManualGradeAction(announcementId){
            return this.ShowMsgBox(Msg.You_ll_be_grading_by_hand, Msg.Just_checking, [{
                text: Msg.Grade_manually.toUpperCase(),
                controller: 'announcement',
                action: 'gradeManually',
                params: [announcementId.valueOf()],
                color: chlk.models.common.ButtonColor.RED.valueOf()
            }, {
                text: Msg.Cancel.toUpperCase(),
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }]);
        },

        [[chlk.models.id.AnnouncementAttachmentId]],
        function viewAttachmentAction(attachmentId){
            var attachments = this.getCachedAnnouncementAttachments();
            attachments = attachments.filter(function(item){
                return item.getId() == attachmentId;
            });

            if (attachments.length == 1){
                var attachmentUrl, res;
                var downloadAttachmentButton = new chlk.models.common.attachments.ToolbarButton(
                    "download-attachment",
                    "Download Attachment",
                    "/AnnouncementAttachment/DownloadAttachment.json?needsDownload=true&announcementAttachmentId=" + attachments[0].getId().valueOf()
                );
                if(attachments[0].getType() == chlk.models.announcement.ApplicationOrAttachmentEnum.PICTURE.valueOf()){
                    attachmentUrl = attachments[0].getUrl();
                    var attachmentViewData = new chlk.models.common.attachments.BaseAttachmentViewData(
                        attachmentUrl,
                        [downloadAttachmentButton],
                        attachments[0].getType()
                    );
                    res = new ria.async.DeferredData(attachmentViewData);
                }else{
                    res = this.announcementService
                        .startViewSession(attachmentId)
                        .then(function(session){
                            attachmentUrl = 'https://crocodoc.com/view/' + session;
                            return new chlk.models.common.attachments.BaseAttachmentViewData(
                                attachmentUrl,
                                [downloadAttachmentButton],
                                attachments[0].getType()
                            );
                        });
                }
                return this.ShadeView(chlk.activities.common.attachments.AttachmentDialog, res);
            }
            return null;
        },

        [[chlk.models.announcement.Announcement]],
        function addAppAttachmentAction(announcement) {
            announcement.setNeedButtons(true);
            announcement.setNeedDeleteButton(true);
            this.prepareAttachments(announcement);
            return this.UpdateView(this.getAnnouncementFormPageType_(), new ria.async.DeferredData(announcement), 'update-attachments');
        },

        function cancelDeleteAction(){
            this.disableAnnouncementSaving(false);
            return null;
        },

        [[chlk.models.id.AnnouncementId, String]],
        function deleteAction(announcementId, typeName) {
            this.disableAnnouncementSaving(true);
            this.ShowMsgBox('You are about to delete this item.\n'+ 'All attachments for this ' + typeName + ' will be gone forever.\n' +
                    'Are you sure?', 'whoa.', [{
                text: "Cancel",
                controller: 'announcement',
                action: 'cancelDelete',
                params:[],
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }, {
                text: 'Delete',
                controller: 'announcement',
                action: 'deleteAnnouncement',
                params: [announcementId.valueOf()],
                color: chlk.models.common.ButtonColor.RED.valueOf()
            }]);
            return null;
        },

        [[chlk.models.id.AnnouncementId]],
        function deleteAnnouncementAction(announcementId) {
            return this.announcementService
                .deleteAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    if(!this.userIsAdmin())
                        chlk.controls.updateWeekCalendar();
                    return this.BackgroundNavigate('feed', 'list', [null, true]);
                }, this);
        },

        function discardAction() {
            this.disableAnnouncementSaving(true);
            var currentPersonId = this.getCurrentPerson().getId();
            return this.announcementService
                .deleteDrafts(currentPersonId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.BackgroundNavigate('feed', 'list', [null, true]);
                }, this);
        },

        [[chlk.models.id.AttachmentId]],
        function deleteAttachmentAction(attachmentId) {
            var result = this.announcementService
                .deleteAttachment(attachmentId)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    this.prepareAttachments(model);
                    return model;
                }, this);
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementApplicationId]],
        function deleteAppAction(announcementAppId) {
            var result = this.announcementService
                .deleteApp(announcementAppId)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    var announcementForm = new chlk.models.announcement.AnnouncementForm();
                    announcementForm.setAnnouncement(model);
                    return this.addEditAction(announcementForm, true).getAnnouncement();
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, 'update-attachments');
        },

        Boolean, function isAnnouncementSavingDisabled(){
            return this.getContext().getSession().get(ChlkSessionConstants.DONT_SAVE, false);
        },

        [[Boolean]],
        function disableAnnouncementSaving(val){
            this.getContext().getSession().set(ChlkSessionConstants.DONT_SAVE, val);
        },


        [[chlk.models.announcement.Announcement]],
        function cacheAnnouncement(announcement){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT, announcement);
        },

        chlk.models.announcement.Announcement, function getCachedAnnouncement(){
             return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT, new chlk.models.announcement.Announcement());
        },


        [[ArrayOf(chlk.models.attachment.Attachment)]],
        function cacheAnnouncementAttachments(attachments){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_ATTACHMENTS, attachments);
        },

        function getCachedAnnouncementAttachments(){
            return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_ATTACHMENTS) || [];
        },

        function getCachedAnnouncementApplications(){
            return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_APPLICATIONS) || [];
        },


        [[ArrayOf(chlk.models.apps.AppAttachment)]],
        function cacheAnnouncementApplications(applications){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_APPLICATIONS, applications);
        },

        [[String, chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
        function listLastAction(announcementTypeName, classId, announcementTypeId, schoolPersonId){
            var result = this.announcementService
                .listLast(classId, announcementTypeId,schoolPersonId)
                .attach(this.validateResponse_())
                .then(function(msgs){
                    return chlk.models.announcement.LastMessages.$create(announcementTypeName, msgs);
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.id.AnnouncementId, String]],
        function saveTitleAction(announcementId, announcementTitle){
            var result = this.announcementService
                .editTitle(announcementId, announcementTitle)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.announcement.Announcement();
                });
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
        },

        [[String, chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function checkTitleAction(title, classId, expiresdate){
            var res = this.announcementService
                .existsTitle(title, classId, expiresdate)
                .attach(this.validateResponse_())
                .then(function(success){
                    return new chlk.models.Success(success);
                });
            return this.UpdateView(this.getAnnouncementFormPageType_(), res, chlk.activities.lib.DontShowLoader());
        },

        function showTitlePopUpAction(){
            return this.ShowMsgBox(Msg.Same_Title_Text, '', [{
                text: Msg.OK.toUpperCase(),
                controller: 'announcement',
                action: 'closeTitlePopUp',
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }]);
        },

        function closeTitlePopUpAction(){
            return this.UpdateView(this.getAnnouncementFormPageType_(), new ria.async.DeferredData(new chlk.models.Success(true)), 'title-popup');
        },

        [[chlk.models.announcement.Announcement]],
        function saveAnnouncementFormAction(announcement){
            announcement.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
            announcement.setApplications(this.getCachedAnnouncementApplications());
            var announcementForm = new chlk.models.announcement.AnnouncementForm();
            announcementForm.setAnnouncement(announcement);
            return this.saveAnnouncement(announcement, announcementForm);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.announcement.Announcement]],
        function saveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var result;
            var session = this.getContext().getSession();
            var submitType = model.getSubmitType();
            var schoolPersonId = model.getPersonId();
            var announcementTypeId = model.getAnnouncementTypeId();
            var announcementTypeName = model.getAnnouncementTypeName();
            var classId = model.getClassId();

            model.setMarkingPeriodId(this.getCurrentMarkingPeriod().getId());

            if (submitType == 'listLast' && !this.userIsAdmin()){
                return this.listLastAction(announcementTypeName, classId, announcementTypeId, schoolPersonId);
            }

            if (submitType == 'saveTitle'){
                return this.saveTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle'){
                return this.checkTitleAction(model.getTitle(), classId, model.getExpiresDate());
            }

            if (submitType == 'save'){
                return this.saveAnnouncementFormAction(model);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                this.saveAnnouncement(model);
                return null;
            }
            var classIdInFinalizedClassIds = session.get(ChlkSessionConstants.FINALIZED_CLASS_IDS).indexOf(classId.valueOf()) > -1;

            if( !this.userInRole(chlk.models.common.RoleEnum.ADMINEDIT) &&
                !this.userInRole(chlk.models.common.RoleEnum.ADMINVIEW) && classIdInFinalizedClassIds){
                var nextMp = model.setMarkingPeriod(this.getMextMarkingPeriod());
                if(nextMp && this.submitAnnouncement(model, submitType == 'submitOnEdit')){
                    return this.ShadeLoader();
                }
            }else{
                if(this.submitAnnouncement(model, submitType == 'submitOnEdit'))
                    return this.ShadeLoader();
            }
            return null;
        },

        //TODO: refactor
        [[chlk.models.announcement.Announcement, chlk.models.announcement.AnnouncementForm]],
        function saveAnnouncement(model, form_){
            if(this.userIsAdmin())
                this.announcementService
                    .saveAdminAnnouncement(
                        model.getId(),
                        model.getAnnRecipients(),
                        model.getTitle(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getAttachments()
                    )
                    .attach(this.validateResponse_());
            else{
                var res = this.announcementService
                    .saveAnnouncement(
                        model.getId(),
                        model.getClassId(),
                        model.getAnnouncementTypeId(),
                        model.getTitle(),
                        model.getContent(),
                        model.getExpiresDate(),
                        Array.isArray(model.getAttachments()) ? model.getAttachments().join(',') : model.getAttachments(),
                        Array.isArray(model.getApplications()) ? model.getApplications().join(',') : model.getApplications(),
                        model.getMarkingPeriodId(),
                        model.getMaxScore(),
                        model.getWeightAddition(),
                        model.getWeightMultiplier(),
                        model.isHiddenFromStudents(),
                        model.isAbleDropStudentScore()
                    )
                    .attach(this.validateResponse_());
                if(form_){
                    res = res.then(function(model){
                        form_.getAnnouncement().setTitle(model.getTitle());
                        form_.getAnnouncement().setCanAddStandard(model.isCanAddStandard());
                        form_.getAnnouncement().setStandards(model.getStandards());
                        form_.getAnnouncement().setGradable(model.isGradable());
                        return this.addEditAction(form_, false);
                    }, this);
                    return this.UpdateView(this.getAnnouncementFormPageType_(), res);
                }
            }
        },

        //TODO: REFACTOR
        [[chlk.models.announcement.Announcement, Boolean]],
        function submitAnnouncement(model, isEdit){
            var res;
            if(this.userIsAdmin())
                res = this.announcementService
                    .submitAdminAnnouncement(
                        model.getId(),
                        model.getAnnRecipients(),
                        model.getTitle(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getAttachments()
                    )
                    .attach(this.validateResponse_());
            else{
                var apps = this.getCachedAnnouncementApplications();
                res = this.announcementService
                    .submitAnnouncement(
                    model.getId(),
                    model.getClassId(),
                    model.getAnnouncementTypeId(),
                    model.getTitle(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getAttachments(),
                    model.getApplicationsIds(),
                    model.getMarkingPeriodId(),
                    model.getMaxScore(),
                    model.getWeightAddition(),
                    model.getWeightMultiplier(),
                    model.isHiddenFromStudents(),
                    model.isAbleDropStudentScore()
                )
                .attach(this.validateResponse_());
            }
            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    if(isEdit)
                        return this.BackgroundNavigate('announcement', 'view', [model.getId()]);
                    else{
                        if(!this.userIsAdmin())
                            chlk.controls.updateWeekCalendar();
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
            return res;
        },

        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function showDuplicateFormAction(announcementId, selectedClassId){
            var classes = this.classService.getClassesForTopBar(false, true);
            classes.forEach(function(item){
                item.setDisabled(true);
            });
            var addDupAnnModel = new chlk.models.announcement.AddDuplicateAnnouncementViewData(announcementId
                , classes, selectedClassId);
            var res = new ria.async.DeferredData(addDupAnnModel);
            return this.ShadeView(chlk.activities.announcement.AddDuplicateAnnouncementDialog, res);
        },

        [[chlk.models.announcement.AddDuplicateAnnouncementViewData]],
        function duplicateAction(model){
            var res = this.announcementService
                .duplicateAnnouncement(model.getAnnouncementId(), model.getSelectedIds())
                .attach(this.validateResponse_())
                .then(function(data){
                    return this.BackgroundNavigate('announcement', 'edit', [model.getAnnouncementId()]);
                }, this);
            return res;
        },

        [[chlk.models.id.AnnouncementId, Boolean]],
        function starAction(id, complete_){
            this.announcementService
                .checkItem(id, complete_)
                .attach(this.validateResponse_());
            return null;
        },

        [[chlk.models.id.AnnouncementId]],
        function makeVisibleAction(id){
            this.announcementService
                .makeVisible(id)
                .attach(this.validateResponse_());
            return null;
        },

        [[chlk.models.announcement.QnAForm]],
        function askQuestionAction(model) {
            var ann = this.announcementService
                .askQuestion(model.getAnnouncementId(), model.getQuestion())
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, ann, 'update-qna');
        },

        [[chlk.models.announcement.QnAForm]],
        function answerQuestionAction(model) {
            var ann;
            if (model.getQuestion()){
                var updateType = model.getUpdateType();
                if(updateType ==  "editQuestion"){
                    ann = this.announcementService
                        .editQuestion(model.getId(), model.getQuestion())
                        .catchError(this.handleNoAnnouncementException_, this)
                        .attach(this.validateResponse_());
                }
                if(updateType == "answer"){
                    ann = this.announcementService
                        .answerQuestion(model.getId(), model.getQuestion(), model.getAnswer())
                        .catchError(this.handleNoAnnouncementException_, this)
                        .attach(this.validateResponse_());
                }
                if(updateType == "editAnswer"){
                    ann = this.announcementService
                        .editAnswer(model.getId(),  model.getAnswer())
                        .catchError(this.handleNoAnnouncementException_, this)
                        .attach(this.validateResponse_());
                }
            }
            else
                ann = this.announcementService
                    .deleteQnA(model.getAnnouncementId(), model.getId())
                    .catchError(this.handleNoAnnouncementException_, this)
                    .attach(this.validateResponse_());
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
         },

        [chlk.controllers.SidebarButton('add-new')],
        [[String, chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function showStandardsAction(typeName, announcementId, classId){
            var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
            var res = this.standardService.getSubjects()
                .then(function(subjects){
                    return new chlk.models.announcement.AddStandardViewData(typeName, announcementId, classId, subjects, standardIds);
                })
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.announcement.AddStandardsDialog, res);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, String, chlk.models.id.StandardId]],
        function showStandardsByCategoryAction(classId, subjectId, description_, standardId_){
            var res = this.standardService.getStandards(classId, subjectId, standardId_)
                .then(function(standards){
                    return new chlk.models.standard.StandardsListViewData(description_, classId, subjectId, standards);
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.standard.Standard]],
        function addStandardsAction(model){
            var res = this.announcementService.addStandard(model.getAnnouncementId(), model.getStandardId())
                .then(function(announcement){
                    this.BackgroundCloseView(chlk.activities.announcement.AddStandardsDialog);
                    this.saveStandardIds(announcement);
                    return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, res);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
        function removeStandardAction(announcementId, standardId){
            var res = this.announcementService.removeStandard(announcementId, standardId)
                .then(function(announcement){
                    this.saveStandardIds(announcement);
                    return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, res);
        }
    ])
});
