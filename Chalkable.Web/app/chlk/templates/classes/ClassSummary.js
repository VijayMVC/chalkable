REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassProfileSummaryViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassSummary*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassSummary.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileSummaryViewData)],
        'ClassSummary', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            chlk.models.classes.ClassSummary, function getClassSummary(){return this.getClazz();},

            [[chlk.models.common.HoverBox.OF(chlk.models.common.HoverBoxItem), String]],
            Object, function buildCommonGlanceBoxData(model, boxTitle){
                var items=[];
                var boxItems = model.getHover();
                for(var i = 0; i < boxItems.length; i++){
                    items.push({
                        data: boxItems[i],
                        getTotalMethod: boxItems[i].getTotal,
                        getSummaryMethod: boxItems[i].getSummary
                    });
                }
                return {
                    value: model.getTitle(),
                    items: items,
                    title: boxTitle
                };
            },

            Object, function buildDisciplineGlanceBoxData(){
                var items=[];
                var box = this.getClassSummary().getClassDisciplineBox();
                var boxItems = box.getHover();
                for(var i = 0; i < boxItems.length; i++){
                    items.push({
                        data: boxItems[i],
                        getTotalMethod: boxItems[i].getCount,
                        getSummaryMethod: boxItems[i].getDisciplineName
                    });
                }
                return {
                    value: box.getTitle(),
                    items: items,
                    title: Msg.Discipline_count
                };
            }
        ])
});