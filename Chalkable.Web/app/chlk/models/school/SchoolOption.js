/**
 * Created with JetBrains WebStorm.
 * User: Yuran
 * Date: 5/28/14
 * Time: 3:14 AM
 * To change this template use File | Settings | File Templates.
 */

NAMESPACE('chlk.models.school', function () {
    "use strict";
/** @class chlk.models.school.SchoolOption*/

    CLASS('SchoolOption', [
        [ria.serialize.SerializeProperty('allowscoreentryforunexcused')],
        Boolean, 'AllowScoreEntryForUnexcused'
    ]);
});
