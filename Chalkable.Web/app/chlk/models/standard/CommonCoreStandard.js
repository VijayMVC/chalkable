/**
 * Created with JetBrains WebStorm.
 * User: Yuran
 * Date: 11/12/14
 * Time: 2:02 AM
 * To change this template use File | Settings | File Templates.
 */

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.CommonCoreStandard*/
    CLASS(
        'CommonCoreStandard', [

            [ria.serialize.SerializeProperty('standardcode')],
            String, 'standardCode',

            [ria.serialize.SerializeProperty('description')],
            String, 'description'
    ]);
});
