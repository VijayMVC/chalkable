REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.classes.Room');
REQUIRE('chlk.models.departments.Department');

NAMESPACE('chlk.models.classes', function(){
   "use strict";

    /**@class chlk.models.classes.ClassInfo*/
    CLASS('ClassInfo', EXTENDS(chlk.models.classes.Class),[

        chlk.models.classes.Room, 'room',
        chlk.models.departments.Department, 'department'
    ]);
});