REQUIRE('chlk.templates.announcement.BaseAnnouncementFormTpl');
REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.classes.ClassesForTopBar');
REQUIRE('chlk.templates.apps.SuggestedAppsListTpl');

NAMESPACE('chlk.templates.announcement', function () {
    "use strict";

    ASSET('~/assets/jade/activities/announcement/BaseAnnouncementForm.jade')();

    /** @class chlk.templates.announcement.AnnouncementFormTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementForm.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.AnnouncementForm)],
        'AnnouncementFormTpl', EXTENDS(chlk.templates.announcement.BaseAnnouncementFormTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassForWeekMask, 'classInfo',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            Array, 'classScheduleDateRanges',

            chlk.models.standard.StandardsListViewData, function prepareStandardsListData() {
                var ann = this.announcement;
                var itemWithClass = ann.getClassAnnouncementData() || ann.getLessonPlanData();
                return new chlk.models.standard.StandardsListViewData(
                    null, itemWithClass ? itemWithClass.getClassId() : null,
                    null, ann.getStandards(),
                    ann.getId()
                );
            },

            chlk.models.apps.SuggestedAppsList, function prepareSuggestedAppListData(){
                var ann = this.getAnnouncement();
                if(!ann) return null;
                return new chlk.models.apps.SuggestedAppsList(ann.getClassId(), ann.getId(), ann.getSuggestedApps(), ann.getStandards(), null, ann.getType());
            }
        ]);
});