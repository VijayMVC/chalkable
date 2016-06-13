REQUIRE('chlk.controllers.AnnouncementController');

NAMESPACE('chlk.controllers.announcement', function (){

    /** @class chlk.controllers.announcement.AdminAnnouncementController */
    CLASS(
        'AdminAnnouncementController', EXTENDS(chlk.controllers.AnnouncementController), [

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveDistrictAdminAction(model){
            var submitType = model.getSubmitType();

            if (submitType == 'saveNoUpdate'){
                this.setNotAblePressSidebarButton(true);
                this.adminAnnouncementService
                    .saveAdminAnnouncement(
                        model.getId(),
                        model.getTitle(),
                        model.getContent(),
                        model.getExpiresDate(),
                        model.getAssignedAttributesPostData()
                    )
                    .attach(this.validateResponse_());
                return null;
            }

            if (submitType == 'saveTitle'){
                return this.saveAdminTitleAction(model.getId(), model.getTitle())
            }

            if (submitType == 'checkTitle'){
                return this.checkAdminTitleAction(model.getTitle(), model.getId());
            }

            var res = this.adminAnnouncementService
                .submitAdminAnnouncement(
                    model.getId(),
                    model.getContent(),
                    model.getTitle(),
                    model.getExpiresDate(),
                    model.getAssignedAttributesPostData()
                )
                .attach(this.validateResponse_());

            res = res.then(function(saved){
                if(saved){
                    this.cacheAnnouncement(null);
                    this.cacheAnnouncementAttachments(null);
                    this.cacheAnnouncementApplications(null);
                    if(model.getSubmitType() == 'submitOnEdit')
                        return this.BackgroundNavigate('announcement', 'view', [model.getId(), chlk.models.announcement.AnnouncementTypeEnum.ADMIN]);
                    else{
                        return this.BackgroundNavigate('feed', 'list', [null, true]);
                    }
                }
            }, this);
            return null;
        },

        [[chlk.models.id.AnnouncementId, String]],
        function saveAdminTitleAction(announcementId, announcementTitle){
            var result = this.adminAnnouncementService
                .editTitle(announcementId, announcementTitle)
                .attach(this.validateResponse_())
                .then(function(data){
                    return new chlk.models.announcement.AnnouncementAttributeListViewData();
                });
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, result, chlk.activities.lib.DontShowLoader());
        },

        [[String, chlk.models.id.AnnouncementId]],
        function checkAdminTitleAction(title, annoId){
            var res = this.adminAnnouncementService
                .existsTitle(title, annoId)
                .attach(this.validateResponse_())
                .then(function(success){
                    return new chlk.models.Success(success);
                });
            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, res, chlk.activities.lib.DontShowLoader());
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId]],
        function showGroupsAction(announcementId){
            this.getContext().getSession().set(ChlkSessionConstants.ANNOUNCEMENT_ID, announcementId);
            var groupsIds = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_IDS, []).map(function (_) { return _.valueOf() });
            return this.Redirect('group', 'show', [{
                selected: groupsIds,
                controller: 'adminannouncement',
                action: 'saveGroupsToAnnouncement',
                resultHidden: 'groupIds',
                hiddenParams: {
                    id: announcementId
                }
            }]);
        },

        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.announcement.FeedAnnouncementViewData]],
        function saveGroupsToAnnouncementAction(model){
            var groups = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_LIST, []);
            var groupIds = model.getGroupIds() ? model.getGroupIds().split(',') : [];
            groups = groups.filter(function(item){
                return groupIds.indexOf(item.getId().valueOf().toString()) > -1
            });
            var recipients = groups.map(function(item){
                return new chlk.models.announcement.AdminAnnouncementRecipient(model.getId(), item.getId(), item.getName());
            });
            model.setRecipients(recipients);
            this.adminAnnouncementService.addGroupsToAnnouncement(model.getId(), model.getGroupIds())
                .then(function(){
                    this.BackgroundCloseView(chlk.activities.announcement.AnnouncementGroupsDialog);
                }, this)
                .attach(this.validateResponse_());
            this.getContext().getSession().set(ChlkSessionConstants.GROUPS_IDS, model.getGroupIds() ? model.getGroupIds().split(',').map(function(item){return new chlk.models.id.GroupId(item)}) : []);
            this.BackgroundUpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, model, 'recipients');
            return this.ShadeLoader();
        },


        [chlk.controllers.NotChangedSidebarButton()],
        [[chlk.models.id.AnnouncementId, chlk.models.id.GroupId]],
        function removeRecipientAction(announcementId, recipientId){

            var res = this.ShowConfirmBox('Are you sure you want to remove that group from announcement?', '')
                .thenCall(this.groupService.list, [])
                .attach(this.validateResponse_())
                .then(function(groups){
                    var groupIds = this.getContext().getSession().get(ChlkSessionConstants.GROUPS_IDS, []);
                    groupIds = groupIds.filter(function(id){
                        return id != recipientId
                    });
                    var groupIdsStr = groupIds.map(function(item){return item.valueOf()}).join(',');

                    groups = groups.filter(function(item){
                        return groupIds.indexOf(item.getId()) > -1
                    });

                    var recipients = groups.map(function(item){
                        return new chlk.models.announcement.AdminAnnouncementRecipient(announcementId, item.getId(), item.getName());
                    });
                    var model = new chlk.models.announcement.FeedAnnouncementViewData();
                    model.setId(announcementId);
                    model.setRecipients(recipients);
                    this.getContext().getSession().set(ChlkSessionConstants.GROUPS_IDS, groupIds);

                    return this.adminAnnouncementService.addGroupsToAnnouncement(model.getId(), groupIdsStr)
                        .then(function(){
                            return model;
                        }, this)
                }, this);

            return this.UpdateView(chlk.activities.announcement.AdminAnnouncementFormPage, res, 'recipients');
        }
    ])
});
