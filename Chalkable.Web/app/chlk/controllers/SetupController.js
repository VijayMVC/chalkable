REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PreferenceService');
REQUIRE('chlk.services.AnnouncementService');
REQUIRE('chlk.services.TeacherCommentService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.ClassroomOptionService');

REQUIRE('chlk.activities.setup.HelloPage');
REQUIRE('chlk.activities.setup.VideoPage');
REQUIRE('chlk.activities.setup.StartPage');
REQUIRE('chlk.activities.setup.CategoriesSetupPage');
REQUIRE('chlk.activities.setup.ClassAnnouncementTypeDialog');
REQUIRE('chlk.activities.setup.CommentsSetupPage');
REQUIRE('chlk.activities.setup.CommentDialog');
REQUIRE('chlk.activities.setup.ClassroomOptionSetupPage');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.grading.Final');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.grading.AnnouncementTypeFinal');
REQUIRE('chlk.models.grading.GradingScale');

NAMESPACE('chlk.controllers', function (){

    var comments;

    /** @class chlk.controllers.SetupController */
    CLASS(
        'SetupController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [ria.mvc.Inject],
            chlk.services.TeacherCommentService, 'teacherCommentService',

            [ria.mvc.Inject],
            chlk.services.GradingService, 'gradingService',

            [ria.mvc.Inject],
            chlk.services.FinalGradeService, 'finalGradeService',

            [ria.mvc.Inject],
            chlk.services.CalendarService, 'calendarService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.PreferenceService, 'preferenceService',

            [ria.mvc.Inject],
            chlk.services.AnnouncementService, 'announcementService',

            [ria.mvc.Inject],
            chlk.services.ClassroomOptionService, 'classroomOptionService',

            [[chlk.models.id.SchoolPersonId]],
            function helloAction(personId_){
                this.getContext().getSession().set(ChlkSessionConstants.FIRST_LOGIN, true);
                var result = new ria.async.DeferredData(this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON));
                return this.PushView(chlk.activities.setup.HelloPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId, Boolean]],
            function categoriesSetupAction(classId_, update_){
                var types;
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);

                if(classId_){
                    var currentClassInfo = this.classService.getClassAnnouncementInfo(classId_);
                    types = currentClassInfo ? currentClassInfo.getTypesByClass() : [];
                }

                var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_GRADE_BOOK_CATEGORIES);

                var result = new ria.async.DeferredData(new chlk.models.setup.CategoriesSetupViewData(topData, types, canEdit));
                if(update_)
                    return this.UpdateView(chlk.activities.setup.CategoriesSetupPage, result);
                return this.PushView(chlk.activities.setup.CategoriesSetupPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function videoAction(personId_){
                var result = this.preferenceService
                    .getPublic(chlk.models.settings.PreferenceEnum.VIDEO_GETTING_INFO_CHALKABLE.valueOf())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        model = model || new chlk.models.settings.Preference();
                        var classes = this.classService.getClassesForTopBarSync();
                        model.setType(classes.length);
                        return model;
                    }, this);
                return this.PushView(chlk.activities.setup.VideoPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function startAction(personId_){
                var model = chlk.models.settings.Preference();
                var classes = this.classService.getClassesForTopBarSync();
                model.setType(classes.length);
                var result =  ria.async.DeferredData(model);
                return this.PushView(chlk.activities.setup.StartPage, result);
            },

            //TODO: refactor
            [[chlk.models.grading.Final]],
            function teacherSettingsEditAction(model){
                var index = model.getNextClassNumber();
                var submitType = model.getSubmitType();
                var backToVideo = index == 1;
                if(submitType == 'back'){
                    var action = backToVideo ? 'video' : 'teacherSettings';
                    var params = backToVideo ? [] : [index - 2];
                    if(model.isChanged()){
                        return this.ShowMsgBox('Are you sure you want to go?', null, [{
                            text: Msg.OK,
                            controller: 'setup',
                            action: action,
                            params: params,
                            color: chlk.models.common.ButtonColor.RED.valueOf()
                        }, {
                            text: Msg.Cancel,
                            color: chlk.models.common.ButtonColor.GREEN.valueOf()
                        }], 'center'), null;
                    }else{
                        return this.Redirect('setup', action, params);
                    }

                }else{
                    if(submitType == "message"){
                        if(index > 1)
                            this.ShowMsgBox('To make things easier we copy and\npaste your choices from the last page.\n\n'+
                                    'Click any number to change it.', 'fyi.', [{
                                text: Msg.GOT_IT.toUpperCase()
                            }])
                    }else{
                        var finalGradeAnnouncementTypes = [], item, ids = model.getFinalGradeAnnouncementTypeIds().split(','),
                            percents = model.getPercents().split(','),
                            dropLowest = model.getDropLowest().split(','),
                            gradingStyle = model.getGradingStyleByType().split(',');
                        ids.forEach(function(id, i){
                            item = {};
                            item.finalGradeAnnouncementTypeId = id;
                            item.percentValue = JSON.parse(percents[i]);
                            item.dropLowest = JSON.parse(dropLowest[i]);
                            item.gradingStyle =JSON.parse(gradingStyle[i]);
                            finalGradeAnnouncementTypes.push(item)
                        });

                        var classes = this.classService.getClassesForTopBarSync();

                        this.finalGradeService.update(model.getId(), model.getParticipation(), model.getAttendance(), model.getDropLowestAttendance(),
                            model.getDiscipline(), model.getDropLowestDiscipline(), model.getGradingStyle(), finalGradeAnnouncementTypes, model.isNeedsTypesForClasses())
                            .then(function(model){

                                if(index < classes.length){
                                    return this.BackgroundNavigate('setup', 'teacherSettings', [index]);
                                }else{
                                    return this.BackgroundNavigate('setup', 'start', []);
                                }

                            }.bind(this));
                        return this.ShadeLoader();
                    }
                }
            },
            //TODO: refactor
            [[chlk.models.people.User]],
            function infoEditAction(model){
                this.personService
                    .changeEmail(model.getEmail())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        if(model.message){
                            this.getView().pop();
                            return this.ShowMsgBox(model.message, ''), null;
                        }
                        return this.BackgroundNavigate('feed', 'list');
                    }, this)
                    .complete(function(){
                        this.getView().pop();
                    }, this);
                return this.ShadeLoader();
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId, Number]],
            function addEditCategoryAction(classId, categoryId_){
                var model;
                if(categoryId_){
                    var currentClassInfo = this.classService.getClassAnnouncementInfo(classId);
                    var types = currentClassInfo.getTypesByClass();
                    model = types.filter(function(item){
                        return item.getId() == categoryId_
                    })[0];
                }else{
                    model = new chlk.models.announcement.ClassAnnouncementType(null, null, classId);
                }

                var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_GRADE_BOOK_CATEGORIES);
                model.setAbleEdit(canEdit);
                
                return this.ShadeView(chlk.activities.setup.ClassAnnouncementTypeDialog, new ria.async.DeferredData(model));
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.announcement.ClassAnnouncementType]],
            function submitClassAnnouncementAction(model){
                var res;
                if(model.getId() && model.getId().valueOf())
                    res = this.announcementService.updateAnnouncementTypes(
                        model.getClassId(),
                        model.getDescription(),
                        model.getName(),
                        model.getHighScoresToDrop(),
                        model.getLowScoresToDrop(),
                        model.isSystem(),
                        model.getPercentage(),
                        model.getId()
                    ).attach(this.validateResponse_());
                else
                    res = this.announcementService.createAnnouncementTypes(
                        model.getClassId(),
                        model.getDescription(),
                        model.getName(),
                        model.getHighScoresToDrop(),
                        model.getLowScoresToDrop(),
                        model.isSystem(),
                        model.getPercentage()
                    ).attach(this.validateResponse_());
                res.thenCall(this.classService.updateClassAnnouncementTypes, [[model.getClassId()]])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return this.BackgroundNavigate('setup', 'categoriesSetup', [model.getClassId(), true]);
                    }, this);
                this.BackgroundCloseView(chlk.activities.setup.ClassAnnouncementTypeDialog);
                return null;
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.announcement.ClassAnnouncementType]],
            function deleteAnnouncementTypesAction(model){
                this.ShowConfirmBox('Do You really want to delete ' + (model.getIds().length > 1 ? 'these categories?' : 'this category?'), "whoa.", null, 'negative-button')
                    .thenCall(this.announcementService.deleteAnnouncementTypes, [model.getIds().split(',')])
                    .attach(this.validateResponse_())
                    .thenCall(this.classService.updateClassAnnouncementTypes, [[model.getClassId()]])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return this.BackgroundNavigate('setup', 'categoriesSetup', [model.getClassId(), true]);
                    }, this);
                return null;
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            function commentsSetupAction(){
                var res = this.teacherCommentService.getTeacherComments(this.getCurrentPerson().getId())
                    .attach(this.validateResponse_())
                    .then(function(items){
                        comments = items;
                        var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_GRADE_BOOK_COMMENTS);
                        return new chlk.models.setup.CommentsSetupViewData(items, canEdit);
                    }, this);

                var result = new ria.async.DeferredData(res);
                return this.PushOrUpdateView(chlk.activities.setup.CommentsSetupPage, result);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.TeacherCommentId]],
            function addEditCommentAction(commentId_){
                var model;
                if(commentId_){
                    model = comments.filter(function(item){
                        return item.getCommentId() == commentId_
                    })[0];
                }else{
                    model = new chlk.models.grading.TeacherCommentViewData();
                }
                var canEdit = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_GRADE_BOOK_COMMENTS);
                model.setAbleEdit(canEdit);
                return this.ShadeView(chlk.activities.setup.CommentDialog, new ria.async.DeferredData(model));
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.grading.TeacherCommentViewData]],
            function submitCommentAction(model){
                var res;
                if(model.getCommentId() && model.getCommentId().valueOf())
                    res = this.teacherCommentService.updateComment(
                        model.getCommentId(),
                        model.getComment()
                    ).attach(this.validateResponse_());
                else
                    res = this.teacherCommentService.createComment(
                        model.getComment()
                    ).attach(this.validateResponse_());
                this.BackgroundCloseView(chlk.activities.setup.CommentDialog);
                return this.Redirect('setup', 'commentsSetup', []);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.grading.TeacherCommentViewData]],
            function deleteCommentsAction(model){
                this.ShowConfirmBox('Do You really want to delete ' + (model.getIds().length > 1 ? 'these comments?' : 'this comment?'), "whoa.", null, 'negative-button')
                    .thenCall(this.teacherCommentService.deleteComments, [model.getIds().split(',')])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return this.BackgroundNavigate('setup', 'commentsSetup', []);
                    }, this);
                return null;
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.id.ClassId]],
            function classroomOptionSetupAction(classId_){
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);
                if(classId_)
                    var res =
                        ria.async.wait([
                            this.classroomOptionService.getClassroomOption(classId_),
                            this.gradingService.getGradingScales()
                        ])
                        .attach(this.validateResponse_())
                        .then(function(data){
                            var options = data[0];
                            var gradingScales = data[1];
                            return new chlk.models.setup.ClassroomOptionSetupViewData(topData, gradingScales, options);
                        });
                else
                    res = new ria.async.DeferredData(new chlk.models.setup.ClassroomOptionSetupViewData(topData));

                return this.PushOrUpdateView(chlk.activities.setup.ClassroomOptionSetupPage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.grading.ClassroomOptionViewData]],
            function submitClassroomOptionAction(model){
                var res = ria.async.wait([
                        this.classroomOptionService.updateClassroomOption(model.getClassId(), model.getAveragingMethod(),
                            model.isCategoryAveraging(), model.isIncludeWithdrawnStudents(), model.isDisplayStudentAverage(),
                            model.isDisplayTotalPoints(), model.isRoundDisplayedAverages(), model.isDisplayAlphaGrade(), model.getStandardsGradingScaleId(),
                            model.getStandardsCalculationMethod(), model.getStandardsCalculationRule(), model.isStandardsCalculationWeightMaximumValues()),
                        this.gradingService.getGradingScales()
                    ])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var topData = new chlk.models.classes.ClassesForTopBar(null, model.getClassId());
                        return new chlk.models.setup.ClassroomOptionSetupViewData(topData, data[1], data[0]);
                    });
                return this.UpdateView(chlk.activities.setup.ClassroomOptionSetupPage, res);
            }
        ])
});
