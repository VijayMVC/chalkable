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

            [[chlk.models.people.User]],
            function prepareProfileData(model){
                var phones = model.getPhones(), addresses = model.getAddresses() ,phonesValue=[], addressesValue=[];
                phones.forEach(function(item){
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        isPrimary: item.isIsPrimary(),
                        value: item.getValue()
                    }
                    phonesValue.push(values);
                    if(item.isIsPrimary() && !model.getPrimaryPhone()){
                        model.setPrimaryPhone(item);
                    }else{
                        if(!model.getHomePhone())
                            model.setHomePhone(item);
                    }
                });

                addresses.forEach(function(item){
                    var values = {
                        id: item.getId().valueOf(),
                        type: item.getType(),
                        value: item.getValue()
                    }
                    addressesValue.push(values);
                });

                model.setPhonesValue(JSON.stringify(phonesValue));
                model.setAddressesValue(JSON.stringify(addressesValue));
                return model;
            },

            [[chlk.models.id.SchoolPersonId]],
            function helloAction(personId_){
                var result = this.teacherService
                    .getInfo(personId_ || this.getContext().getSession().get('currentPerson').getId())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        return this.prepareProfileData(model);
                    }.bind(this));
                return this.PushView(chlk.activities.setup.HelloPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function videoAction(personId_){
                var result = this.preferenceService
                    .getPublic(chlk.models.settings.PreferenceEnum.VIDEO_GETTING_INFO_CHALKABLE.valueOf())
                    .attach(this.validateResponse_())
                    .then(function(model){
                        model = model || new chlk.models.settings.Preference();
                        var classes = this.classService.getClassesForTopBar();
                        model.setType(classes.length);
                        return model;
                    }.bind(this));
                return this.PushView(chlk.activities.setup.VideoPage, result);
            },

            [[chlk.models.id.SchoolPersonId]],
            function startAction(personId_){
                var model = chlk.models.settings.Preference();
                var classes = this.classService.getClassesForTopBar();
                model.setType(classes.length);
                var result =  ria.async.DeferredData(model);
                return this.PushView(chlk.activities.setup.StartPage, result);
            },

            [[Number]],
            function teacherSettingsAction(index){
                var classes = this.classService.getClassesForTopBar();
                var classId = classes[index].getId();

                var result = ria.async.wait([
                    this.finalGradeService.getFinalGrades(classId, false),
                    this.calendarService.getTeacherClassWeek(classId)
                ]).then(function(result){
                    var model = new chlk.models.setup.TeacherSettings();
                    var topModel = new chlk.models.class.ClassesForTopBar();
                    topModel.setTopItems(classes);
                    topModel.setDisabled(true);
                    topModel.setSelectedItemId(classId);
                    model.setTopData(topModel);
                    model.setCalendarInfo(result[1]);
                    var gradesInfo = result[0].getFinalGradeAnnType(), sum=0;
                    gradesInfo.forEach(function(item, index){
                        item.setIndex(index);
                        sum+=(item.getValue() || 0);
                    });
                    gradesInfo.sort(function(a,b){
                        return b.getValue() > a.getValue();
                    });
                    result[0].setNextClassNumber(++index);
                    this.getContext().getSession().set('settingsModel', result[0]);
                    sum+=(result[0].getAttendance() || 0);
                    sum+=(result[0].getParticipation() || 0);
                    sum+=(result[0].getDiscipline() || 0);
                    model.setPercentsSum(sum);
                    model.setGradingInfo(result[0]);
                    return model;
                    }.bind(this));
                return this.PushView(chlk.activities.setup.TeacherSettingsPage, result);
            },

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
                            color: chlk.models.common.ButtonColor.GREEN.valueOf(),
                            close: true
                        }], 'center');
                    }else{
                        return this.redirect_('setup', action, params);
                    }

                }else{
                    if(submitType == "message"){
                        if(index > 1)
                            this.ShowMsgBox('To make things easier we copy and\npaste your choices from the last page.\n\n'+
                                    'Click any number to change it.', 'fyi.', [{
                                text: Msg.GOT_IT.toUpperCase(),
                                close: true
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

                        var classes = this.classService.getClassesForTopBar();

                        this.finalGradeService.update(model.getId(), model.getParticipation(), model.getAttendance(), model.getDropLowestAttendance(),
                            model.getDiscipline(), model.getDropLowestDiscipline(), model.getGradingStyle(), finalGradeAnnouncementTypes, model.isNeedsTypesForClasses())
                            .then(function(model){

                                if(index < classes.length){
                                    this.redirect_('setup', 'teacherSettings', [index]);
                                }else{
                                    this.redirect_('setup', 'start', []);
                                }

                            }.bind(this));
                        return this.StartLoading(chlk.activities.setup.TeacherSettingsPage);
                    }
                }
            },

            [[chlk.models.people.User]],
            function infoEditAction(model){
                this.personService.changePassword(model.getId(), model.getPassword())
                    .then(function(changed){
                        this.teacherService
                            .updateInfo(model.getId(), model.getAddressesValue(), model.getEmail(), model.getFirstName(),
                                model.getLastName(), model.getGender(), model.getPhonesValue(), model.getSalutation(), model.getBirthDate())
                            .attach(this.validateResponse_())
                            .then(function(model){
                                this.StopLoading(chlk.activities.setup.HelloPage);
                                return this.redirect_('setup', 'video', [model.getId().valueOf()]);
                            }.bind(this));
                    }.bind(this));
                return this.StartLoading(chlk.activities.setup.HelloPage);
            }
        ])
});
