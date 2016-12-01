REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassExplorerViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassExplorerTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassExplorer.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassExplorerViewData)],
        'ClassExplorerTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.StandardForClassExplorer), 'standards',

            [[chlk.models.standard.StandardForClassExplorer]],
            String, function getStandardColor(item){
                var grade = item.getNumericGrade();
                if(!grade && grade !== 0)
                    return '';
                if(grade >= 75)
                    return 'green';
                if(grade >= 65)
                    return 'yellow';
                return 'red';
            },

            Boolean, function showMoreButton(){
                return this.getStandards().length > 10
            },

            [[Number]],
            Boolean, function showStandard(index){
                return !this.showMoreButton() || (index < 9);
            }
        ])
});