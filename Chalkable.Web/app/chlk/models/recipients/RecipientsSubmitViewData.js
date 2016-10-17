REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.id.GroupId');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.recipients.RecipientsSubmitViewData*/
    CLASS(
        UNSAFE, 'RecipientsSubmitViewData',
                IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.groupIds = SJX.fromValue(raw.groupIds, String);
                this.studentIds = SJX.fromValue(raw.studentIds, String);
                this.requestId = SJX.fromValue(raw.requestId, String);
                this.name = SJX.fromValue(raw.name, String);
                this.groupId = SJX.fromValue(raw.groupId, chlk.models.id.GroupId);
                if(raw.selectedItems)
                    this.selectedItems = JSON.parse(SJX.fromValue(raw.selectedItems, String));
            },

            String, 'groupIds',
            String, 'studentIds',
            String, 'requestId',
            String, 'name',
            Object, 'selectedItems',
            chlk.models.id.GroupId, 'groupId',

            function getParsedSelected(){
                var selectedItems = this.selectedItems ? ria.__API.clone(this.selectedItems): {groups:[], students:[]};

                selectedItems.groups = selectedItems.groups.map(function(group){
                    return new chlk.models.group.Group(group.name, new chlk.models.id.GroupId(group.id));
                });

                selectedItems.students = selectedItems.students.map(function(student){
                    return new chlk.models.people.ShortUserInfo(null, null, new chlk.models.id.SchoolPersonId(student.id), student.displayname, student.gender);
                });

                return selectedItems
            }
        ]);
});
