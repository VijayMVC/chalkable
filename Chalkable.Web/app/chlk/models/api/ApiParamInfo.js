REQUIRE('chlk.models.api.ApiParamType');

NAMESPACE('chlk.models.api', function () {
    "use strict";

    /** @class chlk.models.api.ApiParamInfo*/
    CLASS(
        'ApiParamInfo', [
            String, 'name',
            String, 'description',
            String, 'value',

            [ria.serialize.SerializeProperty('isnullable')],
            Boolean, 'optional',

            [ria.serialize.SerializeProperty('paramtype')],
            chlk.models.api.ApiParamType, 'paramType',

            String, function getParamTypeAsString() {
                var optional = this.optional ? '?' : '';

                switch (this.paramType) {
                    case chlk.models.api.ApiParamType.UNDEFINED: return 'void';
                    case chlk.models.api.ApiParamType.INTEGER: return 'int' + optional;
                    case chlk.models.api.ApiParamType.STRING: return 'string';
                    case chlk.models.api.ApiParamType.BOOLEAN: return 'bool' + optional;
                    case chlk.models.api.ApiParamType.INTLIST: return 'IntList' + optional;
                    case chlk.models.api.ApiParamType.GUID: return 'Guid' + optional;
                    case chlk.models.api.ApiParamType.GUIDLIST: return 'GuidList' + optional;
                    case chlk.models.api.ApiParamType.LISTOFSTRINGLIST: return 'List<StringList>' + optional;
                    case chlk.models.api.ApiParamType.DATE: return 'DateTime' + optional;
                    default: return '';
                }
            }
        ]);
});
