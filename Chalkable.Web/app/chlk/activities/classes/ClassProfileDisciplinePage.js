REQUIRE('chlk.activities.feed.BaseFeedPage');
REQUIRE('chlk.templates.classes.ClassProfileDisciplineTpl');
REQUIRE('chlk.templates.discipline.ClassDisciplinesTpl');
REQUIRE('chlk.templates.discipline.ClassDisciplineStatsTpl');
REQUIRE('chlk.activities.discipline.ClassDisciplinesPage');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileDisciplinePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileDisciplineTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.discipline.ClassDisciplinesTpl, null, '.disciplines-container' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.discipline.ClassDisciplineStatsTpl, null, '.disciplines-chart' , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileDisciplinePage', EXTENDS(chlk.activities.discipline.ClassDisciplinesPage), [

            OVERRIDE, function isAblePostDisciplines(model){
                return model.getClazz().getDisciplines().isAblePostDiscipline();
            }
        ]);
});