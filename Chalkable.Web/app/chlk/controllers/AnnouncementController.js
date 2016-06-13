REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.GroupService');
REQUIRE('chlk.services.LessonPlanService');
REQUIRE('chlk.services.LpGalleryCategoryService');
REQUIRE('chlk.services.ClassAnnouncementService');
REQUIRE('chlk.services.SupplementalAnnouncementService');
REQUIRE('chlk.services.AdminAnnouncementService');
REQUIRE('chlk.services.AnnouncementAssignedAttributeService');
REQUIRE('chlk.services.AnnouncementAttachmentService');
REQUIRE('chlk.services.AttachmentService');
REQUIRE('chlk.services.AnnouncementQnAService');
REQUIRE('chlk.services.ApplicationService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.SchoolYearService');
REQUIRE('chlk.services.StudentService');

REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.activities.announcement.LessonPlanFormPage');
REQUIRE('chlk.activities.announcement.SupplementalAnnouncementFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');
REQUIRE('chlk.activities.announcement.AdminAnnouncementFormPage');
REQUIRE('chlk.activities.apps.AttachAppsDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');
REQUIRE('chlk.activities.announcement.AddStandardsDialog');
REQUIRE('chlk.activities.announcement.AddDuplicateAnnouncementDialog');
REQUIRE('chlk.activities.announcement.AnnouncementGroupsDialog');
REQUIRE('chlk.activities.announcement.AnnouncementEditGroupsDialog');
REQUIRE('chlk.activities.announcement.GroupStudentsFilterDialog');
REQUIRE('chlk.activities.announcement.AddNewCategoryDialog');
REQUIRE('chlk.activities.announcement.FileCabinetDialog');
REQUIRE('chlk.activities.announcement.AttachFilesDialog');
REQUIRE('chlk.activities.announcement.AnnouncementImportDialog');

REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.announcement.LastMessages');
REQUIRE('chlk.models.attachment.Attachment');
REQUIRE('chlk.models.announcement.StudentAnnouncement');
REQUIRE('chlk.models.apps.AppsForAttachViewData');
REQUIRE('chlk.models.announcement.ShowGradesToStudents');
REQUIRE('chlk.models.announcement.FileAttachViewData');
REQUIRE('chlk.models.people.UsersListSubmit');
REQUIRE('chlk.models.announcement.post.AnnouncementImportPostViewData');

REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.AttachmentId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.StandardSubjectId');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.announcement.QnAForm');
REQUIRE('chlk.models.common.attachments.BaseAttachmentViewData');
REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.announcement.AddDuplicateAnnouncementViewData');
REQUIRE('chlk.models.common.SimpleObject');
REQUIRE('chlk.models.attachment.FileCabinetViewData');
REQUIRE('chlk.models.attachment.FileCabinetPostData');

REQUIRE('chlk.models.announcement.SubmitDroppedAnnouncementViewData');

REQUIRE('chlk.lib.exception.AppErrorException');

