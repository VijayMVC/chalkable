REQUIRE('chlk.controllers.AnnouncementController');

NAMESPACE('chlk.controllers.announcement', function (){

    /** @class chlk.controllers.announcement.LessonPlanController */
    CLASS(
        'LessonPlanController', EXTENDS(chlk.controllers.AnnouncementController), [

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number]],
        OVERRIDE, function addAction(classId_, announcementTypeId_, date_, noDraft_) {
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
                    if(model && model.getAnnouncement()){
                        var resModel =  this.addEditAction(model, false);
                        resModel.getAnnouncement().setCategories(result[1]);
                        this.lpGalleryCategoryService.cacheLessonPlanCategories(result[1]);
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

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var submitType = model.getSubmitType();
            var classId = model.getClassId();

            model.setMarkingPeriodId(this.getCurrentMarkingPeriod().getId());

            if (submitType == 'listLast'){
                return this.listLastAction(classId);
            }

            if (submitType == 'saveTitle'){
                return this.saveTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle' || submitType == 'addToGallery'){
                if(submitType == 'addToGallery' || model.getGalleryCategoryId() && model.getGalleryCategoryId().valueOf())
                    return this.checkLessonPlanTitleAction(model, submitType == 'addToGallery');
                return null;
            }

            if (submitType == 'viewImported'){
                this.getContext().getSession().set(ChlkSessionConstants.CREATED_ANNOUNCEMENTS, model.getCreatedAnnouncements());
                return this.Redirect('feed', 'viewImported', [model.getClassId()]);
            }

            if (submitType == 'save'){
                model.setAnnouncementAttachments(this.getCachedAnnouncementAttachments());
                model.setApplications(this.getCachedAnnouncementApplications());
                model.setCategories(this.lpGalleryCategoryService.getLessonPlanCategoriesSync());
                model.setAnnouncementAttributes(this.getCachedAnnouncementAttributes());
                var announcementForm =  chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(model);
                return this.saveLessonPlanAction(model, announcementForm);
            }

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                return this.saveLessonPlanAction(model);
            }

            if (submitType == 'changeCategory'){
                this.getContext().getSession().set(ChlkSessionConstants.LESSON_PLAN_CATEGORY_FOR_SEARCH, model.getGalleryCategoryForSearch() || null);
                return null;
            }

            if (submitType == 'createFromTemplate'){
                return this.Redirect('announcement', 'createFromTemplate', [model.getAnnouncementForTemplateId(), classId]);
            }

            if(this.submitLessonPlan(model, submitType == 'submitOnEdit'))
                return this.ShadeLoader();

            return null;
        },

        [[chlk.models.id.ClassId]],
        function listLastAction(classId){
            var result = this.lessonPlanService
                .listLast(classId)
                .attach(this.validateResponse_())
                .then(function(msgs){
                    return chlk.models.announcement.LastMessages.$create(Msg.Lesson_Plan, msgs);
                }, this);
            return this.UpdateView(chlk.activities.announcement.LessonPlanFormPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[chlk.models.id.AnnouncementId, String]],
        function saveTitleAction(announcementId, announcementTitle){
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
                                    return new chlk.models.Success(true);
                                }, this);
                        }
                        else{
                            this.ShowMsgBox('There is Lesson Plan with that title in gallery', 'whoa.');
                        }
                    }
                    return new chlk.models.Success(success);
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
                        announcement.setAppsWithContent(model.getAppsWithContent());
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
            if(model.getStartDate().compare(model.getEndDate()) == 1){
                this.ShowMsgBox('Lesson Plan is not valid. Start date is greater the end date', 'whoa.');
                return null;
            }

            var gradingPeriods = this.getContext().getSession().get(ChlkSessionConstants.GRADING_PERIODS, []);

            if(!gradingPeriods.filter(function(_){return model.getStartDate().getDate() >= _.getStartDate().getDate() && model.getStartDate().getDate() <= _.getEndDate().getDate()
                    && model.getEndDate().getDate() >= _.getStartDate().getDate() && model.getEndDate().getDate() <= _.getEndDate().getDate()}).length){
                this.ShowMsgBox('Lesson Plan is not valid. Start date and End date can\'t be in different grading periods', 'whoa.');
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
                    this.lpGalleryCategoryService.emptyLessonPlanCategoriesCache();
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

        [[chlk.models.id.AnnouncementId, chlk.models.id.ClassId]],
        function createFromTemplateAction(announcementId, classId){
            var result = ria.async.wait([
                    this.lessonPlanService.createFromTemplate(announcementId, classId),
                    this.lpGalleryCategoryService.list()
                ])
                .attach(this.validateResponse_())
                .then(function(result){
                    var model = result[0];
                    if(model && model.getAnnouncement()){
                        var resModel =  this.addEditAction(model, false);
                        resModel.getAnnouncement().setCategories(result[1]);
                        this.lpGalleryCategoryService.cacheLessonPlanCategories(result[1]);
                        this.cacheLessonPlanClassId(resModel.getAnnouncement().getLessonPlanData().getClassId());
                        return resModel;
                    }
                    var classes = this.classService.getClassesForTopBarSync();
                    var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                    return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true);
                }, this);
            return this.PushView(chlk.activities.announcement.LessonPlanFormPage, result);
        }
    ])
});
