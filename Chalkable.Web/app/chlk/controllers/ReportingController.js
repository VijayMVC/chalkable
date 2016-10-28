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
                this.customReportTemplateService.list(chlk.models.reports.CustomReportTemplateType.BODY),
                this.customReportTemplateService.getDefaultStudentIdToPrint()
            ])
            .attach(this.validateResponse_())
            .then(function(res){
                var reasons = this.getContext().getSession().get(ChlkSessionConstants.ATTENDANCE_REASONS, []);
                var ableDownload = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.COMPREHENSIVE_PROGRESS_REPORT);
                var isAbleToReadSSNumber = this.hasUserPermission_(chlk.models.people.UserPermissionEnum.STUDENT_SOCIAL_SECURITY_NUMBER);
                return new chlk.models.reports.SubmitReportCardsViewData(res[1], res[0], reasons,ableDownload,isAbleToReadSSNumber, res[2] || chlk.models.reports.StudentIdentifierEnum.NONE);
            }, this);

            return this.ShadeView(chlk.activities.reports.ReportCardsDialog, res);
        },

        [[chlk.models.reports.SubmitReportCardsViewData]],
        function submitReportCardsAction(model){
            if(model.getSubmitType() == "addRecipients"){
                return this.Redirect('reporting', 'showGroupsFromReport', [model.getParsedSelected()])
            }

            if(model.getSubmitType() == 'recipient')
                return this.addRecipients_(model);

            if(!model.getTitle()){
                this.ShowMsgBox('Title field is required.');
                return null;
            }

            if(!model.getCustomReportTemplateId() || !model.getCustomReportTemplateId().valueOf()){
                this.ShowMsgBox('Layout field is required.');
                return null;
            }

            if(!model.getGradingPeriodId() || !model.getGradingPeriodId().valueOf()){
                this.ShowMsgBox('Grading Period field is required.');
                return null;
            }

            if(!model.getAttendanceReasonIds()){
                this.ShowMsgBox('Absence Reasons field is required.');
                return null;
            }

            this.reportingService.submitReportCards(
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
            .attach(this.validateResponse_());

            setTimeout(function(){
                this.BackgroundCloseView(chlk.activities.reports.ReportCardsDialog);
            }.bind(this), 100);

            return this.ShowAlertBox('<b>Your report is being prepared.<br>You will receive a notification when it\'s ready!</b>', null, true, 'report'), null ;
        },

        [[chlk.models.reports.SubmitReportCardsViewData]],
        function addRecipients_(model){
            if(!model.getRecipientsToAdd())
                return null;

            var recipients = model.getRecipientsToAdd().split(','),
                parsedSelected = model.getParsedSelected(),
                selected = JSON.parse(model.getSelectedItems());
            recipients.forEach(function(recipient){
                var arr = recipient.split('|'),
                    id = parseInt(arr[0], 10),
                    type = parseInt(arr[1], 10),
                    description = arr[2];
                if(type == chlk.models.search.SearchTypeEnum.PERSONS.valueOf()){
                    if(selected.students.map(function (item) {return item.id}).indexOf(id) == -1){
                        parsedSelected.students.push(new chlk.models.people.ShortUserInfo(null, null, new chlk.models.id.SchoolPersonId(id), description, arr[3]));
                        selected.students.push({
                            id: id,
                            displayname: description,
                            gender: arr[3]
                        });
                    }
                }else{
                    if(selected.groups.map(function (item) {return item.id}).indexOf(id) == -1){
                        parsedSelected.groups.push(new chlk.models.group.Group(description, new chlk.models.id.GroupId(id)));
                        selected.groups.push({
                            id: id,
                            name: description
                        });
                    }
                }
            });

            var res = ria.async.DeferredData(new chlk.models.reports.ReportCardRecipientsViewData(parsedSelected.groups, parsedSelected.students, selected), 10);
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
