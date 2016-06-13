REQUIRE('chlk.controllers.AnnouncementController');

NAMESPACE('chlk.controllers.announcement', function (){


    /** @class chlk.controllers.announcement.ClassAnnouncementController */
    CLASS(
        'ClassAnnouncementController', EXTENDS(chlk.controllers.AnnouncementController), [

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, Boolean]],
        OVERRIDE, function addAction(classId_, announcementTypeId_, date_, noDraft_) {
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
                .add(classId_, announcementTypeId_, date_)
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

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveAnnouncementFormAction(announcement){
            if(!announcement.getAnnouncementTypeId() || !announcement.getAnnouncementTypeId().valueOf())
                return this.Redirect('error', 'createAnnouncementError', []);
            var announcementForm = this.prepareAnnouncementForm_(announcement);
            return this.saveAnnouncementTeacherAction(announcement, announcementForm);
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
                        announcement.setAppsWithContent(model.getAppsWithContent());
                        announcement.setAssessmentApplicationId(model.getAssessmentApplicationId());
                        announcement.setState(model.getState());
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

        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function prepareAnnouncementForm_(announcement){
            announcement.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
            announcement.setApplications(this.getCachedAnnouncementApplications());
            announcement.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
            return chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(announcement);
        },

        [[chlk.models.announcement.StudentAnnouncement]],
        function updateAnnouncementGradeAction(model){
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

        [[chlk.models.id.AnnouncementApplicationId]],
        function discardAutoGradesAction(appId){
            return null;
        },

        [[chlk.models.id.AnnouncementApplicationId]],
        function applyAutoGradesAction(appId){
            return null;
        }
    ])
});
