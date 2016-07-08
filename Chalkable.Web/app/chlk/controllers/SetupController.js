REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PreferenceService');
REQUIRE('chlk.services.TeacherCommentService');
REQUIRE('chlk.services.GradingService');
REQUIRE('chlk.services.ClassroomOptionService');
REQUIRE('chlk.services.AnnouncementTypeService');
REQUIRE('chlk.services.SchoolYearService');

REQUIRE('chlk.activities.setup.HelloPage');
REQUIRE('chlk.activities.setup.VideoPage');
REQUIRE('chlk.activities.setup.StartPage');
REQUIRE('chlk.activities.setup.CategoriesSetupPage');
REQUIRE('chlk.activities.setup.ClassAnnouncementTypeDialog');
REQUIRE('chlk.activities.setup.CommentsSetupPage');
REQUIRE('chlk.activities.setup.CommentDialog');
REQUIRE('chlk.activities.setup.ClassroomOptionSetupPage');
REQUIRE('chlk.activities.setup.CategoriesImportDialog');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.grading.GradingScale');
REQUIRE('chlk.models.common.BaseCopyImportViewData');

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
            chlk.services.AnnouncementTypeService, 'announcementTypeService',

            [ria.mvc.Inject],
            chlk.services.ClassroomOptionService, 'classroomOptionService',

            [ria.mvc.Inject],
            chlk.services.SchoolYearService, 'schoolYearService',

            [[chlk.models.id.ClassId]],
            function showImportDialogAction(classId){
                var res = this.schoolYearService.listOfSchoolYearClasses()
                    .then(function(classesByYears){
                        return new chlk.models.setup.CategoriesImportViewData(classId, classesByYears);
                    });
                return this.ShadeView(chlk.activities.setup.CategoriesImportDialog, res);
            },

            [[chlk.models.common.BaseCopyImportViewData]],
            function importAction(model){
                var res;
                if(model.getSubmitType() == 'list'){
                    res = this.announcementTypeService.list([model.getClassId()])
                        .then(function(types){
                            return new chlk.models.setup.CategoriesImportViewData(model.getClassId(), null, types);
                        });

                    return this.UpdateView(chlk.activities.setup.CategoriesImportDialog, res, 'list-update');
                }
                else{
                    res = this.announcementTypeService.copy(model.getClassId(), model.getToClassId(), model.getIds())
                        .thenCall(this.classService.updateClassAnnouncementTypes, [[model.getToClassId()]])
                        .then(function(ids){
                            this.BackgroundNavigate('setup', 'categoriesSetup', [model.getToClassId(), true]);
                            this.BackgroundCloseView(chlk.activities.setup.CategoriesImportDialog);
                            return ria.async.BREAK;
                        }, this);

                    return this.UpdateView(chlk.activities.setup.CategoriesImportDialog, res, 'list-update');
                }
            },

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

                var result = this.schoolYearService.listOfSchoolYearClasses()
                    .then(function(classes){
                        return new chlk.models.setup.CategoriesSetupViewData(topData, types, canEdit, classes);
                    });

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
                    res = this.updateClassAnnouncementType_(model);
                else
                    res = this.createClassAnnouncementType_(model);
                res.thenCall(this.classService.updateClassAnnouncementTypes, [[model.getClassId()]])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        return this.BackgroundNavigate('setup', 'categoriesSetup', [model.getClassId(), true]);
                    }, this);
                this.BackgroundCloseView(chlk.activities.setup.ClassAnnouncementTypeDialog);
                return null;
            },


            [[chlk.models.announcement.ClassAnnouncementType]],
            ria.async.Future, function createClassAnnouncementType_(model){
                return this.announcementTypeService.create(
                    model.getClassId(),
                    model.getDescription(),
                    model.getName(),
                    model.getHighScoresToDrop(),
                    model.getLowScoresToDrop(),
                    model.isSystem(),
                    model.getPercentage()
                ).attach(this.validateResponse_());
            },

            [[chlk.models.announcement.ClassAnnouncementType]],
            function updateClassAnnouncementType_(model){
                return this.announcementTypeService.update(
                    model.getClassId(),
                    model.getDescription(),
                    model.getName(),
                    model.getHighScoresToDrop(),
                    model.getLowScoresToDrop(),
                    model.isSystem(),
                    model.getPercentage(),
                    model.getId()
                ).attach(this.validateResponse_());
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.common.BaseCopyImportViewData]],
            function submitAnnouncementTypesAction(model){
                if(model.getSubmitType() == 'copy'){
                    var res = this.announcementTypeService.copy(model.getClassId(), model.getToClassId(), model.getIds())
                        .thenCall(this.classService.updateClassAnnouncementTypes, [[model.getToClassId()]])
                        .then(function(ids){
                            return model;
                        });

                    return this.UpdateView(chlk.activities.setup.CategoriesSetupPage, res, 'copy');
                }

                this.ShowConfirmBox('Do You really want to delete ' + (model.getIds().length > 1 ? 'these categories?' : 'this category?'), "whoa.", null, 'negative-button')
                    .thenCall(this.announcementTypeService.deleteTypes, [model.getIds().split(',')])
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
                return this.PushOrUpdateView(chlk.activities.setup.CommentsSetupPage, res);
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
                return this.teacherCommentService
                    .createOrUpdateComment(model.getCommentId(), model.getComment())
                    .attach(this.validateResponse_())
                    .then(function (comment){
                        this.BackgroundCloseView(chlk.activities.setup.CommentDialog);
                        return this.Redirect('setup', 'commentsSetup', []);
                    }, this);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.grading.TeacherCommentViewData]],
            function deleteCommentsAction(model){


                var commentIds = (model.getIds() || []).split(',').filter(function(item){
                    return item != '';
                });

                this.ShowConfirmBox('Do You really want to delete ' + (commentIds.length > 1 ? 'these comments?' : 'this comment?'), "whoa.", null, 'negative-button')
                    .thenCall(this.teacherCommentService.deleteComments, [commentIds])
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
                            this.gradingService.getGradingScales(),
                            this.schoolYearService.listOfSchoolYearClasses()
                        ])
                        .attach(this.validateResponse_())
                        .then(function(data){
                            var options = data[0];
                            var canCopy = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM);
                            var gradingScales = data[1];
                            var years = data[2];
                            return new chlk.models.setup.ClassroomOptionSetupViewData(topData, gradingScales, options, canCopy, years);
                        }, this);
                else
                    res = new ria.async.DeferredData(new chlk.models.setup.ClassroomOptionSetupViewData(topData));

                return this.PushOrUpdateView(chlk.activities.setup.ClassroomOptionSetupPage, res);
            },

            [chlk.controllers.Permissions([
                [chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM, chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_ADMIN]
            ])],
            [[chlk.models.grading.ClassroomOptionViewData]],
            function submitClassroomOptionAction(model){
                var res;

                if(model.getSubmitType() == 'copy'){
                    res = this.classroomOptionService.copyClassroomOption(model.getClassId(), model.getCopyToClassId())
                        .then(function(ok){
                            return model;
                        });

                    return this.UpdateView(chlk.activities.setup.ClassroomOptionSetupPage, res, 'copy');
                }

                if(model.getSubmitType() == 'import'){
                    res = this.classroomOptionService.copyClassroomOption(model.getImportFromClassId(), model.getClassId())
                        .then(function(ok){
                            this.BackgroundNavigate('setup', 'classroomOptionSetup', [model.getClassId()]);
                            return ria.async.BREAK;
                        }, this);

                    return this.UpdateView(chlk.activities.setup.ClassroomOptionSetupPage, res);
                }

                res = ria.async.wait([
                        this.classroomOptionService.updateClassroomOption(model.getClassId(), model.getAveragingMethod(),
                            model.isCategoryAveraging(), model.isIncludeWithdrawnStudents(), model.isDisplayStudentAverage(),
                            model.isDisplayTotalPoints(), model.isRoundDisplayedAverages(), model.isDisplayAlphaGrade(), model.getStandardsGradingScaleId(),
                            model.getStandardsCalculationMethod(), model.getStandardsCalculationRule(), model.isStandardsCalculationWeightMaximumValues()),
                        this.gradingService.getGradingScales(),
                        this.schoolYearService.listOfSchoolYearClasses()
                    ])
                    .attach(this.validateResponse_())
                    .then(function(data){
                        var topData = new chlk.models.classes.ClassesForTopBar(null, model.getClassId());
                        var canCopy = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_CLASSROOM);
                        var years = data[2];
                        return new chlk.models.setup.ClassroomOptionSetupViewData(topData, data[1], data[0], canCopy, years);
                    });
                return this.UpdateView(chlk.activities.setup.ClassroomOptionSetupPage, res);
            }
        ])
});
