REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.feed.Feed');
REQUIRE('chlk.models.classes.Room');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.classes.ClassSummary*/
    CLASS(
        'ClassSummary', EXTENDS(chlk.models.classes.Class), [

            chlk.models.feed.Feed, 'feed',

            function getStatusText(){
                var arr = [this.getTeacher().getDisplayName()];

                if(this.getPeriods() && this.getPeriods().length){
                    var res = this.getPeriods().length > 1 ? 'Periods: ' : 'Period: ';
                    res += this.getPeriods().join(', ');

                    if(this.getDayTypes() && this.getDayTypes().length){
                        res += (' - ' + this.getDayTypes().join(', '))
                    }

                    arr.push(res);
                }

                if(this.getRoom())
                    arr.push('Room ' + this.getRoom().getRoomNumber());

                return arr.join('\n');
            }
        ]);
});
