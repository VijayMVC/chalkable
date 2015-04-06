REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.standard.StandardTreeItem');

NAMESPACE('chlk.templates.standard', function(){

    /**@class chlk.templates.standard.StandardsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/StandardsColumns.jade')],
        [ria.templates.ModelBind(chlk.models.standard.StandardTreeItem)],
        'StandardsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardSubjectId, 'subjectId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementId, 'announcementId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.Standard), 'standardChildren',

            [[ArrayOf(chlk.models.standard.Standard)]],
            chlk.models.standard.StandardTreeItem, function getStandardWithChildren(standards){
                //if(standards instanceof  ArrayOf(chlk.models.standard.Standard)){
                    var stWithChild = standards.filter(function(item){
                        return item.getStandardChildren && item.hasChildren();
                    });
                    if(stWithChild.length > 0)
                        return stWithChild[0];
                //}
                return null;
            }
    ]);
});