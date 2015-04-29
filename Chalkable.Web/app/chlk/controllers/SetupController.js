REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.TeacherService');
REQUIRE('chlk.services.PersonService');
REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.CalendarService');
REQUIRE('chlk.services.ClassService');
REQUIRE('chlk.services.PreferenceService');

REQUIRE('chlk.activities.setup.HelloPage');
REQUIRE('chlk.activities.setup.VideoPage');
REQUIRE('chlk.activities.setup.StartPage');
REQUIRE('chlk.activities.setup.TeacherSettingsPage');
REQUIRE('chlk.activities.setup.CategoriesSetupPage');

REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.setup.TeacherSettings');
REQUIRE('chlk.models.grading.Final');
REQUIRE('chlk.models.settings.Preference');
REQUIRE('chlk.models.grading.AnnouncementTypeFinal');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.SetupController */
    CLASS(
        'SetupController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.TeacherService, 'teacherService',

            [ria.mvc.Inject],
            chlk.services.PersonService, 'personService',

            [ria.mvc.Inject],
            chlk.services.FinalGradeService, 'finalGradeService',

            [ria.mvc.Inject],
            chlk.services.CalendarService, 'calendarService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [ria.mvc.Inject],
            chlk.services.PreferenceService, 'preferenceService',

            [[chlk.models.id.SchoolPersonId]],
            function helloAction(personId_){
                this.getContext().getSession().set(ChlkSessionConstants.FIRST_LOGIN, true);
                var result = new ria.async.DeferredData(this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON));
                return this.PushView(chlk.activities.setup.HelloPage, result);
            },

            [[chlk.models.id.ClassId]],
            function categoriesSetupAction(classId_){
                var types;
                var topData = new chlk.models.classes.ClassesForTopBar(null, classId_);

                if(classId_){
                    var currentClassInfo = this.classService.getClassAnnouncementInfo(classId_);
                    types = currentClassInfo ? currentClassInfo.getTypesByClass() : [];
                }

                var result = new ria.async.DeferredData(new chlk.models.setup.CategoriesSetupViewData(topData, types));
                return this.PushOrUpdateView(chlk.activities.setup.CategoriesSetupPage, result);
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


            function addCategoryAction(){
                return null;
            },

            [[Number]],
            function editCategoryAction(id){
                return null;
            }
        ])
});
