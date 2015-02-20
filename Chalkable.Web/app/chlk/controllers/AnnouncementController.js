REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.StandardService');
REQUIRE('chlk.services.MarkingPeriodService');

REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');
REQUIRE('chlk.activities.apps.AttachAppDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');
REQUIRE('chlk.activities.announcement.AddStandardsDialog');
REQUIRE('chlk.activities.announcement.AddDuplicateAnnouncementDialog');

REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.LastMessages');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.apps.InstalledAppsViewData');
REQUIRE('chlk.models.announcement.ShowGradesToStudents');
REQUIRE('chlk.models.standard.Standard');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
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

        [ria.mvc.Inject],
        chlk.services.MarkingPeriodService, 'markingPeriodService',

        ArrayOf(chlk.models.attachment.Attachment), 'announcementAttachments',

        function getAnnouncementFormPageType_(){
            if(this.userInRole(chlk.models.common.RoleEnum.TEACHER))
                return chlk.activities.announcement.AnnouncementFormPage;
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

        [[chlk.models.id.AnnouncementApplicationId]],
        function discardAutoGradesAction(appId){
            //this.gradingService.discardAutoGrades(appId);
            return null;
        },

        [[chlk.models.id.AnnouncementApplicationId]],
        function applyAutoGradesAction(appId){
            //this.gradingService.applyAutoGrades(appId);
            return null;
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
            var redirectToNoCategoriesPage;

            var classes = this.classService.getClassesForTopBarSync();
            var markingPeriods = this.markingPeriodService.getMarkingPeriodsSync();
            var classId_ = announcement.getClassId(), classInfo, types;
            var announcementTypeId_ = announcement.getAnnouncementTypeId();
            var savedClassInfo = this.getContext().getSession().get('classInfo', null);
            classes.forEach(function (item) {
                var currentClassInfo = this.classService.getClassAnnouncementInfo(item.getId());
                types = currentClassInfo ? currentClassInfo.getTypesByClass() : [];
                if (types.length)
                    item.setDefaultAnnouncementTypeId(types[0].getId());
                if (currentClassInfo && classId_ && classId_ == item.getId()) {
                    classInfo = currentClassInfo;
                    model.setClassInfo(classInfo);
                    if (savedClassInfo && savedClassInfo.getClassId() == item.getId()) {
                        currentClassInfo = savedClassInfo;
                    } else {
                        var hasType = types.filter(function (type) {
                            return type.getId() == announcementTypeId_
                        }).length;
                        if (!hasType && announcement.getState() && currentClassInfo && announcementTypeId_) {
                            currentClassInfo.getTypesByClass().push(new chlk.models.announcement.ClassAnnouncementType(announcementTypeId_, announcement.getAnnouncementTypeName()));
                            this.getContext().getSession().set('classInfo', currentClassInfo);
                        }
                    }
                    if (currentClassInfo && !currentClassInfo.getTypesByClass().length)
                        throw new chlk.lib.exception.NoClassAnnouncementTypeException();
                    //this.redirectToPage_('error', 'createAnnouncementError', []);
                    //redirectToNoCategoriesPage = true;
                    //this.ShowMsgBox('There are no categories setup', '');

                    var classMarkingPeriods = this.classService.getMarkingPeriodRefsOfClass(classId_);
                    model.setClassScheduleDateRanges(
                        markingPeriods
                            .filter(function (mp) {
                                return classMarkingPeriods.indexOf(mp.getId()) > -1
                            })
                            .map(function (_) {
                                return {
                                    start: _.getStartDate().getDate(),
                                    end: _.getEndDate().getDate()
                                }
                            }));
                }

            }, this);

            //if(redirectToNoCategoriesPage)
            //return this.Redirect('error', 'createAnnouncementError', []);

            var classesBarData = new chlk.models.classes.ClassesForTopBar(
                classes,
                classId_,
                isEdit
            );
            if (announcementTypeId_) {
                if (classId_ && classInfo) {
                    types = classInfo.getTypesByClass();
                    if (types.length > 0) {
                        var typeId = null;

                        types.forEach(function (item) {
                            if (item.getId() == announcementTypeId_)
                                typeId = announcementTypeId_;
                        });
                        if (typeId)
                            model.setSelectedTypeId(typeId);
                        else if (announcementTypeId_) {
                            announcementTypeId_ = types[0].getId();
                            announcement.setAnnouncementTypeId(announcementTypeId_);
                            model.setSelectedTypeId(announcementTypeId_);
                        }
                    }
                }
            }
            model.setTopData(classesBarData);

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
            this.getContext().getSession().set('classInfo', null);
            var classes = this.classService.getClassesForTopBarSync();
            var classesBarData = new chlk.models.classes.ClassesForTopBar(classes), p = false;
            classes.forEach(function(item){
                if(!p && item.getId() == classId_)
                    p = true;
            });
            if(!p) classId_ = null;
            if(classId_ && announcementTypeId_ && announcementTypeId_.valueOf()){
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
                .addAnnouncement(classId_, announcementTypeId_, date_)
                .catchException(chlk.services.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
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
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, date_);
                },this)
                .attach(this.validateResponse_());
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },


        [chlk.controllers.StudyCenterEnabled()],
        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, String, Number]],
        function attachAppAction(announcementId, classId, appUrlAppend_, pageIndex_) {
            var userId = this.getCurrentPerson().getId();
            var mp = this.getCurrentMarkingPeriod();
            var result = this.appMarketService
                .getAppsForAttach(userId, classId, mp.getId(), pageIndex_ | 0, null)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.apps.InstalledAppsViewData(userId, announcementId, data, appUrlAppend_ || '');
                });

            return this.ShadeOrUpdateView(chlk.activities.apps.AttachAppDialog, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [[chlk.models.id.AnnouncementId]],
        function editAction(announcementId) {
            this.getContext().getSession().set('classInfo', null);
            var result = this.announcementService
                .editAnnouncement(announcementId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.addEditAction(model, true);
                }, this);
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },

        function prepareAnnouncementForView(announcement){
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
        },

        [[chlk.models.id.AnnouncementId]],
        function viewAction(announcementId) {
            this.getView().reset();
            var result = this.announcementService
                .getAnnouncement(announcementId)

                .attach(this.validateResponse_())
                .then(function(announcement){
                    return this.prepareAnnouncementForView(announcement);
                }, this);
                //.catchError(this.handleNoAnnouncementException_, this);
            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        function handleNoAnnouncementException_(error){
            if(error.getStatus && error.getStatus() == 500){
                var res = JSON.parse(error.getResponse());
                if(res.exceptiontype == 'NoAnnouncementException')
                    return this.redirectToErrorPage_(error.toString(), 'error', 'viewAnnouncementError', []);
            }
            throw error;
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
            }]), null;
        },

        [[chlk.models.id.AnnouncementAttachmentId, chlk.models.id.AnnouncementId]],
        function viewAttachmentAction(attachmentId, announcementId){
            var attachment = this.getCachedAnnouncementAttachments().filter(function(item){ return item.getId() == attachmentId; })[0];
            if (!attachment)
                return null;

            var attachmentUrl, res;
            var downloadAttachmentButton = new chlk.models.common.attachments.ToolbarButton(
                "download-attachment",
                "Download Attachment",
                "/AnnouncementAttachment/DownloadAttachment.json?needsDownload=true&announcementAttachmentId=" + attachment.getId().valueOf()
            );

            if(attachment.getType() == chlk.models.announcement.ApplicationOrAttachmentEnum.PICTURE.valueOf()){
                attachmentUrl = attachment.getUrl();
                var attachmentViewData = new chlk.models.common.attachments.BaseAttachmentViewData(
                    attachmentUrl,
                    [downloadAttachmentButton],
                    attachment.getType()
                );
                res = new ria.async.DeferredData(attachmentViewData);
            }else{
                var buttons = [downloadAttachmentButton];
                if(this.userInRole(chlk.models.common.RoleEnum.STUDENT) && attachment.isTeachersAttachment())
                    buttons.push(new chlk.models.common.attachments.ToolbarButton('mark-attachment', 'MARK UP', null, null,
                        'announcement', 'cloneAttachment', [attachment.getId().valueOf(), announcementId.valueOf()], true));
                res = this.announcementService
                    .startViewSession(attachmentId)
                    .then(function(session){
                        attachmentUrl = 'https://crocodoc.com/view/' + session;
                        return new chlk.models.common.attachments.BaseAttachmentViewData(
                            attachmentUrl,
                            buttons,
                            attachment.getType()
                        );
                    });
            }
            return this.ShadeView(chlk.activities.common.attachments.AttachmentDialog, res);
        },

        [[chlk.models.id.AnnouncementAttachmentId, chlk.models.id.AnnouncementId]],
        function cloneAttachmentAction(attachmentId, announcementId) {
            var res = this.announcementService
                .cloneAttachment(attachmentId, announcementId)
                .attach(this.validateResponse_())
                .then(function(announcement){
                    var attachments = this.getCachedAnnouncementAttachments().filter(function(item){return item.isOwner()});
                    var newAttachments = announcement.getAnnouncementAttachments().filter(function(item){return item.isOwner()});
                    var clone = newAttachments.filter(function(item){
                        var len = attachments.filter(function(attachment){
                            return attachment.getId() == item.getId()
                        }).length;
                        return !len;
                    })[0];
                    clone.setOpenOnStart(true);
                    return this.prepareAnnouncementForView(announcement);
                }, this);
            this.BackgroundCloseView(chlk.activities.common.attachments.AttachmentDialog);
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res);
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
                    return this.Redirect('feed', 'list', [null, true]);
                }, this);
        },

        function discardAction() {
            this.disableAnnouncementSaving(true);
            var currentPersonId = this.getCurrentPerson().getId();
            return this.announcementService
                .deleteDrafts(currentPersonId)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.Redirect('feed', 'list', [null, true]);
                }, this);
        },

        [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId]],
        function deleteAttachmentAction(attachmentId, announcementId) {
            var result = this.announcementService
                .deleteAttachment(attachmentId, announcementId)
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

        [[String, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.id.AnnouncementId]],
        function checkTitleAction(title, classId, expiresdate, annoId){
            var res = this.announcementService
                .existsTitle(title, classId, expiresdate, annoId)
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
            }]), null;
        },

        function closeTitlePopUpAction(){
            return this.UpdateView(this.getAnnouncementFormPageType_(), new ria.async.DeferredData(new chlk.models.Success(true)), 'title-popup');
        },

        [[chlk.models.announcement.Announcement]],
        function prepareAnnouncementForm_(announcement){
            announcement.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
            announcement.setApplications(this.getCachedAnnouncementApplications());
            return chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(announcement);
        },

        [[chlk.models.announcement.Announcement]],
        function saveAnnouncementFormAction(announcement){
            if(!announcement.getAnnouncementTypeId() || !announcement.getAnnouncementTypeId().valueOf())
                return this.Redirect('error', 'createAnnouncementError', []);
            var announcementForm = this.prepareAnnouncementForm_(announcement);
            return this.Redirect('announcement', 'saveAnnouncement', [announcement, announcementForm]);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.announcement.Announcement]],
        function saveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var session = this.getContext().getSession();
            var submitType = model.getSubmitType();
            var schoolPersonId = model.getPersonId();
            var announcementTypeId = model.getAnnouncementTypeId();
            var announcementTypeName = model.getAnnouncementTypeName();
            var classId = model.getClassId();

            model.setMarkingPeriodId(this.getCurrentMarkingPeriod().getId());

            if (submitType == 'listLast'){
                return this.listLastAction(announcementTypeName, classId, announcementTypeId, schoolPersonId);
            }

            if (submitType == 'saveTitle'){
                return this.saveTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle'){
                return this.checkTitleAction(model.getTitle(), classId, model.getExpiresDate(), model.getId());
            }

            if (submitType == 'save'){
                return this.Redirect('announcement', 'saveAnnouncementForm', [model]);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.Redirect('announcement', 'saveAnnouncement', [model]);
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

        [[chlk.models.announcement.Announcement, chlk.models.announcement.AnnouncementForm]],
        function saveAnnouncementTeacherAction(model, form_) {
            if(!(model.getClassId() && model.getClassId().valueOf() && model.getAnnouncementTypeId() && model.getAnnouncementTypeId().valueOf()))
                return null;
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
                .attach(this.validateResponse_())
                .then(function(model){
                    if (form_){
                        var applications = model.getApplications() || [];
                        this.cacheAnnouncementApplications(applications);
                        var announcement = form_.getAnnouncement();
                        announcement.setTitle(model.getTitle());
                        announcement.setApplications(applications);
                        announcement.setCanAddStandard(model.isCanAddStandard());
                        announcement.setStandards(model.getStandards());
                        announcement.setGradable(model.isGradable());
                        announcement.setGradingStudentsCount(model.getGradingStudentsCount());
                        announcement.setAbleToRemoveStandard(model.isAbleToRemoveStandard());
                        announcement.setSuggestedApps(model.getSuggestedApps());
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setClassName(model.getClassName());
                        form_.setAnnouncement(announcement);
                        return this.addEditAction(form_, false);
                    }
                }, this)
                .attach(this.validateResponse_());

            if (form_)
                return this.UpdateView(this.getAnnouncementFormPageType_(), res);
            return null;
        },

        //TODO: REFACTOR!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        [[chlk.models.announcement.Announcement, Boolean]],
        function submitAnnouncement(model, isEdit){
            var apps = this.getCachedAnnouncementApplications();
            var res = this.announcementService
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

            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    if(isEdit)
                        return this.BackgroundNavigate('announcement', 'view', [model.getId()]);
                    else{
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
            return res;
        },

        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function showDuplicateFormAction(announcementId, selectedClassId){
            var classes = this.classService.getClassesForTopBarSync();
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
            var classesIds = this.getIdsList(model.getSelectedIds(), chlk.models.id.ClassId);
            var res = this.announcementService
                .duplicateAnnouncement(model.getAnnouncementId(), model.getSelectedIds())
                .attach(this.validateResponse_())
                .thenCall(this.classService.updateClassAnnouncementTypes, [classesIds])
                .then(function(data){
                    this.BackgroundCloseView(chlk.activities.announcement.AddDuplicateAnnouncementDialog);
                    this.BackgroundNavigate('announcement', 'edit', [model.getAnnouncementId()]);
                    return ria.async.BREAK;
                }, this)
            return null;
        },

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.id.AnnouncementId, Boolean]],
        function starAction(id, complete_){
            this.announcementService
                .checkItem(id, complete_)
                .attach(this.validateResponse_());
            return null;
        },

        [chlk.controllers.SidebarButton('statistic')],
        [[chlk.models.id.AnnouncementId, Boolean]],
        function starFromStudentGradesAction(id, complete_){
            return this.starAction(id, complete_);
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

        [chlk.controllers.SidebarButton('add-new')],
        [[String, chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function showStandardsAction(typeName, announcementId, classId){
            var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
            var res = this.standardService.getSubjects(classId)
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
                    //return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                    this.prepareAttachments(announcement);
                    return announcement;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, res, 'update-standards-and-suggested-apps');
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
        function removeStandardAction(announcementId, standardId){
            var res = this.announcementService.removeStandard(announcementId, standardId)
                .then(function(announcement){
                    this.saveStandardIds(announcement);
                    //return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                    return announcement;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementFormPage, res, 'update-standards-and-suggested-apps');
        }
    ])
});
