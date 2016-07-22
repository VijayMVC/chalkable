REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.admin.BaseStatistic');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.school', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.school.SchoolTeachersStatisticViewData*/
    CLASS(
        'SchoolTeachersStatisticViewData', EXTENDS(chlk.models.admin.BaseStatistic), IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.SchoolYearId, 'schoolYearId',

            String, 'schoolName',

            chlk.models.id.SchoolId, 'schoolId',

            ArrayOf(chlk.models.classes.Class), 'classes',

            String, 'gender',

            String, function getClassesText(){
                if(!this.classes || !this.classes.length)
                    return '';

                return this.classes.map(function(clazz){return clazz.getName()}).join(', ');
            },

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.name = SJX.fromValue(raw.displayname, String);
                this.gender = SJX.fromValue(raw.gender, String);
                this.classes = SJX.fromArrayOfDeserializables(raw.classes, chlk.models.classes.Class);
            }
        ]);
});
