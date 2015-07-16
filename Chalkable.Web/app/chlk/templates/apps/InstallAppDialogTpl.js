REQUIRE('chlk.models.apps.AppMarketInstallViewData');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.apps', function () {

    function orderByName(_1, _2) {
        var a = _1.name, b = _2.name;
        return a < b ? -1 : a > b ? 1 : 0;
    }

    /** @class chlk.templates.apps.InstallAppDialogTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-install-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppMarketInstallViewData)],


        'InstallAppDialogTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.apps.AppMarketApplication, 'app',

            [ria.templates.ModelPropertyBind],
            Boolean, 'alreadyInstalled',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.AllSchoolsActiveClasses, 'allClasses',

            Object, function calculateClassesTree_() {
                var data = this.allClasses, isInstalled = this.isAlreadyInstalled_;

                var schoolsMap = {};
                data.getSchools()
                    .map( function (_) { return {
                        id: _.getId(),
                        name: _.getName(),
                        courseTypes: []
                    }})
                    .sort(orderByName)
                    .forEach(function (_) { schoolsMap[_.id.valueOf()] = _; });

                var courseTypesMap = {};
                data.getCourseTypes()
                    .map(function (_) {
                        return {
                            id: _.getCourseTypeId(),
                            name: _.getCourseTypeName()
                        }
                    })
                    .forEach(function (_) {
                        courseTypesMap[_.id.valueOf()] = _;
                    });

                var schoolCourseTypes = data.getClasses()
                    .map(function (_) {
                        return {
                            id: _.getId(),
                            name: _.getFullClassName(),
                            schoolId: _.getSchoolId(),
                            courseTypeId: _.getCourseTypeId(),
                            isInstalled: isInstalled(_.getId())
                        }
                    })
                    .sort(orderByName)
                    .reduce(function (acc, _) {
                        var schoolCourseTypeId = _.schoolId.valueOf() + '_' + _.courseTypeId.valueOf(),
                            courseTypeId = _.courseTypeId.valueOf();

                        if (!acc[schoolCourseTypeId]) {
                            acc[schoolCourseTypeId] = {
                                id: courseTypesMap[courseTypeId].id,
                                name: courseTypesMap[courseTypeId].name,
                                schoolId: _.schoolId,
                                classes: [_]
                            }
                        } else {
                            acc[schoolCourseTypeId].classes.push(_);
                        }

                        return acc;
                    }, {});

                schoolsMap = Object.getOwnPropertyNames(schoolCourseTypes)
                    .map(function (_) { return schoolCourseTypes[_]})
                    .reduce(function (acc, _) {
                        _.isInstalled = _.classes.every(function (_) { return _.isInstalled });

                        acc[_.schoolId.valueOf()].courseTypes.push(_);
                        return acc
                    }, schoolsMap);


                return Object.getOwnPropertyNames(schoolsMap)
                    .map(function (_) { return schoolsMap[_]})
                    .filter(function (_) { return _.courseTypes.length })
                    .map(function (_) {
                        _.isInstalled = _.courseTypes.every(function (_) { return _.isInstalled });
                        return _;
                    });
            },

            [[chlk.models.id.ClassId]],
            Boolean, function isAlreadyInstalled_(classId) {
                return this.app.getInstalledForGroups().some(function (_) {
                    return _.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ
                        && _.getId().valueOf() == classId.valueOf()
                        && _.isInstalled();
                })
            },

            Boolean, function isAlreadyInstalledForMe_() {
                return this.app.getInstalledForGroups().some(function (_) {
                    return _.getGroupType() == chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER
                        && _.isInstalled();
                })
            }
        ])
});