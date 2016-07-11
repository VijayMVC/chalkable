REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');
REQUIRE('chlk.models.common.ChlkDate');

REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.ClassAnnouncementViewData*/
    CLASS(
        UNSAFE, 'ClassAnnouncementViewData',
                EXTENDS(chlk.models.announcement.BaseAnnouncementViewData),
                IMPLEMENTS(ria.serialize.IDeserializable), [

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
                this.defaultTitle = SJX.fromValue(raw.defaulttitle, Boolean);
                this.setAnnouncementTypeId(SJX.fromValue(raw.announcementtypeid, Number));
                this.classId = SJX.fromValue(Number(raw.classid), chlk.models.id.ClassId);
                this.shortClassName = SJX.fromValue(raw.classname, String);
                this.className = SJX.fromValue(raw.fullclassname, String);
                this.dropped = SJX.fromValue(raw.dropped, Boolean);
                this.order = SJX.fromValue(raw.order, Number);
                this.maxScore = SJX.fromValue(raw.maxscore, Number);
                this.ableDropStudentScore = SJX.fromValue(raw.candropstudentscore, Boolean);
                this.ableToExempt = SJX.fromValue(raw.maybeexempt, Boolean);
                this.gradable = SJX.fromValue(raw.gradable, Boolean);
                this.ableToGrade = SJX.fromValue(raw.cangrade, Boolean);
                this.hiddenFromStudents = SJX.fromValue(raw.hidefromstudents, Boolean);
                this.weightMultiplier = SJX.fromValue(raw.weightmultiplier, Number);
                this.weightAddition = SJX.fromValue(raw.weightaddition, Number);
            },

            function $(){
                BASE();
                this.announcementTypeId = null;
                this.classId = null;
                this._annTypeEnum = chlk.models.announcement.AnnouncementTypeEnum;
            },

            chlk.models.common.ChlkDate, 'expiresDate',
            String, 'defaultTitle',
            Number, 'announcementTypeId',
            [[Number]],
            VOID, function setAnnouncementTypeId(announcementTypeId){
                this.announcementTypeId = announcementTypeId;
            },
            chlk.models.id.ClassId, 'classId',
            String, 'shortClassName',
            String, 'className',
            Boolean, 'dropped',
            Number, 'order',
            Number, 'maxScore',
            Boolean, 'ableDropStudentScore',
            Boolean, 'ableToExempt',
            Boolean, 'gradable',
            Boolean, 'ableToGrade',
            Boolean, 'hiddenFromStudents',
            Number, 'weightMultiplier',
            Number, 'weightAddition',
            String, 'expiresDateColor',
            String, 'expiresDateText',

            function prepareExpiresDateText(){
                var now = getSchoolYearServerDate();
                var days = 0;
                var expTxt = "";
                var expires = this.getExpiresDate();
                var expiresDate = expires.getDate();
                var date = expires.format('(D m/d)');
                this.setExpiresDateColor('blue');

                if(formatDate(now, 'dd-mm-yy') == expires.format('dd-mm-yy')){
                    this.setExpiresDateColor('blue');
                    this.setExpiresDateText(Msg.Due_today);
                }else{
                    if(now > expires.getDate()){
                        this.setExpiresDateColor('red');
                        days = getDateDiffInDays(expiresDate, now);
                        expTxt = days == 1 ? Msg.Due_yesterday + " " + date : Msg.Due_days_ago(days) + " " + date;

                    }else{
                        days = getDateDiffInDays(now, expiresDate);
                        expTxt = days == 1 ? Msg.Due_tomorrow + " " + date : Msg.Due_in_days(days) + " " + date;
                    }
                    this.setExpiresDateText(expTxt);
                }
            },


            Number, 'avg'
        ]);
});
