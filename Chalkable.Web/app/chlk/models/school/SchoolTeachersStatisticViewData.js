REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.SchoolTeachersStatisticViewData*/
    CLASS(
        'SchoolTeachersStatisticViewData', EXTENDS(chlk.models.admin.BaseStatistic), IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.people.ShortUserInfo, 'staffViewData',

            chlk.models.id.SchoolYearId, 'schoolYearId',

            ArrayOf(chlk.models.classes.Class), 'classes',

            String, function getClassesText(){
                if(!this.classes || !this.classes.length)
                    return '';

                return this.classes.map(function(clazz){return clazz.getName()}).join(', ');
            },

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.staffViewData = SJX.fromDeserializable(raw.staffviewdata, chlk.models.people.ShortUserInfo);
                this.classes = SJX.fromArrayOfDeserializables(raw.classes, chlk.models.classes.Class);
            }
        ]);
});
