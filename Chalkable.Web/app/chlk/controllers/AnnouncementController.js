REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.GradeLevelService');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.services.StandardService');
REQUIRE('chlk.services.MarkingPeriodService');
REQUIRE('chlk.services.GroupService');
REQUIRE('chlk.services.LessonPlanService');
REQUIRE('chlk.services.ClassAnnouncementService');
REQUIRE('chlk.services.AdminAnnouncementService');
REQUIRE('chlk.services.AnnouncementAssignedAttributeService');

REQUIRE('chlk.activities.announcement.AnnouncementFormPage');
REQUIRE('chlk.activities.announcement.LessonPlanFormPage');
REQUIRE('chlk.activities.announcement.AnnouncementViewPage');
REQUIRE('chlk.activities.announcement.AdminAnnouncementFormPage');
REQUIRE('chlk.activities.apps.AttachDialog');
REQUIRE('chlk.activities.common.attachments.AttachmentDialog');
REQUIRE('chlk.activities.announcement.AddStandardsDialog');
REQUIRE('chlk.activities.announcement.AddDuplicateAnnouncementDialog');
REQUIRE('chlk.activities.announcement.AnnouncementGroupsDialog');
REQUIRE('chlk.activities.announcement.AnnouncementEditGroupsDialog');
REQUIRE('chlk.activities.announcement.GroupStudentsFilterDialog');
REQUIRE('chlk.activities.announcement.AddNewCategoryDialog');
REQUIRE('chlk.activities.announcement.FileCabinetDialog');

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
REQUIRE('chlk.models.standard.StandardsTableViewData');
REQUIRE('chlk.models.standard.GetStandardTreePostData');
REQUIRE('chlk.models.common.SimpleObject');
REQUIRE('chlk.models.attachment.FileCabinetViewData');
REQUIRE('chlk.models.attachment.FileCabinetPostData');

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
        chlk.services.LessonPlanService, 'lessonPlanService',

        [ria.mvc.Inject],
        chlk.services.ClassAnnouncementService, 'classAnnouncementService',

        [ria.mvc.Inject],
        chlk.services.AdminAnnouncementService, 'adminAnnouncementService',

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
        chlk.services.GroupService, 'groupService',

        [ria.mvc.Inject],
        chlk.services.MarkingPeriodService, 'markingPeriodService',

        [ria.mvc.Inject],
        chlk.services.AnnouncementAssignedAttributeService, 'assignedAttributeService',

        ArrayOf(chlk.models.attachment.AnnouncementAttachment), 'announcementAttachments',

        function getAnnouncementFormPageType_(type_){
            if(this.userInRole(chlk.models.common.RoleEnum.TEACHER)){
                var announcementType = type_ || this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_TYPE, chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT).valueOf();
                if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT)
                    return chlk.activities.announcement.AnnouncementFormPage;
                return chlk.activities.announcement.LessonPlanFormPage;
            }
            if(this.userInRole(chlk.models.common.RoleEnum.DISTRICTADMIN))
                return chlk.activities.announcement.AdminAnnouncementFormPage;


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

        [[chlk.models.announcement.AnnouncementAttributeViewData]],
        function prepareAttribute(attribute){

            var attributeAttachment = attribute.getAttributeAttachment();

            if (!attributeAttachment) return;

            if(attributeAttachment.getType() == chlk.controllers.AttachmentTypeEnum.PICTURE.valueOf()){
                attributeAttachment.setThumbnailUrl(this.assignedAttributeService.getAttributeAttachmentUri(attribute.getId(), attribute.getAnnouncementType(), false, 170, 110));
                attributeAttachment.setUrl(this.assignedAttributeService.getAttributeAttachmentUri(attribute.getId(), attribute.getAnnouncementType(), false, null, null));
            }
            if(attributeAttachment.getType() == chlk.controllers.AttachmentTypeEnum.OTHER.valueOf()){
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
                    this.BackgroundUpdateView(chlk.activities.grading.GradingClassSummaryGridPage, model, chlk.activities.lib.DontShowLoader());

                    throw error;
                }, this)
                .attach(this.validateResponse_());
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

        [[Object, Boolean]],
        function addEditAction(model, isEdit){
            this.disableAnnouncementSaving(false);
            var announcement = model.getAnnouncement();
            var announcementTypeId_;
            var type = announcement.getType();
            this.cacheAnnouncementType(type);

            var classes = this.classService.getClassesForTopBarSync();
            var markingPeriods = this.markingPeriodService.getMarkingPeriodsSync();
            var classAnnouncement = announcement.getClassAnnouncementData();
            var announcementWithClassItem = classAnnouncement || announcement.getLessonPlanData();
            var classId_ = announcementWithClassItem.getClassId(), classInfo, types;

            var savedClassInfo = this.getContext().getSession().get('classInfo', null);

            if(classAnnouncement)
                announcementTypeId_ = classAnnouncement.getAnnouncementTypeId();

            classes.forEach(function (item) {
                var currentClassInfo = this.classService.getClassAnnouncementInfo(item.getId());
                types = currentClassInfo ? currentClassInfo.getTypesByClass() : [];
                if (types.length)
                    item.setDefaultAnnouncementTypeId(types[0].getId());
                if (currentClassInfo && classId_ && classId_ == item.getId()) {
                    classInfo = currentClassInfo;
                    model.setClassInfo(classInfo);

                    if(classAnnouncement){
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
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true, date_);
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
                        /*if(noDraft_){
                            item.setClassId(classId_ || null);
                        }*/
                        if(classAnnouncement && date_){
                            classAnnouncement.setExpiresDate(date_);
                        }
                        if(resModel.getAnnouncement().getLessonPlanData()){
                            return this.lessonPlanFromModel_(resModel);
                            //return this.Redirect('announcement', 'lessonPlanFromModel', [resModel]);
                        }else
                            return this.classAnnouncementFromModel_(resModel);
                            //return this.Redirect('announcement', 'classAnnouncementFromModel', [resModel]);
                    }
                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                    return this.classAnnouncementFromModel_(chlk.models.announcement.AnnouncementForm.$create(classesBarData, true));
                    //return this.Redirect('announcement', 'classAnnouncementFromModel', [chlk.models.announcement.AnnouncementForm.$create(classesBarData, true)]);
                },this);
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function classAnnouncementFromModel_(model) {
            this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
            return this.PushView(this.getAnnouncementFormPageType_(), ria.async.DeferredData(model, 300));
        },

        [[chlk.models.announcement.AnnouncementForm]],
        function getLessonPlanFromModel_(model) {
            return this.lessonPlanService.listCategories()
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(list){
                    this.cacheLessonPlanCategories(list);
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

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, Boolean]],
        function lessonPlanAction(classId_) {
            this.getView().reset();
            this.getContext().getSession().set('classInfo', null);
            var result = ria.async.wait([
                    this.lessonPlanService.addLessonPlan(classId_),
                    this.lessonPlanService.listCategories()
                ])
                .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                    return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                    throw error;
                }, this)
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    if(model && model.getAnnouncement()){
                        var resModel =  this.addEditAction(model, false);
                        resModel.getAnnouncement().setCategories(result[1]);
                        this.cacheLessonPlanCategories(result[1]);
                        this.cacheLessonPlanClassId(resModel.getAnnouncement().getLessonPlanData().getClassId());
                        return resModel;
                    }
                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true);
                },this)
                .attach(this.validateResponse_());
            return this.PushView(chlk.activities.announcement.LessonPlanFormPage, result);
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.TEACHER,  chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[
            chlk.models.id.AnnouncementId,
            chlk.models.id.ClassId,
            String,
            chlk.models.id.AppId,
            Boolean,
            String,
            chlk.models.announcement.AnnouncementTypeEnum,
            Number
        ]],
        function attachAction(announcementId, classId, appUrlAppend_, assessmentAppId_,
                              canAddStandard, announcementTypeName, announcementType, pageIndex_) {
            var userId = this.getCurrentPerson().getId();
            var mp = this.getCurrentMarkingPeriod();

            var btnsCount = 2;

            if (canAddStandard) ++btnsCount;
            if (assessmentAppId_) ++btnsCount;

            var start = pageIndex_ | 0, count = 16 - btnsCount;

            var result;
            if (this.userIsAdmin()){
                result = this.appMarketService
                    .getInstalledApps(userId, start, null, count)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return new chlk.models.apps.InstalledAppsViewData(announcementId,
                            classId,
                            data,
                            appUrlAppend_ || '',
                            this.isStudyCenterEnabled(),
                            canAddStandard,
                            assessmentAppId_,
                            announcementTypeName,
                            announcementType
                        )}, this);
            }
            else {
                var result = this.appMarketService
                    .getAppsForAttach(userId, classId, mp.getId(), start, count)
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return new chlk.models.apps.InstalledAppsViewData(announcementId,
                            classId,
                            data,
                            appUrlAppend_ || '',
                            this.isStudyCenterEnabled(),
                            canAddStandard,
                            assessmentAppId_,
                            announcementTypeName,
                            announcementType
                        )}, this);
            }

            //new ria.async.DeferredData(chlk.models.apps.InstalledAppsViewData.$createForAdmin(announcementId, announcementType, assessmentAppId_));

            return this.ShadeOrUpdateView(chlk.activities.apps.AttachDialog, result);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId]],
        function fileAttachAction(announcementId, announcementType, assignedAttributeId_){
            var data = new chlk.models.apps.InstalledAppsViewData.$createForAttribute(announcementId, announcementType, assignedAttributeId_);
            var res =  new ria.async.DeferredData(data);
            return this.ShadeOrUpdateView(chlk.activities.apps.AttachDialog, res);
        },


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
            this.BackgroundCloseView(chlk.activities.apps.AttachDialog);
            return this.UpdateView(announcementType == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN ?
                    chlk.activities.announcement.LessonPlanFormPage :
                    chlk.activities.announcement.AnnouncementFormPage,
                this.fetchAddAttributeFuture_(announcementId, announcementType), 'add-attribute');
        },

        [chlk.controllers.AccessForRoles([
            chlk.models.common.RoleEnum.DISTRICTADMIN
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function addAttributeDistrictAdminAction(announcementId, announcementType) {
            this.BackgroundCloseView(chlk.activities.apps.AttachDialog);
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage,
                this.fetchAddAttributeFuture_(announcementId, announcementType), 'add-attribute');
        },

        //---File Cabinet---
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId]],
        function fileCabinetAction(announcementId, announcementType, assignedAtributeId_){
            return this.getAttachments_(announcementId, announcementType, assignedAtributeId_);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.attachment.FileCabinetPostData]],
        function listAttachmentsAction(postData){
            return this.getAttachments_(
                postData.getAnnouncementId(),
                postData.getAnnouncementType(),
                postData.getAssignedAttributeId(),
                postData.getFilter(),
                postData.getSortType(),
                postData.getStart(),
                postData.getCount()
            );
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[
            chlk.models.id.AnnouncementId,
            chlk.models.announcement.AnnouncementTypeEnum,
            chlk.models.id.AnnouncementAssignedAttributeId,
            String,
            chlk.models.attachment.SortAttachmentType,
            Number,
            Number
        ]],
        function pageAttachmentsAction(announcementId, announcementType, assignedAtributeId_, filter_, sortType_, start_, count_){
            return this.getAttachments_(announcementId, announcementType, assignedAtributeId_, filter_, sortType_, start_, count_);
        },

        [[
            chlk.models.id.AnnouncementId,
            chlk.models.announcement.AnnouncementTypeEnum,
            chlk.models.id.AnnouncementAssignedAttributeId,
            String,
            chlk.models.attachment.SortAttachmentType,
            Number,
            Number
        ]],
        function getAttachments_(announcementId, announcementType, assignedAtributeId_,filter_, sortType_, start_, count_){
            var res = this.announcementService.getAttachments(filter_, sortType_, start_, count_)
                .attach(this.validateResponse_())
                .then(function(atts){

                    atts.getItems().forEach(function(attachment){
                        if(attachment.getType() == chlk.controllers.AttachmentTypeEnum.PICTURE.valueOf()){
                            attachment.setThumbnailUrl(this.announcementService.getFileUri(attachment.getId(), false, 170, 110));
                            attachment.setUrl(this.announcementService.getFileUri(attachment.getId(), false, null, null));
                        }
                        if(attachment.getType() == chlk.controllers.AttachmentTypeEnum.OTHER.valueOf()){
                            attachment.setUrl(this.announcementService.getFileUri(attachment.getId(), true, null, null));
                        }
                    }, this);

                    return new chlk.models.attachment.FileCabinetViewData(
                        announcementId,
                        announcementType,
                        atts,
                        sortType_,
                        filter_,
                        assignedAtributeId_
                    )
                }, this);
            return this.ShadeOrUpdateView(chlk.activities.announcement.FileCabinetDialog, res);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, Boolean]],
        function attachFromCabinetAction(announcementId, announcementType, attachmentId, onCreate_){
            var res = this.announcementService
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
            this.BackgroundCloseView(chlk.activities.announcement.FileCabinetDialog);
            var res = this.assignedAttributeService
                .addAttachment(announcementType, announcementId, assignedAttributeId, attachmentId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    return this.prepareAttributeData(announcementId, announcementType, assignedAttributeId, attribute);
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(announcementType), res, 'add-attribute-attachment');
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AttachmentId, chlk.models.id.AnnouncementAssignedAttributeId]],
        function cloneFromCabinetAction(announcementId, announcementType, attachmentId){
            var res = this.announcementService
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
            var res = this.assignedAttributeService
                .cloneAttachmentForAttribute(attachmentId, announcementId, announcementType, assignedAttributeId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
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
            return this.UpdateView(announcementType == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN ?
            chlk.activities.announcement.LessonPlanFormPage :
            chlk.activities.announcement.AnnouncementFormPage,
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
                    if(resModel.getAnnouncement().getLessonPlanData()){
                        return this.getLessonPlanFromModel_(resModel);
                    }else{
                        this.cacheAnnouncementType(chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT);
                        return resModel;
                    }
                }, this);
            if(announcementType == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN)
                return this.PushView(chlk.activities.announcement.LessonPlanFormPage, res);
            return this.PushView(chlk.activities.announcement.AnnouncementFormPage, res);
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
            var announcementForClass = classAnnouncement || lessonPlan;
            var announcementWithExpires = classAnnouncement || adminAnnouncement;

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
            announcement.setAbleEdit(announcement.isAnnOwner() && hasMCPermission);
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

        function handleNoAnnouncementException_(error){
            if(error.getStatus && error.getStatus() == 500){
                var res = JSON.parse(error.getResponse());
                if(res.exceptiontype == 'NoAnnouncementException')
                    return this.redirectToErrorPage_(error.toString(), 'error', 'viewAnnouncementError', []);
            }
            throw error;
        },




        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementAssignedAttributeId, Object]],
        function uploadAttachmentOnCreateAction(announcementId, announcementType, assignedAttributeId, files) {
            this.BackgroundCloseView(chlk.activities.apps.AttachDialog);

            var isStudent =  this.userInRole(chlk.models.common.RoleEnum.STUDENT); //todo remove this later ... this is short fix
            if(assignedAttributeId && assignedAttributeId.valueOf())
                return this.Redirect('announcement', 'addAttributeAttachment', [announcementType, announcementId, assignedAttributeId, files]);
            return this.Redirect('announcement', 'uploadAttachment', [announcementId,  announcementType, files, !isStudent]);
        },

        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId, Object]],
        function addAttributeAttachmentAction(announcementType, announcementId, announcementAssignedAttributeId, files) {
            var result = this.assignedAttributeService
                .uploadAttributeAttachment(announcementType, announcementId, announcementAssignedAttributeId, files)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
                    return this.prepareAttributeData(announcementId, announcementType, announcementAssignedAttributeId, attribute);
                }, this);
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, 'add-attribute-attachment');
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
        [[chlk.models.announcement.AnnouncementTypeEnum, chlk.models.id.AnnouncementId, chlk.models.id.AnnouncementAssignedAttributeId]],
        function removeAttributeAttachmentAction(announcementType, announcementId, announcementAssignedAttributeId) {
            var result = this.assignedAttributeService
                .removeAttributeAttachment(announcementType, announcementId, announcementAssignedAttributeId)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(attribute){
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
            return this.UpdateView(this.getAnnouncementFormPageType_(), result, 'remove-attribute-attachment');
        },


        [[chlk.models.id.AnnouncementId, Object, chlk.models.announcement.AnnouncementTypeEnum]],
            ria.async.Future, function fetchUploadAttachmentFuture_(announcementId, files, announcementType) {

            return this.announcementService
                .uploadAttachment(announcementId, files, announcementType)
                .catchError(this.handleNoAnnouncementException_, this)
                .attach(this.validateResponse_())
                .then(function(announcement){
                    announcement.setNeedButtons(true);
                    announcement.setNeedDeleteButton(true);
                    this.prepareAttachments(announcement);
                    this.cacheAnnouncement(announcement);
                    return announcement;
                }, this);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum, Object, Boolean]],
        function uploadAttachmentAction(announcementId, announcementType, files, onCreate_) {
            this.BackgroundCloseView(chlk.activities.apps.AttachDialog);
            return this.UpdateView(onCreate_ ? this.getAnnouncementFormPageType_(announcementType) : chlk.activities.announcement.AnnouncementViewPage//this.getView().getCurrent().getClass()
                    , this.fetchUploadAttachmentFuture_(announcementId, files, announcementType), 'update-attachments');
        },

        [[chlk.models.id.AnnouncementId,  chlk.models.announcement.AnnouncementTypeEnum, Object, Boolean]],
        function uploadAttachmentDistrictAdminAction(announcementId, announcementType, files, onCreate_) {
            this.BackgroundCloseView(chlk.activities.apps.AttachDialog);
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage
                    , this.fetchUploadAttachmentFuture_(announcementId, files, announcementType), 'update-attachments');
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
                color: chlk.models.common.ButtonColor.RED.valueOf() }, {
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
                this.announcementService.getAttachmentUri(attachment.getId(), true)
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
                        'announcement', 'cloneAttachment', [attachment.getAttachmentId().valueOf(), announcementId.valueOf()], true));
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

            if(attachment.getType() == chlk.models.announcement.ApplicationOrAttachmentEnum.PICTURE.valueOf()){
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


        [[chlk.models.announcement.FeedAnnouncementViewData]],
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

        [[chlk.models.id.AttachmentId, chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function deleteAttachmentAction(attachmentId, announcementId, announcementType) {
            var result = this.announcementService
                .deleteAttachment(attachmentId, announcementId, announcementType)
                .attach(this.validateResponse_())
                .then(function(model){
                    model.setNeedButtons(true);
                    model.setNeedDeleteButton(true);
                    this.prepareAttachments(model);
                    return model;
                }, this);
            return this.UpdateView(this.getView().getCurrent().getClass(), result, 'update-attachments');
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

        [[ArrayOf(chlk.models.announcement.CategoryViewData)]],
        function cacheLessonPlanCategories(categories){
            this.getContext().getSession().set(ChlkSessionConstants.LESSON_PLAN_CATEGORIES, categories);
        },

        ArrayOf(chlk.models.announcement.CategoryViewData), function getCachedLessonPlanCategories(){
            return this.getContext().getSession().get(ChlkSessionConstants.LESSON_PLAN_CATEGORIES, []);
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
            return chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(announcement);
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

            if (submitType == 'saveTitle'){
                return this.saveLessonPlanTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle' || submitType == 'addToGallery'){
                if(submitType == 'addToGallery' || model.getGalleryCategoryId() && model.getGalleryCategoryId().valueOf())
                    return this.checkLessonPlanTitleAction(model, submitType == 'addToGallery');
                return null;
            }

            if (submitType == 'save'){
                model.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
                model.setApplications(this.getCachedAnnouncementApplications());
                model.setCategories(this.getCachedLessonPlanCategories());
                model.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
                var announcementForm =  chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(model);
                return this.saveLessonPlanAction(model, announcementForm);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.saveLessonPlanAction(model);
            }

            if (submitType == 'changeCategory'){
                this.getContext().getSession().set(ChlkSessionConstants.LESSON_PLAN_CATEGORY_FOR_SEARCH, model.getGalleryCategoryForSearch() || '');
                return null;
                //return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, ria.async.DeferredData(model), 'search');
            }

            if (submitType == 'createFromTemplate'){
                return this.Redirect('announcement', 'createFromTemplate', [model.getAnnouncementForTemplateId(), classId]);
            }

            if(this.submitLessonPlan(model, submitType == 'submitOnEdit'))
                return this.ShadeLoader();

            return null;
        },

        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function createFromTemplateAction(announcementId, classId){
            var result = ria.async.wait([
                    this.lessonPlanService.createFromTemplate(announcementId, classId),
                    this.lessonPlanService.listCategories()
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    if(model && model.getAnnouncement()){
                        var resModel =  this.addEditAction(model, false);
                        resModel.getAnnouncement().setCategories(result[1]);
                        this.cacheLessonPlanCategories(result[1]);
                        this.cacheLessonPlanClassId(resModel.getAnnouncement().getLessonPlanData().getClassId());
                        return resModel;
                    }
                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true);
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

        [[chlk.models.id.AnnouncementId, String]],
        function saveLessonPlanTitleAction(announcementId, announcementTitle){
            var result = this.lessonPlanService
                .editTitle(announcementId, announcementTitle)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.announcement.FeedAnnouncementViewData();
                });
            return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, Boolean]],
        function checkLessonPlanTitleAction(model, isAddToGallery_){
            var res = this.lessonPlanService
                .existsInGallery(model.getTitle(), model.getId())
                .attach(this.validateResponse_())
                .then(function(exists){
                    if(exists)
                        this.ShowMsgBox('There is Lesson Plan with that title in gallery', 'whoa.');
                    return new chlk.models.Success(exists);
                }, this);

            return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, res, isAddToGallery_ ? 'addToGallery' : chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.announcement.FeedAnnouncementViewData, chlk.models.announcement.AnnouncementForm]],
        function saveLessonPlanAction(model, form_) {
            if(!(model.getClassId() && model.getClassId().valueOf()))
                return null;
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
                model.getAssignedAttributesPostData()

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
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setState(model.getState());
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

        [[chlk.models.announcement.FeedAnnouncementViewData, Boolean]],
        function submitLessonPlan(model, isEdit){
            if(model.getStartDate() > model.getEndDate()){
                this.ShowMsgBox('Lesson Plan are no valid. Start date is greater the end date', 'whoa.');
                return null;
            }

            var res = this.lessonPlanService
                .submitLessonPlan(
                    model.getId(),
                    model.getClassId(),
                    model.getTitle(),
                    model.getContent(),
                    model.getGalleryCategoryId(),
                    model.getStartDate(),
                    model.getEndDate(),
                    model.isHiddenFromStudents(),
                    model.getAssignedAttributesPostData()
                )
                .attach(this.validateResponse_());

            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheLessonPlanCategories(null);
                    this.cacheLessonPlanClassId(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    this.cacheAnnouncementAttributes(null);
                    if(isEdit)
                        return this.BackgroundNavigate('announcement', 'view', [model.getId(), chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN]);
                    else{
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
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
                    model.getAssignedAttributesPostData()
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
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setState(model.getState());
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
                        model.getAssignedAttributesPostData()
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

        [chlk.controllers.SidebarButton('inbox')],
        [[chlk.models.id.AnnouncementId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
        function starAction(id, complete_, type_){
            this.announcementService
                .checkItem(id, complete_, type_)
                .attach(this.validateResponse_());
            return null;
        },

        [chlk.controllers.SidebarButton('statistic')],
        [[chlk.models.id.AnnouncementId, Boolean, chlk.models.announcement.AnnouncementTypeEnum]],
        function starFromStudentGradesAction(id, complete_, type_){
            return this.starAction(id, complete_, type_);
        },

        [[chlk.models.id.AnnouncementId, chlk.models.announcement.AnnouncementTypeEnum]],
        function makeVisibleAction(id, type){
            this[type == chlk.models.announcement.AnnouncementTypeEnum.CLASS_ANNOUNCEMENT ? 'classAnnouncementService' :
                    (type == chlk.models.announcement.AnnouncementTypeEnum.ADMIN ? 'adminAnnouncementService' : 'lessonPlanService')]
                .makeVisible(id)
                .attach(this.validateResponse_());
            return null;
        },

        [[chlk.models.announcement.QnAForm]],
        function askQuestionAction(model) {
            var ann = this.announcementService
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

        [chlk.controllers.NotChangedSidebarButton()],
        [[String, chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function showStandardsAction(typeName, announcementId, classId){
            var standardIds = this.getContext().getSession().get(ChlkSessionConstants.STANDARD_IDS, []);
            var res = this.standardService.getSubjects(classId)
                .then(function(subjects){
                    return new chlk.models.announcement.AddStandardViewData(typeName, announcementId, classId, subjects, standardIds);
                }, this)
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.announcement.AddStandardsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, String, chlk.models.id.StandardId]],
        function showStandardsByCategoryAction(classId, subjectId, description_, standardId_){
            var res = this.standardService.getStandardColumn(classId, subjectId, standardId_)
                .then(function(standards){
                    var standardTable = new chlk.models.standard.StandardsTable.$createOneColumnTable(standards);
                    return new chlk.models.standard.StandardsTableViewData(description_, classId, subjectId, standardTable);
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.standard.GetStandardTreePostData]],
        function getStandardTreeAction(data){
            var res = this.standardService.getStandardParentsSubTree(data.getStandardId(), data.getClassId())
                .then(function(standardsTable){
                    var description, subjectId;
                    if(standardsTable && standardsTable.getStandardsColumns() && standardsTable.getStandardsColumns().length > 0){
                        var columns = standardsTable.getStandardsColumns();
                        var subjectId = columns[0][0].getSubjectId();
                        var lastSelected = columns[columns.length - 1].filter(function (s){return s.isSelected();});
                        if(lastSelected.length > 0){
                            description = lastSelected[0].getDescription();
                            standardsTable.addColumn([]);
                        }
                    }
                    var res = new chlk.models.standard.StandardsTableViewData(description, data.getClassId(), subjectId, standardsTable, data.getAnnouncementId());
                    return res
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AddStandardsDialog, res, 'rebuild-standard-tree');
        },

        [chlk.controllers.NotChangedSidebarButton()],
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
            return this.UpdateView(this.getAnnouncementFormPageType_(), res, 'update-standards-and-suggested-apps');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, chlk.models.id.StandardId]],
        function removeStandardAction(announcementId, standardId){
            var res = this.announcementService.removeStandard(announcementId, standardId)
                .then(function(announcement){
                    this.saveStandardIds(announcement);
                    //return chlk.models.standard.StandardsListViewData(null, null, null, announcement.getStandards(), announcement.getId());
                    return announcement;
                }, this)
                .attach(this.validateResponse_());
            return this.UpdateView(this.getAnnouncementFormPageType_(), res, 'update-standards-and-suggested-apps');
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
        [[chlk.models.id.GroupId]],
        function showGroupMembersAction(groupId){
            var res = this.groupService.groupExplorer(groupId)
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },


        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId, String]],
        function showGradeLevelMembersAction(groupId, schoolYearId, gradeLevelId, classIds_){
            var res = this.groupService.studentForGroup(groupId, schoolYearId, gradeLevelId, classIds_ && this.getIdsList(classIds_, chlk.models.id.ClassId))
                .then(function(students){
                    return new chlk.models.group.StudentsForGroupViewData(groupId, gradeLevelId, schoolYearId, students);
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res, classIds_ ? 'after-filter' : '');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
        function assignGradeLevelAction(groupId, schoolYearId, gradeLevelId){
            var res = this.groupService.assignGradeLevel(groupId, schoolYearId, gradeLevelId)
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
        function unAssignGradeLevelAction(groupId, schoolYearId, gradeLevelId){
            var res = this.groupService.unassignGradeLevel(groupId, schoolYearId, gradeLevelId)
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolPersonId]],
        function assignStudentAction(groupId, studentId){
            if(!studentId)
                return null;
            var res = this.groupService.assignStudents(groupId, [studentId])
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolPersonId]],
        function unAssignStudentAction(groupId, studentId){
            if(!studentId)
                return null;
            var res = this.groupService.unassignStudents(groupId, [studentId])
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId]],
        function assignSchoolAction(groupId, schoolYearId){
            var res = this.groupService.assignSchool(groupId, schoolYearId)
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId]],
        function unAssignSchoolAction(groupId, schoolYearId){
            var res = this.groupService.unassignSchool(groupId, schoolYearId)
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId]],
        function assignAllSchoolsAction(groupId){
            var res = this.groupService.assignAllSchools(groupId)
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId]],
        function unAssignAllSchoolsAction(groupId){
            var res = this.groupService.unassignAllSchools(groupId)
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.GradeLevelId, chlk.models.id.SchoolYearId, String]],
        function assignAllStudentsAction(groupId, gradeLevelId, schoolYearId, studentIds){
            if(!studentIds)
                return null;

            var res = this.groupService.assignStudents(groupId, this.getIdsList(studentIds, chlk.models.id.SchoolPersonId))
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());
            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.GradeLevelId, chlk.models.id.SchoolYearId, String]],
        function unAssignAllStudentsAction(groupId, gradeLevelId, schoolYearId, studentIds){
            if(!studentIds)
                return null;

            var res = this.groupService.unassignStudents(groupId, this.getIdsList(studentIds, chlk.models.id.SchoolPersonId))
                .then(function(model){
                    return new chlk.models.Success();
                })
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId, chlk.models.id.SchoolYearId, chlk.models.id.GradeLevelId]],
        function selectStudentFiltersAction(groupId, schoolYearId, gradeLevelId){
            var res = this.classService.detailedCourseTypes(schoolYearId, gradeLevelId)
                .then(function(courseTypes){
                    return new chlk.models.announcement.GroupStudentsFilterViewData(groupId, schoolYearId, gradeLevelId, courseTypes)
                })
                .attach(this.validateResponse_());

            return this.ShadeView(chlk.activities.announcement.GroupStudentsFilterDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.GroupStudentsFilterViewData]],
        function filterStudentsAction(model){
            this.BackgroundCloseView(chlk.activities.announcement.GroupStudentsFilterDialog);
            return this.Redirect('announcement', 'showGradeLevelMembers', [model.getGroupId(), model.getSchoolYearId(), model.getGradeLevelId(), model.getClassIds()]);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        function editGroupsAction(){
            var res = this.groupService.list()
                .then(function(groups){
                    return new chlk.models.group.GroupsListViewData(groups);
                })
                .attach(this.validateResponse_());

            return this.ShadeView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
        },

        function afterGroupEdit(groups){
            this.getContext().getSession().set(ChlkSessionConstants.GROUPS_LIST, groups);
            var announcementId = this.getContext().getSession().get(ChlkSessionConstants.ANNOUNCEMENT_ID, null);
            var groupsIds = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_IDS, []);
            var model = new chlk.models.group.AnnouncementGroupsViewData(groups, groupsIds, announcementId);
            this.BackgroundUpdateView(chlk.activities.announcement.AnnouncementGroupsDialog, model);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        function addGroupAction(){
            var res = new ria.async.DeferredData(new chlk.models.group.Group);

            return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId]],
        function tryDeleteGroupAction(groupId) {
            this.ShowConfirmBox('Are you sure you want to delete this group?', "whoa.", null, 'negative-button')
                .then(function (data) {
                    return this.BackgroundNavigate('announcement', 'deleteGroup', [groupId]);
                }, this);
            return null;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.GroupId]],
        function deleteGroupAction(groupId){
            var res = this.groupService.deleteGroup(groupId)
                .then(function(groups){
                    this.afterGroupEdit(groups);
                    return new chlk.models.group.GroupsListViewData(groups);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.group.Group]],
        function createGroupAction(model){
            var res = this.groupService.create(model.getName())
                .then(function(groups){
                    this.afterGroupEdit(groups);
                    return new chlk.models.group.GroupsListViewData(groups);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.group.Group]],
        function editGroupNameAction(model){
            var res = this.groupService.editName(model.getId(), model.getName())
                .then(function(groups){
                    this.afterGroupEdit(groups);
                    return new chlk.models.group.GroupsListViewData(groups);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AnnouncementEditGroupsDialog, res);
        },

        function editCategoriesAction(){
            var res = this.lessonPlanService.listCategories()
                .then(function(list){
                    return new chlk.models.announcement.CategoriesListViewData(list);
                })
                .attach(this.validateResponse_());
            return this.ShadeView(chlk.activities.announcement.AddNewCategoryDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        function addCategoryAction(){
            var res = new ria.async.DeferredData(new chlk.models.announcement.CategoryViewData);

            return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[Number]],
        function tryDeleteCategoryAction(categoryId) {
            this.ShowConfirmBox('Are you sure you want to delete this category?', "whoa.", null, 'negative-button')
                .then(function (data) {
                    return this.BackgroundNavigate('announcement', 'deleteCategory', [categoryId]);
                }, this);
            return null;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[Number]],
        function deleteCategoryAction(categoryId){
            var res = this.lessonPlanService.deleteCategory(categoryId)
                .thenCall(this.lessonPlanService.listCategories, [])
                .then(function(list){
                    this.afterGroupEditAction(list);
                    return new chlk.models.announcement.CategoriesListViewData(list);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.CategoryViewData]],
        function createCategoryAction(model){
            var res = this.lessonPlanService.addCategory(model.getName())
                .then(function(data){
                    if(!data)
                        this.ShowMsgBox("Category with this name already exists");
                    return this.lessonPlanService.listCategories();
                }, this)
                .then(function(list){
                    this.afterGroupEditAction(list);
                    return new chlk.models.announcement.CategoriesListViewData(list);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.CategoryViewData]],
        function editCategoryNameAction(model){
            var res = this.lessonPlanService.updateCategory(Number(model.getId()), model.getName())
                .then(function(data){
                    if(!data)
                        this.ShowMsgBox("Category with this name already exists");
                    return this.lessonPlanService.listCategories();
                }, this)
                .then(function(list){
                    this.afterGroupEditAction(list);
                    return new chlk.models.announcement.CategoriesListViewData(list);
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AddNewCategoryDialog, res);
        },

        function afterGroupEditAction(list){
            var model = new chlk.models.announcement.FeedAnnouncementViewData();
            model.setCategories(list);
            this.BackgroundUpdateView(chlk.activities.announcement.LessonPlanFormPage, model, 'right-categories');
            this.BackgroundUpdateView(chlk.activities.announcement.LessonPlanFormPage, model, 'categories');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, chlk.models.id.GroupId]],
        function removeRecipientAction(announcementId, recipientId){

            var res = this.groupService.list()
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
                }, this)
                .attach(this.validateResponse_());

            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, res, 'recipients');
        },

        function attributeAttachmentExistsAction() {
            return this.ShowMsgBox('You can add only 1 attachment to attribute', 'Error'), null;
        }
    ])
});
