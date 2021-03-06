REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.standard.StandardForExplorer');
REQUIRE('ria.serialize.SJX');


NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentExplorer*/

    var SJX = ria.serialize.SJX;

    /**@class chlk.models.student.StudentClassExplorer*/

    CLASS(
        UNSAFE, 'StudentClassExplorer', IMPLEMENTS(ria.serialize.IDeserializable), [

            chlk.models.classes.Class, 'clazz',
            Number, 'avg',
            chlk.models.announcement.ClassAnnouncementViewData, 'announcement',
            ArrayOf(chlk.models.standard.StandardForExplorer), 'standards',

            VOID, function deserialize(raw){
                this.clazz = SJX.fromDeserializable(raw.class, chlk.models.classes.Class);
                this.announcement = SJX.fromDeserializable(raw.importantannouncement, chlk.models.announcement.ClassAnnouncementViewData);
                this.standards = SJX.fromArrayOfDeserializables(raw.standards, chlk.models.standard.StandardForExplorer);
                this.avg = SJX.fromValue(raw.avg, Number);
            }
    ]);

    CLASS(
        UNSAFE, 'StudentExplorer', EXTENDS(chlk.models.people.ShortUserInfo), [

            ArrayOf(chlk.models.student.StudentClassExplorer), 'classesGradingInfo',

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw.student);
                this.classesGradingInfo = SJX.fromArrayOfDeserializables(raw.classesgradinginfo, chlk.models.student.StudentClassExplorer);
            }
    ]);

});