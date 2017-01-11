REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.StandardsPopupViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.StandardsPopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/GradingStandardPopup.jade')],
        [ria.templates.ModelBind(chlk.models.grading.StandardsPopupViewData)],
        'StandardsPopupTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'studentId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            Array, 'items',

            function getDateStr(item){
                if(item.type == chlk.models.announcement.AnnouncementTypeEnum.LESSON_PLAN.valueOf()){
                    var startDate = getDate(item.lessonplandata.startdate),
                        endDate = getDate(item.lessonplandata.enddate);
                    return startDate && endDate && (startDate.format('m/d/Y') + ' - ' + endDate.format('m/d/Y'));
                }

                var date = getDate(item.classannouncementdata ? item.classannouncementdata.expiresdate : item.supplementalannouncementdata);
                return date && date.format('m/d/Y');
            }
        ])
});