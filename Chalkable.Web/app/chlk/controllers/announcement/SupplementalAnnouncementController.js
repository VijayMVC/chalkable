REQUIRE('chlk.controllers.AnnouncementController');

NAMESPACE('chlk.controllers.announcement', function (){

    /** @class chlk.controllers.announcement.SupplementalAnnouncementController */
    CLASS(
        'SupplementalAnnouncementController', EXTENDS(chlk.controllers.AnnouncementController), [

        [chlk.controllers.Permissions([
            [chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM, chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM_ADMIN]
        ])],
        [chlk.controllers.SidebarButton('add-new')],
        [[chlk.models.id.ClassId, Number, chlk.models.common.ChlkDate, Boolean]],
        OVERRIDE, function addAction(classId_, announcementTypeId_, date_, noDraft_) {
            this.getView().reset();
            this.getContext().getSession().set('classInfo', null);
            var result =
                ria.async.wait(
                    this.supplementalAnnouncementService.create(classId_, date_),
                    this.studentService.getStudents(classId_, null, true, true, 0, 999)
                    )
                    .catchException(chlk.lib.exception.NoClassAnnouncementTypeException, function(ex){
                        return this.redirectToErrorPage_(ex.toString(), 'error', 'createAnnouncementError', []);
                        throw error;
                    }, this)
                    .then(function(result){
                        var model = result[0];
                        var students = result[1];
                        model.setStudents(students.getItems());
                        if(model && model.getAnnouncement())
                            return this.addEditAction(model, false);

                        var classes = this.classService.getClassesForTopBarSync();
                        var classesBarData = new chlk.models.classes.ClassesForTopBar(classes);
                        return chlk.models.announcement.AnnouncementForm.$create(classesBarData, true);
                    },this)
                    .attach(this.validateResponse_());
            return this.PushView(chlk.activities.announcement.SupplementalAnnouncementFormPage, result);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveAction(model) {
            if(this.isAnnouncementSavingDisabled()) return;
            this.disableAnnouncementSaving(false);
            var submitType = model.getSubmitType();

            model.setMarkingPeriodId(this.getCurrentMarkingPeriod().getId());

            if (submitType == 'listLast'){
                return null;
            }

            if (submitType == 'saveTitle'){
                return this.saveTitleAction(model.getId(), model.getTitle())
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
                var announcementForm =  chlk.models.announcement.AnnouncementForm.$createFromAnnouncement(model);
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

        [[chlk.models.id.AnnouncementId, String]],
        function saveTitleAction(announcementId, announcementTitle){
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
                    model.getAnnouncementTypeId()
                )
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
    ])
});
