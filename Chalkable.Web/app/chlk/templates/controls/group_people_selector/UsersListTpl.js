REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.recipients.UsersListViewData');

NAMESPACE('chlk.templates.controls.group_people_selector', function () {

    /** @class chlk.templates.controls.group_people_selector.UsersListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/users-list.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.UsersListViewData)],
        'UsersListTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName',
        
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'schoolId',
        
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',
        
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradeLevelId, 'gradeLevelId',
        
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ProgramId, 'programId',
        
            [ria.templates.ModelPropertyBind],
            String, 'filter',
        
            [ria.templates.ModelPropertyBind],
            Number, 'start',
        
            [ria.templates.ModelPropertyBind],
            Number, 'count',
        
            [ria.templates.ModelPropertyBind],
            String, 'submitType',
        
            [ria.templates.ModelPropertyBind],
            Boolean, 'my',
        
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'users',
        
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradeLevel), 'gradeLevels',
        
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.School), 'schools',
        
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.classes.Class), 'classes',
        
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.recipients.Program), 'programs',
            
            Array, 'selected',
        
            Boolean, 'hasAccessToLE',
            
            Boolean, 'messagingDisabled',

            Object, 'messagingSettings',

            chlk.models.recipients.SelectorModeEnum, 'selectorMode',

            Number, 'allCount',
        
            function getCountText() {
                var currentCount = this.getUsers().getTotalCount(), allCount = this.getAllCount(),
                    text = (!this.isMy() && this.getUserRole().isStudent()) ? ' Teacher' : ' Student';

                var res = currentCount;

                if(allCount > currentCount)
                    res += ' of ' + allCount;

                res += text + (currentCount == 1 ? "" : "s");
                return res;
            }
        ])
});