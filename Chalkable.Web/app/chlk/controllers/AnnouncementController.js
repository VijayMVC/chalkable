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
REQUIRE('chlk.activities.announcement.LessonPlanFormDialog');
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
REQUIRE('chlk.activities.announcement.AnnouncementChatPage');

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

REQUIRE('chlk.models.announcement.StudentAnnouncementApplicationMeta');

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
                        this.userTrackingService.importActivities();
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

        function getAnnouncementFormPageType_(type_, isDialog_){
            var announcementType = type_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf();

            if(isDialog_)
                return chlk.activities.announcement.LessonPlanFormDialog;

            if(this.userInRole(chlk.models.common.RoleEnum.TEACHER)){
                if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT)
                    return chlk.activities.announcement.AnnouncementFormPage;

                if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.SUPPLEMENTAL_ANNOUNCEMENT)
                    return chlk.activities.announcement.SupplementalAnnouncementFormPage;

                return chlk.activities.announcement.LessonPlanFormPage;
            }

            if(this.userInRole(chlk.models.common.RoleEnum.DISTRICTADMIN)){
                if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN)
                    return chlk.activities.announcement.LessonPlanFormDialog;

                return chlk.activities.announcement.AdminAnnouncementFormPage;
            }


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
            var needToHideLoader = true;
            var result = this.setAnnouncementGrade(model)
                .catchError(function (error) {
                    var announcement = this.getCachedAnnouncement();
                    this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementViewPage, announcement);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(currentItem){
                    var announcement = this.getCachedAnnouncement(),
                    needToHideLoader = false;
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


        [[chlk.models.announcement.SubmitDroppedAnnouncementViewData]],
        function setAnnouncementDroppedAction(model){
            var res = (model.isDropped()
                    ? this.classAnnouncementService.dropAnnouncement(model.getAnnouncementId())
                    : this.classAnnouncementService.unDropAnnouncement(model.getAnnouncementId())
            )
                .attach(this.validateResponse_())
                .then(function(data){
                    var announcement = this.getCachedAnnouncement();
                    announcement.getStudentAnnouncements().getItems().forEach(function(item){
                        item.setAutomaticalyDropped(model.isDropped());
                    });
                    announcement.calculateGradesAvg();
                    this.cacheAnnouncement(announcement);
                    return announcement.getStudentAnnouncements();
                }, this);
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, res, '');
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
        function addClassAnnouncementAction(classId_, announcementTypeId_, date_, noDraft_) {
            this.getView().reset();
            this.getContext().getSession().set('classInfo', null);
            this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
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
            var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
            var result = this.classAnnouncementService
                .addClassAnnouncement(classId_, announcementTypeId_, date_)
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(model){
                    if(model && model.getAnnouncement()){
                        var announcement = model.getAnnouncement();
                        var classAnnouncement = announcement.getClassAnnouncementData();
                        if(noDraft_){
                            classAnnouncement.setClassId(classId_ || null);
                            classAnnouncement.setAnnouncementTypeId(announcementTypeId_ || null);
                        }
                        if(!classAnnouncement.getClassId() || !classAnnouncement.getClassId().valueOf())
                            classAnnouncement.setAnnouncementTypeId(null);
                        if(date_){
                            classAnnouncement.setExpiresDate(date_);
                        }
                        return this.addEditAction(model, false);
                    }
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, schoolYear);
                },this)
                .attach(this.validateResponse_());
            return this.PushView(this.getAnnouncementFormPageType_(), result);
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
                    var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
                    var res = this.classAnnouncementFromModel_(chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, schoolYear));
                    return res;
                },this);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.apps.Application, Boolean]],
        function getAppRecommendedContents_(ann, app, isDialog_){
            if(ann.getStandards().length > 0)
                var emptyModel = new  chlk.models.apps.AppContentListViewData();
                this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType(), isDialog_), emptyModel,  'before-app-contents-loaded');
                this.applicationService.getApplicationContents(
                        app.getUrl(),
                        ann.getId(),
                        ann.getType(),
                        ann.getStandards(),
                        app.getEncodedSecretKey())
                    //.attach(this.validateResponse_())
                    .catchError(function(e){
                        this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType(), isDialog_), null, 'app-contents-fail');
                        throw e;
                    }, this)
                    .then(function(paginatedContents){

                        if(paginatedContents.getItems() && paginatedContents.getItems().length > 0){

                           var res = chlk.models.apps.AppContentListViewData(app, ann.getId(), ann.getType()
                               , ann.getClassId(), paginatedContents, ann.getStandards());

                           this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType(), isDialog_), res, 'update-app-contents');
                        }
                        else
                            this.BackgroundUpdateView(this.getAnnouncementFormPageType_(ann.getType(), isDialog_), null, 'app-contents-fail');
                    }, this);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, Boolean]],
        function getListOfAppRecommendedContents_(announcement, isDialog_){
            if(announcement.getStandards().length > 0)
                (announcement.getAppsWithContent() || [])
                    .forEach(function(app) {
                        this.getAppRecommendedContents_(announcement, app, isDialog_);
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

        [[chlk.models.announcement.AnnouncementForm]],
        function classAnnouncementFromModel_(model) {
            this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
            return this.PushView(this.getAnnouncementFormPageType_(), ria.async.DeferredData(model, 300));
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function getSupplementalAnnouncementFromModel_(model) {
            var classId = model.getAnnouncement().getSupplementalAnnouncementData().getClassId();
            return this.studentService.getStudents(classId, null, true, true, 0, 999,true)
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
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function supplementalAnnouncementFromModel_(model) {
            this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.SUPPLEMENTAL_ANNOUNCEMENT);
            var result = this.getSupplementalAnnouncementFromModel_(model);
            return this.PushView(this.getAnnouncementFormPageType_(), result);
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
                    this.cacheLessonPlanClassId(model.getAnnouncement().getLessonPlanData().getClassId());
                    model.getAnnouncement().setCategories(list);
                    return model;
                },this)
                .attach(this.validateResponse_());
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function lessonPlanFromModel_(model) {
            var result = this.getLessonPlanFromModel_(model);
            return this.PushView(chlk.activities.announcement.LessonPlanFormPage, result);
        },

        [chlk.controllers.NotChangedSidebarButton],
        function lessonPlanFromGalleryAction() {
            var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
            var result = ria.async.wait([
                    this.lessonPlanService.addLessonPlan(),
                    this.lpGalleryCategoryService.list()
                ])
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    if(model && model.getAnnouncement()){
                        var announcement = model.getAnnouncement();
                        var type = announcement.getType();
                        this.cacheAnnouncementType(type);
                        this.updateAttributesWithFilesList_(announcement);
                        this.prepareAnnouncementAttachedItems(announcement);
                        this.getListOfAppRecommendedContents_(announcement, true);

                        model.getAnnouncement().setCategories(result[1]);
                        model.setSchoolYear(schoolYear);
                        return model;
                    }
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, schoolYear);
                },this)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.announcement.LessonPlanFormDialog, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number]],
        function lessonPlanAction(classId_) {
            this.getView().reset();
            this.getContext().getSession().set('classInfo', null);
            var result = ria.async.wait([
                    this.lessonPlanService.addLessonPlan(classId_),
                    this.lpGalleryCategoryService.list()
                ])
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    var resModel =  this.addEditAction(model, false);
                    resModel.getAnnouncement().setCategories(result[1]);
                    return resModel;
                },this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.announcement.LessonPlanFormPage, result);
        },

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
        function supplementalAnnouncementAction(classId_, date_, studentIds_) {
            this.getView().reset();
            this.getContext().getSession().set('classInfo', null);
            var result =
                ria.async.wait(
                    this.supplementalAnnouncementService.create(classId_, date_),
                    this.studentService.getStudents(classId_, null, true, true, 0, 999, true)
                )
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .then(function(result){
                    var model = result[0];
                    var students = result[1];
                    model.setStudents(students.getItems());

                    if(studentIds_){
                        if(!Array.isArray(studentIds_))
                            studentIds_ = studentIds_.split(',');
                        var ids = studentIds_.map(function(id){return new chlk.models.id.SchoolPersonId(id)});
                        model.getAnnouncement().getSupplementalAnnouncementData().setSelectedStudentsIds(ids);
                    }

                    if(model && model.getAnnouncement())
                        return this.addEditAction(model, false);

                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                    var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, schoolYear);
                },this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.announcement.SupplementalAnnouncementFormPage, result);
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
            String,
            Boolean
        ]],
        function attachAction(announcementId, classId, announcementTypeName, announcementType, appUrlAppend_, isDialog_) {

            var result = this.announcementService.getAttachSettings(announcementId, announcementType)
                .then(function(options){
                    //_DEBUG && options.setAssessmentAppId(chlk.models.id.AppId('56c14655-2897-4073-bb48-32dfd61264b5'));

                    options.updateByValues(null, null, announcementId, classId, announcementTypeName,
                        announcementType, null, appUrlAppend_, null, isDialog_);
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

                    options.updateByValues(null, false, announcementId, null, null, announcementType);
                    this.getContext().getSession().set(ChlkSessionConstants.ATTACH_OPTIONS, options);
                    return new chlk.models.common.BaseAttachViewData(options);
                }, this);

            return this.ShadeOrUpdateView(chlk.activities.announcement.AttachFilesDialog, result);
        },

        //todo move attribute methods to separate controller

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function fetchAddAttributeFuture_(announcementId, announcementType, isDialog_) {
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
                    isDialog_ && attribute.setDialog(isDialog_);
                    return attribute;
                }, this);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function addAttributeTeacherAction(announcementId, announcementType, isDialog_) {
            this.BackgroundCloseView(chlk.activities.apps.AttachAppsDialog);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_),
                this.fetchAddAttributeFuture_(announcementId, announcementType, isDialog_), 'add-attribute');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function addAttributeDistrictAdminAction(announcementId, announcementType, isDialog_) {
            this.BackgroundCloseView(chlk.activities.apps.AttachAppsDialog);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_),
                this.fetchAddAttributeFuture_(announcementId, announcementType, isDialog_), 'add-attribute');
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
        function attachFromCabinetAction(announcementId, announcementType, attachmentId, isDialog_){
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
            return this.UpdateView(!isStudent ? this.getAnnouncementFormPageType_(announcementType, isDialog_) : chlk.activities.announcement.AnnouncementViewPage, res, 'update-attachments');

        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId, Boolean]],
        function attachFromCabinetToAttributeAction(announcementId, announcementType, attachmentId, assignedAttributeId, isDialog_){
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
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_), res, 'add-attribute-attachment');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId, Boolean]],
        function cloneFromCabinetAction(announcementId, announcementType, attachmentId, isDialog_){
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
            return this.UpdateView(!isStudent ? this.getAnnouncementFormPageType_(announcementType, isDialog_) : chlk.activities.announcement.AnnouncementViewPage, res, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId, Boolean]],
        function cloneFromCabinetToAttributeAction(announcementId, announcementType, attachmentId, assignedAttributeId, isDialog_){
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
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_), res, 'add-attribute-attachment');
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
        [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function removeAttributeTeacherAction(announcementId, attributeId, announcementType, isDialog_) {
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_),
                this.fetchRemoveAttributeFuture_(announcementId, attributeId, announcementType),  'remove-attribute');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function removeAttributeDistrictAdminAction(announcementId, attributeId, announcementType, isDialog_) {
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_),
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

            this.prepareCommentsAttachments_(announcement.getAnnouncementComments());
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
                    announcement = this.prepareAnnouncementForView(announcement);
                    this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_FOR_QNAS, announcement);
                    return announcement;
                }, this);
            return this.PushView(chlk.activities.announcement.AnnouncementViewPage, result);
        },

        function chatAction() {
            var announcement = this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_FOR_QNAS, null);
            if(announcement.getType() != chlk.models.announcement.AnnouncementTypeEnum.ADMIN){
                var clazz = this.classService.getClassById(announcement.getClassId());
                announcement.setClazz(clazz);
            }
            return this.StaticView(chlk.activities.announcement.AnnouncementChatPage, ria.async.DeferredData(announcement, 100));
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId]],
        function viewTemplateAction(announcementId) {
            var announcementType = chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN;

            var result = ria.async.wait([
                    this.announcementService.editAnnouncement(announcementId, announcementType),
                    this.lpGalleryCategoryService.list()
                ])
                .then(function(result){
                    var model = result[0];
                    var announcement = model.getAnnouncement();
                    var type = announcement.getType();
                    this.cacheAnnouncementType(type);
                    this.updateAttributesWithFilesList_(announcement);
                    this.prepareAnnouncementAttachedItems(announcement);
                    this.getListOfAppRecommendedContents_(announcement, true);

                    model.getAnnouncement().setCategories(result[1]);
                    return model;
                },this)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.announcement.LessonPlanFormDialog, result);
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
                    announcement = this.prepareAnnouncementForView(announcement);
                    this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_FOR_QNAS, announcement);
                    return announcement;
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

            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'remove-attribute-attachment');
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
                    /*attribute.setAnnouncementType(announcementType);
                    this.prepareAttribute(attribute, 51, 33);
                    var attachment = attribute.getAttributeAttachment();

                    var model = new chlk.models.attachment.AnnouncementAttachment(0, null, null, files[0].name);
                    model.setId(new chlk.models.id.AnnouncementAttachmentId(attribute.getId().valueOf()));
                    model.setThumbnailUrl(attachment.getThumbnailUrl());
                    model.setAnnouncementId(announcementId);
                    model.setAnnouncementType(announcementType);
                    model.setAttributeAttachment(true);
                    model.setTotal(files[0].size);
                    return model;*/
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

        [[chlk.models.id.AnnouncementId]],
        function applyAutoGradeAction(announcementId){
            var result = this.gradingService
                .applyAutoGrade(announcementId)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementViewPage, result, 'attachment-uploaded');
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
                color: chlk.models.common.ButtonColor.RED.valueOf() }, {
                text: Msg.Cancel.toUpperCase(),
                color: chlk.models.common.ButtonColor.GREEN.valueOf()
            }]), null;
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

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function refreshAttachmentsAction(announcementId, announcementType_, isDialog_) {
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

            return this.UpdateView(this.getAnnouncementFormPageType_(null, isDialog_), result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId, Boolean]],
        function refreshAttributeAction(announcementId, announcementType, assignedAttributeId, isDialog_) {
            var result = this.announcementService
                .getAnnouncement(announcementId, announcementType)
                .attach(this.validateResponse_())
                .then(function (announcement) {
                    var attribute = announcement.getAnnouncementAttributes().filter(function(a){return a.getId() == assignedAttributeId})[0];
                    this.cacheAnnouncement(announcement);
                    return this.prepareAttributeData(announcementId, announcementType, assignedAttributeId, attribute);
                }, this);

            return this.UpdateView(this.getAnnouncementFormPageType_(null, isDialog_), result, 'add-attribute-attachment');
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

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function discardAction(announcementType, isDialog_) {
            this.disableAnnouncementSaving(true);
            var currentPersonId = this.getCurrentPerson().getId();
            isDialog_ && this.BackgroundCloseView(chlk.activities.announcement.LessonPlanFormDialog);
            var res =  this.announcementService
                .deleteDrafts(currentPersonId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    if(!isDialog_)
                        return this.Redirect('feed', 'list', [null, true]);
                }, this);

            if(isDialog_)
                return null;

            return res;
        },

        [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function deleteAppAction(announcementAppId, announcementType, isDialog_) {
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
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType, isDialog_), result, 'update-attachments');
        },

        [[chlk.models.id.AnnouncementApplicationId, chlk.models.announcement.AnnouncementTypeEnum, Boolean]],
        function deleteAppDistrictAdminAction(announcementAppId, announcementType, isDialog_) {
            var result = this.announcementService
                .deleteApp(announcementAppId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    this.prepareAnnouncementAttachedItems(model);
                    return model;
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType), result, 'update-attachments');
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

        [[String, chlk.models.id.ClassId, Number, chlk.models.id.SchoolPersonId]],
        function listLastAction(announcementTypeName, classId, announcementTypeId, schoolPersonId){
            var result = this.classAnnouncementService
                .listLast(classId, announcementTypeId,schoolPersonId)
                .attach(this.validateResponse_())
                .then(function(msgs){
                    return chlk.models.announcement.LastMessages.$create(announcementTypeName, msgs);
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.id.AnnouncementId, String]],
        function saveTitleAction(announcementId, announcementTitle){
            var result = this.classAnnouncementService
                .editTitle(announcementId, announcementTitle)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.announcement.FeedAnnouncementViewData();
                });
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
        },

        [[String, chlk.models.id.ClassId, chlk.models.common.ChlkDate, chlk.models.id.AnnouncementId]],
        function checkTitleAction(title, classId, expiresdate, annoId){
            var res = this.classAnnouncementService
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

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function prepareAnnouncementForm_(announcement){
            announcement.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
            announcement.setApplications(this.getCachedAnnouncementApplications());
            announcement.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
            var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
            return chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(announcement, schoolYear);
        },

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveAnnouncementFormAction(announcement){
            if(!announcement.getAnnouncementTypeId() || !announcement.getAnnouncementTypeId().valueOf())
                return this.Redirect('error', 'createAnnouncementError', []);
            var announcementForm = this.prepareAnnouncementForm_(announcement);
            return this.saveAnnouncementTeacherAction(announcement, announcementForm);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveDistrictAdminAction(model){
            var submitType = model.getSubmitType();

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                this.adminAnnouncementService
                    .saveAdminAnnouncement(
                        model.getId(),
                        model.getTitle(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getAssignedAttributesPostData()
                    )
                    .attach(this.validateResponse_());
                return null;
            }

            if (submitType == 'saveTitle'){
                return this.saveAdminTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle'){
                return this.checkAdminTitleAction(model.getTitle(), model.getId());
            }

            var res = this.adminAnnouncementService
                .submitAdminAnnouncement(
                    model.getId(),
                    model.getContent(),
                    model.getTitle(),
                    model.getExpiresDate(),
                    model.getAssignedAttributesPostData()
                )
                .attach(this.validateResponse_());

            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    if(model.getSubmitType() == 'submitOnEdit')
                        return this.BackgroundNavigate('announcement', 'view', [model.getId(), chlk.models.announcement.AnnouncementTypeEnum.ADMIN]);
                    else{
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
            return null;
        },

        [[chlk.models.id.AnnouncementId, String]],
        function saveAdminTitleAction(announcementId, announcementTitle){
            var result = this.adminAnnouncementService
                .editTitle(announcementId, announcementTitle)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.announcement.AnnouncementAttributeListViewData();
                });
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[String, chlk.models.id.AnnouncementId]],
        function checkAdminTitleAction(title, annoId){
            var res = this.adminAnnouncementService
                .existsTitle(title, annoId)
                .attach(this.validateResponse_())
                .then(function(success){
                    return new chlk.models.Success(success);
                });
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, res, chlk.activities.lib.DontShowLoader());
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function onLessonPlanSaveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var submitType = model.getSubmitType();
            var classId = model.getClassId();

            model.setMarkingPeriodId(this.getCurrentMarkingPeriod().getId());

            if (submitType == 'listLast'){
                return this.listLastLessonPlanAction(classId);
            }

            //TODO CONTINUE.....

            if (submitType == 'viewImported'){
                this.getContext().getSession().set(ChlkSessionConstants.CREATED_ANNOUNCEMENTS, model.getCreatedAnnouncements());
                return this.Redirect('feed', 'viewImported', [model.getClassId()]);
            }

            if (submitType == 'save'){
                model.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
                model.setApplications(this.getCachedAnnouncementApplications());
                model.setCategories(this.lpGalleryCategoryService.getLessonPlanCategoriesSync());
                model.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
                var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
                var announcementForm =  chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(model, schoolYear);
                return this.saveLessonPlanAction(model, announcementForm);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.saveLessonPlanAction(model);
            }

            if (submitType == 'createFromTemplate'){
                return this.Redirect('announcement', 'createFromTemplate', [model.getAnnouncementForTemplateId(), classId]);
            }

            var res = this.submitLessonPlan(model);
            if(res){
                res = res.then(function(saved){
                    if(saved){
                        this.cacheAnnouncement(null);
                        this.lpGalleryCategoryService.emptyLessonPlanCategoriesCache();
                        this.cacheLessonPlanClassId(null);
                        this.cacheAnnouncementAttachments(null);
                        this.cacheAnnouncementApplications(null);
                        this.cacheAnnouncementAttributes(null);
                        if(submitType == 'submitOnEdit')
                            this.BackgroundNavigate('announcement', 'view', [model.getId(), chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN]);
                        else{
                            this.BackgroundNavigate('feed', 'list', [null, true]);
                        }
                        return ria.async.BREAK;
                    }
                }, this);
                return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, res);
            }
            return null;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function onLessonPlanTemplateSaveAction(model) {
            var submitType = model.getSubmitType();

            if (submitType == 'saveNoUpdate'){
                return this.saveLessonPlanAction(model);
            }

            var res = this.submitLessonPlan(model)
                .attach(this.validateResponse_())
                .then(function(model){
                    this.BackgroundNavigate('lessonplangallery', 'gallery');
                    return ria.async.BREAK;
                }, this);

            return this.UpdateView(chlk.activities.announcement.LessonPlanFormDialog, res);
        },

        [[chlk.models.id.AnnouncementId, String]],
        function saveSupplementalTitleAction(announcementId, announcementTitle){
            var result = this.supplementalAnnouncementService
                .editTitle(announcementId, announcementTitle)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.announcement.FeedAnnouncementViewData();
                });
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, chlk.activities.lib.DontShowLoader());
        },

        [[String, chlk.models.id.AnnouncementId]],
        function checkSupplementalTitleAction(title, annoId){
            var res = this.supplementalAnnouncementService
                .existsTitle(title, annoId)
                .attach(this.validateResponse_())
                .then(function(success){
                    return new chlk.models.Success(success);
                });
            return this.UpdateView(this.getAnnouncementFormPageType_(), res, chlk.activities.lib.DontShowLoader());
        },

        function saveSupplementalAnnouncement_(model){
            return this.supplementalAnnouncementService
                .saveSupplementalAnnouncement(
                    model.getId(),
                    model.getClassId(),
                    model.getTitle(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.isHiddenFromStudents(),
                    model.getAssignedAttributesPostData(),
                    model.getRecipientIds(),
                    model.isDiscussionEnabled(),
                    model.isPreviewCommentsEnabled(),
                    model.isRequireCommentsEnabled(),
                    model.getAnnouncementTypeId()
                )
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.announcement.AnnouncementForm]],
        function saveSupplementalAnnouncementAction(model, form_) {
            if(!(model.getClassId() && model.getClassId().valueOf()))
                return null;

            var res;

            if(form_)
                res = ria.async.wait([this.saveSupplementalAnnouncement_(model), this.studentService.getStudents(model.getClassId(), null, true, true, 0, 999)]);
            else
                res = this.saveSupplementalAnnouncement_(model);


           res = res
                .attach(this.validateResponse_())
                .then(function(result){
                    if (form_){
                        var model = result[0];
                        var students = result[1].getItems();
                        var applications = model.getApplications() || [];
                        this.cacheAnnouncementApplications(applications);

                        var announcement = form_.getAnnouncement();
                        announcement.setSupplementalAnnouncementData(model.getSupplementalAnnouncementData());
                        announcement.setTitle(model.getTitle());
                        announcement.setApplications(applications);
                        announcement.setCanAddStandard(model.isCanAddStandard());
                        announcement.setStandards(model.getStandards());
                        announcement.setAnnouncementAttributes(model.getAnnouncementAttributes());
                        announcement.setGradingStudentsCount(model.getGradingStudentsCount());
                        announcement.setAbleToRemoveStandard(model.isAbleToRemoveStandard());
                        announcement.setSuggestedApps(model.getSuggestedApps());
                        announcement.setAppsWithContent(model.getAppsWithContent());
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setState(model.getState());
                        form_.setAnnouncement(announcement);
                        form_.setStudents(students);
                        return this.addEditAction(form_, false);
                    }
                }, this)
                .attach(this.validateResponse_());

            if (form_)
                return this.UpdateView(chlk.activities.announcement.SupplementalAnnouncementFormPage, res);
            return null;
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, Boolean]],
        function submitSupplementalAnnouncement(model, isEdit){
            if(!model.getRecipientIds()){
                this.ShowMsgBox('Please select recipients', 'whoa.');
                return null;
            }

            var res = this.supplementalAnnouncementService
                .submitSupplementalAnnouncement(
                    model.getId(),
                    model.getClassId(),
                    model.getTitle(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.isHiddenFromStudents(),
                    model.getAssignedAttributesPostData(),
                    model.getRecipientIds(),
                    model.isDiscussionEnabled(),
                    model.isPreviewCommentsEnabled(),
                    model.isRequireCommentsEnabled(),
                    model.getAnnouncementTypeId()
                )
                .attach(this.validateResponse_());

            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    this.cacheAnnouncementAttributes(null);
                    if(isEdit)
                        return this.BackgroundNavigate('announcement', 'view', [model.getId(), chlk.models.announcement.AnnouncementTypeEnum.SUPPLEMENTAL_ANNOUNCEMENT]);
                    else{
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
            return res;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function onSupplementalAnnouncementSaveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var submitType = model.getSubmitType();

            model.setMarkingPeriodId(this.getCurrentMarkingPeriod().getId());

            if (submitType == 'listLast'){
                return null;
            }

            if (submitType == 'saveTitle'){
                return this.saveSupplementalTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle'){
                return this.checkSupplementalTitleAction(model.getTitle(), model.getId());
            }

            if (submitType == 'viewImported'){
                this.getContext().getSession().set(ChlkSessionConstants.CREATED_ANNOUNCEMENTS, model.getCreatedAnnouncements());
                return this.Redirect('feed', 'viewImported', [model.getClassId()]);
            }

            if (submitType == 'save'){
                model.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
                model.setApplications(this.getCachedAnnouncementApplications());
                model.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
                var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
                var announcementForm =  chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(model, schoolYear);
                return this.saveSupplementalAnnouncementAction(model, announcementForm);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.saveSupplementalAnnouncementAction(model);
            }

            if (submitType == 'save'){
                model.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
                model.setApplications(this.getCachedAnnouncementApplications());
                model.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
                var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
                var announcementForm =  chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(model, schoolYear);
                return this.saveSupplementalAnnouncementAction(model, announcementForm);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.saveSupplementalAnnouncementAction(model);
            }

            if(this.submitSupplementalAnnouncement(model, submitType == 'submitOnEdit'))
                return this.ShadeLoader();

            return null;
        },

        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function createFromTemplateAction(announcementId, classId_){
            var result = ria.async.wait([
                    this.lessonPlanService.createFromTemplate(announcementId, classId_),
                    this.lpGalleryCategoryService.list()
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    if(model && model.getAnnouncement()){
                        var resModel =  this.addEditAction(model, false);
                        resModel.getAnnouncement().setCategories(result[1]);
                        this.cacheLessonPlanClassId(resModel.getAnnouncement().getLessonPlanData().getClassId());
                        return resModel;
                    }
                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                    var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, schoolYear);
                }, this);
            return this.PushView(chlk.activities.announcement.LessonPlanFormPage, result);
        },

        [[chlk.models.id.ClassId]],
        function listLastLessonPlanAction(classId){
            var result = this.lessonPlanService
                .listLast(classId)
                .attach(this.validateResponse_())
                .then(function(msgs){
                    return chlk.models.announcement.LastMessages.$create(Msg.Lesson_Plan, msgs);
                }, this);
            return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function checkLessonPlanTitle_(model){
            var res = this.lessonPlanService
                .getLessonPlanTemplateByTitle(model.getTitle())
                .attach(this.validateResponse_())
                .then(function(lpInGallery){

                    var success = !!lpInGallery;
                    if(lpInGallery){
                        if(lpInGallery.isAnnOwner() || this.getCurrentPerson().hasPermission(chlk.models.people.UserPermissionEnum.CHALKABLE_ADMIN)){
                            return this.ShowMsgBox('You are replacing the existing lesson plan \- do you want to continue ?', null,
                                    [{text: 'NO', clazz: 'blue-button'},
                                    {text: 'Continue', value: 'ok'}]
                                )
                                .then(function(msResult){
                                    if(msResult){
                                        return this.lessonPlanService.replaceLessonPlanInGallery(lpInGallery.getId(), model.getId())
                                                   .attach(this.validateResponse_())
                                                   .then(function(data){return new chlk.models.Success(!data)});
                                    }
                                    return msResult;
                                }, this);
                        }
                        else{
                            this.ShowMsgBox('There is Lesson Plan with that title in gallery', 'whoa.');
                            return null;
                        }
                    }
                    return new chlk.models.Success(success);
                }, this);

            return res;
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.announcement.AnnouncementForm]],
        function saveLessonPlanAction(model, form_) {
            var res = this.lessonPlanService
                .saveLessonPlan(
                    model.getId(),
                    model.getClassId(),
                    model.getTitle(),
                    model.getContent(),
                    model.getGalleryCategoryId(),
                    model.getStartDate(),
                    model.getEndDate(),
                    model.isHiddenFromStudents(),
                    model.getAssignedAttributesPostData(),
                    model.isDiscussionEnabled(),
                    model.isPreviewCommentsEnabled(),
                    model.isRequireCommentsEnabled(),
                    model.isInGallery()

                )
                .attach(this.validateResponse_())
                .then(function(model){
                    if (form_){
                        var applications = model.getApplications() || [];
                        this.cacheAnnouncementApplications(applications);
                        this.cacheLessonPlanClassId(model.getLessonPlanData().getClassId());
                        var announcement = form_.getAnnouncement();
                        announcement.setLessonPlanData(model.getLessonPlanData());
                        announcement.setTitle(model.getTitle());
                        announcement.setApplications(applications);
                        announcement.setCanAddStandard(model.isCanAddStandard());
                        announcement.setStandards(model.getStandards());
                        announcement.setAnnouncementAttributes(model.getAnnouncementAttributes());
                        announcement.setGradingStudentsCount(model.getGradingStudentsCount());
                        announcement.setAbleToRemoveStandard(model.isAbleToRemoveStandard());
                        announcement.setSuggestedApps(model.getSuggestedApps());
                        announcement.setAppsWithContent(model.getAppsWithContent());
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setState(model.getState());
                        announcement.setDiscussionEnabled(model.isDiscussionEnabled());
                        announcement.setPreviewCommentsEnabled(model.isPreviewCommentsEnabled());
                        announcement.setRequireCommentsEnabled(model.isRequireCommentsEnabled());

                        //announcement.setClassName(model.getLessonPlanData().getClassName());
                        form_.setAnnouncement(announcement);
                        return this.addEditAction(form_, false);
                    }
                }, this)
                .attach(this.validateResponse_());

            if (form_)
                return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, res);
            return null;
        },

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function submitLessonPlan(model){
            if(model.getStartDate().compare(model.getEndDate()) == 1){
                this.ShowMsgBox('Lesson Plan is not valid. Start date is greater the end date', 'whoa.');
                return null;
            }

            if(model.isInGallery() && !(model.getGalleryCategoryId() && model.getGalleryCategoryId().valueOf())){
                this.ShowMsgBox('Cannot create lesson plan template without category!', 'whoa.');
                return null;
            }

            var gradingPeriods = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIODS, []);
            var schoolYear = this.getContext().getSession().get(ChlkSessionConstants.SCHOOL_YEAR, []);

            if((!model.isInGallery() && !gradingPeriods.filter(function(_){return model.getStartDate().getDate() >= _.getStartDate().getDate() && model.getStartDate().getDate() <= _.getEndDate().getDate()
                    && model.getEndDate().getDate() >= _.getStartDate().getDate() && model.getEndDate().getDate() <= _.getEndDate().getDate()}).length) ||
                (model.isInGallery() && !(model.getStartDate().getDate() >= schoolYear.getStartDate().getDate() && model.getStartDate().getDate() <= schoolYear.getEndDate().getDate()
                && model.getEndDate().getDate() >= schoolYear.getStartDate().getDate() && model.getEndDate().getDate() <= schoolYear.getEndDate().getDate()))){
                this.ShowMsgBox('Lesson Plan is not valid. Start date and End date can\'t be in different grading periods', 'whoa.');
                return null;
            }

            var res = this.checkLessonPlanTitle_(model)
                .then(function(result){
                    return result ? this.lessonPlanService.submitLessonPlan(model.getId(),
                        model.getClassId(),
                        model.getTitle(),
                        model.getContent(),
                        model.getGalleryCategoryId(),
                        model.getStartDate(),
                        model.getEndDate(),
                        model.isHiddenFromStudents(),
                        model.getAssignedAttributesPostData(),
                        model.isDiscussionEnabled(),
                        model.isPreviewCommentsEnabled(),
                        model.isRequireCommentsEnabled(),
                        model.isInGallery()) : ria.async.BREAK;
                }, this)
                .attach(this.validateResponse_());

            return res;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var session = this.getContext().getSession();
            var submitType = model.getSubmitType();
            var schoolPersonId = model.getPersonId();
            var announcementTypeId = model.getAnnouncementTypeId();
            var announcementTypeName = model.getAnnouncementTypeName();
            var classId = model.getClassId();

            if(!announcementTypeId)
                return this.Redirect('announcement', 'lessonPlan', [classId]);

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

            if (submitType == 'viewImported'){
                this.getContext().getSession().set(ChlkSessionConstants.CREATED_ANNOUNCEMENTS, model.getCreatedAnnouncements());
                return this.Redirect('feed', 'viewImported', [model.getClassId()]);
            }

            if (submitType == 'save'){
                return this.saveAnnouncementFormAction(model);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.saveAnnouncementTeacherAction(model);
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

        [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.announcement.AnnouncementForm]],
        function saveAnnouncementTeacherAction(model, form_) {
            if(!(model.getClassId() && model.getClassId().valueOf() && model.getAnnouncementTypeId() && model.getAnnouncementTypeId().valueOf()))
                return null;
            var res = this.classAnnouncementService
                .saveClassAnnouncement(
                    model.getId(),
                    model.getClassId(),
                    model.getAnnouncementTypeId(),
                    model.getTitle(),
                    model.getContent(),
                    model.getExpiresDate(),
                    model.getMaxScore(),
                    model.getWeightAddition(),
                    model.getWeightMultiplier(),
                    model.isHiddenFromStudents(),
                    model.isAbleDropStudentScore(),
                    model.getAssignedAttributesPostData(),
                    model.isDiscussionEnabled(),
                    model.isPreviewCommentsEnabled(),
                    model.isRequireCommentsEnabled()
                )
                .attach(this.validateResponse_())
                .then(function(model){
                    if (form_){
                        var applications = model.getApplications() || [];
                        this.cacheAnnouncementApplications(applications);

                        var announcement = form_.getAnnouncement();
                        announcement.setClassAnnouncementData(model.getClassAnnouncementData());
                        announcement.setTitle(model.getTitle());
                        announcement.setApplications(applications);
                        announcement.setCanAddStandard(model.isCanAddStandard());
                        announcement.setStandards(model.getStandards());
                        announcement.setAnnouncementAttributes(model.getAnnouncementAttributes());
                        announcement.setGradingStudentsCount(model.getGradingStudentsCount());
                        announcement.setAbleToRemoveStandard(model.isAbleToRemoveStandard());
                        announcement.setSuggestedApps(model.getSuggestedApps());
                        announcement.setAppsWithContent(model.getAppsWithContent());
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setState(model.getState());
                        announcement.setAbleUseExtraCredit(model.isAbleUseExtraCredit());
                        announcement.setDiscussionEnabled(model.isDiscussionEnabled());
                        announcement.setPreviewCommentsEnabled(model.isPreviewCommentsEnabled());
                        announcement.setRequireCommentsEnabled(model.isRequireCommentsEnabled());

                        //announcement.setClassName(model.getClassAnnouncementData().getClassName());
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
        [[chlk.models.announcement.FeedAnnouncementViewData, Boolean]],
        function submitAnnouncement(model, isEdit){
            var res;
            if(this.userInRole(chlk.models.common.RoleEnum.TEACHER))
                res = this.classAnnouncementService
                    .submitClassAnnouncement(
                        model.getId(),
                        model.getClassId(),
                        model.getAnnouncementTypeId(),
                        model.getTitle(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getMaxScore(),
                        model.getWeightAddition(),
                        model.getWeightMultiplier(),
                        model.isHiddenFromStudents(),
                        model.isAbleDropStudentScore(),
                        model.isGradable(),
                        model.getAssignedAttributesPostData(),
                        model.isDiscussionEnabled(),
                        model.isPreviewCommentsEnabled(),
                        model.isRequireCommentsEnabled()
                    )
                    .attach(this.validateResponse_());
            else
                res = this.adminAnnouncementService
                    .submitAdminAnnouncement(
                        model.getId(),
                        model.getContent(),
                        model.getTitle(),
                        model.getExpiresDate(),
                        model.getAssignedAttributesPostData()
                    )
                    .attach(this.validateResponse_());

            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    this.cacheAnnouncementAttributes(null);
                    if(isEdit)
                        return this.BackgroundNavigate('announcement', 'view', [model.getId(), chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT]);
                    else{
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
            return res;
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
                .then(function(model){
                    this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementViewPage, model, 'update-qna');
                    return model;
                }, this)
                .attach(this.validateResponse_())
                .catchError(this.handleNoAnnouncementException_, this);
            return this.UpdateView(chlk.activities.announcement.AnnouncementChatPage, ann);
        },

        function closeChatAction() {
            return this.CloseView(chlk.activities.announcement.AnnouncementChatPage);
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
            ann = ann.then(function(model){
                this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementViewPage, model, 'update-qna');
                return model;
            }, this);
            return this.UpdateView(chlk.activities.announcement.AnnouncementChatPage, ann);
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
        [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId, Boolean]],
        function removeStandardAction(announcementId, standardId, isDialog_){
            var res = this.announcementService.removeStandard(announcementId, standardId)
                .then(function(announcement){
                    this.saveStandardIds(announcement);
                    //return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                    this.prepareAttachments(announcement);

                    this.getListOfAppRecommendedContents_(announcement, isDialog_);

                    return announcement;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(this.getAnnouncementFormPageType_(null, isDialog_), res, 'update-standards-and-suggested-apps');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId]],
        function showGroupsAction(announcementId){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_ID, announcementId);
            var groupsIds = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_IDS, []).map(function (_) { return _.valueOf() });
            return this.Redirect('group', 'show', [{
                selected: groupsIds,
                controller: 'announcement',
                action: 'saveGroupsToAnnouncement',
                resultHidden: 'groupIds',
                hiddenParams: {
                    id: announcementId
                }
            }]);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveGroupsToAnnouncementAction(model){
            var groups = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_LIST, []);
            var groupIds = model.getGroupIds() ? model.getGroupIds().split(',') : [];
            groups = groups.filter(function(item){
                return groupIds.indexOf(item.getId().valueOf().toString()) > -1
            });
            var recipients = groups.map(function(item){
                return new chlk.models.announcement.AdminAnnouncementRecipient(model.getId(), item.getId(), item.getName());
            });
            model.setRecipients(recipients);
            this.adminAnnouncementService.addGroupsToAnnouncement(model.getId(), model.getGroupIds())
                .then(function(){
                    this.BackgroundCloseView(chlk.activities.announcement.AnnouncementGroupsDialog);
                }, this)
                .attach(this.validateResponse_());
            this.getContext().getSession().set(ChlkSessionConstants.GROUPS_IDS, model.getGroupIds() ? model.getGroupIds().split(',').map(function(item){return new chlk.models.id.GroupId(item)}) : []);
            this.BackgroundUpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, model, 'recipients');
            return this.ShadeLoader();
        },


        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, chlk.models.id.GroupId]],
        function removeRecipientAction(announcementId, recipientId){

            var res = this.ShowConfirmBox('Are you sure you want to remove that group from announcement?', '')
                .thenCall(this.groupService.list, [])
                .attach(this.validateResponse_())
                .then(function(groups){
                    var groupIds = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_IDS, []);
                    groupIds = groupIds.filter(function(id){
                        return id != recipientId
                    });
                    var groupIdsStr = groupIds.map(function(item){return item.valueOf()}).join(',');

                    groups = groups.filter(function(item){
                        return groupIds.indexOf(item.getId()) > -1
                    });

                    var recipients = groups.map(function(item){
                        return new chlk.models.announcement.AdminAnnouncementRecipient(announcementId, item.getId(), item.getName());
                    });
                    var model = new chlk.models.announcement.FeedAnnouncementViewData();
                    model.setId(announcementId);
                    model.setRecipients(recipients);
                    this.getContext().getSession().set(ChlkSessionConstants.GROUPS_IDS, groupIds);

                    return this.adminAnnouncementService.addGroupsToAnnouncement(model.getId(), groupIdsStr)
                        .then(function(){
                            return model;
                        }, this)
                }, this);

            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, res, 'recipients');
        },

        function attributeAttachmentExistsAction() {
            this.showAttributesFilesUploadMsg_();
            return null;
        },

        // ------------------ DISCUSSION ------------------------


        function prepareCommentAttachment_(attachment, width_, height_){
            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.PICTURE){
                attachment.setThumbnailUrl(this.attachmentService.getDownloadUri(attachment.getId(), false, width_ || 170, height_ || 110));
                attachment.setUrl(this.attachmentService.getDownloadUri(attachment.getId(), false, null, null));
            }
            if(attachment.getType() == chlk.models.attachment.AttachmentTypeEnum.OTHER){
                attachment.setUrl(this.attachmentService.getDownloadUri(attachment.getId(), true, null, null));
            }
        },

        function prepareCommentsAttachments_(comments){
            var that = this;
            comments.forEach(function(comment){
                if(comment.getAttachments())
                    comment.getAttachments().forEach(function(attachment){
                        that.prepareCommentAttachment_(attachment);
                    });

                if(comment.getSubComments())
                    that.prepareCommentsAttachments_(comment.getSubComments())
            });
        }
    ])
});
