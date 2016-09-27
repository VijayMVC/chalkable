REQUIRE('chlk.controllers.BaseController');

REQUIRE('chlk.services.ReportingService');
REQUIRE('chlk.services.GradingPeriodService');
REQUIRE('chlk.services.CustomReportTemplateService');
REQUIRE('chlk.services.StudentService');
REQUIRE('chlk.services.GroupService');

REQUIRE('chlk.activities.reports.StudentReportDialog');
REQUIRE('chlk.activities.reports.ReportCardsDialog');
REQUIRE('chlk.models.reports.SubmitReportCardsViewData');

NAMESPACE('chlk.controllers', function (){

    /** @class chlk.controllers.ReportingController*/
    CLASS(
        'ReportingController', EXTENDS(chlk.controllers.BaseController), [

        [ria.mvc.Inject],
        chlk.services.ReportingService, 'reportingService',

        [ria.mvc.Inject],
        chlk.services.GradingPeriodService, 'gradingPeriodService',

        [ria.mvc.Inject],
        chlk.services.StudentService, 'studentService',

        [ria.mvc.Inject],
        chlk.services.GroupService, 'groupService',

        [ria.mvc.Inject],
        chlk.services.CustomReportTemplateService, 'customReportTemplateService',

        function studentComprehensiveProgressReportAction(){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var res = this.gradingPeriodService.getList()
                .then(function(data){
                    var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT);
                    //var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                    return new chlk.models.reports.SubmitStudentReportViewData(data, this.getCurrentPerson().getId(), ableDownload);
                }, this);

            return this.ShadeView(chlk.activities.reports.StudentReportDialog, res);
        },

        [[chlk.models.reports.SubmitStudentReportViewData]],
        function submitStudentComprehensiveProgressReportAction(reportViewData){
            var result = this.reportingService.submitStudentComprehensiveProgressReport(
                    reportViewData.getGradingPeriodId(),
                    this.getCurrentPerson().getId()
                )
                .attach(this.validateResponse_())
                .then(function () {
                    this.BackgroundCloseView(chlk.activities.reports.StudentReportDialog);
                }, this)
                .thenBreak();

            return this.UpdateView(chlk.activities.reports.StudentReportDialog, result);
        },

        function reportCardsAction(){
            if (this.isDemoSchool())
                return this.ShowMsgBox('Not available for demo', 'Error'), null;

            var res = ria.async.wait([
                this.gradingPeriodService.getList(),
                this.customReportTemplateService.list(chlk.models.reports.CustomReportTemplateType.BODY)
            ])
            .attach(this.validateResponse_())
            .then(function(res){
                var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT);
                var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                return new chlk.models.reports.SubmitReportCardsViewData(res[1], res[0], reasons,ableDownload,isAbleToReadSSNumber);
            }, this);

            return this.ShadeView(chlk.activities.reports.ReportCardsDialog, res);
        },

        [[chlk.models.reports.SubmitReportCardsViewData]],
        function submitReportCardsAction(model){
            if(model.getSubmitType() == "addRecipients"){
                return this.Redirect('reporting', 'showGroupsFromReport', [model.getParsedSelected()])
            }

            if(model.getSubmitType() == 'recipient')
                return this.addRecipient_(model);

            var result = this.reportingService.submitReportCards(
                model.getCustomReportTemplateId(),
                model.getTitle(),
                model.getLogo(),
                model.getRecipient(),
                model.getOrderBy(),
                model.getIdToPrint(),
                model.getStandardType(),
                this.getIdsList(model.getGroupIds(), chlk.models.id.GroupId),
                model.getGradingPeriodId(),
                this.getIdsList(model.getAttendanceReasonIds(), chlk.models.id.AttendanceReasonId),
                model.getIncludeOptions(),
                this.getIdsList(model.getStudentIds(), chlk.models.id.SchoolPersonId)
            )
            .attach(this.validateResponse_())
            .then(function () {
                this.BackgroundCloseView(chlk.activities.reports.ReportCardsDialog);
            }, this)
            .thenBreak();
            return this.UpdateView(chlk.activities.reports.ReportCardsDialog, result);
        },

        [[chlk.models.reports.SubmitReportCardsViewData]],
        function addRecipient_(model){
            var recipient = model.getReportRecipient(), arr = recipient.split('|'),
                parsedSelected = model.getParsedSelected(),type = parseInt(arr[1], 10),
                id = parseInt(arr[0], 10), selected = JSON.parse(model.getSelectedItems()), res;
            if(type == chlk.models.search.SearchTypeEnum.PERSONS.valueOf()){
                if(selected.students.map(function (item) {return item.id}).indexOf(id) > -1)
                    return null;
                res = this.studentService.getInfo(new chlk.models.id.SchoolPersonId(id))
                    .then(function(info){
                        parsedSelected.students.push(info);
                        selected.students.push({
                            id: id,
                            displayname: info.getDisplayName(),
                            gender: info.getGender()
                        });
                        return new chlk.models.reports.ReportCardRecipientsViewData(parsedSelected.groups, parsedSelected.students, selected);
                    });
            }
            else{
                if(selected.groups.map(function (item) {return item.id}).indexOf(id) > -1)
                    return null;
                res = this.groupService.info(new chlk.models.id.GroupId(id))
                    .then(function(info){
                        parsedSelected.groups.push(info);
                        selected.groups.push({
                            id: id,
                            name: info.getName()
                        });
                        return new chlk.models.reports.ReportCardRecipientsViewData(parsedSelected.groups, parsedSelected.students, selected);
                    });
            }                

            return this.UpdateView(chlk.activities.reports.ReportCardsDialog, res, 'recipients');
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[Object]],
        function showGroupsFromReportAction(selected){
            this.WidgetStart('group', 'show', [{
                    selected: selected
                }])
                .then(function(model){
                    this.BackgroundNavigate('reporting', 'saveGroupsToReport', [model]);
                }, this)
                .attach(this.validateResponse_());
            return null;
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.recipients.RecipientsSubmitViewData]],
        function saveGroupsToReportAction(model){
            var selected = model.getParsedSelected();
            var model = new chlk.models.reports.ReportCardRecipientsViewData(selected.groups, selected.students, model.getSelectedItems());
            return this.UpdateView(chlk.activities.reports.ReportCardsDialog, ria.async.DeferredData(model), 'recipients');
        }
    ])
});