NAMESPACE('chlk.controllers', function (){


    /** @class chlk.controllers.AnnouncementController */
    CLASS(
        'AnnouncementController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.AnnouncementService, 'announcementService',

        [ria.mvc.Inject],
        chlk.services.LessonPlanService, 'lessonPlanService',

        [ria.mvc.Inject],
        chlk.services.LpGalleryCategoryService, 'lpGalleryCategoryService',

        [ria.mvc.Inject],
        chlk.services.ClassAnnouncementService, 'classAnnouncementService',

        [ria.mvc.Inject],
        chlk.services.SupplementalAnnouncementService, 'supplementalAnnouncementService',

        [ria.mvc.Inject],
        chlk.services.AdminAnnouncementService, 'adminAnnouncementService',

        [ria.mvc.Inject],
        chlk.services.StudentService, 'studentService',

        [ria.mvc.Inject],
        chlk.services.ClassService, 'classService',

        [ria.mvc.Inject],
        chlk.services.CalendarService, 'calendarService',

        [ria.mvc.Inject],
        chlk.services.PersonService, 'personService',

        [ria.mvc.Inject],
        chlk.services.GradingService, 'gradingService',

        [ria.mvc.Inject],
        chlk.services.GradeLevelService, 'gradeLevelService',

        [ria.mvc.Inject],
        chlk.services.GroupService, 'groupService',

        [ria.mvc.Inject],
        chlk.services.MarkingPeriodService, 'markingPeriodService',

        [ria.mvc.Inject],
        chlk.services.AnnouncementAssignedAttributeService, 'assignedAttributeService',

        [ria.mvc.Inject],
        chlk.services.AnnouncementAttachmentService, 'announcementAttachmentService',

        [ria.mvc.Inject],
        chlk.services.AttachmentService, 'attachmentService',

        [ria.mvc.Inject],
        chlk.services.AnnouncementQnAService, 'announcementQnAService',

        [ria.mvc.Inject],
        chlk.services.ApplicationService, 'applicationService',

        [ria.mvc.Inject],
        chlk.services.SchoolYearService, 'schoolYearService',

        ArrayOf(chlk.models.attachment.AnnouncementAttachment), 'announcementAttachments',


        [[chlk.models.id.ClassId]],
        function showImportDialogAction(classId){
            var classScheduleDateRanges = this.getContext().getSession().get(ChlkSessionConstants.CLASS_SCHEDULE_DATE_RANGES, []);
            var activity = this.getView().getCurrent().getClass();
            var res = this.WidgetStart('announcement', 'showImportDialog', [classId, classScheduleDateRanges])
                .then(function(data){
                    this.BackgroundUpdateView(activity, data, 'after-import');
                }, this);
            return null;
        },

        [[String, chlk.models.id.ClassId, Array]],
        function showImportDialogWidgetAction(requestId, classId, classScheduleDateRanges){
            var res = this.schoolYearService.listOfSchoolYearClasses()
                .then(function(classesByYears){
                    return new chlk.models.announcement.AnnouncementImportViewData(classId, classesByYears, null, requestId, classScheduleDateRanges);
                });
            return this.ShadeView(chlk.activities.announcement.AnnouncementImportDialog, res);
        },

        [[chlk.models.announcement.post.AnnouncementImportPostViewData]],
        function importAction(model){
            var res;
            if(model.getSubmitType() == 'list')
                res = this.calendarService.listByDateRange(null, null, model.getClassId())
                    .then(function(announcements){
                        return new chlk.models.announcement.AnnouncementImportViewData(model.getClassId(), null, announcements);
                    });
            else
                res = this.announcementService.copy(this.getCurrentClassId(),model.getToClassId(), model.getAnnouncementsToCopy(), model.getCopyStartDate())
                    .then(function(createdList){
                        this.WidgetComplete(model.getRequestId(), createdList);
                        this.BackgroundCloseView(chlk.activities.announcement.AnnouncementImportDialog);
                        return ria.async.BREAK;
                    }, this);

            return this.UpdateView(chlk.activities.announcement.AnnouncementImportDialog, res, 'list-update');
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        function indexAction() {
            var classId = this.getCurrentClassId();
            return this.Redirect('announcement', 'add', [classId]);
        },


        VOID, function updateAttributesWithFilesList_(announcement){
            var res = [];
            announcement.getAnnouncementAttributes().forEach(function(attribute){
                if(attribute.getAttributeAttachment())
                    res.push(attribute.getId())
            });

            this.getContext().getSession().set(ChlkSessionConstants.ATTRIBUTES_WITH_FILES, res);
        },

        VOID, function afterAttributeFileAttach_(attributeId){
            var attrs = this.getContext().getSession().get(ChlkSessionConstants.ATTRIBUTES_WITH_FILES, []);
            if(attrs.indexOf(attributeId) == -1)
                attrs.push(attributeId);

            this.getContext().getSession().set(ChlkSessionConstants.ATTRIBUTES_WITH_FILES, attrs);
        },

        VOID, function afterAttributeFileRemove_(attributeId){
            var attrs = this.getContext().getSession().get(ChlkSessionConstants.ATTRIBUTES_WITH_FILES, []);
            attrs.splice(attrs.indexOf(attributeId), 1);

            this.getContext().getSession().set(ChlkSessionConstants.ATTRIBUTES_WITH_FILES, attrs);
        },

        Boolean, function isAttributeWithFile_(attributeId){
            var attrs = this.getContext().getSession().get(ChlkSessionConstants.ATTRIBUTES_WITH_FILES, []);
            return attrs.indexOf(attributeId) > -1;
        },

        function getAnnouncementFormPageType_(type_){
            if(this.userInRole(chlk.models.common.RoleEnum.TEACHER)){
                var announcementType = type_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf();
                if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT)
                    return chlk.activities.announcement.AnnouncementFormPage;

                if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.SUPPLEMENTAL_ANNOUNCEMENT)
                    return chlk.activities.announcement.SupplementalAnnouncementFormPage;

                return chlk.activities.announcement.LessonPlanFormPage;
            }

            if(this.userInRole(chlk.models.common.RoleEnum.DISTRICTADMIN))
                return chlk.activities.announcement.AdminAnnouncementFormPage;

            if(this.userInRole(chlk.models.common.RoleEnum.STUDENT))
                return chlk.activities.announcement.AnnouncementViewPage;
        },

        [[chlk.models.announcement.ShowGradesToStudents]],
        function setShowGradesToStudentsAction(model) {
            this.announcementService
                .setShowGradesToStudents(model.getAnnouncementId(), model.isShowToStudents())
                .attach(this.validateResponse_());
        },

        function prepareAttachment(attachment, width_, height_){
            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attachment.setThumbnailUrl(this.announcementAttachmentService.getAttachmentUri(attachment.getId(), false, width_ || 170, height_ || 110));
                attachment.setUrl(this.announcementAttachmentService.getAttachmentUri(attachment.getId(), false, null, null));
            }
            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.OTHER){
                attachment.setUrl(this.announcementAttachmentService.getAttachmentUri(attachment.getId(), true, null, null));
            }
        },

        [[chlk.models.announcement.AnnouncementAttributeViewData, Number, Number]],
        function prepareAttribute(attribute, width_, height_){

            var attributeAttachment = attribute.getAttributeAttachment();

            if (!attributeAttachment) return;

            if(attributeAttachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attributeAttachment.setThumbnailUrl(this.assignedAttributeService.getAttributeAttachmentUri(attribute.getId(), attribute.getAnnouncementType(), false, width_ || 170, height_ || 110));
                attributeAttachment.setUrl(this.assignedAttributeService.getAttributeAttachmentUri(attribute.getId(), attribute.getAnnouncementType(), false, null, null));
            }
            if(attributeAttachment.getType() == chlk.models.attachment.AttachmentTypeEnum.OTHER){
                attributeAttachment.setUrl(this.assignedAttributeService.getAttributeAttachmentUri(attribute.getId(), attribute.getAnnouncementType(), true, null, null));
            }

            attribute.setAttributeAttachment(attributeAttachment);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function prepareAttachments(announcement){
            var attachments = announcement.getAnnouncementAttachments() || [], ids = [];
            attachments.forEach(function(item){
                ids.push(item.getId().valueOf());
                this.prepareAttachment(item);
            }, this);
            announcement.setAttachments(ids.join(','));
            this.cacheAnnouncementAttachments(attachments);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function prepareAttributes(announcement){
            var attributes = announcement.getAnnouncementAttributes() || [];
            attributes.forEach(function(item){
                item.setAnnouncementType(announcement.getType());
                item.setAttributeTypes(this.assignedAttributeService
                    .getAnnouncementAttributeTypesList());
                this.prepareAttribute(item);
            }, this);
            announcement.setAnnouncementAttributes(attributes);
            this.cacheAnnouncementAttributes(attributes);
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
                    studentAnnouncement.isCommentChanged(),
                    fromGrid_ ? chlk.models.announcement.ShortStudentAnnouncementViewData : null
                );
            return result;
        },

        [chlk.controllers.SidebarButton('statistic')],
        [[chlk.models.announcement.StudentAnnouncement]],
        function updateAnnouncementGradeFromGridAction(model){
            var result = this.setAnnouncementGrade(model, true)
                .catchError(function (error) {
                    model.setGradeValue(model.getOldGradeValue());
                    model.setDropped(model.isOldDropped());
                    model.setLate(model.isOldLate());
                    model.setIncomplete(model.isOldIncomplete());
                    model.setExempt(model.isOldExempt());
                    this.BackgroundUpdateView(this.getView().getCurrent().getClass(), model, chlk.activities.lib.DontShowLoader());

                    throw error;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(this.getView().getCurrent().getClass(), result, chlk.activities.lib.DontShowLoader());
        },

        [[Object, Boolean]],
        function addEditAction(model, isEdit){
            this.disableAnnouncementSaving(false);
            var announcement = model.getAnnouncement();
            var announcementTypeId_;
            var type = announcement.getType();
            this.cacheAnnouncementType(type);
            this.updateAttributesWithFilesList_(announcement);

            var classes = this.classService.getClassesForTopBarSync();
            var announcementWithTypes = announcement.getClassAnnouncementData() || announcement.getSupplementalAnnouncementData();
            var announcementWithClassItem = announcementWithTypes || announcement.getLessonPlanData() || announcement.getSupplementalAnnouncementData();
            var classId_ = announcementWithClassItem.getClassId(), classInfo, types;

            var savedClassInfo = this.getContext().getSession().get('classInfo', null);

            if(announcementWithTypes)
                announcementTypeId_ = announcementWithTypes.getAnnouncementTypeId();

            var gradingPeriods = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIODS, []);

            classes.forEach(function (item) {
                var currentClassInfo = this.classService.getClassAnnouncementInfo(item.getId());
                types = currentClassInfo ? currentClassInfo.getTypesByClass() : [];
                if (types.length)
                    item.setDefaultAnnouncementTypeId(types[0].getId());
                if (currentClassInfo && classId_ && classId_ == item.getId()) {
                    classInfo = currentClassInfo;
                    model.setClassInfo(classInfo);

                    //prepare class schedule date range
                    var classScheduleDateRanges = gradingPeriods
                        .filter(function(gp){
                            return item.getMarkingPeriodsId().filter(function(mpId){return mpId == gp.getMarkingPeriodId()}).length > 0
                        })
                        .map(function (_) {
                            return {
                                start: _.getStartDate().getDate(),
                                end: _.getEndDate().getDate()
                            }
                        });

                    model.setClassScheduleDateRanges(classScheduleDateRanges);

                    this.getContext().getSession().set(ChlkSessionConstants.CLASS_SCHEDULE_DATE_RANGES, classScheduleDateRanges);

                    if(announcementWithTypes){
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
                    }
                }

            }, this);

           // var currentClass = classes.filter(function(item){ return classId_ && item.getId() == classId_},this);

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

            this.prepareAnnouncementAttachedItems(announcement);

            //processing apps recommended content
            this.getListOfAppRecommendedContents_(announcement);

            return model;
        },

        function prepareAnnouncementAttachedItems(announcement){
            this.prepareAttachments(announcement);

            this.prepareAttributes(announcement);

            var applications = announcement.getApplications() || [];
            this.cacheAnnouncementApplications(applications);

            var applicationsIds = applications.map(function(item){
                return item.getId().valueOf()
            }).join(',');
            announcement.setApplicationsIds(applicationsIds);

            this.saveStandardIds(announcement);
        },

        VOID, function saveStandardIds(announcement){
            var standardsIds = [], standards = announcement.getStandards() || [];
            standards.forEach(function(item){
                standardsIds.push(item.getStandardId().valueOf());
            });

            this.getContext().getSession().set(ChlkSessionConstants.STANDARD_IDS, standardsIds);
            this.getContext().getSession().set(ChlkSessionConstants.STANDARDS, standards);
        },

        [[chlk.models.common.ChlkDate, Boolean, chlk.models.id.ClassId]],
        function addViaCalendarTeacherAction(date_, noDraft_, classId_){
            return this.addAction(classId_, null, date_, noDraft_);
        },

        [[chlk.models.common.ChlkDate, Boolean, chlk.models.id.ClassId]],
        function addViaCalendarDistrictAdminAction(date_, noDraft_, classId_){
            return this.addDistrictAdminAction(date_);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.common.ChlkDate]],
        function addDistrictAdminAction(date_) {
            var result = this.adminAnnouncementService
                .addAdminAnnouncement(date_)
                .attach(this.validateResponse_())
                .then(function(model){
                    var announcement = model.getAnnouncement();
                    this.getContext().getSession().set(ChlkSessionConstants.GROUPS_IDS, (announcement.getRecipients() || []).map(function(item){return item.getGroupId()}));
                    if(date_){
                        announcement.setExpiresDate(date_);
                    }
                    this.prepareAnnouncementAttachedItems(announcement);
                    return model;
                }, this);
            return this.PushView(chlk.activities.announcement.AdminAnnouncementFormPage, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function editDistrictAdminAction(announcementId, announcementType) {
            var result = this.announcementService
                .editAnnouncement(announcementId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    var announcement = model.getAnnouncement();
                    this.getContext().getSession().set(ChlkSessionConstants.GROUPS_IDS, (announcement.getRecipients() || []).map(function(item){return item.getGroupId()}));
                    this.prepareAnnouncementAttachedItems(announcement);
                    return model;
                }, this);
            return this.PushView(chlk.activities.announcement.AdminAnnouncementFormPage, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, Boolean]],
        function addAction(classId_, announcementTypeId_, date_, noDraft_) {
            this.getView().reset();
            this.getView().pushD(new chlk.activities.lib.PendingActionDialog(), ria.async.Future.$fromData({}));
            this.getContext().getSession().set('classInfo', null);
            return this.announcementService
                .addAnnouncement(classId_, announcementTypeId_)
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    if(model && model.getAnnouncement()){

                        var resModel =  this.addEditAction(model, false);
                        var classAnnouncement = resModel.getAnnouncement().getClassAnnouncementData();
                        var supplementalAnnouncement = resModel.getAnnouncement().getSupplementalAnnouncementData();

                        if((classAnnouncement || supplementalAnnouncement) && date_){
                            (classAnnouncement || supplementalAnnouncement).setExpiresDate(date_);
                        }

                        if(resModel.getAnnouncement().getLessonPlanData())
                            return this.lessonPlanFromModel_(resModel);

                        if(supplementalAnnouncement)
                            return this.supplementalAnnouncementFromModel_(resModel);

                        return this.classAnnouncementFromModel_(resModel);

                    }
                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);

                    var res = this.classAnnouncementFromModel_(chlk.models.announcement.AnnouncementForm.$create(classesBarData, true));
                    return res;
                },this);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.apps.Application]],
        function getAppRecommendedContents_(ann, app){
            if(ann.getStandards().length > 0)
                var emptyModel = new  chlk.models.apps.AppContentListViewData();
                this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType()), emptyModel,  'before-app-contents-loaded');
                this.applicationService.getApplicationContents(
                        app.getUrl(),
                        ann.getId(),
                        ann.getType(),
                        ann.getStandards(),
                        app.getEncodedSecretKey())
                    //.attach(this.validateResponse_())
                    .catchError(function(e){
                        this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType()), null, 'app-contents-fail');
                        throw e;
                    }, this)
                    .then(function(paginatedContents){

                        if(paginatedContents.getItems() && paginatedContents.getItems().length > 0){

                           var res = chlk.models.apps.AppContentListViewData(app, ann.getId(), ann.getType()
                               , ann.getClassId(), paginatedContents, ann.getStandards());

                           this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType()), res, 'update-app-contents');
                        }
                        else
                            this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType()), null, 'app-contents-fail');
                    }, this);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function getListOfAppRecommendedContents_(announcement){
            if(announcement.getStandards().length > 0)
                announcement.getAppsWithContent()
                    .forEach(function(app) {
                        this.getAppRecommendedContents_(announcement, app);
                },this);
        },


        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        function updateSuggestedAppsAction() {
            var res = this.announcementService
                .addAnnouncement()
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    this.BackgroundCloseView(chlk.activities.lib.PendingActionDialog);
                    this.BackgroundCloseView(chlk.activities.apps.InstallAppDialog);
                    var announcement = model.getAnnouncement();
                    var suggestedApps = announcement.getSuggestedApps();
                    var suggestedAppsListData = new chlk.models.apps.SuggestedAppsList(
                        announcement.getClassId(),
                        announcement.getId(),
                        suggestedApps,
                        announcement.getStandards(),
                        null,
                        announcement.getType()
                    );
                    this.BackgroundUpdateView(this.getAnnouncementFormPageType_(announcement.getType()), suggestedAppsListData);
                    return model;
                },this);

            return this.ShadeLoader();

        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN, chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[
            chlk.models.id.AnnouncementId,
            chlk.models.id.ClassId,
            String,
            chlk.models.announcement.AnnouncementTypeEnum,
            String
        ]],
        function attachAction(announcementId, classId, announcementTypeName, announcementType, appUrlAppend_) {

            var result = this.announcementService.getAttachSettings(announcementId, announcementType)
                .then(function(options){
                    //_DEBUG && options.setAssessmentAppId(chlk.models.id.AppId('56c14655-2897-4073-bb48-32dfd61264b5'));

                    options.updateByValues(null, null, announcementId, classId, announcementTypeName,
                        announcementType, null, appUrlAppend_);
                    this.getContext().getSession().set(ChlkSessionConstants.ATTACH_OPTIONS, options);
                    return new chlk.models.common.BaseAttachViewData(options);
                }, this);

            return this.ShadeOrUpdateView(chlk.activities.announcement.AttachFilesDialog, result);
        },
            
        [chlk.controllers.SidebarButton('add-new')],
        function attachFilesAction() {
            var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);
            var result = new ria.async.DeferredData(new chlk.models.common.BaseAttachViewData(options));

            return this.ShadeOrUpdateView(chlk.activities.announcement.AttachFilesDialog, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[Number]],
        function attachAppsDistrictAdminAction(start_) {
            var start = start_ || 0, count = 12;
            var result = this.applicationService
                .getAppsForAttach(start, count)
                .attach(this.validateResponse_())
                .then(function(data) {
                    var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);
                    return new chlk.models.apps.AppsForAttachViewData(options, data);
                }, this);
            return this.ShadeOrUpdateView(chlk.activities.apps.AttachAppsDialog, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[Number]],
        function attachAppsTeacherAction(start_) {

            var start = start_ || 0, count = 12;
            var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);

            var result = this.applicationService
                .getAppsForAttach(start, count)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.apps.AppsForAttachViewData(options, data, start);
                }, this);

            return this.ShadeOrUpdateView(chlk.activities.apps.AttachAppsDialog, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN,
            chlk.models.common.RoleEnum.TEACHER
        ])],
        //---File Cabinet---
        [chlk.controllers.SidebarButton('add-new')],
        function fileCabinetAction(){
            return this.getAttachments_();
        },

        [[String, chlk.models.attachment.SortAttachmentType, Number, Number]],
        function getAttachments_(filter_, sortType_, start_, count_){
            var res = this.attachmentService.getAttachments(filter_, sortType_, start_, count_)
                .attach(this.validateResponse_())
                .then(function(atts){

                    atts.getItems().forEach(function(attachment){
                        if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                            attachment.setThumbnailUrl(this.attachmentService.getDownloadUri(attachment.getId(), false, 170, 110));
                            attachment.setUrl(this.attachmentService.getDownloadUri(attachment.getId(), false, null, null));
                        }
                        if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.OTHER){
                            attachment.setUrl(this.attachmentService.getDownloadUri(attachment.getId(), true, null, null));
                        }
                    }, this);

                    var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);

                    return new chlk.models.attachment.FileCabinetViewData(options, atts, sortType_, filter_);
                }, this);
            return this.ShadeOrUpdateView(chlk.activities.announcement.FileCabinetDialog, res);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId]],
        function fileAttachAction(announcementId, announcementType, assignedAttributeId, appUrlAppend_){
            var result = this.announcementService.getAttachSettings(announcementId, announcementType)
                .then(function(options){
                    var standards = this.getContext().getSession().get(ChlkSessionConstants.STANDARDS, []);
                    var appUrlAppend = (standards || []).map(function (c, index) { return c.getUrlComponents(index); }).join('&');

                    options.updateByValues(false, false, announcementId, null, null,
                        announcementType, assignedAttributeId, appUrlAppend, false);
                    this.getContext().getSession().set(ChlkSessionConstants.ATTACH_OPTIONS, options);
                    return new chlk.models.common.BaseAttachViewData(options);
                }, this);

            return this.ShadeOrUpdateView(chlk.activities.announcement.AttachFilesDialog, result);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function fileAttachStudentAction(announcementId, announcementType){
            var result = this.announcementService.getAttachSettings(announcementId, announcementType)
                .then(function(options){

                    var isAssessmentEnabled = this.getContext().getSession().get(ChlkSessionConstants.ASSESSMENT_ENABLED, false);

                    options.updateByValues(null, false, announcementId, null, null, announcementType);
                    this.getContext().getSession().set(ChlkSessionConstants.ATTACH_OPTIONS, options);
                    return new chlk.models.common.BaseAttachViewData(options);
                }, this);

            return this.ShadeOrUpdateView(chlk.activities.announcement.AttachFilesDialog, result);
        },

        //todo move attribute methods to separate controller

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function fetchAddAttributeFuture_(announcementId, announcementType) {
            var attributeId = this.assignedAttributeService
                .getAnnouncementAttributeTypesList()[0].getId();

            return this.assignedAttributeService
                .addAnnouncementAttribute(announcementId, attributeId, announcementType)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    attribute.setAnnouncementType(announcementType);
                    attribute.setAnnouncementId(announcementId);
                    attribute.setAttributeTypes(this.assignedAttributeService
                        .getAnnouncementAttributeTypesList());
                    attribute.setReadOnly(false);
                    this.prepareAttribute(attribute);
                    var attributes = this.getCachedAnnouncementAttributes();
                    attributes.push(attribute);
                    this.cacheAnnouncementAttributes(attributes);
                    return attribute;
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function addAttributeTeacherAction(announcementId, announcementType) {
            this.BackgroundCloseView(chlk.activities.apps.AttachAppsDialog);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType),
                this.fetchAddAttributeFuture_(announcementId, announcementType), 'add-attribute');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function addAttributeDistrictAdminAction(announcementId, announcementType) {
            this.BackgroundCloseView(chlk.activities.apps.AttachAppsDialog);
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage,
                this.fetchAddAttributeFuture_(announcementId, announcementType), 'add-attribute');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN,
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.attachment.FileCabinetPostData]],
        function listAttachmentsAction(postData){
            return this.getAttachments_(
                postData.getFilter(),
                postData.getSortType(),
                postData.getStart(),
                postData.getCount()
            );
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN,
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[
            String,
            chlk.models.attachment.SortAttachmentType,
            Number,
            Number
        ]],
        function pageAttachmentsAction(filter_, sortType_, start_, count_){
            return this.getAttachments_(filter_, sortType_, start_, count_);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, Boolean]],
        function attachFromCabinetAction(announcementId, announcementType, attachmentId, onCreate_){
            var res = this.announcementAttachmentService
                .addAttachment(announcementId, announcementType, attachmentId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(announcement){
                    announcement.setNeedButtons(true);
                    announcement.setNeedDeleteButton(true);
                    this.prepareAttachments(announcement);
                    this.cacheAnnouncement(announcement);
                    return announcement;
                }, this);
            this.BackgroundCloseView(chlk.activities.announcement.FileCabinetDialog);
            var isStudent =  this.userInRole(chlk.models.common.RoleEnum.STUDENT);
            return this.UpdateView(!isStudent ? this.getAnnouncementFormPageType_(announcementType) : chlk.activities.announcement.AnnouncementViewPage, res, 'update-attachments');

        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId]],
        function attachFromCabinetToAttributeAction(announcementId, announcementType, attachmentId, assignedAttributeId){
            if(this.isAttributeWithFile_(assignedAttributeId))
                return this.attributeAttachmentExistsAction();

            this.BackgroundCloseView(chlk.activities.announcement.FileCabinetDialog);
            var res = this.assignedAttributeService
                .addAttachment(announcementType, announcementId, assignedAttributeId, attachmentId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    this.afterAttributeFileAttach_(assignedAttributeId);
                    return this.prepareAttributeData(announcementId, announcementType, assignedAttributeId, attribute);
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType), res, 'add-attribute-attachment');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId]],
        function cloneFromCabinetAction(announcementId, announcementType, attachmentId){
            var res = this.announcementAttachmentService
                .cloneAttachment(attachmentId, announcementId, announcementType)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(announcement){
                    announcement.setNeedButtons(true);
                    announcement.setNeedDeleteButton(true);
                    this.prepareAttachments(announcement);
                    this.cacheAnnouncement(announcement);
                    return announcement;
                }, this);
            this.BackgroundCloseView(chlk.activities.announcement.FileCabinetDialog);
            var isStudent =  this.userInRole(chlk.models.common.RoleEnum.STUDENT);
            return this.UpdateView(!isStudent ? this.getAnnouncementFormPageType_(announcementType) : chlk.activities.announcement.AnnouncementViewPage, res, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId]],
        function cloneFromCabinetToAttributeAction(announcementId, announcementType, attachmentId, assignedAttributeId){
            if(this.isAttributeWithFile_(assignedAttributeId))
                return this.attributeAttachmentExistsAction();

            var res = this.assignedAttributeService
                .cloneAttachmentForAttribute(attachmentId, announcementId, announcementType, assignedAttributeId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    this.afterAttributeFileAttach_(assignedAttributeId);
                    return this.prepareAttributeData(announcementId, announcementType, assignedAttributeId, attribute);
                }, this);
            this.BackgroundCloseView(chlk.activities.announcement.FileCabinetDialog);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType), res, 'add-attribute-attachment');
        },


        [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum]],
        function fetchRemoveAttributeFuture_(announcementId, attributeId, announcementType) {
            return this.assignedAttributeService
                .removeAnnouncementAttribute(announcementId, attributeId, announcementType)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(deleted){
                    this.afterAttributeFileRemove_(attributeId);
                    var attributes = this.getCachedAnnouncementAttributes().filter(function(attribute){return attribute.getId() != attributeId});
                    this.cacheAnnouncementAttributes(attributes);
                    return new chlk.models.announcement.AnnouncementAttributeViewData.$fromId(attributeId, deleted);
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum]],
        function removeAttributeTeacherAction(announcementId, attributeId, announcementType) {
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType),
                this.fetchRemoveAttributeFuture_(announcementId, attributeId, announcementType),  'remove-attribute');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum]],
        function removeAttributeDistrictAdminAction(announcementId, attributeId, announcementType) {
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage,
                this.fetchRemoveAttributeFuture_(announcementId, attributeId, announcementType),  'remove-attribute');
        },


        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function editAction(announcementId, announcementType) {
            this.getContext().getSession().set('classInfo', null);
            var res =  this.announcementService
                .editAnnouncement(announcementId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    var resModel =  this.addEditAction(model, true);
                    if(resModel.getAnnouncement().getLessonPlanData())
                        return this.getLessonPlanFromModel_(resModel);

                    if(resModel.getAnnouncement().getSupplementalAnnouncementData())
                        return this.getSupplementalAnnouncementFromModel_(resModel);

                    this.cacheAnnouncementType(announcementType);
                    return resModel;

                }, this);
            return this.PushView(this.getAnnouncementFormPageType_(announcementType), res);
        },

        function prepareAnnouncementForView(announcement){
            this.prepareAttachments(announcement);
            this.prepareAttributes(announcement);
            var studentAnnouncements = announcement.getStudentAnnouncements();
            if(studentAnnouncements){
                var studentItems = studentAnnouncements.getItems(), that = this;
                studentItems.forEach(function(item){
                    item.getAttachments().forEach(function(attachment){
                        that.prepareAttachment(attachment);
                    });
                });
            }
            var classAnnouncement = announcement.getClassAnnouncementData();
            var lessonPlan = announcement.getLessonPlanData();
            var adminAnnouncement = announcement.getAdminAnnouncementData();
            var supplementalAnnouncement = announcement.getSupplementalAnnouncementData();
            var announcementForClass = classAnnouncement || lessonPlan || supplementalAnnouncement;
            var announcementWithExpires = classAnnouncement || adminAnnouncement || supplementalAnnouncement;

            if(announcementForClass && this.userInRole(chlk.models.common.RoleEnum.TEACHER)){
                var classInfo = this.classService.getClassAnnouncementInfo(announcementForClass.getClassId());
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
            announcementWithExpires && announcementWithExpires.prepareExpiresDateText();
            //announcement.setCurrentUser(this.getCurrentPerson());
            var hasMCPermission = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM) ||
                this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN);
            if(!hasMCPermission && classAnnouncement){
                classAnnouncement.setAbleToGrade(false);
            }
            announcement.setAbleEdit(true);//announcement.isAnnOwner() && hasMCPermission);
