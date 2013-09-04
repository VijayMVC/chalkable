REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.FinalGradeService');
REQUIRE('chlk.services.ClassService');

REQUIRE('chlk.activities.grading.TeacherSettingsPage');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.GradingController */
    CLASS(
        'GradingController', EXTENDS(chlk.controllers.BaseController), [

            [ria.mvc.Inject],
            chlk.services.FinalGradeService, 'finalGradeService',

            [ria.mvc.Inject],
            chlk.services.ClassService, 'classService',

            [chlk.controllers.SidebarButton('statistic')],
            [[chlk.models.id.ClassId]],
            function teacherSettingsAction(classId_){
                var classes = this.classService.getClassesForTopBar();
                var model = new chlk.models.setup.TeacherSettings();
                var topModel = new chlk.models.classes.ClassesForTopBar();
                topModel.setTopItems(classes);
                topModel.setDisabled(true);
                classId_ && topModel.setSelectedItemId(classId_);
                model.setTopData(topModel);
                var result
                if(classId_){
                    result = this.finalGradeService
                        .getFinalGrades(classId_, false).then(function(result){
                            var gradesInfo = result.getFinalGradeAnnType(), sum=0;
                            gradesInfo.forEach(function(item, index){
                                item.setIndex(index);
                                sum+=(item.getValue() || 0);
                            });
                            gradesInfo.sort(function(a,b){
                                return b.getValue() > a.getValue();
                            });
                            sum+=(result.getAttendance() || 0);
                            sum+=(result.getParticipation() || 0);
                            sum+=(result.getDiscipline() || 0);
                            model.setPercentsSum(sum);
                            model.setGradingInfo(result);
                            return model;
                        }.bind(this));
                }else{
                    result = new ria.async.DeferredData(model);
                }
                return this.PushView(chlk.activities.grading.TeacherSettingsPage, result);
            },

            [[chlk.models.grading.Final]],
            function teacherSettingsEditAction(model){
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

                this.finalGradeService.update(model.getId(), model.getParticipation(), model.getAttendance(), model.getDropLowestAttendance(),
                    model.getDiscipline(), model.getDropLowestDiscipline(), model.getGradingStyle(), finalGradeAnnouncementTypes, model.isNeedsTypesForClasses())
                    .then(function(model){
                        this.Redirect('grading', 'teacherSettings', []);
                    }.bind(this));
                return this.StartLoading(chlk.activities.grading.TeacherSettingsPage);
            }
        ])
});
