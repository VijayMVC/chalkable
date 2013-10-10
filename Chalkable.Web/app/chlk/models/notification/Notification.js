REQUIRE('chlk.models.id.NotificationId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.id.MarkingPeriodId');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.ClassPeriodId');

NAMESPACE('chlk.models.notification', function () {

    /** @class chlk.models.notification.NotificationTypeEnum*/
    ENUM(
        'NotificationTypeEnum', {
            SIMPLE: 0,
            ANNOUNCEMENT:1,
            MESSAGE: 2,
            QUESTION: 3,
            ITEM_TO_GRADE: 4,
            APP_BUDGET_BALANCE: 5,
            APPLICATION: 6,
            MARKING_PERIOD_ENDING: 7,
            ATTENDANCE: 8,
            NO_TAKE_ATTENDANCE: 9
        });

    "use strict";
    /** @class chlk.models.notification.Notification*/
    CLASS(
        'Notification', [
            chlk.models.id.NotificationId, 'id',
            chlk.models.notification.NotificationTypeEnum, 'type',
            String, 'message',
            Boolean, 'shown',
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',
            [ria.serialize.SerializeProperty('privatemessageid')],
            chlk.models.id.MessageId, 'privateMessageId',
            [ria.serialize.SerializeProperty('markingperiodid')],
            chlk.models.id.MarkingPeriodId, 'markingPeriodId',
            chlk.models.people.ShortUserInfo, 'person',
            [ria.serialize.SerializeProperty('applicationid')],
            chlk.models.id.AppId, 'applicationId',
            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',
            [ria.serialize.SerializeProperty('classperiodid')],
            chlk.models.id.ClassPeriodId, 'classPeriodId',
            [ria.serialize.SerializeProperty('applicationname')],
            String, 'applicationName',
            [ria.serialize.SerializeProperty('announcementtype')],
            Number, 'announcementType',
            [ria.serialize.SerializeProperty('announcementtypename')],
            String, 'announcementTypeName',
            chlk.models.common.ChlkDate, 'created',

            String, function getCreatedTime(){
                var created = this.getCreated();
                return created ? this.convertToTime_(created) : '';
            },

            [[chlk.models.common.ChlkDate]],
            String, function convertToTime_(date){
                var now = new chlk.models.common.ChlkDate(getDate()), mins;
                if(now.isSameDay(date)){
                    mins = Math.floor((now.getDate() - date.getDate()) / (1000 * 60));
                    if(mins < 60) return Msg.minutes_ago(mins);
                    else return Msg.hours_ago(Math.floor(mins/60));
                }
                return date.toString('hh:mm tt');
            }
        ]);
});