//                    announcement.setAbleChangeDate(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.CHANGE_ACTIVITY_DATES));
            announcement.calculateGradesAvg();
            var schoolOptions = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_OPTIONS, null);
            if(studentAnnouncements)
                announcement.getStudentAnnouncements().setSchoolOptions(schoolOptions);

            this.cacheAnnouncement(announcement);
            announcement.setHasAccessToLE(this.hasUserPermission_(chlk.models.people.UserPermissionEnum.AWARD_LE_CREDITS_CLASSROOM));
            return announcement;
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function viewAction(announcementId, announcementType_) {
            this.getView().reset();
            var result = this.announcementService
                .getAnnouncement(announcementId, announcementType_)
                .attach(this.validateResponse_())
                .catchError(this.handleNoAnnouncementException_, this)
                .then(function(announcement){
                    this.cacheAnnouncementType(announcement.getType());
                    return this.prepareAnnouncementForView(announcement);
                }, this);
            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function viewDistrictAdminAction(announcementId, announcementType_) {
            this.getView().reset();

            var result = ria.async.wait([
                    this.announcementService.getAnnouncement(announcementId, announcementType_),
                    this.adminAnnouncementService.getAdminAnnouncementRecipients(announcementId)
                ])
                .attach(this.validateResponse_())
                .catchError(this.handleNoAnnouncementException_, this)
                .then(function(res){
                    var announcement = res[0], students = this.prepareUsers(res[1]);
                    announcement.setStudents(students);
                    this.cacheAnnouncementType(announcement.getType());
                    return this.prepareAnnouncementForView(announcement);
                }, this);

            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        [[chlk.models.people.UsersListSubmit]],
        function loadStudentsAction(model){
            var start = model.getStart();
            var result = this.adminAnnouncementService.getAdminAnnouncementRecipients(model.getAnnouncementId(), start, model.getCount())
                .then(function(usersData){
                    return this.prepareUsers(usersData, start);
                }, this);
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result, chlk.activities.lib.DontShowLoader());
        },

        function handleNoAnnouncementException_(error){
            if(error.getStatus && error.getStatus() == 500){
                var res = JSON.parse(error.getResponse());
                if(res.exceptiontype == 'NoAnnouncementException')
                    return this.redirectToErrorPage_(error.toString(), 'error', 'viewAnnouncementError', []);
            }
            throw error;
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId, Number, Object]],
        function uploadAttachmentOnCreateAction(announcementId, announcementType, assignedAttributeId, fileIndex, files) {
            this.BackgroundCloseView(chlk.activities.apps.AttachAppsDialog);

            var isStudent =  this.userInRole(chlk.models.common.RoleEnum.STUDENT); //todo remove this later ... this is short fix
            if(assignedAttributeId && assignedAttributeId.valueOf())
                return this.Redirect('announcement', 'addAttributeAttachment', [announcementType, announcementId, assignedAttributeId, files, fileIndex]);
            return this.Redirect('announcement', 'uploadAttachment', [announcementId,  announcementType, files, !isStudent, fileIndex]);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementAttributeViewData]],
        function prepareAttributeData(announcementId, announcementType, announcementAssignedAttributeId, attribute){
            var attachment = attribute.getAttributeAttachment();
            attachment.setReadOnly(false);
            attachment.setAnnouncementType(announcementType);
            attachment.setAnnouncementId(announcementId);
            attachment.setAttributeId(announcementAssignedAttributeId);
            attribute.setAttributeAttachment(attachment);
            attribute.setAttributeTypes(this.assignedAttributeService
                .getAnnouncementAttributeTypesList());
            attribute.setAnnouncementType(announcementType);
            this.prepareAttribute(attribute);
            var attributes = this.getCachedAnnouncementAttributes();
            var attribute2 = attributes.filter(function(attr){return attr.getId() == attribute.getId()})[0];
            attribute2.setAttributeAttachment(attribute.getAttributeAttachment());
            attribute2.setAttributeTypes(this.assignedAttributeService
                .getAnnouncementAttributeTypesList());
            attribute2.setAnnouncementType(announcementType);
            this.cacheAnnouncementAttributes(attributes);
            return attribute;
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, Number]],
        function removeAttributeAttachmentAction(announcementType, announcementId, announcementAssignedAttributeId, fileIndex_) {

            var fromDialog = fileIndex_ || fileIndex_ === 0;

            var result = this.assignedAttributeService
                .removeAttributeAttachment(announcementType, announcementId, announcementAssignedAttributeId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    this.afterAttributeFileRemove_(announcementAssignedAttributeId);

                    if(fromDialog)
                        return new chlk.models.attachment.AnnouncementAttachment(fileIndex_);

                    attribute.setAnnouncementType(announcementType);
                    attribute.setAnnouncementId(announcementId);
                    attribute.setReadOnly(false);
                    attribute.setAttributeTypes(this.assignedAttributeService
                        .getAnnouncementAttributeTypesList());

                    var attributes = this.getCachedAnnouncementAttributes();
                    var attribute2 = attributes.filter(function(attr){return attr.getId() == attribute.getId()})[0];
                    attribute2.setAttributeAttachment(null);
                    this.cacheAnnouncementAttributes(attributes);

                    return attribute;
                }, this);

            if(fromDialog)
                return this.UpdateView(chlk.activities.announcement.AttachFilesDialog, result, 'delete-attachment');

            return this.UpdateView(this.getAnnouncementFormPageType_(), result, 'remove-attribute-attachment');
        },

        [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Number]],
        function deleteAttachmentAction(attachmentId, announcementId, announcementType, fileIndex_) {

            var fromDialog = fileIndex_ || fileIndex_ === 0;

            var result = this.announcementAttachmentService
                .deleteAttachment(attachmentId, announcementId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    if(fromDialog)
                        return new chlk.models.attachment.AnnouncementAttachment(fileIndex_);

                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    this.prepareAttachments(model);
                    return model;
                }, this);

            if(fromDialog)
                return this.UpdateView(chlk.activities.announcement.AttachFilesDialog, result, 'delete-attachment');

            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'update-attachments');
        },

        function uploadDisabledAction(){
            this.showAttributesFilesUploadMsg_();
            return null;
        },

        function showAttributesFilesUploadMsg_(){
            this.ShowMsgBox('You can add only 1 attachment to attribute', 'Error');
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, Object, Number]],
        function addAttributeAttachmentAction(announcementType, announcementId, announcementAssignedAttributeId, files, fileIndex_) {
            if(this.isAttributeWithFile_(announcementAssignedAttributeId))
                return this.attributeAttachmentExistsAction();

            var firstModel = new chlk.models.attachment.AnnouncementAttachment(0, files[0].size, null, files[0].name);
            this.BackgroundUpdateView(chlk.activities.announcement.AttachFilesDialog, firstModel, 'attachment-progress');

            if(files.length > 1)
                this.showAttributesFilesUploadMsg_();

            var result = this.assignedAttributeService
                .uploadAttributeAttachment(announcementType, announcementId, announcementAssignedAttributeId, [files[0]])
                .handleProgress(function(event){
                    this.afterAttributeFileAttach_(announcementAssignedAttributeId);
                    var model = new chlk.models.attachment.AnnouncementAttachment(0, event.total, event.loaded, files[0].name);
                    this.BackgroundUpdateView(chlk.activities.announcement.AttachFilesDialog, model, 'attachment-progress');
                }, this)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    this.BackgroundCloseView(chlk.activities.announcement.AttachFilesDialog);
                }, this);
            return this.UpdateView(chlk.activities.announcement.AttachFilesDialog, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Object, Boolean, Number]],
        function uploadAttachmentAction(announcementId, announcementType, files, onCreate_, fileIndex_) {

            var fileIndex = fileIndex_ || 0;

            var result = this.uploadAttachment_(announcementId, files[0], announcementType, fileIndex);

            if(files.length > 1)
                for(var i = 1; i < files.length; i++){
                    this.uploadAttachment_(announcementId, files[i], announcementType, fileIndex + i)
                        .then(function(attachment){
                            this.BackgroundUpdateView(chlk.activities.announcement.AttachFilesDialog, attachment, chlk.activities.lib.DontShowLoader());
                        }, this);
                }

            return this.UpdateView(chlk.activities.announcement.AttachFilesDialog, result, chlk.activities.lib.DontShowLoader());
        },

        function uploadAttachment_(announcementId, file, announcementType, fileIndex){
            var firstModel = new chlk.models.attachment.AnnouncementAttachment(fileIndex, file.size, null, file.name);

            var res = this.announcementAttachmentService
                .uploadAttachment(announcementId, [file], announcementType)
                .catchException(chlk.lib.exception.FileSizeExceedException, function(exception){

                    this.BackgroundUpdateView(chlk.activities.announcement.AttachFilesDialog, firstModel, 'cancel-attachment-upload');
                    return this.ShowMsgBox(exception.getMessage(), 'Error', [{text: 'Ok'}], 'center').thenBreak();
                }, this)
                .handleProgress(function(event){
                    var model = new chlk.models.attachment.AnnouncementAttachment(fileIndex, event.total, event.loaded, file.name);
                    this.BackgroundUpdateView(chlk.activities.announcement.AttachFilesDialog, model, 'attachment-progress');
                }, this)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attachment){
                    this.prepareAttachment(attachment, 51, 33);
                    attachment.setAnnouncementId(announcementId);
                    attachment.setAnnouncementType(announcementType);
                    attachment.setFileIndex(fileIndex || 0);
                    attachment.setTotal(file.size);
                    return attachment;
                }, this);

            this.BackgroundUpdateView(chlk.activities.announcement.AttachFilesDialog, firstModel, 'attachment-progress');
            return res;
        },

        [[chlk.models.id.AnnouncementAttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function viewAttachmentAction(attachmentId, announcementId, announcementType_){
            var attachment = this.getCachedAnnouncementAttachments().filter(function(item){ return item.getId() == attachmentId; })[0];
            if (!attachment)
                return null;

            var attachmentUrl, res;
            var downloadAttachmentButton = new chlk.models.common.attachments.ToolbarButton(
                "download-attachment",
                "Download Attachment",
                this.announcementAttachmentService.getAttachmentUri(attachment.getId(), true, null, null)
            );

            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attachmentUrl = attachment.getUrl();
                var attachmentViewData = new chlk.models.common.attachments.BaseAttachmentViewData(
                    attachmentUrl,
                    [downloadAttachmentButton],
                    attachment.getType()
                );
                res = new ria.async.DeferredData(attachmentViewData);
            }else{
                var buttons = [downloadAttachmentButton];
                if(this.userInRole(chlk.models.common.RoleEnum.STUDENT) && attachment.isTeachersAttachment() && attachment.getAttachmentId() &&
                    announcementType_ && announcementType_ == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT)
                        buttons.push(new chlk.models.common.attachments.ToolbarButton('mark-attachment', 'MARK UP', null, null,
                            'announcement', 'cloneAttachment', [attachment.getAttachmentId().valueOf(), announcementId.valueOf()], true));
                res = this.announcementAttachmentService
                    .startViewSession(attachmentId)
                    .then(function(session){
                        return new chlk.models.common.attachments.BaseAttachmentViewData(
                            this.attachmentService.getViewSessionUrl(session),
                            buttons,
                            attachment.getType()
                        );
                    }, this);
            }
            return this.ShadeView(chlk.activities.common.attachments.AttachmentDialog, res);
        },

        [[chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId]],
        function viewAttributeAttachmentAction(attributeId, announcementType, announcementId){
            var attribute = this.getCachedAnnouncementAttributes().filter(function(item){ return item.getId() == attributeId; })[0];
            if (!attribute || !attribute.getAttributeAttachment())
                return null;
            var attachment = attribute.getAttributeAttachment();

            var attachmentUrl, res;
            var downloadAttachmentButton = new chlk.models.common.attachments.ToolbarButton(
                "download-attachment",
                "Download Attachment",
                "/AnnouncementAttribute/DownloadAttributeAttachment.json?needsDownload=true&assignedAttributeId=" + attributeId.valueOf() +
                    "&announcementType=" +announcementType.valueOf() + "&time=" + getDate().getTime()
            );

            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attachmentUrl = attachment.getUrl() + "&time=" + getDate().getTime();
                var attachmentViewData = new chlk.models.common.attachments.BaseAttachmentViewData(
                    attachmentUrl,
                    [downloadAttachmentButton],
                    attachment.getType()
                );
                res = new ria.async.DeferredData(attachmentViewData);
            }else{
                var buttons = [downloadAttachmentButton];
                res = this.assignedAttributeService
                    .startAttributeAttachmentViewSession(attributeId)
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


        [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId]],
        function cloneAttachmentAction(attachmentId, announcementId) {
            var res = this.announcementAttachmentService
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
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, 'update-attachments');
        },


        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function addAppAttachmentAction(announcement) {
            announcement.setNeedButtons(true);
            announcement.setNeedDeleteButton(true);
            this.prepareAttachments(announcement);
            return this.UpdateView(this.getAnnouncementFormPageType_(), new ria.async.DeferredData(announcement), 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function refreshAttachmentsAction(announcementId, announcementType_) {
            var result = this.announcementService
                .getAnnouncement(announcementId, announcementType_)
                .attach(this.validateResponse_())
                .then(function (announcement) {
                    announcement.setNeedButtons(true);
                    announcement.setNeedDeleteButton(true);
                    this.prepareAttachments(announcement);
                    this.cacheAnnouncement(announcement);
                    return announcement
                }, this);

            return this.UpdateView(this.getAnnouncementFormPageType_(), result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId]],
        function refreshAttributeAction(announcementId, announcementType, assignedAttributeId) {
            var result = this.announcementService
                .getAnnouncement(announcementId, announcementType)
                .attach(this.validateResponse_())
                .then(function (announcement) {
                    var attribute = announcement.getAnnouncementAttributes().filter(function(a){return a.getId() == assignedAttributeId})[0];
                    this.cacheAnnouncement(announcement);
                    return this.prepareAttributeData(announcementId, announcementType, assignedAttributeId, attribute);
                }, this);

            return this.UpdateView(this.getAnnouncementFormPageType_(), result, 'add-attribute-attachment');
        },

        function cancelDeleteAction(){
            this.disableAnnouncementSaving(false);
            return null;
        },

        [[chlk.models.id.AnnouncementId, String, chlk.models.announcement.AnnouncementTypeEnum]],
        function deleteAction(announcementId, typeName, announcementType) {
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
                params: [announcementId.valueOf(), announcementType],
                color: chlk.models.common.ButtonColor.RED.valueOf()
            }]);
            return null;
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function deleteAnnouncementAction(announcementId, announcementType) {
            return this.announcementService
                .deleteAnnouncement(announcementId, announcementType)
                .catchException(chlk.lib.exception.ChalkableSisException, function(exception){
                    this.disableAnnouncementSaving(false);
                    var msg = this.mapSisErrorMessage(exception.getMessage());

                    return this.ShowMsgBox(msg, 'oops',[{ text: Msg.GOT_IT.toUpperCase() }])
                        .then(function(){
                            this.BackgroundCloseView(chlk.activities.lib.PendingActionDialog);
                        }, this)
                        .thenBreak();
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.Redirect('feed', 'list', [null, true]);
                }, this);
        },

        [[chlk.models.announcement.AnnouncementTypeEnum]],
        function discardAction(announcementType) {
            this.disableAnnouncementSaving(true);
            var currentPersonId = this.getCurrentPerson().getId();
            return this.announcementService
                .deleteDrafts(currentPersonId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    return this.Redirect('feed', 'list', [null, true]);
                }, this);
        },

        [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum]],
        function deleteAppAction(announcementAppId, announcementType) {
            var result = this.announcementService
                .deleteApp(announcementAppId, announcementType)
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

        [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum]],
        function deleteAppDistrictAdminAction(announcementAppId, announcementType) {
            var result = this.announcementService
                .deleteApp(announcementAppId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    this.prepareAnnouncementAttachedItems(model);
                    return model;
                }, this);
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, result, 'update-attachments');
        },

        Boolean, function isAnnouncementSavingDisabled(){
            return this.getContext().getSession().get(ChlkSessionConstants.DONT_SAVE, false);
        },

        [[Boolean]],
        function disableAnnouncementSaving(val){
            this.getContext().getSession().set(ChlkSessionConstants.DONT_SAVE, val);
        },


        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function cacheAnnouncement(announcement){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT, announcement);
        },

        chlk.models.announcement.FeedAnnouncementViewData, function getCachedAnnouncement(){
             return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT, new chlk.models.announcement.FeedAnnouncementViewData());
        },



        [[chlk.models.id.ClassId]],
        function cacheLessonPlanClassId(classId){
            this.getContext().getSession().set(ChlkSessionConstants.CURRENT_CLASS_ID, classId);
        },

        chlk.models.id.ClassId, function getCachedLessonPlanClassId(){
            return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_CLASS_ID, null);
        },

        [[chlk.models.announcement.AnnouncementTypeEnum]],
        function cacheAnnouncementType(type){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_TYPE, type);
        },

        function getCachedAnnouncementType(){
            return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
        },

        [[ArrayOf(chlk.models.attachment.AnnouncementAttachment)]],
        function cacheAnnouncementAttachments(attachments){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_ATTACHMENTS, attachments);
        },

        function getCachedAnnouncementAttachments(){
            return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_ATTACHMENTS) || [];
        },

        [[ArrayOf(chlk.models.announcement.AnnouncementAttributeViewData)]],
        function cacheAnnouncementAttributes(attributes){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_ASSIGNED_ATTRIBUTES, attributes);
        },

        function getCachedAnnouncementAttributes(){
            return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_ASSIGNED_ATTRIBUTES) || [];
        },

        function getCachedAnnouncementApplications(){
            return this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_APPLICATIONS) || [];
        },


        [[ArrayOf(chlk.models.apps.AppAttachment)]],
        function cacheAnnouncementApplications(applications){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_APPLICATIONS, applications);
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

        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId, chlk.models.announcement.AnnouncementTypeEnum]],
        function showDuplicateFormAction(announcementId, selectedClassId, type){
            var classes = this.classService.getClassesForTopBarSync();
            var addDupAnnModel = new chlk.models.announcement.AddDuplicateAnnouncementViewData(announcementId
                , classes, selectedClassId, type);
            var res = new ria.async.DeferredData(addDupAnnModel);
            return this.ShadeView(chlk.activities.announcement.AddDuplicateAnnouncementDialog, res);
        },

        [[chlk.models.announcement.AddDuplicateAnnouncementViewData]],
        function duplicateAction(model){
            var classesIds = this.getIdsList(model.getSelectedIds(), chlk.models.id.ClassId);
            var method = model.getType() == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT ? this.classAnnouncementService.duplicateAnnouncement :
                this.lessonPlanService.duplicateLessonPlan;
            var res = method(model.getAnnouncementId(), model.getSelectedIds())
                .attach(this.validateResponse_())
                .thenCall(this.classService.updateClassAnnouncementTypes, [classesIds])
                .then(function(data){
                    this.BackgroundCloseView(chlk.activities.announcement.AddDuplicateAnnouncementDialog);
                    this.BackgroundNavigate('announcement', 'edit', [model.getAnnouncementId(), model.getType()]);
                    return ria.async.BREAK;
                }, this);
            return null;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
        function starAction(id, complete_, type_){
            this.announcementService
                .checkItem(id, complete_, type_)
                .attach(this.validateResponse_());
            return null;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
        function starFromStudentGradesAction(id, complete_, type_){
            return this.starAction(id, complete_, type_);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function makeVisibleAction(id, type){
            this[type == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT ? 'classAnnouncementService' :
                    (type == chlk.models.announcement.AnnouncementTypeEnum.ADMIN ? 'adminAnnouncementService' :
                    (type == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN ? 'lessonPlanService' : 'supplementalAnnouncementService'))]
                .makeVisible(id)
                .attach(this.validateResponse_());
            return null;
        },

        [[chlk.models.announcement.QnAForm]],
        function askQuestionAction(model) {
            var ann = this.announcementQnAService
                .askQuestion(model.getAnnouncementId(), model.getQuestion())
                .attach(this.validateResponse_())
                .catchError(this.handleNoAnnouncementException_, this);
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, ann, 'update-qna');
        },

        [[chlk.models.announcement.QnAForm]],
        function answerQuestionAction(model) {
            var ann;
            if (model.getQuestion()){
                var updateType = model.getUpdateType();
                if(updateType ==  "editQuestion"){
                    ann = this.announcementQnAService
                        .editQuestion(model.getId(), model.getQuestion())
                        .catchError(this.handleNoAnnouncementException_, this)
                        .attach(this.validateResponse_());
                }
                if(updateType == "answer"){
                    ann = this.announcementQnAService
                        .answerQuestion(model.getId(), model.getQuestion(), model.getAnswer())
                        .catchError(this.handleNoAnnouncementException_, this)
                        .attach(this.validateResponse_());
                }
                if(updateType == "editAnswer"){
                    ann = this.announcementQnAService
                        .editAnswer(model.getId(),  model.getAnswer())
                        .catchError(this.handleNoAnnouncementException_, this)
                        .attach(this.validateResponse_());
                }
            }
            else
                ann = this.announcementQnAService
                    .deleteQnA(model.getAnnouncementId(), model.getId())
                    .catchError(this.handleNoAnnouncementException_, this)
                    .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, ann, 'update-qna');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        function addStandardsAction(data){
            var options = this.getContext().getSession().get(ChlkSessionConstants.ATTACH_OPTIONS, null);
            this.getContext().getSession().remove(ChlkSessionConstants.STANDARD_BREADCRUMBS);
            this.getContext().getSession().remove(ChlkSessionConstants.STANDARD_LAST_ARGUMENTS);

            var res = this.announcementService.addStandards(options.getAnnouncementId(), data.standardIds ? data.standardIds.split(',').filter(function(item){return item}) : [])
                .then(function(announcement){
                    this.BackgroundCloseView(chlk.activities.announcement.AddStandardsDialog);
                    this.saveStandardIds(announcement);
                    this.prepareAttachments(announcement);
                    this.getListOfAppRecommendedContents_(announcement);
                    this.BackgroundUpdateView(this.getAnnouncementFormPageType_(), announcement, 'update-standards-and-suggested-apps');

                    return ria.async.BREAK;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
        function removeStandardAction(announcementId, standardId){
            var res = this.announcementService.removeStandard(announcementId, standardId)
                .then(function(announcement){
                    this.saveStandardIds(announcement);
                    //return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                    this.prepareAttachments(announcement);

                    this.getListOfAppRecommendedContents_(announcement);

                    return announcement;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(this.getAnnouncementFormPageType_(), res, 'update-standards-and-suggested-apps');
        },

        function attributeAttachmentExistsAction() {
            this.showAttributesFilesUploadMsg_();
            return null;
        },


        //******************** CLASS ANNOUNCEMENT **************************


        [[chlk.models.announcement.AnnouncementForm]],
        function classAnnouncementFromModel_(model) {
            this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
            return this.PushView(this.getAnnouncementFormPageType_(), ria.async.DeferredData(model, 300));
        },


        //******************** LESSON PLAN **************************


        [[chlk.models.announcement.AnnouncementForm]],
        function lessonPlanFromModel_(model) {
            var result = this.getLessonPlanFromModel_(model);
            return this.PushView(chlk.activities.announcement.LessonPlanFormPage, result);
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function getLessonPlanFromModel_(model) {
            return this.lpGalleryCategoryService.list()
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(list){
                    this.lpGalleryCategoryService.cacheLessonPlanCategories(list);
                    this.cacheLessonPlanClassId(model.getAnnouncement().getLessonPlanData().getClassId());
                    model.getAnnouncement().setCategories(list);
                    return model;
                },this)
                .attach(this.validateResponse_());
        },


        //******************** SUPPLEMENTAL ANNOUNCEMENT **************************


        [[chlk.models.announcement.AnnouncementForm]],
        function supplementalAnnouncementFromModel_(model) {
            this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.SUPPLEMENTAL_ANNOUNCEMENT);
            var result = this.getSupplementalAnnouncementFromModel_(model);
            return this.PushView(this.getAnnouncementFormPageType_(), result);
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function getSupplementalAnnouncementFromModel_(model) {
            var classId = model.getAnnouncement().getSupplementalAnnouncementData().getClassId();
            return this.studentService.getStudents(classId, null, true, true, 0, 999)
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(students){
                    model.setStudents(students.getItems());
                    return model;
                },this)
                .attach(this.validateResponse_());
        }


        //******************** ADMIN ANNOUNCEMENT **************************

    ])
});
