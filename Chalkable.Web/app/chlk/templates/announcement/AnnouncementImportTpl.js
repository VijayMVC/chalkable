REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.AnnouncementImportViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AnnouncementImportTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementImportViewData)],
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementImport.jade')],
        'AnnouncementImportTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.FeedAnnouncementViewData), 'announcements',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.schoolYear.YearAndClasses), 'classesByYears',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            String, 'requestId',

            [ria.templates.ModelPropertyBind],
            Array, 'classScheduleDateRanges',

            function getClassesForSelect(){
                var items = this.getClassesByYears(), res = [];
                items.forEach(function(item){
                    var classes = item.getClasses();
                    if(classes.length){
                        res.push({
                            name: item.getSchoolYear().getName(),
                            values: classes.map(function(clazz){
                                return {
                                    name: clazz.getFullClassName(),
                                    id: clazz.getId().valueOf()
                                }
                            })
                        })
                    }
                });
                return res;
            }
        ])
});